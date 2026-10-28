using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;

public class SlidingDoor : MonoBehaviour
{
    [SerializeField] bool isButtonDoor;

    [Header ("Door Collision stuff")]
    [SerializeField] bool collisionAdjustmentNeeded;
    public BoxCollider collider;

    [Header("Adjust the size as seen fit (Can go unsued if not needed)")]
    public Vector3 colliderSizeClosed;
    public Vector3 colliderSizeOpen;

    [Header("Move the centre as seen fit (Can go unsued if not needed)")]
    public Vector3 colliderCentreClosed;
    public Vector3 colliderCentreOpen;

    public bool isLargeDoor;
    public string doorMessage;

    //[Header("Do these things on Interaction")]
    //public UnityEvent onDoorInteraction;

    public Animator animator;
    public string playerTag;
    public string openCloseAnimBoolName;
    [SerializeField] string isOpenOrClosed;
    public AudioSource audioSource;
    public AudioSource audioSourceAlarm;
    public AudioClip openAudio;
    public AudioClip closeAudio;
    public AudioClip alarmAudio;
    [SerializeField] bool doorOpen;

    [SerializeField] int animLayer;
 
    public bool doorIsPlaying(Animator anim, string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(animLayer).IsName(stateName) &&
                anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }

    void Start()
    {
        if (!animator)
            animator = GetComponent<Animator>();
        if (!audioSource)
            audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == playerTag && !isButtonDoor)
        {
            animator.SetBool(openCloseAnimBoolName, true);
            audioSource.clip = openAudio;
            audioSource.Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == playerTag && !isButtonDoor)
        {
            animator.SetBool(openCloseAnimBoolName, false);
            audioSource.clip = closeAudio;
            audioSource.Play();
        }
    }

    public void DoorInteract()
    {
        //onDoorInteraction.Invoke();
        if(isButtonDoor && animator.GetBool(openCloseAnimBoolName) != true)
        {
            animator.SetBool(openCloseAnimBoolName, true);
            audioSource.clip = openAudio;
            audioSource.Play();
            if(collisionAdjustmentNeeded)
            {
                collider.size = colliderSizeOpen;
                collider.center = colliderCentreOpen;
            }
            /*if(isLargeDoor)
            {
            }*/
        }
        else
        {
            var isPlayingAnimation = animator.GetCurrentAnimatorStateInfo(0).IsName ("PistolRecoil");
            if(isButtonDoor && animator.GetBool(openCloseAnimBoolName) == true)
            {
                animator.SetBool(openCloseAnimBoolName, false);
                audioSource.clip = closeAudio;
                audioSource.Play();
                if(collisionAdjustmentNeeded)
            {
                collider.size = colliderSizeClosed;
                collider.center = colliderCentreClosed;
            }
            }
            /*if(isLargeDoor)
            {
                interactCollider.enabled = true;
                collisionCollider.enabled = false;
            }*/
        }
    }
}
