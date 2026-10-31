using System;
using UnityEngine;

public struct CameraInput
{
    public Vector2 look;
}

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] float lookSensitivity = 0.1f;
    [SerializeField] float maxLookUp = 89f;
    [SerializeField] float maxLookDown;

    Vector3 _eulerAngles;

    public void Initialize(Transform target)
    {
        transform.position = target.position;
        transform.eulerAngles = _eulerAngles = target.eulerAngles;
    }

    public void UpdateRotation(CameraInput input)
    {
        _eulerAngles += new Vector3(-input.look.y, input.look.x) * lookSensitivity;
        _eulerAngles.x = Mathf.Clamp(_eulerAngles.x, -maxLookUp, -maxLookDown);
        transform.eulerAngles = _eulerAngles;
    }

    public void UpdatePosition(Transform target)
    {
        transform.position = target.position;
    }
}
