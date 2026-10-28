using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDoorRaycast : MonoBehaviour
{
    [SerializeField] private float rayLength = 5.0f;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private Globals globalBools;
    [SerializeField] private string excludeLayerName = null;

    private ButtonDoorController raycastedObj;
    private SlidingDoor slidingDoor;

    [SerializeField] private PlayerInputHandler playerInputHandler;
    public Image crosshair = null;
    public GameObject DoorInteractText = null;
    [SerializeField] private GameObject crosshairObject = null;
    private bool isCrosshairActive;
    private bool doOnce;
    public string openCloseAnimBoolName = "DoorOpen";

    private const string interactableTag = "DoorButton";

    private void Update()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        int mask = 1 << LayerMask.NameToLayer(excludeLayerName) | layerMaskInteract.value;

        if (Physics.Raycast(transform.position, fwd, out hit, rayLength, mask))
        {
            if (hit.collider.CompareTag(interactableTag))
            {
                if (!doOnce)
                {
                    raycastedObj = hit.collider.gameObject.GetComponent<ButtonDoorController>();
                    CrosshairChange(true);
                }

                isCrosshairActive = true;
                doOnce = true;

                if (playerInputHandler.InteractTriggered)
                {
                    //raycastedObj.
                    //slidingDoor.animator.SetBool(openCloseAnimBoolName, true);
                    raycastedObj.PlayAnimation();
                }
            }
        }

        else
        {
            if (isCrosshairActive)
            {
                CrosshairChange(false);
                doOnce = false;
            }
        }
    }
    
    void CrosshairChange(bool on)
    {
        if (on && !doOnce)
        {
            crosshairObject.SetActive(true);
            DoorInteractText.SetActive(true);
            crosshair.color = Color.yellow;
        }
        else
        {
            crosshairObject.SetActive(false);
            DoorInteractText.SetActive(false);
            crosshair.color = Color.white;
            isCrosshairActive = false;
        }
    }
}
