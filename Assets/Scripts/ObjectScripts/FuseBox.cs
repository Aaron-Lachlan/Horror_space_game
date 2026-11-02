using UnityEngine;

public class FuseBox : MonoBehaviour
{

    public Transform[] fuseSlot;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Fuse")) return;

        foreach (Transform slot in fuseSlot)
        {
            if (slot.childCount == 0)
            {
                Rigidbody rb = other.attachedRigidbody;
                if (rb != null)
                {
                    rb.isKinematic = true;                              // physics stops moving it
                    rb.constraints = RigidbodyConstraints.FreezeAll;    // locks position + rotation
                }

                other.transform.SetPositionAndRotation(slot.position, slot.rotation);
                other.transform.SetParent(slot);
                return;
            }
        }
    }
}
