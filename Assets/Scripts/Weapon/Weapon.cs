using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public int damage;

    public abstract void Attack();

    public abstract void EnemyAttack(Transform target);
}
