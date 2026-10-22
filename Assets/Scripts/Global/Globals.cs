using System.Collections;
using Unity.VisualScripting;
//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Globals : MonoBehaviour
{
    [Header("Global Ints")]
    public int firstLaunch;

    //[Header("Item Bools")] If items need to be tracked. Can be removed if new system is being implemented
    //public bool playerHasAndroidFloppyDisk = false;
    //public bool playerHasSecurityKeyCard = false;

    //[Header("Story Bools")] Same as above
    //public bool playerUsedAndroidFloppyDisk = false;
    //public bool playerReadAndroidFloppyDisk = false;

    [Header("Global Bools")]
    public bool oneSecondSlept = false;
    public  bool enableMovement = true;
    public  bool toggleCrouch = false;
    public bool jumpEnable;
    public bool flashlightEnabled = false;

    [Header("Cinematic Bools")] //Used to handle cinematics. Can be removed if new system is being implemented or let Sam handle cinematics
    public bool isCinematic = false;
    public bool cinematicIsSkipable = false;
    public bool cinematicSkipped = false;

    [Header("Global SFX")]
    public AudioClip radioInClip;
    public AudioClip radioLoopClip;
    public AudioClip radioOutClip;
    public AudioSource globalSFXSource;

    [Header("Camera Variables")]
    private int settingsFOV;
    public int playerFOV;
    public Camera playerCamera;
    public Camera cinematicCamera;
    int defaultFOV = 90;

    void Start()
    {
        //radioInClip = Resources.Load <AudioClip> ("Audio/SFX/Global/RadioStart");
        //radioLoopClip = Resources.Load <AudioClip> ("Audio/SFX/Global/RadioLoop");
        //radioOutClip = Resources.Load <AudioClip> ("Audio/SFX/Global/RadioEnd");

        DontDestroyOnLoad(gameObject);
        firstLaunch = PlayerPrefs.GetInt("firstRun", 0);

        if(firstLaunch == 0)
        {
            print("First Launch, Setting Fov");
            playerFOV = PlayerPrefs.GetInt("chosenFov", defaultFOV);
            settingsFOV = defaultFOV;
        }
        //SceneManager.LoadScene(1);
    }

    public IEnumerator UpdatePlayerFOV()
    {
        playerCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();

         yield return new WaitForSeconds(0.1f);
        if(playerCamera != null)
            print("Camera found");

        playerCamera.fieldOfView = playerFOV;
    }
    
    public IEnumerator OneSecondSleep()
    {
        yield return new WaitForSeconds(1);
        oneSecondSlept = true;
        Debug.Log("Complete");
        yield return null;
    }

    //Set camera to cinematic camera. Use if not using player camera for FP cutscenes.
    public IEnumerator CinematicCameraControl()
    {
        cinematicCamera = GameObject.Find("CinematicCamera").GetComponent<Camera>();
        playerCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();

        yield return new WaitForSeconds(0.1f);
        if(cinematicCamera != null && playerCamera != null)
            print("Cameras found");

        cinematicCamera.enabled = true;
        playerCamera.enabled = false;
        print("Cinematic camera enabled");

        yield return null;
    }

    //Set camera to player camera. Use if returning control to player.
    public IEnumerator PlayerCameraControl()
    {
        cinematicCamera = GameObject.Find("CinematicCamera").GetComponent<Camera>();
        playerCamera = GameObject.Find("PlayerCamera").GetComponent<Camera>();

        yield return new WaitForSeconds(0.1f);
        if(cinematicCamera != null && playerCamera != null)
            print("Cameras found");

        playerCamera.enabled = true;
        cinematicCamera.enabled = false;
        print("Player camera enabled");

        yield return null;
    }
}
