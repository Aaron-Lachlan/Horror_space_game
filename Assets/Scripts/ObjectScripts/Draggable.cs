using UnityEngine;

// Put this on any object the player should be able to drag.
// The object needs a Rigidbody (added automatically) and a Collider.
[RequireComponent(typeof(Rigidbody))]
public class Draggable : MonoBehaviour
{
    [Tooltip("Multiplies how fast this object follows the player. Lower = feels heavier.")]
    [Range(0.1f, 2f)] public float followMultiplier = 1f;

    [Tooltip("Drag along the floor (gravity keeps it down) instead of lifting it up in front of the camera.")]
    public bool dragAlongGround = false;
}
