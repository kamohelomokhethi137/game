using UnityEngine;

public class Treasure : MonoBehaviour
{
    public int points = 10;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager.Instance.AddScore(points);
        Destroy(gameObject);
    }
}
