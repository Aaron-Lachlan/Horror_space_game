using UnityEngine;
using KinematicCharacterController;
using UnityEngine.Rendering;
using Unity.Mathematics;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using NUnit.Framework;

public enum CrouchInput
{
    None, Toggle, Crouching, Uncrouch
}

public enum SprintInput
{
    None, Sprint
}

public enum Stance
{
    Stand, Crouch, Sprint, Slide
}

public struct CharacterState
{
    public bool grounded;
    public Stance stance;
    public Vector3 velocity;
    public Vector3 acceleration;
}

public struct CharacterInput
{
    public Quaternion rotation;
    public Vector2 move;
    public bool jump;
    public CrouchInput crouch;
    public SprintInput sprint;
}

public class PlayerCharacter : MonoBehaviour, ICharacterController
{
    [Header("Links")]
    [SerializeField] KinematicCharacterMotor motor;
    [SerializeField] Transform cameraTarget;

    [Header("Movement Variables")]
    public float speed;
    [SerializeField] float responce;
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float sprintSpeed = 12f;
    [SerializeField] float crouchSpeed = 3f;
    [SerializeField] float walkResponce = 20f;
    [SerializeField] float crouchResponce = 15f;
    [SerializeField] float sprintResponce = 25f;
    [SerializeField] Vector3 moveVelocity;
    public Vector3 currentPlayerVelocity;

    [Header("Air Control")]
    [SerializeField] float airSpeed;
    [SerializeField] float airAcceleration;

    [Header("Jump Variables")]
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float coyoteTime = 0.1f;
    [SerializeField] float gravity = -11f;

    [Header("Slide Variables")]
    [SerializeField] float slideStartSpeed;
    //If below this value, return to crouch
    [SerializeField] float slideEndSpeed;
    //Rate which slide speed will be lost
    [SerializeField] float slideFriction;

    [Header("Height Variables")]
    [SerializeField] float standHeight = 2.1f;
    [SerializeField] float crouchHeight = 1.2f;
    [SerializeField] float crouchHeightResponce = 10f;
    //[Range(0f, 2f)]
    [SerializeField] Vector3 standCameraTargetHeight;
    [SerializeField] Vector3 crouchCameraTargetHeight;
    [SerializeField] Vector3 cameraTargetHeight;

    private CharacterState _state;
    private CharacterState _lastState;
    private CharacterState _tempState;

    private Quaternion _requestedRotation;
    public Vector3 _requestedMovement;
    private bool _requestedJump;
    private bool _requestedCrouch;
    private bool _requestedSprint;
    private bool _ungroundedDueToJump;

    private float _timeSinceUngrounded;
    private float _timeSinceJumpRequested;

    private Collider[] _uncrouchOverlapResults;

    [Header("Movement Bools")]
    public bool isWalking;
    public bool isSprinting;
    public bool isCrouching;

    public void Initialize()
    {
        _state.stance = Stance.Stand;
        _lastState = _state;
        isWalking = false;

        _uncrouchOverlapResults = new Collider[8];

        motor.CharacterController = this;
    }

    public void UpdateInput(CharacterInput input)
    {
        _requestedRotation = input.rotation;
        //Get 2D input vector and create 3D movement on XZ plane
        _requestedMovement = new Vector3(input.move.x, 0f, input.move.y);
        //Clamp length to 1 so movement is not faster on diagonal.
        _requestedMovement = Vector3.ClampMagnitude(_requestedMovement, 1f);
        //Orient the input to direct of facing.
        _requestedMovement = input.rotation * _requestedMovement;

        var wasRequestingJump = _requestedJump;
        _requestedJump = _requestedJump || input.jump;
        if(_requestedJump && !wasRequestingJump)
            _timeSinceJumpRequested = 0f;

        _requestedCrouch = input.crouch switch
        {
            CrouchInput.Toggle => !_requestedCrouch,
            CrouchInput.None => _requestedCrouch,
            CrouchInput.Crouching => true,
            CrouchInput.Uncrouch => false,
            _ => _requestedCrouch
        };
        _requestedSprint = input.sprint switch
        {
            SprintInput.Sprint => true,
            SprintInput.None => false,
            _ => _requestedSprint
        };
    }

