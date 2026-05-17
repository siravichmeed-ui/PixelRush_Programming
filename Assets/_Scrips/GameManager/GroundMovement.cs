using UnityEngine;

public class GroundMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField]
    private float moveMultiplier = 1f;

    void Update()
    {
        if (GameManager.Instance == null)
            return;

        float speed =
            GameManager.Instance.speed *
            moveMultiplier;

        transform.position +=
            Vector3.left *
            speed *
            Time.deltaTime;
    }
}