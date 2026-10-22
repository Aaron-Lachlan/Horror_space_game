using TMPro;
using UnityEngine;

public class ItemPickUpHandler : MonoBehaviour
{
    [Header("Script References")]
    [SerializeField] Globals globals;
    [SerializeField] PlayerInputHandler playerInputHandler;

    [Header("Game Object References")]
    [SerializeField] GameObject globalsObject;
    [SerializeField] GameObject pickUpItemTextObject;
    //[SerializeField] GameObject pickUpItem;

    [Header("Text References")]
    [SerializeField] string Prefix = "Hold E To Pick Up ";
    public TextAsset loreEntryTextAsset;
    public string loreEntry;
    [SerializeField] TextMeshProUGUI pickUpItemText;

    //[Header("Lore Item Index")]
    //public int loreItemIndex;

    [Header("Bools")]
    bool touchingItemCollider = false;

    [Header("Name to Display")]
    [SerializeField] string objectName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //LoreEntryTextAsset = LoreEntry;
        //LoreEntry = LoreEntryTextAsset;
    }

    void Update()
    {
        if(globals == null)
        {
            globalsObject = GameObject.Find("GlobalObject");
            globals = globalsObject.GetComponent<Globals>();
        }

        if(touchingItemCollider)
        {
            pickUpItemTextObject.SetActive(true);
        }
        else
        {
            touchingItemCollider = false;
            pickUpItemTextObject.SetActive(false);
        }

        if(playerInputHandler.InteractTriggered && touchingItemCollider)
        {
            //DO STUFF HERE
            /*globals.playerHasAndroidFloppyDisk = true;
            StartCoroutine(globals.OneSecondSleep());
            print("Player is no longer toching " + objectName);
            touchingItemCollider = false;
            pickUpItemTextObject.SetActive(false);
            Destroy(gameObject);*/

        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            //print("Player is toching " + objectName);
            pickUpItemText.text = Prefix + objectName;
            touchingItemCollider = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            print("Player is no longer toching " + objectName);
            touchingItemCollider = false;
        }
    }
}
