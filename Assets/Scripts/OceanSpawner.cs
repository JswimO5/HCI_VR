using System.Collections.Generic;
using UnityEngine;

public class OceanBandSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Species
    {
        public string name = "Fish";
        public GameObject prefab;
        public int initialCount = 10;
        public int maxAlive = 30;
        public float respawnInterval = 2f;

        [Header("Spawn Band (depth from sea surface)")]
        public float minDepthBelowSurface = 1f;
        public float maxDepthBelowSurface = 15f;

        [Header("Horizontal Area")]
        public float radius = 60f;

        [Header("Extras")]
        public Transform parent;
        public bool randomYaw = true;
        public bool avoidAboveGround = true;
        public LayerMask groundMask = ~0;
        public float groundClearance = 0.5f;
    }

    [Header("Sea Surface")]
    public Transform seaSurface;
    public float seaLevelY = 0f;

    [Header("Follow Target")]
    public Transform followTarget;
    public float followLerpSpeed = 3f;

    public List<Species> species = new();

    readonly Dictionary<Species, List<GameObject>> _alive = new();
    readonly Dictionary<Species, float> _timers = new();

    void Awake()
    {
        foreach (var s in species)
        {
            if (!s.prefab) continue;
            _alive[s] = new List<GameObject>(s.maxAlive);
            _timers[s] = 0f;

            int toSpawn = Mathf.Clamp(s.initialCount, 0, s.maxAlive);
            for (int i = 0; i < toSpawn; i++) TrySpawn(s);
        }
    }

    void Update()
    {
        if (followTarget)
        {
            Vector3 t = new Vector3(followTarget.position.x, transform.position.y, followTarget.position.z);
            transform.position = Vector3.Lerp(transform.position, t, Time.deltaTime * followLerpSpeed);
        }

        foreach (var s in species)
        {
            if (!s.prefab) continue;

            var list = _alive[s];
            for (int i = list.Count - 1; i >= 0; i--)
                if (list[i] == null) list.RemoveAt(i);

            _timers[s] += Time.deltaTime;
            if (_timers[s] >= s.respawnInterval)
            {
                _timers[s] = 0f;
                if (list.Count < s.maxAlive) TrySpawn(s);
            }
        }
    }

    bool TrySpawn(Species s)
    {
        float seaY = seaSurface ? seaSurface.position.y : seaLevelY;

        Vector2 circle = Random.insideUnitCircle * s.radius;
        Vector3 pos = new Vector3(
            transform.position.x + circle.x,
            seaY,
            transform.position.z + circle.y
        );

        float depth = Random.Range(s.minDepthBelowSurface, s.maxDepthBelowSurface);
        pos.y = seaY - depth;

        if (s.avoidAboveGround &&
            Physics.Raycast(new Vector3(pos.x, seaY + 50f, pos.z), Vector3.down,
                            out RaycastHit hit, 200f, s.groundMask, QueryTriggerInteraction.Ignore))
        {
            float floorY = hit.point.y + s.groundClearance;
            if (pos.y < floorY) pos.y = Mathf.Min(seaY - s.minDepthBelowSurface, floorY);
        }

        Quaternion rot = s.randomYaw ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) : Quaternion.identity;
        Transform parent = s.parent ? s.parent : transform;

        // --- spawn + movement hookup -------------
        GameObject g = Instantiate(s.prefab, pos, rot, parent);
        _alive[s].Add(g);

        var swim = g.GetComponent<SimpleSwimAndDespawn>();
        if (swim)
        {
            swim.boundsCenter = this.transform;                             // follow this spawner
            swim.despawnRadius = s.radius + 30f;                            // let them drift a bit past the edge
            float avgDepth = Random.Range(s.minDepthBelowSurface, s.maxDepthBelowSurface);
            swim.targetY = (seaSurface ? seaSurface.position.y : seaLevelY) - avgDepth;
        }
        // ----------------------------------------

        return true;
    }
}
