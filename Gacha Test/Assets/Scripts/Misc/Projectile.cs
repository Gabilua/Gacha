using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    public PlayerCombat player;
    public float damage;
    public float speed;

    public float DealDamage()
    {
        return damage;
    }

    public void Destruct()
    {
        Destroy(gameObject);
    }
}
