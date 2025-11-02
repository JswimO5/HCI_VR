using UnityEngine;

public class GazeRaycast : MonoBehaviour
{
    private GazeTrigger currentTarget;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 5f))
        {
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
            currentTarget?.OnPointerExit();
            currentTarget = null;
        }
    }
}
