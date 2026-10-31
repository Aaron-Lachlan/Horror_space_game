using System.Collections;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using UnityEditor;
using UnityEngine;

public class HUDHelpText : MonoBehaviour
{
    [SerializeField] PauseManager pauseManager;
    public static HUDHelpText instance;
    //[SerializeField] HelpTriggers helpTriggers;
    public string setText;
    public TextMeshProUGUI helpText;
    public TextMeshProUGUI interactionText;
    [SerializeField] GameObject helpTextObject;
    [SerializeField] GameObject interactionTextObject;
    //[SerializeField] GameObject interactTextObject;
    //public float fadeTime;
    //[Tooltip("Higher = Faster")]

    //Make Appear
    public bool textAppear;

    //Made Disappear
    public bool textDisappear;

    void Start()
    {
        instance = this;
        helpText.text = string.Empty;
        interactionText.text = string.Empty;
        helpTextObject.SetActive(false);
        interactionText.gameObject.SetActive(false);
    }

    void Update()
    {
        if(textAppear == true)
        {
            textAppear = false;
            StartCoroutine(HelptextAppear());
        }
        if(textDisappear == true)
        {
            textDisappear = false;
            StartCoroutine(HelpTextDisappear());
        }
    }

    IEnumerator HelptextAppear()
    {
        helpText.text = setText;
        helpTextObject.SetActive(true);

        print("Help text appeared");
        yield return new WaitForSeconds(5f);
        StartCoroutine(HelpTextDisappear());

        yield return null;
    }

    IEnumerator HelpTextDisappear()
    {
        helpTextObject.SetActive(false);
        print("Help text faded out");
        helpText.text = string.Empty;

        yield return null;
    }

    public void EnableInteractionText(string text)
    {
        print("Help Text Displayed");
        //!When pause system implemented. Reenable this. This checks if a menu is being displayed currently before enabling interaction text (Probably redundant but I added it for a reason I think)
        /*if(!pauseManager.isDocumentScreen)
        {
            interactionText.text = text;
            interactionText.gameObject.SetActive(true);
        }*/
        interactionText.text = text;
        interactionTextObject.SetActive(true);
    }

    public void DisableInteractionText()
    {
        interactionText.text = string.Empty;
        interactionTextObject.SetActive(false);
    }
}
