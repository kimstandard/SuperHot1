using UnityEngine;

public class Knife : Melee
{
    private void Start()
    {
        weaponName = "Knife";
        damage = 8;
        attackRange = 1.2f;
    }

    protected override void PerformSwing()
    {
    }

    public override void EnemyAttack(Transform target)
    {
    }
}
