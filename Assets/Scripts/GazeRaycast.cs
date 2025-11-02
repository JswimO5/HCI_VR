using UnityEngine;

public class GazeRaycast : MonoBehaviour
{
    public float rayDistance = 15f;
    public bool logEveryFrame = false;
    public float logInterval = 0.25f;
    private float logTimer = 0f;

    private GazeTrigger currentTarget;

    void Update()
    {
        Debug.DrawRay(transform.position, transform.forward * rayDistance, Color.green);

        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (logEveryFrame || (logTimer <= 0f))
            {
                Debug.Log($"[GazeRaycast] Raycast HIT: {hit.collider.name} (distance {hit.distance:F2})");
                logTimer = logInterval;
            }

            GazeTrigger target = hit.collider.GetComponent<GazeTrigger>();
            if (target != currentTarget)
            {
                currentTarget?.OnPointerExit();
                currentTarget = target;
                currentTarget?.OnPointerEnter();
            }
        }
        else
        {
            if (logEveryFrame || (logTimer <= 0f))
            {
                Debug.Log("[GazeRaycast] Raycast HIT: nothing");
                logTimer = logInterval;
            }

            if (currentTarget != null)
            {
                currentTarget.OnPointerExit();
                currentTarget = null;
            }
        }

        if (logTimer > 0f) logTimer -= Time.deltaTime;
    }
}
