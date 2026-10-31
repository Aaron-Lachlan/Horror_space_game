using Unity.VisualScripting;
using UnityEngine;

public class WeaponAnimationHandler : MonoBehaviour
{
    public Animator weaponAnimator;
    //[SerializeField] PlayerController playerController;
    [SerializeField] PlayerCharacter playerCharacter;
    [SerializeField] bool isArms;

    [Header ("Audio References")]
    [SerializeField] AudioSource animationAudioSource;
    [SerializeField] AudioClip[] audioResources;

    void Awake()
    {
        playerCharacter = GameObject.FindWithTag("PlayerCharacter").GetComponent<PlayerCharacter>();
        if(playerCharacter != null)
        {
            print("Player Controller Found");
        }
        //animator.SetBool("Sprinting", false);
    }

    void Update()
    {
        //if(state.stance is Stance.Stand)
        if(playerCharacter != null)
        {
            var state = playerCharacter.GetState();
            //Debug.Log(state.stance);
            //var grounded = playerCharacter.GetGroundStatus();

            //Movement Animaions
            if(state.grounded)
            {
                //print(state.grounded);
                if(!isArms)
                {
                    weaponAnimator.SetBool("InAirJump", false);
                    
                    if(playerCharacter.isWalking)
                    {
                        weaponAnimator.SetBool("Walking", true);
                    }
                    else
                    {
                        //print("Walk off");
                        weaponAnimator.SetBool("Walking", false);
                    }

                    if(state.stance is Stance.Crouch)
                    {
                        weaponAnimator.SetBool("Crouching", true);
                        if(state.stance is Stance.Crouch && playerCharacter._requestedMovement.magnitude > 0)
                        {
                            weaponAnimator.SetBool("Walking", true);
                        }
                        else
                        {
                            weaponAnimator.SetBool("Walking", false);
                        }
                    }
                    else
                    {
                        weaponAnimator.SetBool("Crouching", false);
                    }
                }

                if(playerCharacter.isSprinting)
                {
                    //print("Sprint Anim");
                    weaponAnimator.SetBool("Sprinting", true);
                }
                else
                {
                    weaponAnimator.SetBool("Sprinting", false);
                }
            }
            else
            {
                weaponAnimator.SetBool("Sprinting", false);
                if(!isArms)
                {
                    weaponAnimator.SetBool("Walking", false);
                }
            }
        }

        /*if(!playerController.characterController.isGrounded && playerController.hasJumped)
        {
            animator.SetBool("InAirJump", true);
            animator.SetBool("Walking", false);
            animator.SetBool("Sprinting", false);
        }*/
    }

    public void PlaySoundEvent(int index)
    {
        this.animationAudioSource.PlayOneShot(this.audioResources[index]);
    }
}
