using UnityEngine;

public class Bat : Melee
{
    private void Start()
    {
        weaponName = "Bat";
        damage = 15;
        attackRange = 2f;
    }

    protected override void PerformSwing()
    {
        
    }

    public override void EnemyAttack(Transform target)
    {
    }
}
