using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;      // character body to follow
    public Transform pivot;       // pivot at character head height
    public float distance = 3.5f;
    public float height = 1.6f;
    public float sideOffset = 0.4f; // over-the-shoulder
    public float yawSpeed = 120f;
    public float pitchSpeed = 80f;
    public float minPitch = -25f;
    public float maxPitch = 40f;
    public float smoothSpeed = 10f;
    public float fieldOfView = 60f;

    [HideInInspector]
    public float yaw;
    [HideInInspector]
    public float pitch;

    private Vector3 smoothVelocity;

    void Start()
    {
        if (target != null)
        {
            yaw = target.eulerAngles.y;
            pitch = 10f;
        }
    }

    public void RotateYaw(float delta)
    {
        yaw += delta;
    }

    void LateUpdate()
    {
        if (target == null || pivot == null)
        {
            return;
        }

        // Build camera position behind/above the character
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 direction = rotation * Vector3.forward;

        Vector3 desiredPos = pivot.position + rotation * new Vector3(sideOffset, height, -distance);

        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref smoothVelocity, 1f / smoothSpeed);

        // Look at character head level
        Vector3 lookAt = pivot.position + Vector3.up * 0.9f;
        transform.LookAt(lookAt, Vector3.up);

        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.fieldOfView = fieldOfView;
        }
    }
}
