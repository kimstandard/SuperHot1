using UnityEngine;

public class Pistol : Gun
{
    private void Start()
    {
        weaponName = "Pistol";
        damage = 10;
    }

    // 발사 쿨타임
    public float fireCooldown = 2f;
    public bool isRecoiling = false;
    private float lastFireTime = -Mathf.Infinity;

    void Update()
    {
        // 쿨다운이 끝나면 recoil 상태 해제
        if (isRecoiling && (Time.time - lastFireTime >= fireCooldown))
        {
            isRecoiling = false;
        }
    }

    public override void Attack()
    {
        if (Time.time - lastFireTime < fireCooldown)
        {
            return;
        }

        Vector3 direction = GetAimDirection();
        SpawnBullet(direction);

        isRecoiling = true;

        lastFireTime = Time.time;
    }

    //적이 플레이어한테
    public override void EnemyAttack(Transform target)
    {        
        if (Time.time - lastFireTime < fireCooldown)
        {
            return;
        }
        
        Vector3 direction = (target.position - firePoint.position).normalized;
        
        SpawnBullet(direction);
        
        isRecoiling = true;
        lastFireTime = Time.time;
    }
}
