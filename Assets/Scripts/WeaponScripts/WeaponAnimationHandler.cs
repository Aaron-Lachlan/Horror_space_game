using Unity.VisualScripting;
using UnityEngine;

public class WeaponAnimationHandler : MonoBehaviour
{
    [SerializeField] Animator animator;
    //[SerializeField] PlayerController playerController;
    [SerializeField] PlayerCharacter playerCharacter;
    [SerializeField] bool isArms;

    void Awake()
    {
        /*playerCharacter = GameObject.FindWithTag("Player").GetComponent<PlayerCharacter>();
        if(playerCharacter != null)
        {
            print("Player Controller Found");
        }*/
        //animator.SetBool("Sprinting", false);
    }

    void Update()
    {
        //if(state.stance is Stance.Stand)
        var state = playerCharacter.GetState();
        //Debug.Log(state.stance);
        //var grounded = playerCharacter.GetGroundStatus();

        //Movement Animaions
        if(state.grounded)
        {
            //print(state.grounded);
            if(!isArms)
            {
                animator.SetBool("InAirJump", false);
                
                if(playerCharacter.isWalking)
                {
                    animator.SetBool("Walking", true);
                }
                else
                {
                    //print("Walk off");
                    animator.SetBool("Walking", false);
                }

                if(state.stance is Stance.Crouch)
                {
                    animator.SetBool("Crouching", true);
                    if(state.stance is Stance.Crouch && playerCharacter._requestedMovement.magnitude > 0)
                    {
                        animator.SetBool("Walking", true);
                    }
                    else
                    {
                        animator.SetBool("Walking", false);
                    }
                }
                else
                {
                    animator.SetBool("Crouching", false);
                }
            }

            if(playerCharacter.isSprinting)
            {
                //print("Sprint Anim");
                animator.SetBool("Sprinting", true);
            }
            else
            {
                animator.SetBool("Sprinting", false);
            }
        }
        else
        {
            animator.SetBool("Sprinting", false);
            if(!isArms)
            {
                animator.SetBool("Walking", false);
            }
        }

        /*if(!playerController.characterController.isGrounded && playerController.hasJumped)
        {
            animator.SetBool("InAirJump", true);
            animator.SetBool("Walking", false);
            animator.SetBool("Sprinting", false);
        }*/
    }
}
