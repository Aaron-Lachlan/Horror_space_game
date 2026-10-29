using System.Collections.Generic;
using UnityEngine;

public class HighlightInteractable : MonoBehaviour
{

    [SerializeField] List<GameObject> interactables;

    void Update()
    {
        if(Input.GetKey(KeyCode.T))
        {
            print("Highlighting Objects");
            foreach(GameObject gameObject in interactables)
            {
                gameObject.GetComponent<Interactable>().EnableOutLine();
            }
        }
        if(Input.GetKeyUp(KeyCode.T))
        {
            print("Disable Highlights");
            foreach(GameObject gameObject in interactables)
            {
                gameObject.GetComponent<Interactable>().DisableOutline();
            }
        }
    }

    /*void OnTriggerEnter(Collider other)
    {
        interactables.Add(other.gameObject);
    }

    void OnTriggerExit(Collider other)
    {
        foreach(GameObject gameObject in interactables)
        {
            gameObject.GetComponent<Interactable>().DisableOutline();
        }
        interactables.Remove(other.gameObject);
    }*/
}
