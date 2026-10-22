using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    [Header("Input Reference")]
    [SerializeField] PlayerInputHandler playerInputHandler;
    [SerializeField] PauseButtonsHandler pauseResumeButton;
    [SerializeField] PauseButtonsHandler loreResumeButton;
    [SerializeField] PauseButtonsHandler documentResumeButton;
    [SerializeField] Globals globals;
    [SerializeField] FadeInOut fadeInOut;
    [SerializeField] List<ItemPickUpHandler> loreItems;
    [SerializeField] typewriterUI_v2 typeWriterEffect;

    [Header("Game Object References")]
    //[SerializeField] GameObject globalsObject;
    [SerializeField] GameObject pauseMenuCavas;
    [SerializeField] GameObject loreScreenCanvas;
    [SerializeField] GameObject documentScreenCanvas;
    [SerializeField] GameObject cinematicBoarderCanvas;
    [SerializeField] GameObject cinematicSkipFadeCanvas;
    [SerializeField] GameObject loreTextObject;

    [Header("Text References")]
    [SerializeField] TextMeshProUGUI loreEntryText;

    //[Header("Strings")]
    //[SerializeField] string ;

    [Header("Floats")]
    [SerializeField] float listenerFadeTime;

    [Header("Ints")]
    public int loreItemIndex;

    public enum PauseMenuButtons { resume, options, mainMenu };
    
    public bool isPaused;
    public bool isLoreScreen;
    public bool isDocumentScreen;
    public bool loadMenu;

    void Awake()
    {
        
    }

    private void Update()
    {
        if(globals == null)
        {
            globals = GameObject.Find("GlobalObject").GetComponent<Globals>();
        }

        if(!globals.isCinematic)
        {
            if (Input.GetKeyDown ("escape") || pauseResumeButton.resumedClicked || loreResumeButton.resumedClicked || documentResumeButton.resumedClicked)
            {
                if(!isPaused)
                {
                    PauseGame();
                }
                else
                {
                    if(isLoreScreen)
                    {
                        UnpauseGameLoreScreen();
                    }
                    if(isDocumentScreen)
                    {
                        UnpauseGameDocumentScreen();
                    }
                    else
                    {
                        UnpauseGame();
                    }
                    pauseResumeButton.resumedClicked = false;
                    loreResumeButton.resumedClicked = false;
                    documentResumeButton.resumedClicked = false;
                }
            }
        }
        if(globals.isCinematic && globals.cinematicIsSkipable && Input.GetKeyDown ("escape"))
        {
            //cinematicSkipFadeInOut.skipFade = true;
            fadeInOut.skipFade = true;
            globals.cinematicSkipped = true;
            cinematicSkipFadeCanvas.SetActive(true);
            Debug.Log("Skipped.");
            fadeInOut.FadeIn();
            StartCoroutine(AudioFadeInOut.FadeOutListener(listenerFadeTime));
        }
    }

    public void PauseGame()
    {
        pauseMenuCavas.SetActive(true);
        cinematicBoarderCanvas.SetActive(false);
        isPaused = true;
        OpenPauseMenu();
        globals.enableMovement = false;
        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void PauseGameLoreScreen()
    {
        Debug.Log(loreItems[loreItemIndex].loreEntryTextAsset.ToString());
        loreEntryText.text = loreItems[loreItemIndex].loreEntryTextAsset.ToString();
        isPaused = true;
        isLoreScreen = true;
        StartCoroutine(LoreScreenTextSet());
        loreScreenCanvas.SetActive(true);
        OpenPauseMenu();
        globals.enableMovement = false;
        //Time.timeScale = 0f;
        //AudioListener.pause = true;
    }

    public void PauseGameDocumentScreen()
    {
        isPaused = true;
        isDocumentScreen = true;
        HUDHelpText.instance.DisableInteractionText();
        documentScreenCanvas.SetActive(true);
        OpenPauseMenu();
        globals.enableMovement = false;
        //Time.timeScale = 0f;
        //AudioListener.pause = true;
    }

    public void UnpauseGame()
    {
        pauseMenuCavas.SetActive(false);
        if(globals.isCinematic)
        {
            cinematicBoarderCanvas.SetActive(true);
        }
        isPaused = false;
        ClosePauseMenu();
        globals.enableMovement = true;
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
    public void UnpauseGameLoreScreen()
    {
        loreTextObject.SetActive(false);
        loreScreenCanvas.SetActive(false);
        isPaused = false;
        isLoreScreen = false;
        ClosePauseMenu();
        globals.enableMovement = true;
        Time.timeScale = 1f;
        //AudioListener.pause = false;
    }

    public void UnpauseGameDocumentScreen()
    {
        //loreTextObject.SetActive(false);
        documentScreenCanvas.SetActive(false);
        isPaused = false;
        isDocumentScreen = false;
        ClosePauseMenu();
        globals.enableMovement = true;
        Time.timeScale = 1f;
        //AudioListener.pause = false;
    }

    void OpenPauseMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if(loadMenu)
            //SceneManager.LoadScene("Menu_A");
            Debug.Log("Paused!");

    }

    /*void OpenLoreScreen()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if(loadMenu)
            //SceneManager.LoadScene("Menu_A");
            Debug.Log("Paused!");

    }*/

    void ClosePauseMenu()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /*void CloseLoreScreen()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }*/

    IEnumerator LoreScreenTextSet()
    {
        yield return new WaitForSeconds(1f);
        //typeWriterEffect[loreItemIndex].StartTypewriter();
        loreTextObject.SetActive(true);
        yield return null;
    }
}
