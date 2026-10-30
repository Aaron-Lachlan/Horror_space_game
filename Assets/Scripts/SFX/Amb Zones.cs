using UnityEngine;

public class AmbZones : Amb
{
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
            
            AmbManager.ChangeAmbRequest(currentSettings);
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            AmbManager.ChangeAmbRequest(AmbTypes.Globle);
        }
    }
}
