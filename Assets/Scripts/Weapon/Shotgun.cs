using UnityEngine;

public class Shotgun : Gun
{
    [Header("샷건세팅")]
    public int pelletCount = 8;       // 한 번에 나가는 총알 수
    public float spreadAngle = 5f;    // 퍼짐 각도

    private void Start()
    {
        weaponName = "Shotgun";
        damage = 8; 
    }

    public override void Attack()
    {
        Vector3 baseDirection = GetAimDirection();

        for (int i = 0; i < pelletCount; i++)
        {            
            Vector3 spreadDir = GetSpreadDirection(baseDirection, spreadAngle);
            SpawnBullet(spreadDir);
        }
    }

    public override void EnemyAttack(Transform target)
    {
        Vector3 baseDirection = (target.position - firePoint.position).normalized;

        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 spreadDir = GetSpreadDirection(baseDirection, spreadAngle);
            SpawnBullet(spreadDir);
        }
    }

    private Vector3 GetSpreadDirection(Vector3 baseDir, float angle)
    {
        float yaw = Random.Range(-angle, angle);
        float pitch = Random.Range(-angle, angle);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        return rotation * baseDir;
    }
}
