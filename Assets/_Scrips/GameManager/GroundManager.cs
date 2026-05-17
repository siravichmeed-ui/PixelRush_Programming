using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [Header("Ground")]
    [SerializeField]
    private Transform[] grounds;

    [Header("Setting")]
    [SerializeField]
    private float groundWidth = 20f;

    [SerializeField]
    private float resetPosition = -10f;

    void Update()
    {
        foreach (Transform ground in grounds)
        {
            // 👉 ขอบขวาของพื้น
            float rightEdge =
                ground.position.x +
                (groundWidth / 2f);

            // 👉 ถ้าหลุดจอ
            if (rightEdge < resetPosition)
            {
                MoveGround(ground);
            }
        }
    }

    void MoveGround(Transform ground)
    {
        // 👉 หาอันขวาสุด
        float maxX = grounds[0].position.x;

        foreach (Transform g in grounds)
        {
            if (g.position.x > maxX)
            {
                maxX = g.position.x;
            }
        }

        // 👉 ย้ายไปต่อท้าย
        ground.position =
            new Vector3(
                maxX + groundWidth,
                ground.position.y,
                ground.position.z
            );
    }
}