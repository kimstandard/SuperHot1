using UnityEngine;

public abstract class Melee : Weapon
{
    public float attackRange = 2f;    
    public Camera playerCamera;

    public override void Attack()
    {
        PerformSwing(); // 애니메이션, 효과음 등

        // 레이캐스트로 적 타격
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange))
        {
            Debug.Log($"[Melee] 맞은 오브젝트: {hit.collider.name}");
            Vector3 pos = hit.point;
            RagdollChanger ragdollChanger = hit.collider.GetComponentInParent<RagdollChanger>();
            if (ragdollChanger != null)
            {
                ragdollChanger.TakeDamage(pos);
            }
        }
        else
        {
            Debug.Log("[Melee] 아무것도 맞추지 못함.");
        }
    }

    protected abstract void PerformSwing(); // 하위 클래스가 구현: 애니메이션, 이펙트 등
}
