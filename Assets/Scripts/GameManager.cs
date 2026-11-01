using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score = 0;
    public int treasuresToCollect = 5;
    public int pointsPerTreasure = 10;
    public bool gameOver = false;

    public Camera thirdPersonCamera;
    public bool thirdPerson = false;

    private InputAction cameraToggle;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        cameraToggle = new InputAction("CameraToggle", InputActionType.Button, "Keyboard&");
        cameraToggle.AddBinding("<keyboard>/c");
        cameraToggle.Enable();
    }

    void OnDestroy()
    {
        cameraToggle?.Disable();
    }

    void Update()
    {
        bool cameraPressed = false;
        if (cameraToggle != null)
        {
            cameraPressed = cameraToggle.WasPressedThisFrame();
        }

        if (cameraPressed)
        {
            SetCameraMode(!thirdPerson);
        }
    }

    public void SetCameraMode(bool thirdPerson)
    {
        this.thirdPerson = thirdPerson;

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.enabled = thirdPerson;
        }

        if (Camera.main != null)
        {
            Camera.main.enabled = !thirdPerson;
        }
    }

    public void AddScore(int points)
    {
        score += points;
        if (!gameOver && score >= treasuresToCollect * pointsPerTreasure)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        gameOver = true;
        Debug.Log("Game Over. Final score: " + score);
    }

    void OnGUI()
    {
        GUIStyle hud = new GUIStyle(GUI.skin.label);
        hud.fontSize = 28;
        hud.fontStyle = FontStyle.Bold;
        hud.normal.textColor = Color.white;
        GUI.Label(new Rect(20, 20, 480, 60), "Score: " + score, hud);

        if (gameOver)
        {
            GUIStyle center = new GUIStyle(hud);
            center.alignment = TextAnchor.MiddleCenter;
            center.fontSize = 40;
            GUI.Label(new Rect(0, Screen.height / 2 - 80, Screen.width, 160),
                "Game Over!\nFinal Score: " + score, center);
        }
    }
}
