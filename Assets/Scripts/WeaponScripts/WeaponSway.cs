using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float smooth;
    [SerializeField] private float mouseSwayMultiplierUpDown;
    [SerializeField] private float mouseSwayMultiplierLeftRight;
    [SerializeField] private float mouseSwayRotation;
    [SerializeField] private float keyboardSwayLeftRight;
    [SerializeField] private float keyboardMultiplierFowardBack;
    [SerializeField] private float keyboardSwayRotation;
    [SerializeField] PlayerInputHandler playerInputHandler;
    [SerializeField] bool debugMouse;

    void Awake()
    {
        playerInputHandler = GameObject.Find("PlayerInputHandler").GetComponent<PlayerInputHandler>();
    }

    void Update()
    {
        float keyboardX = playerInputHandler.MovementInput.x * keyboardSwayLeftRight;
        float keyboardZ = playerInputHandler.MovementInput.x * -keyboardSwayRotation;
        float keyboardY = playerInputHandler.MovementInput.y * keyboardMultiplierFowardBack;
        float mouseX = playerInputHandler.RotationInput.x * mouseSwayMultiplierLeftRight;
        float mouseZ = playerInputHandler.RotationInput.x * mouseSwayRotation;
        float mouseY = playerInputHandler.RotationInput.y * mouseSwayMultiplierUpDown;

        if (debugMouse)
        {
            Debug.Log(mouseZ);
            Debug.Log(mouseX);
            Debug.Log(mouseY);
        }

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX + -keyboardX, Vector3.up);
        Quaternion rotationZ = Quaternion.AngleAxis(mouseZ + -keyboardZ, Vector3.back);
        Vector3 moveY = new Vector3(0, 0, -keyboardY);

        Quaternion targetRotation = rotationX * rotationY * rotationZ;
        //Vector3 targetLocation = mouseY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
        transform.localPosition = Vector3.Lerp(transform.localPosition, moveY, smooth * Time.deltaTime);
    }
}