using UnityEngine;

public class AmbZones : Amb
{
    //[SerializeField] string playerTag;
    public AmbTypes currentSettings;

    private AmbManager AmbManager;

    private void Start()
    {
        AmbManager = FindAnyObjectByType<AmbManager>();
    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("PlayerCharacter"))
        {
            print("Change Amb");
            AmbManager.ChangeAmbRequest(currentSettings);
        }

    }
    /*private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerCharacter"))
        {

            AmbManager.ChangeAmbRequest(AmbTypes.Globle);
        }
    }*/

    public void RequestAmbChange()
    {
        AmbManager.ChangeAmbRequest(currentSettings);
    }
}
