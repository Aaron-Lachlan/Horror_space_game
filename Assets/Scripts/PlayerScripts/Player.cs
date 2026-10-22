using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Globals globals;
    [SerializeField] GameObject globalsObject;
    [SerializeField] PlayerCharacter playerCharacter;
    [Header("Camera")]
    [SerializeField] PlayerCamera playerCamera;
    [SerializeField] CameraSpring cameraSpring;
    [SerializeField] CameraLean cameraLean;

    [Space]
    [SerializeField] GameObject playerFlashlight;

    [SerializeField] bool flashlightToggle = true;
    [SerializeField] AudioClip flashlightOnClip;
    [SerializeField] AudioClip flashlightOffClip;
    [SerializeField] AudioSource flashlightAudioSource;

    PlayerInput _inputActions;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _inputActions = new PlayerInput();
        _inputActions.Enable();

        playerCharacter.Initialize();
        playerCamera.Initialize(playerCharacter.GetCameraTarget());

        cameraSpring.Initialize();
        cameraLean.Initialize();
    }

    void OnDestroy()
    {
        _inputActions.Dispose();
    }

    // Update is called once per frame
    void Update()
    {
        if(globals == null)
        {
            globals = GameObject.Find("GlobalObject").GetComponent<Globals>();
        }

        var input = _inputActions.Player;
        var deltaTime = Time.deltaTime;

        //Find camera input and update its rotation.
        var cameraInput = new CameraInput { look = input.Look.ReadValue<Vector2>() };
        if(globals.enableMovement)
        {
            playerCamera.UpdateRotation(cameraInput);
            playerCamera.UpdatePosition(playerCharacter.GetCameraTarget());
        }

        var characterinput = new CharacterInput
        {
            rotation = playerCamera.transform.rotation,
            move = input.Movement.ReadValue<Vector2>(),
            jump = input.Jump.WasPressedThisFrame(),
            /*crouch = input.Crouch.WasPerformedThisFrame()
                ? CrouchInput.Toggle
                //? CrouchInput.Crouch
                //? CrouchInput.Uncrouch
                : CrouchInput.None,*/
            crouch = input.CrouchHeld.IsPressed()
                ? CrouchInput.Crouching
                : CrouchInput.Uncrouch,
            sprint = input.Sprint.IsPressed()
                ? SprintInput.Sprint
                : SprintInput.None
        };

        if(globals.enableMovement)
        {
            playerCharacter.UpdateInput(characterinput);
            playerCharacter.UpdateBody(deltaTime);
            if(flashlightToggle && globals.flashlightEnabled && Input.GetKeyDown(KeyCode.Q))
            {
                flashlightToggle = false;
                //flashlightAudioSource.PlayOneShot(flashlightOffClip);
                playerFlashlight.SetActive(false);
                globals.flashlightEnabled = false;
            }
            if(flashlightToggle && !globals.flashlightEnabled && Input.GetKeyDown(KeyCode.Q))
            {
                flashlightToggle = false;
                //flashlightAudioSource.PlayOneShot(flashlightOnClip);
                playerFlashlight.SetActive(true);
                globals.flashlightEnabled = true;
            }
            if(Input.GetKeyUp(KeyCode.Q))
            {
                flashlightToggle = true;
            }
        }
    }

    void LateUpdate()
    {
        var deltaTime = Time.deltaTime;
        var cameraTarget = playerCharacter.GetCameraTarget();
        var state = playerCharacter.GetState();

        cameraSpring.UpdateSpring(deltaTime, cameraTarget.up);
        cameraLean.UpdateLean(deltaTime, state.acceleration, cameraTarget.up);
        //print("Call Update Lean");
    }
}
