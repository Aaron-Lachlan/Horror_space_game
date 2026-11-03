using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
public class PlayerDrag : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Camera the trace is fired from. Uses Camera.main if empty.")]
    public Camera playerCamera;
    [Tooltip("Player's own collider (CharacterController counts). Held objects won't collide with it. Auto-found if empty.")]
    public Collider playerCollider;

    [Header("Trace")]
    public float reach = 3f;
    [Tooltip("Layers the trace can hit.")]
    public LayerMask traceMask = ~0;

    [Header("Input (legacy Input Manager)")]
    public KeyCode grabKey = KeyCode.E;

    [Header("Dragging")]
    [Tooltip("How strongly the object is pulled toward the hold point.")]
    public float followStrength = 12f;
    [Tooltip("Top speed the object can be dragged at.")]
    public float maxDragSpeed = 8f;
    [Tooltip("Closest/farthest the object is held from the camera.")]
    public float minHoldDistance = 1.2f;
    public float maxHoldDistance = 3f;
    [Tooltip("If the object gets stuck this far from where it should be, it's dropped.")]
    public float breakDistance = 2.5f;
    [Tooltip("Stops the object spinning wildly while held (0 = no damping, 1 = no spin).")]
    [Range(0f, 1f)] public float angularDamping = 0.6f;
    [Tooltip("Caps the speed when you let go so objects can't be flung.")]
    public float maxReleaseSpeed = 3f;

    // ---- read-only status (crosshair UI, prompts, alien noise...) ----
    public bool IsDragging => heldBody != null;
    public Draggable LookingAt { get; private set; }

    Rigidbody heldBody;
    Draggable heldDraggable;
    Collider[] heldColliders;
    Vector3 grabOffset;          // object centre relative to the grab point (world space, fixed at grab)
    float holdDistance;
    RigidbodyInterpolation prevInterpolation;

    void Awake()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (playerCollider == null) playerCollider = GetComponentInParent<Collider>();
    }

    void Update()
    {
        LookingAt = Trace(out RaycastHit hit);

        bool held = GrabHeld();

        if (!IsDragging && held && LookingAt != null)
            Grab(LookingAt, hit);
        else if (IsDragging && !held)
            Release();
    }

    void FixedUpdate()
    {
        if (!IsDragging) return;

        if (heldBody == null || !heldBody.gameObject.activeInHierarchy)
        {
            Release();
            return;
        }

        Vector3 holdPoint = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;
        Vector3 targetCentre = holdPoint + grabOffset;
        Vector3 toTarget = targetCentre - heldBody.position;

        if (toTarget.magnitude > breakDistance)
        {
            Release();
            return;
        }

        Vector3 vel = toTarget * followStrength * heldDraggable.followMultiplier;
        vel = Vector3.ClampMagnitude(vel, maxDragSpeed);

        if (heldDraggable.dragAlongGround)
            vel.y = GetVelocity(heldBody).y; // let gravity keep it on the floor

        SetVelocity(heldBody, vel);
        heldBody.angularVelocity *= 1f - angularDamping;
    }

    // ---------------------------------------------------------------------

    Draggable Trace(out RaycastHit hit)
    {
        hit = default;
        if (playerCamera == null) return null;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (!Physics.Raycast(ray, out hit, reach, traceMask, QueryTriggerInteraction.Ignore))
            return null;

        return hit.collider.GetComponentInParent<Draggable>();
    }

    void Grab(Draggable d, RaycastHit hit)
    {
        heldDraggable = d;
        heldBody = d.GetComponent<Rigidbody>();
        heldColliders = d.GetComponentsInChildren<Collider>();

        grabOffset = heldBody.position - hit.point;
        holdDistance = Mathf.Clamp(hit.distance, minHoldDistance, maxHoldDistance);

        prevInterpolation = heldBody.interpolation;
        heldBody.interpolation = RigidbodyInterpolation.Interpolate;
        heldBody.WakeUp();

        SetPlayerCollision(true);
    }

    void Release()
    {
        if (heldBody != null)
        {
            SetVelocity(heldBody, Vector3.ClampMagnitude(GetVelocity(heldBody), maxReleaseSpeed));
            heldBody.interpolation = prevInterpolation;
        }

        SetPlayerCollision(false);

        heldBody = null;
        heldDraggable = null;
        heldColliders = null;
    }

    // Stops the held object shoving the player around.
    void SetPlayerCollision(bool ignore)
    {
        if (playerCollider == null || heldColliders == null) return;
        foreach (var c in heldColliders)
            if (c != null) Physics.IgnoreCollision(c, playerCollider, ignore);
    }

    bool GrabHeld()
    {
#if ENABLE_INPUT_SYSTEM
        // New Input System: maps the KeyCode to the same key on the keyboard.
        if (Keyboard.current == null) return false;
        return grabKey == KeyCode.E ? Keyboard.current.eKey.isPressed
                                    : Keyboard.current[KeyToInputSystem(grabKey)].isPressed;
#else
        return Input.GetKey(grabKey);
#endif
    }

#if ENABLE_INPUT_SYSTEM
    static Key KeyToInputSystem(KeyCode k)
    {
        // Covers letter keys; extend if you rebind to something else.
        if (k >= KeyCode.A && k <= KeyCode.Z) return Key.A + (k - KeyCode.A);
        return Key.E;
    }
#endif

    // Unity 6 renamed velocity -> linearVelocity.
    static Vector3 GetVelocity(Rigidbody rb)
    {
#if UNITY_6000_0_OR_NEWER
        return rb.linearVelocity;
#else
        return rb.velocity;
#endif
    }

    static void SetVelocity(Rigidbody rb, Vector3 v)
    {
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = v;
#else
        rb.velocity = v;
#endif
    }

    void OnDisable()
    {
        if (IsDragging) Release();
    }
}
