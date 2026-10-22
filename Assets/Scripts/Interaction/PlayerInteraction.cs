using System;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    public float playerReach;
    Interactable currentInteractable;

    // Update is called once per frame
    void Update()
    {
        CheckInteractionRayCast();
        //CheckInteractionRadius();
        if(Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    void CheckInteractionRayCast()
    {
        //Vector3 forward = transform.TransformDirection(Vector3.forward) * playerReach;
        RaycastHit hit;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * playerReach);

        if(Physics.Raycast(ray, out hit, playerReach))
        {
            if(hit.collider.tag == "Interactable")
            {
                Interactable newInteractable = hit.collider.GetComponent<Interactable>();

                if(currentInteractable && newInteractable != currentInteractable)
                {
                    currentInteractable.DisableOutline();
                }
                if(newInteractable.enabled)
                {
                    SetNewInteractable(newInteractable);
                }
                else
                {
                    DisableCurrentInteractable();
                }
            }
            else //If not interactable
            {
                DisableCurrentInteractable();
            }
        }
        else //If not in reach
        {
            DisableCurrentInteractable();
        }

    }

    /*void CheckInteractionRadius()
    {
        if(currentInteractable.isDoor)
    }*/

    void SetNewInteractable(Interactable newInteractable)
    {
        //print("Enable interact");
        currentInteractable = newInteractable;

        if(!currentInteractable.isDoor)
            currentInteractable.EnableOutLine();

        HUDHelpText.instance.EnableInteractionText(currentInteractable.message);
    }

    void DisableCurrentInteractable()
    {
        //print("Disable interact");
        //HUDHelpText.instance.DisableInteractionText();
        if(currentInteractable && !currentInteractable.isDoor)
        {
            currentInteractable.DisableOutline();
        }
        if(currentInteractable)
        {
            currentInteractable = null;
        }
    }
}
