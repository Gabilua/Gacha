using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public ParticleSystem hitFX;
    public GameObject deathFX;
    [SerializeField] Animator anim;

    public float currentHealth, maxHealth;
    public float[] baseAtkDmg;
    public float corpseTime;

    public Image healthBar;

    public bool isDead;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    void Death()
    {
        if (!isDead)
        {
            isDead = true;

            anim.SetTrigger("Death");

            if (GetComponent<EnemyCombat>())
                GetComponent<EnemyCombat>().EnemyDeath();
        }
    }

    void ReceiveDamage(float amount)
    {
        float result = currentHealth - amount;

        if(result <= 0)
        {
            currentHealth = 0;
            Death();
        }
        else
        {
            currentHealth -= amount;            
        }

        if (healthBar && !healthBar.transform.parent.gameObject.activeInHierarchy)
            healthBar.transform.parent.gameObject.SetActive(true);

        anim.SetTrigger("Hurt");
        hitFX.Play();
    }

    float DealDamage(string source)
    {
        return baseAtkDmg[int.Parse(source)];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDead)
        {
            if (other.tag == "Damage")
            {
                if (other.GetComponentInParent<CombatManager>() && other.GetComponentInParent<CombatManager>() != this)
                    ReceiveDamage(other.GetComponentInParent<CombatManager>().DealDamage(other.name));

                if (other.GetComponentInParent<Projectile>() && other.GetComponentInParent<Projectile>().player != this)
                {
                    ReceiveDamage(other.GetComponentInParent<Projectile>().DealDamage());
                    other.GetComponentInParent<Projectile>().Destruct();
                }
            }
        }
    }
}
