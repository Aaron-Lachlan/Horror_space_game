using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class FuseBox : MonoBehaviour
{

    [System.Serializable]
    public class FuseSlot
    {
        public Transform slot;

        [FormerlySerializedAs("onFusePlaced")]
        public UnityEvent onFuseInserted;   // e.g. slot 1 = lights, slot 2 = shutter, slot 3 = door
    }

    public FuseSlot[] fuseSlots;

    [Tooltip("Fires after every slot is filled.")]
    public UnityEvent onAllFusesPlaced;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Fuse")) return;

        for (int i = 0; i < fuseSlots.Length; i++)
        {
            Transform slot = fuseSlots[i].slot;
            if (slot == null || slot.childCount > 0) continue;

            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.constraints = RigidbodyConstraints.FreezeAll;
            }

            other.transform.SetPositionAndRotation(slot.position, slot.rotation);
            other.transform.SetParent(slot);

            fuseSlots[i].onFuseInserted?.Invoke();

            if (i == fuseSlots.Length - 1) onAllFusesPlaced?.Invoke();
            return;
        }
    }
}
