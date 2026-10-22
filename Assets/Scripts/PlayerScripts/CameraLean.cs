using UnityEngine;

public class CameraLean : MonoBehaviour
{
    [SerializeField] float attackDamping;
    [SerializeField] float decayDamping;
    [SerializeField] float strength;

    Vector3 _dampedAcceleration;
    Vector3 _dampedAccelerationVel;

    public void Initialize()
    {
        print("Lean Init");
    }

    public void UpdateLean(float deltaTime, Vector3 acceleration, Vector3 up)
    {
        //print("Lean Update");
        var planarAcceleration = Vector3.ProjectOnPlane(acceleration, up);
        var damping = planarAcceleration.magnitude > _dampedAcceleration.magnitude
            ? attackDamping
            : decayDamping;

        _dampedAcceleration = Vector3.SmoothDamp
        (
            current: _dampedAcceleration,
            target: planarAcceleration,
            currentVelocity: ref _dampedAccelerationVel,
            smoothTime: damping,
            maxSpeed: float.PositiveInfinity,
            deltaTime: deltaTime
        );

        var leanAxis = Vector3.Cross(_dampedAcceleration.normalized, up).normalized;
        //Debug.Log(acceleration);

        //Reset Rot
        transform.localRotation = Quaternion.identity;

        transform.rotation = Quaternion.AngleAxis(_dampedAcceleration.magnitude * strength, leanAxis) * transform.rotation;
        Debug.DrawRay(transform.position, acceleration, Color.red);
    }
}
