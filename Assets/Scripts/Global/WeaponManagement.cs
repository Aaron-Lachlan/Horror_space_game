using SaintsField.ComponentHeader;
using UnityEngine;

public class WeaponManagement : MonoBehaviour
{
    [SerializeField] bool startUnarmed;

    [Header ("Weapon Slots")]
    public GameObject primaryWeapon;
    public GameObject secondaryWeapon;

    [Header ("Weapons")]
    public GameObject unarmed;

    [Header ("FP Container")]
    public GameObject FPHolder;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(startUnarmed)
        {
            GameObject FPArms = Instantiate(unarmed, FPHolder.transform.position, unarmed.transform.rotation);
            FPArms.transform.SetParent(FPHolder.transform, true);
        }
    }
}
