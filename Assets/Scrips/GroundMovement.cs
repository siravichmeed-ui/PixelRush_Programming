using UnityEngine;

public class GroundMovement : MonoBehaviour
{
    public float speed = 2f;

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Destroy Obstacle"))
        {
            gameObject.SetActive(false);
        }
    }
}