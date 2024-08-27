using UnityEngine;

public class NoteMover : MonoBehaviour
{
    private float moveSpeed;
    private Vector3 movementDirection;
    private Vector3 startPoint;
    public Vector3 endPoint { get; private set; }
    private NoteBehaviour noteBehaviour;

    public void Init(float speed, Vector3 direction, Vector3 start, Vector3 end)
    {
        moveSpeed = speed;
        movementDirection = direction;
        startPoint = start;
        endPoint = end;
        noteBehaviour = GetComponent<NoteBehaviour>();
    }

    void Update()
    {
        transform.Translate(movementDirection * moveSpeed * Time.deltaTime, Space.World);

        if (Vector3.Dot(transform.position - endPoint, movementDirection) > 0)
        {
            if (noteBehaviour != null && !noteBehaviour.isPlayed)
            {
                noteBehaviour.MissNote();
            }
        }
    }
}