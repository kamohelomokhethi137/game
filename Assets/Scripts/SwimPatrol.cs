using UnityEngine;

public class SwimPatrol : MonoBehaviour
{
    public float radius = 9f;
    public float height = 2.3f;
    public float degreesPerSecond = 9f;
    public float startAngle = 0f;
    public float bobAmplitude = 0.25f;
    public float bobSpeed = 0.6f;
    public float yawOffset = 0f;

    private float phase;

    void Start()
    {
        phase = startAngle * Mathf.Deg2Rad;
        ApplyTransform(0f);
    }

    void Update()
    {
        ApplyTransform(Time.time);
    }

    void ApplyTransform(float t)
    {
        float a = phase + t * degreesPerSecond * Mathf.Deg2Rad;
        Vector3 next = new Vector3(Mathf.Cos(a) * radius, height + Mathf.Sin(t * bobSpeed) * bobAmplitude, Mathf.Sin(a) * radius);
        Vector3 delta = next - transform.position;

        transform.position = next;

        if (delta.sqrMagnitude > 0.00001f)
        {
            transform.rotation = Quaternion.LookRotation(delta.normalized, Vector3.up) * Quaternion.Euler(0f, yawOffset, 0f);
        }
    }
}
