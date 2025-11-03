using UnityEngine;
using UnityEngine.UI;

public class ReticleController : MonoBehaviour
{
    [Header("Reticle Settings")]
    public Image reticleImage;
    public float idleSize = 10f;
    public float activeSize = 20f;
    public float smoothSpeed = 5f;

    [Header("Colors")]
    public Color idleColor = new Color(1f, 1f, 1f, 0.6f);   // white, semi-transparent
    public Color targetColor = new Color(1f, 0.9f, 0f, 1f); // yellow, fully opaque

    [Header("Ray Settings")]
    public float rayDistance = 15f;

    private float currentSize;
    private Color currentColor;

    void Start()
    {
        currentSize = idleSize;
        currentColor = idleColor;

        if (reticleImage != null)
        {
            reticleImage.rectTransform.sizeDelta = new Vector2(currentSize, currentSize);
            reticleImage.color = currentColor;
        }
    }

    void Update()
    {
        // Default (no hit)
        float targetSize = idleSize;
        Color nextColor = idleColor;

        // Cast a ray from the camera center
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, rayDistance))
        {
            targetSize = activeSize;

            // ✅ Highlight if looking at an object with GazeTrigger
            if (hit.collider.GetComponent<GazeTrigger>() != null)
                nextColor = targetColor;
        }

        // Smooth transitions
        currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * smoothSpeed);
        currentColor = Color.Lerp(currentColor, nextColor, Time.deltaTime * smoothSpeed);

        // Apply to UI Image
        if (reticleImage != null)
        {
            reticleImage.rectTransform.sizeDelta = new Vector2(currentSize, currentSize);
            reticleImage.color = currentColor;
        }
    }
}
