using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Transform head;
    public Transform visibleBody;      // the visible capsule you control
    public ThirdPersonCamera thirdPersonCameraScript;
    public float speed = 2.5f;
    public float turnSpeed = 20f;      // fast snap to camera direction

    private readonly List<UnityEngine.XR.InputDevice> handDevices = new List<UnityEngine.XR.InputDevice>();

    void Update()
    {
        float forward = 0f;
        float strafe = 0f;
        float up = 0f;

        handDevices.Clear();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, handDevices);

        bool inThirdPerson = GameManager.Instance != null &&
            GameManager.Instance.thirdPerson &&
            GameManager.Instance.thirdPersonCamera != null &&
            GameManager.Instance.thirdPersonCamera.enabled;

        if (handDevices.Count > 0)
        {
            if (handDevices[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.primary2DAxis, out Vector2 stick))
            {
                strafe = stick.x;
                forward = stick.y;
            }
            // Right thumbstick rotates camera in TPP
            if (inThirdPerson && handDevices[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.secondary2DAxis, out Vector2 lookStick))
            {
                thirdPersonCameraScript?.RotateYaw(lookStick.x * 120f * Time.deltaTime);
            }
        }
        else if (Keyboard.current != null)
        {
            forward = (Keyboard.current.wKey.isPressed ? 1f : 0f) - (Keyboard.current.sKey.isPressed ? 1f : 0f);
            strafe = (Keyboard.current.dKey.isPressed ? 1f : 0f) - (Keyboard.current.aKey.isPressed ? 1f : 0f);
            up = (Keyboard.current.spaceKey.isPressed ? 1f : 0f) - (Keyboard.current.shiftKey.isPressed ? 1f : 0f);

            // Mouse rotates camera only in third person
            if (inThirdPerson && Mouse.current != null)
            {
                float mouseX = Mouse.current.delta.x.ReadValue();
                thirdPersonCameraScript?.RotateYaw(mouseX * 0.15f);
            }
        }

        if (Mathf.Abs(forward) < 0.01f && Mathf.Abs(strafe) < 0.01f && Mathf.Abs(up) < 0.01f)
        {
            return;
        }

        // Move relative to camera yaw (TPP) or headset yaw (FPP).
        Transform moveRef = head;
        if (inThirdPerson && thirdPersonCameraScript != null)
        {
            moveRef = thirdPersonCameraScript.transform;
        }

        // Yaw only — camera pitch must not affect movement direction.
        Quaternion yawRot = Quaternion.Euler(0f, moveRef.eulerAngles.y, 0f);
        Vector3 forwardDir = yawRot * Vector3.forward;
        Vector3 right = yawRot * Vector3.right;

        Vector3 move = forwardDir * forward + right * strafe;
        move.y = up;

        transform.position += move * speed * Time.deltaTime;

        // Face where you're going.
        Quaternion targetRot = Quaternion.Euler(0f, moveRef.eulerAngles.y, 0f);
        if (visibleBody != null)
        {
            visibleBody.rotation = Quaternion.Slerp(visibleBody.rotation, targetRot, turnSpeed * Time.deltaTime);
        }
    }
}
