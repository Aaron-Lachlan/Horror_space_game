using System.Collections;
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
        if(!pauseManager.isDocumentScreen)
        {
            interactionText.text = text;
            interactionText.gameObject.SetActive(true);
        }
    }

    public void DisableInteractionText()
    {
        interactionText.text = string.Empty;
        interactionText.gameObject.SetActive(false);
    }
}
