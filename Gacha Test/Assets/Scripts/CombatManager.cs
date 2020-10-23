using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    [SerializeField] EnemyController myUnit;
    [SerializeField] PlayerController myPlayer;

    public float currentHealth, maxHealth;
    public float[] baseAtkDmg;
    public float corpseTime;

    public Image healthBar;

    public bool isDead;

    private void Start()
    {
        if (GetComponent<PlayerController>())
            myPlayer = GetComponent<PlayerController>();
        else if (GetComponent<EnemyController>())
            myUnit = GetComponent<EnemyController>();

        currentHealth = maxHealth;
    }

    void Death()
    {
        if (!isDead)
        {
            isDead = true;

            if (myUnit != null)
                EnemyDeath();
        }
    }

    void EnemyDeath()
    {
        Invoke("Destruction", corpseTime);
        healthBar.gameObject.SetActive(false);
        GameManager.instance.EnemyKilled();
    }

    public void Destruction()
    {
        Destroy(gameObject);
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

        if (myPlayer != null)
            UIManager.instance.UpdateHealthBar(currentHealth, maxHealth);
        else if (myUnit != null)
        {
            if (!healthBar.transform.parent.gameObject.activeInHierarchy)
                healthBar.transform.parent.gameObject.SetActive(true);

            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }

    float DealDamage(string source)
    {
        return baseAtkDmg[int.Parse(source)];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Damage" && other.GetComponentInParent<CombatManager>() != this)
            ReceiveDamage(other.GetComponentInParent<CombatManager>().DealDamage(other.name));
    }
}
