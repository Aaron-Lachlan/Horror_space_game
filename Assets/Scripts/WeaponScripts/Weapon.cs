using SaintsField.ComponentHeader;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Outline outline;
    HUDHelpText hudHelptext;
    WeaponManagement weaponManagement;
    WeaponAnimationHandler weaponAnimationHandler;

    [Header ("Bools")]
    public bool isDropped;
    public bool isEquiped;
    public bool firstDraw;

    bool playerWithinReach;

    /*[Header ("Model References")]
    public GameObject worldModel;
    public GameObject firstPersonModel;*/

    [Header ("Prompts")]
    public string weaponPickUpText;
    public string weaponSwapText;

    [Header ("Prefab References")]
    public GameObject pickedUpWeapon;
    public GameObject droppedWeapon;

    [Header("Equipped Ammo Ints")]
    public int loadedAmmo;
    public int reserveAmmo;

    [Header ("Dropped Ammo Ints")]
    [Tooltip ("If being placed in the world, set this value to load that much ammo in the gun when player picks it up. If being swapped, this value will be used to store how much ammo the player had loaded before dropping")]
    public int storedLoadedAmmo;
    [Tooltip ("If being placed in the world, set this value to give that much ammo reserve to the gun when player picks it up. If being swapped, this value will be used to store how much ammo the player had in reserve before dropping")]
    public int storedReserveAmmo;

    [Header ("Audio References")]
    [SerializeField] AudioSource fireAudioSource;
    [SerializeField] AudioClip fireSound;

    void Awake()
    {
        if(weaponManagement == null)
        {
            weaponManagement = GameObject.Find("WeaponManagement").GetComponent<WeaponManagement>();
        }
        if(hudHelptext == null)
        {
            hudHelptext = GameObject.Find("LevelManagement").GetComponent<HUDHelpText>();
        }
        outline = GetComponent<Outline>();
        outline.enabled = false;
        if(isDropped)
        {
            firstDraw = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && isDropped && playerWithinReach)
        {
            WeaponPickup();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerCharacter" && isDropped && weaponManagement.playerIsUnarmed || weaponManagement.weaponOneEquipped && !weaponManagement.weaponTwoEquipped)
        {
            PlayerInteraction.instance.playerOnWeapon = true;
            print("Player within reach");
            playerWithinReach = true;
            hudHelptext.interactionText.text = weaponPickUpText;
            hudHelptext.interactionText.gameObject.SetActive(true);
        }
        if (other.tag == "PlayerCharacter" && isDropped && weaponManagement.weaponOneEquipped && weaponManagement.weaponTwoEquipped)
        {
            PlayerInteraction.instance.playerOnWeapon = true;
            playerWithinReach = true;
            hudHelptext.interactionText.gameObject.SetActive(true);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerCharacter" && isDropped)
        {
            PlayerInteraction.instance.playerOnWeapon = false;
            HUDHelpText.instance.DisableInteractionText();
            playerWithinReach = false;
        }
    }

    public void WeaponPickup()
    {
        //Needs to happen no matter what
        playerWithinReach = false;
        PlayerInteraction.instance.playerOnWeapon = false;
        HUDHelpText.instance.DisableInteractionText();
        GameObject weapon = Instantiate(pickedUpWeapon, weaponManagement.FPHolder.transform.position, weaponManagement.GunContainer.transform.rotation);
        weapon.transform.SetParent(weaponManagement.GunContainer.transform, true);
        weaponAnimationHandler = GameObject.Find("PistolFPModel(Clone)").GetComponent<WeaponAnimationHandler>();
        weaponAnimationHandler.weaponAnimator.SetTrigger("FirstDraw");
        foreach(Transform child in weapon.transform)
        {
            var gameObject = child.gameObject;
            gameObject.layer = 6;
        }
        Destroy(GameObject.Find("FPArms(Clone)"));

        weapon.GetComponent<Weapon>().loadedAmmo = storedLoadedAmmo;
        weapon.GetComponent<Weapon>().reserveAmmo = storedReserveAmmo;
        weapon.GetComponent<Weapon>().droppedWeapon = droppedWeapon;
        weapon.GetComponent<Weapon>().enabled = true;

        //If the player is unarmed make the picked up weapon, weapon one
        if(weaponManagement.playerIsUnarmed)
        {
            weaponManagement.weaponOne = weapon;
            weaponManagement.weaponOneEquipped = true;
        }

        //If the player only has weapon one, make picked up weapon weapon two and equip it
        else if(weaponManagement.weaponOne && weaponManagement.weaponOneEquipped && !weaponManagement.weaponTwoEquipped && !weaponManagement.weaponTwo)
        {
            weaponManagement.weaponTwo = weapon;
            weaponManagement.weaponTwoEquipped = true;
            weaponManagement.weaponOneEquipped = false;
            weaponManagement.weaponOne.SetActive(false);
        }

        //If the player has two weapons, swap current weapon slot with new weapon
        else if(weaponManagement.weaponOne && weaponManagement.weaponTwo)
        {
            if(weaponManagement.weaponOneEquipped && !weaponManagement.weaponTwoEquipped)
            {
                weaponManagement.weaponOne = weapon;
            }
            if(!weaponManagement.weaponOneEquipped && weaponManagement.weaponTwoEquipped)
            {
                weaponManagement.weaponTwo = weapon;
            }
        }
        weaponManagement.playerIsUnarmed = false;
        print("Weapon Picked Up");
        Destroy(this.gameObject);
    }
}
