using UnityEngine;

public class IndicatorFollower : MonoBehaviour
{
    public Transform targetNote;
    public Vector3 offset;
    public float scaleMultiplier = 1f;

    private void Start()
    {
        transform.localScale = Vector3.one * scaleMultiplier;
    }

    private void Update()
    {
        if (targetNote != null)
        {
            transform.position = targetNote.position + offset;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}