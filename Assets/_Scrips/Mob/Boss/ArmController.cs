using UnityEngine;

public class ArmController : MonoBehaviour
{
    [SerializeField] private Boss boss;

    // เรียกจาก Animation Event
    public void ShootArm()
    {
        if (boss != null)
            boss.ShootArm();
    }
}