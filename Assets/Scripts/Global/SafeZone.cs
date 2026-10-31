using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public List<AlienAI> alienAI;

    void Start()
    {
        FindAllAliensInScene();
    }

    void FindAllAliensInScene()
    {
        AlienAI[] foundAliens = FindObjectsByType<AlienAI>();

        alienAI.Clear();

        alienAI.AddRange(foundAliens);
        print(alienAI);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            //print("Player in safe zone");
            foreach (var alien in alienAI)
            {
                alien.playerIsSafe = true;
                //print("Player Hidden");
            }
        }
    }
}
