using UnityEngine;

public class SimpleSwimAndDespawn : MonoBehaviour
{
    [Header("Speed")]
    public Vector2 speedRange = new Vector2(1.5f, 4f);

    [Header("Wander")]
    public float turnJitter = 0.8f;              // how much the heading wiggles
    public float maxTurnDegreesPerSec = 60f;     // how fast it can turn

    [Header("Bounds / Despawn")]
    [Tooltip("If set, this Transform is used to measure distance for despawn checks.")]
    public Transform boundsCenter;               // can be camera or spawner
    [Tooltip("If true, the fish uses staticCenter instead of a moving Transform.")]
    public bool useStaticCenter = false;
    [Tooltip("Fixed position used if useStaticCenter is true.")]
    public Vector3 staticCenter;
    [Tooltip("Distance at which the fish despawns.")]
    public float despawnRadius = 120f;

    [Header("Vertical Bias (optional)")]
    [Tooltip("Target Y position to loosely stay near (e.g., within a depth band).")]
    public float targetY = float.NaN;
    public float verticalSmoothing = 0.5f;

    float _speed;
    Vector3 _dir;
    float _noiseSeed;

    void Start()
    {
        // random start speed & direction
        _speed = Random.Range(speedRange.x, speedRange.y);
        Vector2 r = Random.insideUnitCircle.normalized;
        _dir = new Vector3(r.x, 0f, r.y);
        _noiseSeed = Random.value * 1000f;
    }

    void Update()
    {
        float t = Time.time;

        // gently rotate heading
        float yawNoise = (Mathf.PerlinNoise(_noiseSeed, t * turnJitter) - 0.5f) * 2f;
        float maxStep = maxTurnDegreesPerSec * Time.deltaTime;
        float desiredYaw = Mathf.Clamp(yawNoise * maxTurnDegreesPerSec, -maxStep, maxStep);
        _dir = Quaternion.Euler(0f, desiredYaw, 0f) * _dir;
        _dir.Normalize();

        // optional vertical bias (keeps them near a depth)
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

        // figure out where the "center" is for despawn
        Vector3 center = useStaticCenter
            ? staticCenter
            : (boundsCenter ? boundsCenter.position : Vector3.zero);

        // distance check
        if ((transform.position - center).sqrMagnitude > despawnRadius * despawnRadius)
        {
            Destroy(gameObject);
        }
    }
}
