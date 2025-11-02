using UnityEngine;
using UnityEngine.UI;  // needed for Image

public class GazeTrigger : MonoBehaviour
{
    public enum Direction { Up, Down }
    public Direction direction;
    public float gazeTime = 2f;

    private float timer = 0f;
    private bool isGazedAt = false;
    private VerticalMovement playerMove;
    public Image progressBar;  // assign the white bar Image here

    void Start()
    {
        playerMove = Camera.main.transform.parent.GetComponent<VerticalMovement>();
        if (progressBar != null)
            progressBar.fillAmount = 0f; // empty at start
    }

    void Update()
    {
        if (playerMove == null) return;

        if (isGazedAt)
        {
            timer += Time.deltaTime;

            // Update progress bar
            if (progressBar != null)
                progressBar.fillAmount = Mathf.Clamp01(timer / gazeTime);

            if (timer >= gazeTime)
            {
                if (direction == Direction.Up)
                    playerMove.MoveUp(true);
                else
                    playerMove.MoveDown(true);
            }
        }
        else
        {
            timer = 0f;
            if (progressBar != null)
                progressBar.fillAmount = 0f;

            playerMove.MoveUp(false);
            playerMove.MoveDown(false);
        }
    }

    // Called by raycast
    public void OnPointerEnter() => isGazedAt = true;
    public void OnPointerExit() => isGazedAt = false;

    // Mouse hover support
    private void OnMouseEnter() => isGazedAt = true;
    private void OnMouseExit() => isGazedAt = false;
}
