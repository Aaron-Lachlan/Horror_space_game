using UnityEngine;
using UnityEngine.AI;

// Alien Isolation-style hunter.
// States: Patrol -> (hears noise) Investigate -> Search
//         any    -> (sees player) Chase -> Attack
//         Chase  -> (loses sight)  Search -> Patrol
// Walls: movement is restricted to the baked NavMesh, so the alien can never walk
// through walls. Sight is blocked by any collider between its eyes and the player,
// so the player can break line of sight by hiding behind obstacles.
[RequireComponent(typeof(NavMeshAgent))]
public class AlienAI : MonoBehaviour
{
    public enum AlienState { Patrol, Investigate, Chase, Search, Attack }

    [Header("References")]
    [Tooltip("Player to hunt. Auto-finds an object tagged 'Player' if left empty.")]
    public Transform player;
    [Tooltip("Head bone/transform. Sight is cast from here and it sweeps left/right while hunting. Leave empty to use the body.")]
    public Transform head;
    [Tooltip("Tick this if an Animator drives the head bone (the sweep is layered on top of the animation).")]
    public bool headIsAnimated = false;
    [Tooltip("Eye height used when no head is assigned.")]
    public float eyeHeight = 1.8f;

    [Header("Sight (yellow = sight cone, orange = close sense)")]
    public float sightRadius = 18f;
    [Range(10f, 360f)] public float sightAngle = 110f;
    [Tooltip("Inside this range the alien notices you even outside its view cone (still needs line of sight).")]
    public float closeSenseRadius = 3f;
    [Tooltip("Layers that can block sight (walls, crates, doors, cover). The player's and alien's own colliders are ignored automatically.")]
    public LayerMask obstacleMask = ~0;
    [Tooltip("How often (seconds) sight is re-checked.")]
    public float sightCheckInterval = 0.1f;
    [Tooltip("Height offsets on the player checked for line of sight. Only used if the player has no Collider.")]
    public float[] fallbackTargetHeights = { 0.5f, 1.6f };

    [Header("Hearing (cyan)")]
    [Tooltip("Other scripts call HearNoise(position, loudness). Noises inside this radius (x loudness) get investigated.")]
    public float hearingRadius = 10f;

    [Header("Attack (red)")]
    public float attackRadius = 2f;
    public float attackDamage = 100f;
    public float attackInterval = 1.5f;
    public float turnSpeed = 8f;

    [Header("Movement Speeds")]
    public float patrolSpeed = 1.8f;
    public float investigateSpeed = 2.6f;
    public float searchSpeed = 2.2f;
    public float chaseSpeed = 5f;
    [Tooltip("How often the chase path is recalculated (seconds).")]
    public float chaseRepathInterval = 0.2f;

    [Header("Patrol")]
    [Tooltip("Optional patrol route. If empty, the alien roams random reachable NavMesh points within Patrol Radius.")]
    public Transform[] patrolPoints;
    public bool randomPatrolOrder = true;
    public float patrolRadius = 20f;
    public float patrolWaitMin = 1f;
    public float patrolWaitMax = 4f;

    [Header("Chase / Losing the Player")]
    [Tooltip("After losing sight, the alien keeps tracking the player's real position this long (it 'senses' where you went) before it starts searching.")]
    public float loseSightGraceTime = 1.5f;

    [Header("Search (magenta, around last known position)")]
    public float searchDuration = 15f;
    public float searchRadius = 7f;
    public float searchPointWaitMin = 1f;
    public float searchPointWaitMax = 3f;

    [Header("Head Look-Around (walks while looking)")]
    [Tooltip("How far left/right the head (and sight cone) sweeps while patrolling/searching. Also the max head turn while chasing.")]
    public float headSweepAngle = 60f;
    public float headSweepSpeed = 1.2f;
    public float headTurnSpeed = 5f;

    [Header("Audio - Footsteps")]
    public AudioClip[] footstepSounds;
    [Range(0f, 1f)] public float footstepVolume = 0.8f;
    [Tooltip("Turn off if you trigger Footstep() from animation events instead.")]
    public bool distanceBasedFootsteps = true;
    [Tooltip("Distance travelled between steps when walking.")]
    public float walkStepDistance = 1.4f;
    [Tooltip("Distance travelled between steps when running (chasing).")]
    public float runStepDistance = 2.2f;

    [Header("Audio - Attack")]
    public AudioClip[] attackSounds;
    [Range(0f, 1f)] public float attackVolume = 1f;

