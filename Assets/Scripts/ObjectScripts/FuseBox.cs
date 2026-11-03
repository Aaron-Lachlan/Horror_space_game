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

    [Header("Spark Sounds")]
    public AudioSource sparkSource;
    public AudioClip[] sparkSounds;
    [Range(0f, 1f)] public float sparkVolume = 1f;
    [Range(0f, 0.5f)] public float sparkPitchVariation = 0.1f;

    [Tooltip("Random wait between sparks (seconds).")]
    public float sparkIntervalMin = 2f;
    public float sparkIntervalMax = 6f;

    [Tooltip("Spark on a loop from the start of the level.")]
    public bool sparkOnLoop = true;
    [Tooltip("Also spark every time a fuse goes in.")]
    public bool sparkOnInsert = true;
    [Tooltip("Stop the looping sparks once every fuse is in (box is fixed).")]
    public bool stopSparksWhenFull = true;

    [Tooltip("Optional particle effect played with each spark.")]
    public ParticleSystem sparkParticles;

    public int FusesInserted { get; private set; }
    public bool IsFull => FusesInserted >= fuseSlots.Length;

    int[] sparkOrder;
    int sparkIndex;
    int lastSpark = -1;
    float nextSparkTime;

    private void Awake()
    {
        if (sparkSource == null) sparkSource = GetComponent<AudioSource>();

        // Catch setup problems that make slots get skipped.
        for (int i = 0; i < fuseSlots.Length; i++)
        {
            if (fuseSlots[i].slot == null)
                Debug.LogWarning($"{name}: Fuse Slot {i + 1} has no Transform assigned.", this);
            else if (fuseSlots[i].slot.childCount > 0)
                Debug.LogWarning($"{name}: Fuse Slot {i + 1} ({fuseSlots[i].slot.name}) already has a child, so it counts as full. Use an empty object for the slot.", this);
        }

        ShuffleSparks();
        ScheduleNextSpark();
    }

    private void Update()
    {
        if (!sparkOnLoop || (IsFull && stopSparksWhenFull)) return;

        if (Time.time >= nextSparkTime)
        {
            PlaySpark();
            ScheduleNextSpark();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Always work with the whole fuse, even if a child collider triggered.
        Transform fuse = other.attachedRigidbody != null ? other.attachedRigidbody.transform : other.transform;

        if (!other.CompareTag("Fuse") && !fuse.CompareTag("Fuse")) return;
        if (IsInASlot(fuse)) return; // already inserted (e.g. second collider on the same fuse)

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

            fuse.SetPositionAndRotation(slot.position, slot.rotation);
            fuse.SetParent(slot);
            FusesInserted++;
            Debug.Log($"[{name}] Fuse {FusesInserted}/{fuseSlots.Length} inserted into slot {i + 1}");

            if (sparkOnInsert) PlaySpark();

            fuseSlots[i].onFuseInserted?.Invoke();

            if (FusesInserted == fuseSlots.Length)
            {
                Debug.Log($"[{name}] ALL FUSES PLACED fired");
                onAllFusesPlaced?.Invoke();
            }
            return;
        }
    }

    bool IsInASlot(Transform fuse)
    {
        foreach (var s in fuseSlots)
            if (s.slot != null && fuse.parent == s.slot) return true;
        return false;
    }

    // ---------------------------------------------------------------------
    //  SPARKS
    // ---------------------------------------------------------------------
    public void PlaySpark()
    {
        if (sparkSource == null || sparkSounds == null || sparkSounds.Length == 0) return;

        if (sparkIndex >= sparkOrder.Length) ShuffleSparks();

        int clipIndex = sparkOrder[sparkIndex++];
        lastSpark = clipIndex;

        AudioClip clip = sparkSounds[clipIndex];
        if (clip != null)
        {
            sparkSource.pitch = 1f + Random.Range(-sparkPitchVariation, sparkPitchVariation);
            sparkSource.PlayOneShot(clip, sparkVolume);
        }

        if (sparkParticles != null) sparkParticles.Play();
    }

    // Random order of every clip; the first one is never the clip that just played.
    void ShuffleSparks()
    {
        int count = sparkSounds != null ? sparkSounds.Length : 0;
        if (sparkOrder == null || sparkOrder.Length != count) sparkOrder = new int[count];

        for (int i = 0; i < count; i++) sparkOrder[i] = i;

        for (int i = count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (sparkOrder[i], sparkOrder[j]) = (sparkOrder[j], sparkOrder[i]);
        }

        if (count > 1 && sparkOrder[0] == lastSpark)
            (sparkOrder[0], sparkOrder[1]) = (sparkOrder[1], sparkOrder[0]);

        sparkIndex = 0;
    }

    void ScheduleNextSpark()
    {
        nextSparkTime = Time.time + Random.Range(sparkIntervalMin, sparkIntervalMax);
    }
}
