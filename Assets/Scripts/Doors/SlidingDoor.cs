using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting;

public class SlidingDoor : MonoBehaviour
{
    [SerializeField] bool isButtonDoor;

    [Header ("Door Collision stuff")]
    public BoxCollider interactCollider;
    public bool isLargeDoor;

    [Header ("Door Messages")]
    public string doorOpenMessage;
    public string doorCloseMessage;

    //[Header("Do these things on Interaction")]
    //public UnityEvent onDoorInteraction;

    [Space]
    public Animator animator;
    public string playerTag;
    public string enemyTag;
    public string openCloseAnimBoolName;
    [SerializeField] string isOpenOrClosed;
    public AudioSource audioSource;
    public AudioSource audioSourceAlarm;
    public AudioClip openAudio;
    public AudioClip closeAudio;
    public AudioClip alarmAudio;
    public bool doorOpen;

    [SerializeField] int animLayer;

    [SerializeField] bool animPlaying;
 
    public bool doorIsPlaying(Animator anim, string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(animLayer).IsName(stateName) &&
                anim.GetCurrentAnimatorStateInfo(animLayer).normalizedTime < 1.0f)
        {
            animPlaying = true;
            return true;
        }
        else
        {
            animPlaying = false;
            return false;
        }
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

        /*if (other.tag == enemyTag && !isButtonDoor)
        {
            print("Enemy Open Door");
            animator.SetBool(openCloseAnimBoolName, true);
            audioSource.clip = openAudio;
            audioSource.Play();
        }*/
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == playerTag && !isButtonDoor)
        {
            animator.SetBool(openCloseAnimBoolName, false);
            audioSource.clip = closeAudio;
            audioSource.Play();
        }

        /*if (other.tag == enemyTag && !isButtonDoor)
        {
            animator.SetBool(openCloseAnimBoolName, false);
            audioSource.clip = closeAudio;
            audioSource.Play();
        }*/
    }

    public void DoorInteract()
    {
        print("Door Interacted With");
        //onDoorInteraction.Invoke();
        if(isButtonDoor && animator.GetBool(openCloseAnimBoolName) != true)
        {
            animator.SetBool(openCloseAnimBoolName, true);
            audioSource.clip = openAudio;
            audioSource.Play();
            doorOpen = true;
            /*if(collisionAdjustmentNeeded)
            {
                collider.size = colliderSizeOpen;
                collider.center = colliderCentreOpen;
            }*/
            /*if(isLargeDoor)
            {
            }*/
        }
        else
        {
            if(isButtonDoor && animator.GetBool(openCloseAnimBoolName) == true)
            {
                animator.SetBool(openCloseAnimBoolName, false);
                audioSource.clip = closeAudio;
                audioSource.Play();
                doorOpen = false;
                /*if(collisionAdjustmentNeeded)
                {
                    collider.size = colliderSizeClosed;
                    collider.center = colliderCentreClosed;
                }*/
            }
            /*if(isLargeDoor)
            {
                interactCollider.enabled = true;
                collisionCollider.enabled = false;
            }*/
        }
    }
}