    public void UpdateBody(float deltaTime)
    {
        var currentHeight = motor.Capsule.height;

        if(_state.stance is Stance.Stand || _state.stance is Stance.Sprint)
        {
            cameraTargetHeight = standCameraTargetHeight;
        }

        if(_state.stance is Stance.Crouch)
        {
            cameraTargetHeight = crouchCameraTargetHeight;
        }
        /*var cameraTargetHeight =
        (
            _state.stance is Stance.Stand
                ? standCameraTargetHeight
                : crouchCameraTargetHeight
        );*/

        //var rootTargetscale = new Vector3(1f, normalizedHeight, 1f)

        cameraTarget.localPosition = Vector3.Lerp
        (
            a: cameraTarget.localPosition,
            b: new Vector3(0f, cameraTargetHeight.y, 0f),
            t: 1f - Mathf.Exp(-crouchHeightResponce * deltaTime)
        );
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        _state.acceleration = Vector3.zero;
        //If player is on ground
        if(motor.GroundingStatus.IsStableOnGround)
        {
            _timeSinceUngrounded = 0f;
            _ungroundedDueToJump = false;

            //Set move direction to angle of surface player is on
            var groundedMovement = motor.GetDirectionTangentToSurface
            (
                direction: _requestedMovement,
                surfaceNormal: motor.GroundingStatus.GroundNormal
            ) * _requestedMovement.magnitude;

            /*//Start slide
            {
                var moving = groundedMovement.sqrMagnitude > 0f;
                var crouching = _state.stance is Stance.Crouch;
                var wasStanding = _lastState.stance is Stance.Stand;
                var wasInAir = !_lastState.grounded;

                if(moving && crouching && (wasStanding || wasInAir))
                {
                    _state.stance = Stance.Slide;

                    var slideSpeed = Mathf.Max(slideStartSpeed, currentVelocity.magnitude);
                    currentVelocity = motor.GetDirectionTangentToSurface
                    (
                        direction: currentVelocity,
                        surfaceNormal: motor.GroundingStatus.GroundNormal
                    ) * slideSpeed;
                }
            }*/

            //Move
            if(_state.stance is Stance.Stand or Stance.Crouch or Stance.Sprint)
            {
                if(_state.stance is Stance.Stand)
                {
                    speed = walkSpeed;
                    responce = walkResponce;
                }

                if(_state.stance is Stance.Crouch)
                {
                    speed = crouchSpeed;
                    responce = crouchResponce;
                }

                if(_state.stance is Stance.Sprint)
                {
                    speed = sprintSpeed;
                    responce = sprintResponce;
                }

                //Move along the ground in said direction
                var targetVelocity = groundedMovement * speed;
                moveVelocity = Vector3.Lerp
                (
                    a: currentVelocity,
                    b: targetVelocity,
                    t: 1f - Mathf.Exp(-responce * deltaTime)
                );
                _state.acceleration = moveVelocity - currentVelocity;
                
                currentVelocity = moveVelocity;
                currentPlayerVelocity = currentVelocity;
            }
            /*//Continue Slide
            else
            {
                //Apply Friction
                currentVelocity -= currentVelocity * (slideFriction * deltaTime);
                
                //Stop Slide
                if(currentVelocity.magnitude < slideEndSpeed)
                    _state.stance = Stance.Crouch;
            }*/
        }
        //If in air
        else
        {
            _timeSinceUngrounded += deltaTime;

            if(_requestedMovement.sqrMagnitude > 0f)
            {
                var planarMovement = Vector3.ProjectOnPlane
                (
                    vector: _requestedMovement,
                    planeNormal: motor.CharacterUp
                ) * _requestedMovement.normalized.magnitude;

                var currentPlanarVelocity = Vector3.ProjectOnPlane
                (
                    vector: currentVelocity,
                    planeNormal: motor.CharacterUp
                );

                var movementForce = planarMovement * airAcceleration * deltaTime;

                if(currentPlanarVelocity.magnitude < airSpeed)
                {
                    var targetPlanarVelocity = currentPlanarVelocity + movementForce;

                    //Limit target velocity to air speed
                    targetPlanarVelocity = Vector3.ClampMagnitude(targetPlanarVelocity, airSpeed);

                    movementForce = targetPlanarVelocity - currentPlanarVelocity;
                }
                else if(Vector3.Dot(currentPlanarVelocity, movementForce) > 0f)
                {
                    var constrainedMovementForce = Vector3.ProjectOnPlane
                    (
                        vector: movementForce,
                        planeNormal: currentPlanarVelocity.normalized
                    );

                    movementForce = constrainedMovementForce;
                }

                //Prevent vertical climbing
                if(motor.GroundingStatus.FoundAnyGround)
                {
                    if(Vector3.Dot(movementForce, currentVelocity + movementForce) > 0f)
                    {
                        var obstructionNormal = Vector3.Cross
                        (
                            motor.CharacterUp,
                            Vector3.Cross
                            (
                                motor.CharacterUp,
                                motor.GroundingStatus.GroundNormal
                            )
                        ).normalized;

                        //Project movement force
                        movementForce = Vector3.ProjectOnPlane(movementForce, obstructionNormal);
                    }
                }

                currentVelocity += movementForce;
            }
            //Gravity
            currentVelocity += motor.CharacterUp * gravity * deltaTime;
        }

        //Jump
        if(_requestedJump)
        {
            var grounded = motor.GroundingStatus.IsStableOnGround;
            var canCoyoteJump = _timeSinceUngrounded < coyoteTime && !_ungroundedDueToJump;

            if(grounded || canCoyoteJump)
            {
                //print("Jumped");
                _requestedJump = false;
                //_requestedCrouch = false;

                //unstick player
                motor.ForceUnground(time: 0.05f);
                _ungroundedDueToJump = true;

                //Apply jump force
                var currentVerticalSpeed = Vector3.Dot(currentVelocity, motor.CharacterUp);
                var targetVerticalSpeed = Mathf.Max(currentVerticalSpeed, jumpSpeed);

                currentVelocity += motor.CharacterUp * (targetVerticalSpeed - currentVerticalSpeed);
            }
            else
            {
                _timeSinceJumpRequested += deltaTime;
                
                var canJumpLater = _timeSinceJumpRequested < coyoteTime;
                _requestedJump = canJumpLater;
            }
        }

        /*if(moveVelocity.magnitude <= 0 && _state.stance is Stance.Sprint)
        {
            _state.stance = Stance.Stand;
        }*/
        //if(moveVelocity.magnitude <= 0)
    }
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        var forward = Vector3.ProjectOnPlane
        (
            _requestedRotation * Vector3.forward,
            motor.CharacterUp
        );

