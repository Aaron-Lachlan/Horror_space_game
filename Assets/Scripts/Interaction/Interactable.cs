using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    Outline outline;
    public string message;

    public UnityEvent onInteraction;

    public bool isDocumentItem;
    public TextAsset loreText;
    public TextMeshProUGUI loreTextDisplay;

    [Header("Door Variables")]
    public bool isDoor;
    public Animator animator;
    public string openDoorBoolName;
    //public string closeTriggerName;

    void Start()
    {
        outline = GetComponent<Outline>();
        if(!isDoor)
            DisableOutline();
    }

    public void Interact()
    {
        onInteraction.Invoke();
        if(isDoor && animator.GetBool(openDoorBoolName) != true)
        {
            animator.SetBool(openDoorBoolName, true);
        }
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    public void EnableOutLine()
    {
        outline.enabled = true;
    }

    public void IsLoreItem()
    {
        loreTextDisplay.text = loreText.ToString();
        /*if(isComputerItem)
        {
            
        }*/
    }
}
