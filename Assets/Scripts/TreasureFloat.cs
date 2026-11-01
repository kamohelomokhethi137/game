using UnityEngine;

public class TreasureFloat : MonoBehaviour
{
    public float bobHeight = 0.15f;
    public float bobSpeed = 1.5f;
    public float spinSpeed = 45f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.position = startPosition + Vector3.up * Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);
    }
}
