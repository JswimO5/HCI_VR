using UnityEngine;

public class GazeRaycast : MonoBehaviour
{
    public float rayDistance = 15f;

    [Header("Reticle Settings")]
    public GameObject reticlePrefab; // assign a small dot prefab
    private GameObject reticleInstance;

    [HideInInspector]
    public GazeTrigger currentTarget;

    void Start()
    {
        if (reticlePrefab != null)
            reticleInstance = Instantiate(reticlePrefab);
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            GazeTrigger target = hit.collider.GetComponent<GazeTrigger>();
            if (target != currentTarget)
            {
                currentTarget?.OnPointerExit();
                currentTarget = target;
                currentTarget?.OnPointerEnter();
            }

            // Reticle visible wherever the ray hits
            if (reticleInstance != null)
            {
                reticleInstance.SetActive(true);
                reticleInstance.transform.position = hit.point;
                reticleInstance.transform.rotation = Quaternion.LookRotation(hit.normal);
            }
        }
        else
        {
            if (currentTarget != null)
            {
                currentTarget.OnPointerExit();
                currentTarget = null;
            }

            if (reticleInstance != null)
                reticleInstance.SetActive(false);
        }
    }
}
