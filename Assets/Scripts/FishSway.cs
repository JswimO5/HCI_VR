using UnityEngine;

public class FishSway : MonoBehaviour
{
    [Header("Sway Settings")]
    public float swayAmplitude = 0.5f;    // How far side to side (in meters)
    public float swaySpeed = 1.5f;        // How fast the sway moves
    public Vector3 swayDirection = Vector3.right; // Direction of sway (default: side to side)

    [Header("Rotation (optional)")]
    public bool rotateWithSway = true;    // If true, the fish rotates slightly as it sways
    public float rotationAmount = 10f;    // Max degrees to rotate during sway

    private Vector3 startPosition;
    private float timeOffset;

    void Start()
    {
        startPosition = transform.position;
        timeOffset = Random.Range(0f, 2f * Mathf.PI); // Randomize phase so fish don't all sync
    }

    void Update()
    {
        float sway = Mathf.Sin(Time.time * swaySpeed + timeOffset) * swayAmplitude;
        transform.position = startPosition + swayDirection.normalized * sway;

        if (rotateWithSway)
        {
            float rotZ = Mathf.Sin(Time.time * swaySpeed + timeOffset) * rotationAmount;
            transform.rotation = Quaternion.Euler(0f, 0f, rotZ);
        }
    }
}