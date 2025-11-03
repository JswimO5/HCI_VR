using UnityEngine;

public class SimpleSwimAndDespawn : MonoBehaviour
{
    [Header("Speed")]
    public Vector2 speedRange = new Vector2(1.5f, 4f);

    [Header("Wander")]
    [Tooltip("How quickly the heading wiggles")]
    public float turnJitter = 0.8f;
    [Tooltip("How sharply it can turn (deg/sec)")]
    public float maxTurnDegreesPerSec = 60f;

    [Header("Bounds / Despawn")]
    [Tooltip("Center to measure exit distance from (usually your spawner or player).")]
    public Transform boundsCenter;
    [Tooltip("Distance at which we consider it 'out of scene' and despawn.")]
    public float despawnRadius = 120f;
    [Tooltip("Optional soft clamp to keep them roughly in the upper band (world Y). Set to NaN to ignore.")]
    public float targetY = float.NaN;
    public float verticalSmoothing = 0.5f;

    float _speed;
    Vector3 _dir;
    float _noiseSeed;

    void Start()
    {
        _speed = Random.Range(speedRange.x, speedRange.y);
        // random initial direction on XZ
        Vector2 r = Random.insideUnitCircle.normalized;
        _dir = new Vector3(r.x, 0f, r.y);
        _noiseSeed = Random.value * 1000f;
    }

    void Update()
    {
        // add a little wandery chaos to heading (perlin-based, frame-rate friendly)
        float t = Time.time;
        float yawNoise = (Mathf.PerlinNoise(_noiseSeed, t * turnJitter) - 0.5f) * 2f; // -1..1
        float maxStep = maxTurnDegreesPerSec * Time.deltaTime;
        float desiredYaw = yawNoise * maxTurnDegreesPerSec;

        // rotate heading around Y
        _dir = Quaternion.Euler(0f, Mathf.Clamp(desiredYaw, -maxStep, maxStep), 0f) * _dir;
        _dir.Normalize();

        // gentle vertical pull toward targetY if set
        if (!float.IsNaN(targetY))
        {
            float dy = Mathf.Clamp((targetY - transform.position.y) * verticalSmoothing, -1.5f, 1.5f);
            Vector3 vPull = new Vector3(0f, dy, 0f);
            Vector3 flat = new Vector3(_dir.x, 0f, _dir.z).normalized;
            _dir = (flat + vPull * 0.1f).normalized;
        }

        // move + face velocity
        transform.position += _dir * _speed * Time.deltaTime;
        if (_dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_dir, Vector3.up), 8f * Time.deltaTime);

        // Despawn when out of bounds
        Vector3 center = boundsCenter ? boundsCenter.position : Vector3.zero;
        if ((transform.position - center).sqrMagnitude > despawnRadius * despawnRadius)
        {
            Destroy(gameObject);
        }
    }
}
