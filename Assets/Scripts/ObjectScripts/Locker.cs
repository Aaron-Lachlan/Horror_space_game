using UnityEngine;

public class Locker : MonoBehaviour
{
    [Header ("Messages")]
    public string openMessage;
    public string closeMessage;

    [Space]
    [SerializeField] Animator lockerAnimator;
    [SerializeField] string openCloseAnimBoolName;
    [SerializeField] AudioSource lockerAudioSource;
    [SerializeField] AudioClip openAudio;
    [SerializeField] AudioClip closeAudio;
    [SerializeField] bool playerInRange;
    public bool lockerOpen;
    [SerializeField] bool animPlaying;
    public bool doorIsPlaying(Animator anim, string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
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
        if (!lockerAnimator)
            lockerAnimator = GetComponent<Animator>();
        if (!lockerAudioSource)
            lockerAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(playerInRange)
        {
            if(Input.GetKey(KeyCode.E) && !lockerOpen && !animPlaying)
            {
                lockerOpen = true;
                lockerAnimator.SetBool(openCloseAnimBoolName, true);
                lockerAudioSource.clip = openAudio;
                lockerAudioSource.Play();
            }
            if(Input.GetKey(KeyCode.E) && lockerOpen && !animPlaying)
            {
                lockerOpen = false;
                lockerAnimator.SetBool(openCloseAnimBoolName, false);
                lockerAudioSource.clip = closeAudio;
                lockerAudioSource.Play();
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "PlayerCharacter")
        {
            playerInRange = true;
            if(!lockerOpen)
                HUDHelpText.instance.EnableInteractionText(openMessage);
            
            if(lockerOpen)
                HUDHelpText.instance.EnableInteractionText(closeMessage);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerCharacter")
        {
            playerInRange = false;
        }
    }
}