    [Header("Audio - Spotted (scream when it first sees you)")]
    public AudioClip[] spottedSounds;
    [Range(0f, 1f)] public float spottedVolume = 1f;
    public float spottedCooldown = 8f;

    [Header("Audio - Searching (screams / croaks / hisses)")]
    public AudioClip[] searchSounds;
    [Range(0f, 1f)] public float searchVolume = 0.9f;
    public float searchSoundIntervalMin = 4f;
    public float searchSoundIntervalMax = 10f;
    public bool searchSoundsWhilePatrolling = true;

    [Header("Audio - Sources")]
    [Tooltip("Voice: attacks, screams, croaks. Auto-added if empty.")]
    public AudioSource voiceSource;
    [Tooltip("Footsteps. Auto-added if empty.")]
    public AudioSource footstepSource;
    [Range(0f, 0.5f)] public float audioPitchVariation = 0.08f;

    [Header("Debug")]
    [Tooltip("Show gizmos even when the alien isn't selected.")]
    public bool alwaysShowGizmos = false;

    // ---- read-only status for other scripts (music, UI, motion tracker...) ----
    public AlienState State => state;
    public bool CanSeePlayer => canSeePlayer;

    // =====================================================================
    //  VENTS & ENEMY HIDING SPOTS  --  INTENTIONALLY LEFT BLANK
    // =====================================================================
    // This section is empty on purpose. How the alien uses vents depends
    // entirely on how the level is built, and that isn't decided yet:
    //   - Are vents part of the NavMesh (a separate NavMesh area or agent
    //     type for crawling), or connected with NavMesh Links?
    //   - Where are the vent entry/exit points and enemy hiding spots?
    //   - Is there a crawl animation, and does the collider need to shrink?
    // Writing this before those level-design choices are made would mean
    // rewriting it later. Once they're made, the vent entry/exit points,
    // hiding spots, a "Vent" state and the logic for dropping out near the
    // player's last known position go here (hook into SetState/TickState).
    // =====================================================================


    // =====================================================================
    //  JUMPSCARES  --  INTENTIONALLY LEFT BLANK
    // =====================================================================
    // This section is empty on purpose. Jumpscares rely on assets that don't
    // exist yet (a scare/grab animation, camera takeover, sting audio, screen
    // flash) and on design decisions that haven't been made yet: does the
    // attack kill instantly, is the camera grabbed, is there one scare per
    // encounter or only scripted moments?
    // Hook points once those exist: DoAttack() for the kill/grab scare,
    // EnterState(Chase) for a "spotted" scare, and vent exits (section above).
    // =====================================================================


    // ---- internals ----
    NavMeshAgent agent;
    NavMeshPath tempPath;
    Collider playerCollider;
    AlienState state = AlienState.Patrol;
    public bool canSeePlayer;
    Vector3 lastKnownPlayerPos;
    bool hasLastKnownPos;
    Vector3 investigatePos;
    float timeSinceSeen = Mathf.Infinity;
    float sightTimer, waitTimer, attackTimer, searchTimer, repathTimer;
    bool searchReachedLastKnown;
    int patrolIndex = -1;
    float headYaw;
    Quaternion headBaseLocalRot;
    Vector3 lastStepPos;
    float stepAccum;
    float nextSearchSoundTime;
    float voiceBusyUntil;
    float lastSpottedTime = -999f;
    int lastFootIdx = -1, lastAttackIdx = -1, lastSpottedIdx = -1, lastSearchIdx = -1;

    static readonly RaycastHit[] sightHits = new RaycastHit[16];

    // ---------------------------------------------------------------------
    //  SETUP
    // ---------------------------------------------------------------------
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updatePosition = true;
        agent.updateRotation = true;
        tempPath = new NavMeshPath();

        if (player == null)
        {
            var pgo = GameObject.FindGameObjectWithTag("Player");
            if (pgo != null) player = pgo.transform;
        }
        if (player != null)
            playerCollider = player.GetComponentInChildren<Collider>(); // CharacterController counts

        if (head != null) headBaseLocalRot = head.localRotation;

        if (voiceSource == null) voiceSource = gameObject.AddComponent<AudioSource>();
        if (footstepSource == null) footstepSource = gameObject.AddComponent<AudioSource>();
        foreach (var src in new[] { voiceSource, footstepSource })
        {
            src.playOnAwake = false;
            src.spatialBlend = 1f; // 3D - world enemy
        }

