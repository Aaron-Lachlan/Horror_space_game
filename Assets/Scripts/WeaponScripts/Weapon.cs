using UnityEditor;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public bool isDropped;
    public bool isEquiped;

    /*[Header ("Model References")]
    public GameObject worldModel;
    public GameObject firstPersonModel;*/

    public string weaponPickUpText;

    public Animator weaponAnimator;

    // Update is called once per frame
    void Update()
    {
        
    }
}
