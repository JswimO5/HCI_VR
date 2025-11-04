using System.Collections.Generic;
using UnityEngine;

public class OceanSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Species
    {
        public string name = "Fish";
        public GameObject prefab;

        [Header("Counts")]
        public int initialCount = 10;
        public int maxAlive = 30;
        public float respawnInterval = 2f;

        [Header("Spawn Band (depth from sea surface)")]
        public float minDepthBelowSurface = 1f;
        public float maxDepthBelowSurface = 12f;

        [Header("Horizontal Spawn (Disk mode)")]
        public float radius = 60f;

        [Header("Gaze Cone (VR camera mode)")]
        public bool spawnUsingGaze = false;
        public float coneHalfAngleDeg = 45f;
        public Vector2 distanceRange = new Vector2(8f, 25f);

        [Header("Extras")]
        public Transform parent;
        public bool randomYaw = true;
        public bool avoidAboveGround = true;
        public LayerMask groundMask = ~0;
        public float groundClearance = 0.5f;

        [Header("Despawn")]
        public float extraDespawnMargin = 30f;
    }

    [Header("Sea Surface")]
    public Transform seaSurface;
    public float seaLevelY = 0f;

    [Header("Optional Follow (legacy disk mode)")]
    public Transform followTarget;
    public float followLerpSpeed = 3f;

    [Header("VR Camera (gaze mode)")]
    public Transform cameraTransform;

    [Header("Despawn Center Settings (for disk mode only)")]
    public bool useStaticCenter = true;
    public Transform boundsAnchor;

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
        Vector3 pos;
        Quaternion rot;

        if (s.spawnUsingGaze && cameraTransform)
        {
            float half = Mathf.Clamp(s.coneHalfAngleDeg, 1f, 89f) * Mathf.Deg2Rad;
            float u = Random.value, v = Random.value;
            float cosTheta = Mathf.Lerp(Mathf.Cos(half), 1f, u);
            float sinTheta = Mathf.Sqrt(1f - cosTheta * cosTheta);
            float phi = 2f * Mathf.PI * v;

            Vector3 camFwd = cameraTransform.forward; camFwd.y = 0f; camFwd.Normalize();
            Vector3 camRight = cameraTransform.right;  camRight.y = 0f; camRight.Normalize();
            Vector3 dir = (camRight * (sinTheta * Mathf.Cos(phi)) + camFwd * (sinTheta * Mathf.Sin(phi))).normalized;

            float dist = Random.Range(s.distanceRange.x, s.distanceRange.y);
            pos = cameraTransform.position + dir * dist;

            float minD = Mathf.Min(s.minDepthBelowSurface, s.maxDepthBelowSurface);
            float maxD = Mathf.Max(s.minDepthBelowSurface, s.maxDepthBelowSurface);
            float depth = Random.Range(minD, maxD);
            pos.y = seaY - depth;

            if (s.avoidAboveGround &&
                Physics.Raycast(new Vector3(pos.x, seaY + 50f, pos.z), Vector3.down,
                                out RaycastHit hit, 200f, s.groundMask, QueryTriggerInteraction.Ignore))
            {
                float floorY = hit.point.y + s.groundClearance;
                if (pos.y < floorY) pos.y = Mathf.Min(seaY - minD, floorY);
            }

            rot = s.randomYaw ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) : Quaternion.LookRotation(dir, Vector3.up);
            Transform parent = s.parent ? s.parent : transform;
            GameObject g = Instantiate(s.prefab, pos, rot, parent);
            _alive[s].Add(g);

            var swim = g.GetComponent<SimpleSwimAndDespawn>();
            if (swim)
            {
                swim.useStaticCenter = false;
                swim.boundsCenter = cameraTransform;
                swim.despawnRadius = s.distanceRange.y + Mathf.Max(0f, s.extraDespawnMargin);

                float avgDepth = Random.Range(minD, maxD);
                swim.targetY = seaY - avgDepth;
            }

            return true;
        }
        else
        {
            Vector2 circle = Random.insideUnitCircle * s.radius;
            pos = new Vector3(transform.position.x + circle.x, seaY, transform.position.z + circle.y);

            float minD = Mathf.Min(s.minDepthBelowSurface, s.maxDepthBelowSurface);
            float maxD = Mathf.Max(s.minDepthBelowSurface, s.maxDepthBelowSurface);
            float depth = Random.Range(minD, maxD);
            pos.y = seaY - depth;

            if (s.avoidAboveGround &&
                Physics.Raycast(new Vector3(pos.x, seaY + 50f, pos.z), Vector3.down,
                                out RaycastHit hit, 200f, s.groundMask, QueryTriggerInteraction.Ignore))
            {
                float floorY = hit.point.y + s.groundClearance;
                if (pos.y < floorY) pos.y = Mathf.Min(seaY - minD, floorY);
            }

            rot = s.randomYaw ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) : Quaternion.identity;

            Transform parent = s.parent ? s.parent : transform;
            GameObject g = Instantiate(s.prefab, pos, rot, parent);
            _alive[s].Add(g);

            var swim = g.GetComponent<SimpleSwimAndDespawn>();
            if (swim)
            {
                swim.despawnRadius = s.radius + Mathf.Max(0f, s.extraDespawnMargin);

                if (useStaticCenter)
                {
                    swim.useStaticCenter = true;
                    swim.staticCenter = boundsAnchor ? boundsAnchor.position : transform.position;
                }
                else
                {
                    swim.useStaticCenter = false;
                    swim.boundsCenter = boundsAnchor ? boundsAnchor : this.transform;
                }

                float avgDepth = Random.Range(minD, maxD);
                swim.targetY = seaY - avgDepth;
            }

            return true;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (species == null) return;

        foreach (var s in species)
        {
            if (s == null) continue;

            if (s.spawnUsingGaze && cameraTransform)
            {
                Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
                float far = s.distanceRange.y;
                Vector3 camPos = cameraTransform.position;
                Vector3 camFwd = cameraTransform.forward; camFwd.y = 0f; camFwd.Normalize();

                Vector3 rimR = Quaternion.AngleAxis(s.coneHalfAngleDeg, Vector3.up) * camFwd;
                Vector3 rimL = Quaternion.AngleAxis(-s.coneHalfAngleDeg, Vector3.up) * camFwd;

                Gizmos.DrawLine(camPos, camPos + camFwd * far);
                Gizmos.DrawLine(camPos, camPos + rimR.normalized * far);
                Gizmos.DrawLine(camPos, camPos + rimL.normalized * far);
            }
            else
            {
                Gizmos.color = new Color(0f, 1f, 1f, 0.35f);
                Gizmos.DrawWireSphere(transform.position, s.radius);
            }
        }
    }
}
