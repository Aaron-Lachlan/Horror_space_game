using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseButtonsHandler : MonoBehaviour
{
    public bool resumedClicked = false;
    public bool resetLevelClicked = false;
    [SerializeField] PauseManager.PauseMenuButtons _buttonType;
    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    public void ResumeButton()
    {
        resumedClicked = true;
        Debug.Log("Resumed.");
    }

    public void MainMenuButton()
    {
        Debug.Log("Main Menu.");
        SceneManager.LoadScene(1);
    }

    public void ResetLevelButton()
    {
        resetLevelClicked = true;
        print("Reset called.");
    }

    public void QuitButton()
    {
        print("Quit");
        Application.Quit();
    }
}