        if(forward != Vector3.zero)
            currentRotation = Quaternion.LookRotation(forward, motor.CharacterUp);
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        _tempState = _state;
        //Crouch
        if(_requestedCrouch && _state.stance is Stance.Stand) //|| _state.stance is Stance.Sprint)
        {
            _state.stance = Stance.Crouch;
            motor.SetCapsuleDimensions
            (
                radius: motor.Capsule.radius,
                height: crouchHeight,
                yOffset: crouchHeight * 0.5f
            );

        }

        if(_requestedSprint && _state.stance is Stance.Stand && _state.stance is not Stance.Crouch) //&& _state.stance is not Stance.Slide)
        {
            _state.stance = Stance.Sprint;
        }
    }
    public void PostGroundingUpdate(float deltaTime)
    {
        /*if(!motor.GroundingStatus.IsStableOnGround && _state.stance is Stance.Slide)
        {
            _state.stance = Stance.Crouch;
        }*/
    }
    public void AfterCharacterUpdate(float deltaTime)
    {
        //var totalAcceleration = (_state.velocity - _lastState.velocity) / deltaTime;
        //_state.acceleration = Vector3.ClampMagnitude(_state.acceleration, totalAcceleration.magnitude);
        //Uncrouch
        if(!_requestedCrouch && _state.stance is not Stance.Stand)
        {
            //Fake Stand
            motor.SetCapsuleDimensions
            (
                radius: motor.Capsule.radius,
                height: standHeight,
                yOffset: standHeight * 0.5f
            );

            var pos = motor.TransientPosition;
            var rot = motor.TransientRotation;
            var mask = motor.CollidableLayers;

            //See if overlap
            if(motor.CharacterOverlap(pos, rot, _uncrouchOverlapResults, mask, QueryTriggerInteraction.Ignore) > 0)
            {
                //Re-crouch
                _requestedCrouch = true;
                motor.SetCapsuleDimensions
                (
                    radius: motor.Capsule.radius,
                    height: crouchHeight,
                    yOffset: crouchHeight * 0.5f
                );
            }
            else
            {
                _state.stance = Stance.Stand;
            }
        }

        if(!_requestedSprint && _state.stance is not Stance.Crouch)
        {
            _state.stance = Stance.Stand;
        }
        if(_requestedSprint && _state.stance is not Stance.Crouch && motor.GroundingStatus.IsStableOnGround)
        {
            //print("Sprint Stance");
            _state.stance = Stance.Sprint;
        }

        if(currentPlayerVelocity.magnitude <= 4.5 && _state.stance is Stance.Crouch)
        {
            isCrouching = true;
            isSprinting = false;
        }

        if(_requestedMovement.magnitude > 0 && currentPlayerVelocity.magnitude < 6.5 && _state.stance is not Stance.Crouch)
        {
            //print("Walking");
            isWalking = true;
            isSprinting = false;
        }

        if(_requestedMovement.magnitude > 0 && _state.stance is not Stance.Crouch && _state.stance is Stance.Sprint)
        {
            //print("Sprinting");
            isWalking = true;
            isSprinting = true;
        }
        
        if(_requestedMovement.magnitude <= 0)
        {
            isWalking = false;
            isSprinting = false;
        }

        /*if(_requestedMovement.magnitude <= 0)
        {
            _state.stance
        }*/
        
        //Update States
        _state.grounded = motor.GroundingStatus.IsStableOnGround;
        _lastState= _tempState;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        
    }
    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        _state.acceleration = Vector3.ProjectOnPlane(_state.acceleration, hitNormal);
    }

    public bool IsColliderValidForCollisions(Collider coll) => true;

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
        
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
        
    }

    public Transform GetCameraTarget() => cameraTarget;

    public CharacterState GetState() => _state;
    public CharacterState GetLastState() => _lastState;
}
