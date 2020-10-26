using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    [SerializeField] EnemyController myUnit;
    [SerializeField] PlayerController myPlayer;

    [SerializeField] ParticleSystem hitFX;
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

    private void Update()
    {
        if (healthBar && healthBar.gameObject.activeInHierarchy)
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / maxHealth, 5* Time.deltaTime);
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
        healthBar.gameObject.SetActive(false);
        Invoke("Destruction", corpseTime);
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

            myUnit.anim.SetTrigger("Hurt");
        }

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
            if (other.tag == "Damage" && other.GetComponentInParent<CombatManager>() != this)
                ReceiveDamage(other.GetComponentInParent<CombatManager>().DealDamage(other.name));
        }
    }
}
