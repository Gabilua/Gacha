using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCombat : MonoBehaviour
{
    [SerializeField] EnemyController myMonster;
    public ParticleSystem hitFX;
    public GameObject deathFX;
    public Image healthBar;

    public bool isDead;

    public float currentHealth;
    public float maxHealth;
    public float currentLevel;
    public float def;
    public float atk;
    public float skill;
    public float critRate;
    public float critDmg;
    public float corpseTime;

    private void Start()
    {
        SetupAttributes();

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (healthBar && healthBar.gameObject.activeInHierarchy)
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / maxHealth, 5 * Time.deltaTime);
    }

    void SetupAttributes()
    {
        def = myMonster.monsterInfo.def;
        atk = myMonster.monsterInfo.atk;
        skill = myMonster.monsterInfo.skill;
        critRate = myMonster.monsterInfo.critRate;
        critDmg = myMonster.monsterInfo.critDmg;
        currentLevel = myMonster.monsterInfo.level;
        maxHealth = myMonster.monsterInfo.maxHP;
        corpseTime = myMonster.monsterInfo.corpseTime;
    }

    void ReceiveDamage(float amount, PlayerCombat source)
    {
        if (!myMonster.isAggro)
        {
            myMonster.combatTarget = source.transform;
            myMonster.isAggro = true;
        }

        float result = amount * 10 / def;

        UIManager.instance.SpawnDamageDisplay(transform.position, result);
       

        if (currentHealth - result <= 0)
        {
            currentHealth = 0;
            Death();
        }
        else
        {
            currentHealth -= result;
        }

        if (healthBar && !healthBar.transform.parent.gameObject.activeInHierarchy)
            healthBar.transform.parent.gameObject.SetActive(true);

        myMonster.anim.SetTrigger("Hurt");
        hitFX.Play();

        GameManager.instance.StartCoroutine("HitStop", source.activeCharacterInfo.weaponType);
    }    

    public float BaseAtkDamage(string source)
    {
        return Random.Range(0.9f,1.1f) * ( atk * myMonster.monsterInfo.baseAtkMultipliers[int.Parse(source)] );
    }

    void Death()
    {
        if (!isDead)
        {
            isDead = true;

            myMonster.anim.SetTrigger("Death");
            healthBar.gameObject.SetActive(false);
            GameManager.instance.EnemyKilled();
            Invoke("Destruction", corpseTime);
        }
    }

    public void Destruction()
    {
        GameObject fx = Instantiate(deathFX, hitFX.transform.position, deathFX.transform.rotation);
        Destroy(fx, 2f);

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDead)
        {
            if (other.tag == "Damage")
            {
                if (other.GetComponentInParent<PlayerCombat>() && other.GetComponentInParent<PlayerCombat>() != this)
                {
                    if (other.GetComponentInParent<PlayerCombat>())
                    {
                        ReceiveDamage(other.GetComponentInParent<PlayerCombat>().BaseAtkDamage(other.name), other.GetComponentInParent<PlayerCombat>());
                    }
                }

                if (other.GetComponent<Projectile>())
                {
                    ReceiveDamage(other.GetComponent<Projectile>().DealDamage(), other.GetComponent<Projectile>().player);
                    other.GetComponent<Projectile>().Destruct();
                }
            }
        }
    }
}
