using UnityEngine;

public class VerticalMovement : MonoBehaviour
{
    public float moveSpeed = 2f;      // vertical speed
    public float minY = 1f;           // lowest camera height
    public float maxY = 400f;           // highest camera height

    private float targetY;
    private bool movingUp = false;
    private bool movingDown = false;

    void Start()
    {
        targetY = transform.position.y;
    }

    void Update()
    {
        if (movingUp)
            targetY += moveSpeed * Time.deltaTime;
        else if (movingDown)
            targetY -= moveSpeed * Time.deltaTime;

        // Clamp targetY between minY and maxY
        targetY = Mathf.Clamp(targetY, minY, maxY);

        Vector3 pos = transform.position;
        pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * 3f);
        transform.position = pos;
    }

    public void MoveUp(bool state) => movingUp = state;
    public void MoveDown(bool state) => movingDown = state;
}
