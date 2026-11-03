using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TerminalManager : MonoBehaviour
{
    [Header ("Prefab references")]
    public GameObject directoryLine;
    public GameObject responseLine;

    [Space]
    public TMP_InputField terminalInput;
    public GameObject userInputLine;
    public ScrollRect sr;
    public GameObject msgList;
    [SerializeField] AudioClip[] keySFX;
    //public float soundVolume;

    [Space]
    TerminalInterpreter interpreter;
    int totalLines;
    GameObject topline;
    AudioSource audioSource;
    int LastPlayedClip;
    int randomIndex;

    void Awake()
    {
        RefocusInputField();
        interpreter = GetComponent<TerminalInterpreter>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnGUI()
    {
        if(terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            //Grab players text and store
            string userInput = terminalInput.text;

            //Clear Input Field
            ClearInputField();

            //Instantiate game object with a directory prefix
            AddDirectoryLine(userInput);

            //Add the interpretation lines
            int lines = AddInterpreterLines(interpreter.Interpret(userInput));

            //Scroll to the bottom of the scroll rect
            ScrollToBottom(lines);

            /*GameObject[] allLineOjbects = GameObject.FindGameObjectsWithTag("TerminalText");
            totalLines = 0;
            foreach(GameObject line in allLineOjbects) 
            {
                totalLines++;
            }
            print(totalLines);

            if(totalLines > 24)
            {
                Destroy(topline);
            }*/

            //Move user input line to the bottom
            userInputLine.transform.SetAsLastSibling();

            //Refocus the input
            RefocusInputField();
        }
    }

    void ClearInputField()
    {
        terminalInput.text = "";
    }

    void AddDirectoryLine(string userInput)
    {
        //Resizing the command line container
        Vector2 msgListSize = msgList.GetComponent<RectTransform>().sizeDelta;
        msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(msgListSize.x, msgListSize.y + 35.0f);

        //Instantiate directory line
        GameObject msg = Instantiate(directoryLine, msgList.transform);

        //Set child index
        msg.transform.SetSiblingIndex(msgList.transform.childCount - 1);

        //Set the text of this new game object
        msg.GetComponentsInChildren<TextMeshProUGUI>()[1].text = userInput;

        topline = msgList.transform.GetChild(0).gameObject;
        //print(topline);

    }

    void RefocusInputField()
    {
        //Refocus the input
        terminalInput.ActivateInputField();
        terminalInput.Select();
    }

    int AddInterpreterLines(List<string> interpretation)
    {
        for(int i = 0; i < interpretation.Count; i++)
        {
            //Instatiate response line
            GameObject res = Instantiate(responseLine, msgList.transform);

            //Set to last message in the terminal
            res.transform.SetAsLastSibling();

            //Get size of message list and resize
            Vector2 listSize = msgList.GetComponent<RectTransform>().sizeDelta;
            msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(listSize.x, listSize.y + 35.0f);

            //Set the text of the response line to the interpreter string
            res.GetComponentInChildren<TextMeshProUGUI>().text = interpretation[i];

        }

        return interpretation.Count;
    }

    void ScrollToBottom(int lines)
    {
        if(lines > 2)
        {
            sr.velocity = new Vector2(0, 450);
        }
        else
        {
            sr.verticalNormalizedPosition = 0;
        }
    }

    public void PlayKeySFX()
    {
        if(Input.anyKeyDown)
        {
            while (randomIndex == LastPlayedClip)
            {
                randomIndex = UnityEngine.Random.Range(0, keySFX.Length);
            }
                
            audioSource.PlayOneShot(keySFX[randomIndex]);
            LastPlayedClip = randomIndex;
        }

        return;
    }
}
