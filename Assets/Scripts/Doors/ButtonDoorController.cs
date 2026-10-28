using System.Collections;
using UnityEngine;

public class ButtonDoorController : MonoBehaviour
{
    public bool buttonNeedsKeyCard = false;
    [SerializeField] SlidingDoor doorA;
    [SerializeField] SlidingDoor doorB;
    [SerializeField] SlidingDoor doorC;
    [SerializeField] Animator weaponAnimator;
    [SerializeField] ButtonDoorRaycast buttonDoorRaycast;
    public Material doorkeypadRed;
    public Material doorkeypadGreen;
    [SerializeField] GameObject doorObject;
    [SerializeField] bool doorOpen;
    public string openCloseAnimBoolName;

    [SerializeField] private float waitTime;
    [SerializeField] private bool pauseInteraction = false;
    IEnumerator PauseDoorInteraction()
    {
        pauseInteraction = true;
        yield return new WaitForSeconds(waitTime);
        pauseInteraction = false;
    }

    public void PlayAnimation()
    {
        if (!doorOpen && !pauseInteraction)
        {
            doorObject.GetComponent<Renderer>().material = doorkeypadGreen;
            weaponAnimator.SetTrigger("button interact");
            doorA.animator.SetBool(openCloseAnimBoolName, true);
            doorOpen = true;
            doorA.audioSource.clip = doorA.openAudio;
            doorB.audioSource.clip = doorB.openAudio;
            doorC.audioSource.clip = doorC.openAudio;
            doorA.audioSource.Play();
            doorB.audioSource.Play();
            doorC.audioSource.Play();
            if(doorA.isLargeDoor)
            {
                doorA.audioSourceAlarm.clip = doorA.alarmAudio;
                doorB.audioSourceAlarm.clip = doorB.alarmAudio;
                doorB.audioSourceAlarm.Play();
                doorA.audioSourceAlarm.Play();
            }
            StartCoroutine(PauseDoorInteraction());
        }
        
        else if(doorOpen && !pauseInteraction)
        {
            doorObject.GetComponent<Renderer>().material = doorkeypadRed;
            weaponAnimator.SetTrigger("button interact");
            doorA.animator.SetBool(openCloseAnimBoolName, false);
            doorOpen = false;
            doorA.audioSource.clip = doorA.closeAudio;
            doorA.audioSource.Play();
            if(doorA.isLargeDoor)
            {
                doorA.audioSourceAlarm.clip = doorA.alarmAudio;
            }
            StartCoroutine(PauseDoorInteraction());
        }
    }
}
