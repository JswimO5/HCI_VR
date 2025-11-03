using UnityEngine;

public class GazeTrigger : MonoBehaviour
{
    public enum Direction { Up, Down }
    public Direction direction;
    public float gazeTime = 1.0f;

    private VerticalMovement playerMove;
    private bool isGazedAt = false;
    private float timer = 0f;

    void Start()
    {
        playerMove = Camera.main.transform.parent.GetComponent<VerticalMovement>();
        if (playerMove == null)
            Debug.LogError("VerticalMovement not found on camera's parent!");
    }

    void Update()
    {
        if (!isGazedAt || playerMove == null)
        {
            timer = 0f;
            return;
        }

        timer += Time.deltaTime;

        if (timer >= gazeTime)
        {
            // Only trigger movement if this arrow is the currentTarget in GazeRaycast
            GazeRaycast gazeRay = Camera.main.GetComponent<GazeRaycast>();
            if (gazeRay != null && gazeRay.currentTarget == this)
            {
                if (direction == Direction.Up)
                    playerMove.MoveUp(true);
                else
                    playerMove.MoveDown(true);
            }
        }
    }

    public void OnPointerEnter() => isGazedAt = true;
    public void OnPointerExit()
    {
        isGazedAt = false;

        // Stop movement immediately if the gaze leaves
        if (playerMove != null)
        {
            if (direction == Direction.Up)
                playerMove.MoveUp(false);
            else
                playerMove.MoveDown(false);
        }

        timer = 0f;
    }
}
