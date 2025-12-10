using UnityEngine;

public abstract class Gun : Weapon
{
    [Header("세팅")]
    public Camera cam;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 5f;

    protected Vector3 GetAimDirection()
    {        
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint;
        
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            targetPoint = hit.point;
        }
        else
        {            
            targetPoint = ray.origin + ray.direction * 1000f;
        }

        // 총구에서 목표 지점 방향 구하기
        Vector3 direction = (targetPoint - firePoint.position).normalized;
        return direction;
    }

    protected void SpawnBullet(Vector3 direction)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * bulletSpeed;
        }
    }
}