        lastStepPos = transform.position;
        ScheduleNextSearchSound();
    }

    void Start()
    {
        EnterState(AlienState.Patrol, AlienState.Patrol);
    }

    // ---------------------------------------------------------------------
    //  MAIN LOOP
    // ---------------------------------------------------------------------
    void Update()
    {
        float dt = Time.deltaTime;

        sightTimer -= dt;
        if (sightTimer <= 0f)
        {
            canSeePlayer = CheckSight();
            sightTimer = sightCheckInterval;
        }

        if (canSeePlayer)
        {
            print("Player sighted");
            lastKnownPlayerPos = player.position;
            hasLastKnownPos = true;
            timeSinceSeen = 0f;
        }
        else timeSinceSeen += dt;

        attackTimer = Mathf.Max(0f, attackTimer - dt);

        UpdateTransitions();
        TickState(dt);
        UpdateFootsteps();
        UpdateSearchSounds();
    }

    void UpdateTransitions()
    {
        float dist = FlatDistanceToPlayer();

        if (canSeePlayer)
        {
            // small hysteresis so it doesn't flicker between Chase/Attack at the edge
            bool inAttackRange = dist <= attackRadius ||
                                 (state == AlienState.Attack && dist <= attackRadius * 1.2f);
            SetState(inAttackRange ? AlienState.Attack : AlienState.Chase);
        }
        else if (state == AlienState.Chase || state == AlienState.Attack)
        {
            if (timeSinceSeen > loseSightGraceTime) SetState(AlienState.Search);
            else if (state == AlienState.Attack) SetState(AlienState.Chase);
        }
    }

    void TickState(float dt)
    {
        switch (state)
        {
            case AlienState.Patrol:      TickPatrol(dt); break;
            case AlienState.Investigate: TickInvestigate(); break;
            case AlienState.Chase:       TickChase(dt); break;
            case AlienState.Search:      TickSearch(dt); break;
            case AlienState.Attack:      TickAttack(); break;
        }
    }

    // ---------------------------------------------------------------------
    //  STATE CHANGES
    // ---------------------------------------------------------------------
    void SetState(AlienState newState)
    {
        if (state == newState) return;
        AlienState prev = state;
        state = newState;
        EnterState(newState, prev);
    }

    void EnterState(AlienState s, AlienState prev)
    {
        if (agent.isOnNavMesh) agent.isStopped = false;

        switch (s)
        {
            case AlienState.Patrol:
                agent.speed = patrolSpeed;
                agent.stoppingDistance = 0.3f;
                waitTimer = Random.Range(0.2f, 1f);
                if (agent.isOnNavMesh) agent.ResetPath();
                break;

            case AlienState.Investigate:
                agent.speed = investigateSpeed;
                agent.stoppingDistance = 0.5f;
                SetDestinationSafe(investigatePos);
                break;

            case AlienState.Chase:
                agent.speed = chaseSpeed;
                agent.stoppingDistance = attackRadius * 0.8f;
                repathTimer = 0f;
                if (prev != AlienState.Chase && prev != AlienState.Attack)
                    PlaySpotted();
                break;

            case AlienState.Search:
                agent.speed = searchSpeed;
                agent.stoppingDistance = 0.5f;
                searchTimer = searchDuration;
                searchReachedLastKnown = false;
                SetDestinationSafe(lastKnownPlayerPos);
                break;

            case AlienState.Attack:
                if (agent.isOnNavMesh) agent.isStopped = true;
                break;
        }
    }

    // ---------------------------------------------------------------------
    //  STATE BEHAVIOURS
    // ---------------------------------------------------------------------
    void TickPatrol(float dt)
    {
        if (!ReachedDestination()) return;

        waitTimer -= dt;
        if (waitTimer <= 0f)
        {
            GoToNextPatrolPoint();
            waitTimer = Random.Range(patrolWaitMin, patrolWaitMax);
        }
    }

    void TickInvestigate()
    {
        if (!ReachedDestination()) return;

        // Got to the noise, nothing there -> search around it.
        lastKnownPlayerPos = investigatePos;
        hasLastKnownPos = true;
        SetState(AlienState.Search);
    }

    void TickChase(float dt)
    {
        if (player == null) { SetState(AlienState.Patrol); return; }

        // Grace period: it can't see you, but still "senses" where you went.
        if (!canSeePlayer) lastKnownPlayerPos = player.position;

        repathTimer -= dt;
        if (repathTimer <= 0f)
        {
            SetDestinationSafe(player.position);
            repathTimer = chaseRepathInterval;
        }
    }

    void TickSearch(float dt)
    {
        searchTimer -= dt;
        if (searchTimer <= 0f) { SetState(AlienState.Patrol); return; }

        if (!ReachedDestination()) return;

        if (!searchReachedLastKnown)
        {
            searchReachedLastKnown = true;
            waitTimer = Random.Range(searchPointWaitMin, searchPointWaitMax);
        }

        waitTimer -= dt;
        if (waitTimer <= 0f)
        {
            if (RandomNavPoint(lastKnownPlayerPos, searchRadius, out Vector3 p))
                SetDestinationSafe(p);
            waitTimer = Random.Range(searchPointWaitMin, searchPointWaitMax);
        }
    }

    void TickAttack()
    {
        if (player == null) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        FacePlayer(toPlayer);

        if (attackTimer <= 0f)
        {
            DoAttack();
            attackTimer = attackInterval;
        }
    }

    void DoAttack()
    {
        if (player != null)
        {
            if (!player.TryGetComponent(out IDamageable dmg))
                dmg = player.GetComponentInParent<IDamageable>();
            dmg?.TakeDamage(attackDamage);
        }

        float len = PlayRandom(voiceSource, attackSounds, ref lastAttackIdx, attackVolume);
        voiceBusyUntil = Time.time + len;
    }

    // ---------------------------------------------------------------------
    //  HEARING  (call from player footsteps, thrown objects, gunshots...)
    //  e.g.  alien.HearNoise(transform.position, sprinting ? 1.5f : 0.5f);
    // ---------------------------------------------------------------------
    public void HearNoise(Vector3 position, float loudness = 1f)
    {
        if (state == AlienState.Chase || state == AlienState.Attack) return;

        float range = hearingRadius * loudness;
        if ((position - transform.position).sqrMagnitude > range * range) return;

        investigatePos = position;
        if (state == AlienState.Investigate) SetDestinationSafe(position);
        else SetState(AlienState.Investigate);
    }

    // ---------------------------------------------------------------------
    //  SIGHT  (cone + range + line of sight, so cover hides the player)
    // ---------------------------------------------------------------------
    bool CheckSight()
    {
        if (player == null) return false;

        Vector3 eye = EyePosition();
        Vector3 forward = LookDirection();

        if (playerCollider != null && playerCollider.enabled)
        {
            // Body centre + near the top of the collider (follows crouching
            // if the controller height shrinks).
            Bounds b = playerCollider.bounds;
            if (CanSeePoint(eye, forward, b.center)) return true;
            if (CanSeePoint(eye, forward, b.center + Vector3.up * b.extents.y * 0.8f)) return true;
            return false;
        }

        foreach (float h in fallbackTargetHeights)
            if (CanSeePoint(eye, forward, player.position + Vector3.up * h)) return true;
        return false;
    }

    bool CanSeePoint(Vector3 eye, Vector3 forward, Vector3 point)
    {
        Vector3 toPoint = point - eye;
        float dist = toPoint.magnitude;
        if (dist > sightRadius || dist < 0.001f) return false;

        if (dist > closeSenseRadius)
        {
            Vector3 flat = toPoint;
            flat.y = 0f; // horizontal cone, so height differences (stairs) don't break it
            if (Vector3.Angle(forward, flat) > sightAngle * 0.5f) return false;
        }

        // Anything between the eye and the point that isn't the alien or the
        // player counts as cover.
        int count = Physics.RaycastNonAlloc(eye, toPoint / dist, sightHits, dist,
                                            obstacleMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < count; i++)
        {
            Transform t = sightHits[i].transform;
            if (t.IsChildOf(transform)) continue;
            if (t.IsChildOf(player)) continue;
            return false;
        }
        return true;
    }

    Vector3 EyePosition()
    {
        return head != null ? head.position : transform.position + Vector3.up * eyeHeight;
    }

    // Body forward rotated by the head sweep, so the sight cone moves with the
    // head regardless of which axis the head bone points along.
    Vector3 LookDirection()
    {
        return Quaternion.AngleAxis(headYaw, Vector3.up) * FlatForward();
    }

    Vector3 FlatForward()
    {
        Vector3 f = transform.forward;
        f.y = 0f;
        return f.sqrMagnitude > 0.0001f ? f.normalized : Vector3.forward;
    }

    // ---------------------------------------------------------------------
    //  HEAD LOOK-AROUND (LateUpdate so it layers on top of any Animator)
    // ---------------------------------------------------------------------
    void LateUpdate()
    {
        float target;
        if ((state == AlienState.Chase || state == AlienState.Attack) && player != null)
        {
            Vector3 to = player.position - transform.position;
            to.y = 0f;
            target = Mathf.Clamp(Vector3.SignedAngle(FlatForward(), to, Vector3.up),
                                 -headSweepAngle, headSweepAngle);
        }
        else
        {
            target = Mathf.Sin(Time.time * headSweepSpeed) * headSweepAngle;
        }

        headYaw = Mathf.LerpAngle(headYaw, target, headTurnSpeed * Time.deltaTime);

        if (head != null)
        {
            if (!headIsAnimated) head.localRotation = headBaseLocalRot;
            head.rotation = Quaternion.AngleAxis(headYaw, transform.up) * head.rotation;
        }
    }

    // ---------------------------------------------------------------------
    //  AUDIO
    // ---------------------------------------------------------------------
    void UpdateFootsteps()
    {
        Vector3 pos = transform.position;
        Vector3 delta = pos - lastStepPos;
        delta.y = 0f;
        lastStepPos = pos;

        if (!distanceBasedFootsteps) return;

        stepAccum += delta.magnitude;
        float speed01 = Speed01();
        float stepLen = Mathf.Lerp(walkStepDistance, runStepDistance, speed01);
        if (stepAccum >= stepLen)
        {
            stepAccum = 0f;
            PlayFootstep(speed01);
        }
    }

    // Call from an Animation Event on each foot plant (untick Distance Based Footsteps).
    public void Footstep()
    {
        PlayFootstep(Speed01());
    }

    void PlayFootstep(float speed01)
    {
        PlayRandom(footstepSource, footstepSounds, ref lastFootIdx,
                   footstepVolume * Mathf.Lerp(0.7f, 1f, speed01));
    }

    float Speed01()
    {
        return Mathf.InverseLerp(patrolSpeed, chaseSpeed, agent.velocity.magnitude);
    }

    void UpdateSearchSounds()
    {
        bool searching = state == AlienState.Search || state == AlienState.Investigate ||
                         (searchSoundsWhilePatrolling && state == AlienState.Patrol);
        if (!searching || Time.time < nextSearchSoundTime) return;

        if (Time.time < voiceBusyUntil)
        {
            nextSearchSoundTime = voiceBusyUntil + 0.5f;
            return;
        }

        float len = PlayRandom(voiceSource, searchSounds, ref lastSearchIdx, searchVolume);
        voiceBusyUntil = Time.time + len;
        ScheduleNextSearchSound();
    }

    void ScheduleNextSearchSound()
    {
        nextSearchSoundTime = Time.time + Random.Range(searchSoundIntervalMin, searchSoundIntervalMax);
    }

    void PlaySpotted()
    {
        if (Time.time - lastSpottedTime < spottedCooldown) return;
        lastSpottedTime = Time.time;
        float len = PlayRandom(voiceSource, spottedSounds, ref lastSpottedIdx, spottedVolume);
        voiceBusyUntil = Time.time + len;
    }

    // Picks a random clip (never the same twice in a row), returns its playback length.
    float PlayRandom(AudioSource src, AudioClip[] clips, ref int lastIndex, float volume)
    {
        if (src == null || clips == null || clips.Length == 0) return 0f;

        int index = 0;
        if (clips.Length > 1)
        {
            do { index = Random.Range(0, clips.Length); } while (index == lastIndex);
        }
        lastIndex = index;

        AudioClip clip = clips[index];
        if (clip == null) return 0f;

        src.pitch = 1f + Random.Range(-audioPitchVariation, audioPitchVariation);
        src.PlayOneShot(clip, volume);
        return clip.length / src.pitch;
    }

    // ---------------------------------------------------------------------
    //  NAVIGATION HELPERS
    // ---------------------------------------------------------------------
    void GoToNextPatrolPoint()
    {
        Vector3 target;
        bool hasRoute = patrolPoints != null && patrolPoints.Length > 0;

        if (hasRoute)
        {
            if (randomPatrolOrder && patrolPoints.Length > 1)
            {
                int i;
                do { i = Random.Range(0, patrolPoints.Length); } while (i == patrolIndex);
                patrolIndex = i;
            }
            else patrolIndex = (patrolIndex + 1) % patrolPoints.Length;

            if (patrolPoints[patrolIndex] == null) return;
            target = patrolPoints[patrolIndex].position;
        }
        else if (!RandomNavPoint(transform.position, patrolRadius, out target)) return;

        SetDestinationSafe(target);
    }

    // Random point on the NavMesh that the alien can actually reach
    // (rejects spots on the other side of walls with no route).
    bool RandomNavPoint(Vector3 center, float radius, out Vector3 result)
    {
        for (int i = 0; i < 8; i++)
        {
            Vector2 r = Random.insideUnitCircle * radius;
            Vector3 candidate = center + new Vector3(r.x, 0f, r.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 2f, NavMesh.AllAreas) &&
                agent.isOnNavMesh &&
                NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, tempPath) &&
                tempPath.status == NavMeshPathStatus.PathComplete)
            {
                result = hit.position;
                return true;
            }
        }
        result = transform.position;
        return false;
    }

    void SetDestinationSafe(Vector3 p)
    {
        if (agent.isOnNavMesh) agent.SetDestination(p);
    }

    bool ReachedDestination()
    {
        if (!agent.isOnNavMesh || agent.pathPending) return false;
        if (!agent.hasPath) return true;
        return agent.remainingDistance <= agent.stoppingDistance + 0.1f;
    }

    float FlatDistanceToPlayer()
    {
        if (player == null) return Mathf.Infinity;
        Vector3 d = player.position - transform.position;
        d.y = 0f;
        return d.magnitude;
    }

    void FacePlayer(Vector3 toPlayer)
    {
        if (toPlayer.sqrMagnitude < 0.001f) return;
        Quaternion target = Quaternion.LookRotation(toPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, turnSpeed * Time.deltaTime);
    }

    // ---------------------------------------------------------------------
    //  GIZMOS
    //  yellow = sight radius + view cone   orange = close sense
    //  cyan   = hearing                    red    = attack
    //  magenta = search area (play mode)   green/grey line = can/can't see player
    //  white  = current path               blue   = noise being investigated
    // ---------------------------------------------------------------------
    void OnDrawGizmos()
    {
        if (alwaysShowGizmos) DrawAlienGizmos();
    }

    void OnDrawGizmosSelected()
    {
        if (!alwaysShowGizmos) DrawAlienGizmos();
    }

    void DrawAlienGizmos()
    {
        Vector3 pos = transform.position;
        Vector3 eye = EyePosition();

        // Hearing
        Gizmos.color = new Color(0f, 1f, 1f, 0.6f);
        Gizmos.DrawWireSphere(pos, hearingRadius);

        // Sight radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eye, sightRadius);

        // View cone (moves with the head sweep in play mode)
        if (sightAngle < 360f)
        {
            Vector3 fwd = LookDirection();
            float half = sightAngle * 0.5f;
            Vector3 left = Quaternion.AngleAxis(-half, Vector3.up) * fwd;
            Vector3 right = Quaternion.AngleAxis(half, Vector3.up) * fwd;
            Gizmos.DrawLine(eye, eye + left * sightRadius);
            Gizmos.DrawLine(eye, eye + right * sightRadius);

            const int segments = 24;
            Vector3 prev = eye + left * sightRadius;
            for (int i = 1; i <= segments; i++)
            {
                Vector3 dir = Quaternion.AngleAxis(-half + sightAngle * i / segments, Vector3.up) * fwd;
                Vector3 next = eye + dir * sightRadius;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }

        // Close sense
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(eye, closeSenseRadius);

        // Attack
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos, attackRadius);

        if (!Application.isPlaying) return;

        // Search area around last known position
        if (hasLastKnownPos)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(lastKnownPlayerPos, searchRadius);
            Gizmos.DrawSphere(lastKnownPlayerPos, 0.25f);
        }

        // Noise being investigated
        if (state == AlienState.Investigate)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(investigatePos, 0.3f);
        }

        // Line of sight to player
        if (player != null)
        {
            Gizmos.color = canSeePlayer ? Color.green : Color.gray;
            Gizmos.DrawLine(eye, player.position + Vector3.up);
        }

        // Current path
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.white;
            Vector3[] corners = agent.path.corners;
            for (int i = 1; i < corners.Length; i++)
                Gizmos.DrawLine(corners[i - 1], corners[i]);
        }

#if UNITY_EDITOR
        UnityEditor.Handles.Label(pos + Vector3.up * (eyeHeight + 0.6f), state.ToString());
#endif
    }
}
