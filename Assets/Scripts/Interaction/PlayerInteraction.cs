using System;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] Camera playerCamera;
    public float playerReach;
    Interactable currentInteractable;
    SlidingDoor currentInteractableDoor;
    //Interactable currentInteractable;

    // Update is called once per frame
    void Update()
    {
        CheckInteractionRayCast();
        //CheckInteractionRadius();
        if(Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
        if(Input.GetKeyDown(KeyCode.E) && currentInteractableDoor != null)
        {
            currentInteractableDoor.DoorInteract();
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
            //If it is an interactable OTHER than a door
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

        if(Physics.Raycast(ray, out hit, playerReach))
        {
            if(hit.collider.tag == "DoorInteractable")
            {
                SlidingDoor newInteractableDoor = hit.collider.GetComponent<SlidingDoor>();

                //Unfinised! Can be completed if we want a outline to appear around doors that are interactable
                /*if(currentInteractableDoor && newInteractableDoor != currentInteractableDoor)
                {
                    currentInteractableDoor.DisableOutline();
                }*/
                if(newInteractableDoor.enabled && !newInteractableDoor.doorIsPlaying(newInteractableDoor.animator, "DoorOpening") && !newInteractableDoor.doorIsPlaying(newInteractableDoor.animator, "DoorClosing"))
                {
                    SetNewInteractableDoor(newInteractableDoor);
                }
                else
                {
                    DisableCurrentInteractableDoor();
                }
            }
            else //If not interactable
            {
                DisableCurrentInteractableDoor();
            }
        }
        else //If not in reach
        {
            DisableCurrentInteractableDoor();
        }

    }

    //Unknown
    /*void CheckInteractionRadius()
    {
        if(currentInteractable.isDoor)
    }*/

    void SetNewInteractable(Interactable newInteractable)
    {
        //print("Enable interact");
        currentInteractable = newInteractable;

        if(!currentInteractable.isDoor)
        {
            currentInteractable.EnableOutLine();
            HUDHelpText.instance.EnableInteractionText(currentInteractable.message);
        }
    }

    void SetNewInteractableDoor(SlidingDoor newInteractableDoor)
    {
        currentInteractableDoor = newInteractableDoor;

        //currentInteractableDoor.EnableOutLine();
        if(!newInteractableDoor.doorOpen)
        {
            print("Set open text");
            HUDHelpText.instance.EnableInteractionText(currentInteractableDoor.doorOpenMessage);
        }

        if(newInteractableDoor.doorOpen)
        {
            print("Set close text");
            HUDHelpText.instance.EnableInteractionText(currentInteractableDoor.doorCloseMessage);
        }
    }

    void DisableCurrentInteractable()
    {
        //print("Disable interact");
        HUDHelpText.instance.DisableInteractionText();
        if(currentInteractable && !currentInteractable.isDoor)
        {
            currentInteractable.DisableOutline();
        }
        if(currentInteractable)
        {
            currentInteractable = null;
        }
    }

    void DisableCurrentInteractableDoor()
    {
        //print("Disable interact");
        HUDHelpText.instance.DisableInteractionText();
        /*if(currentInteractableDoor && !currentInteractableDoor.isDoor)
        {
            currentInteractable.DisableOutline();
        }*/
        if(currentInteractableDoor)
        {
            currentInteractableDoor = null;
        }
    }
}
