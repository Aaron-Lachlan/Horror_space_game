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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(startUnarmed)
        {
            playerIsUnarmed = true;
            GameObject FPArms = Instantiate(unarmed, FPHolder.transform.position, FPHolder.transform.rotation);
            FPArms.transform.SetParent(FPHolder.transform, true);
            startUnarmed = false;
        }
    }
}
