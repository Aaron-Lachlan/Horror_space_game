using SaintsField.ComponentHeader;
using UnityEngine;

public class WeaponManagement : MonoBehaviour
{
    [SerializeField] bool startUnarmed;
    public bool weaponOneEquipped;
    public bool weaponTwoEquipped;
    public bool playerIsUnarmed;

    [Header ("Weapon Slots")]
    public GameObject weaponOne;
    public GameObject weaponTwo;

    [Header ("Unarmed")]
    public GameObject unarmed;

    [Header ("FP Containers")]
    public GameObject FPHolder;
    public GameObject GunContainer;

    PlayerInteraction playerInteraction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        playerInteraction = GameObject.Find("NewPlayer").GetComponent<PlayerInteraction>();
        FPHolder = GameObject.Find("FPHolder");
        GunContainer = GameObject.Find("GunContainer");
        if(startUnarmed)
        {
            playerIsUnarmed = true;
            GameObject FPArms = Instantiate(unarmed, FPHolder.transform.position, FPHolder.transform.rotation);
            FPArms.transform.SetParent(FPHolder.transform, true);
            playerInteraction.playerFPAnimator = GameObject.Find("FPArms(Clone)").GetComponent<Animator>();
            startUnarmed = false;
        }
    }
}
