using UnityEngine;

public class AmbienceSound : MonoBehaviour
{
    [SerializeField] AmbManager ambManager;
    [SerializeField] AmbZones requestAmbChange;
    //[SerializeField] Amb currentSettings;
    public GameObject[] allAmbienceVolumes;
    public GameObject nearestAmbVol;

    public GameObject ambienceTracker;

    [SerializeField] float distanceToCurrent;
    [SerializeField] float distanceToNearest;

    [SerializeField] Collider ambienceArea;
    //[SerializeField] AudioSource audioSource;
    

    void Awake()
    {
        if(ambienceTracker == null)
        {
            ambienceTracker = GameObject.Find("AmbienceTracking");
        }

        allAmbienceVolumes = GameObject.FindGameObjectsWithTag("AmbienceVolume");
    }

    void Update()
    {
        nearestAmbVol = allAmbienceVolumes[0];

        distanceToNearest = Vector3.Distance(ambienceTracker.transform.position, nearestAmbVol.transform.position);

        //Loop through list and find closest ambience volume
        for(int i = 1; i < allAmbienceVolumes.Length; i++)
        {
            distanceToCurrent = Vector3.Distance(ambienceTracker.transform.position, allAmbienceVolumes[i].transform.position);

            if (distanceToCurrent < distanceToNearest)
            {
                nearestAmbVol = allAmbienceVolumes[i];
                distanceToNearest = distanceToCurrent;
            }
        }

        ambienceArea = nearestAmbVol.GetComponent<Collider>();
        requestAmbChange = nearestAmbVol.GetComponent<AmbZones>();
        requestAmbChange.RequestAmbChange();

        //Position Audio Source to nearest point to player
        //Vector3 closestPoint = ambienceArea.ClosestPoint(ambienceTracker.transform.position);
        //ambManager.ActiveAS.transform.position = closestPoint;
    }
}
