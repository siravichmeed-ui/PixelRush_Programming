using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
    public GameObject roadSection;
    private bool spawned = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!spawned && other.CompareTag("Player"))
        {
            spawned = true;

            ObjectPool.Instance.Spawn(
                roadSection.name,
                transform.parent.position + new Vector3(10f, 0, 0),
                Quaternion.identity
            );
        }
    }
}