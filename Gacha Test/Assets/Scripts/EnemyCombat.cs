using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : CombatManager
{
    [SerializeField] EnemyController myUnit;

    void Update()
    {
        if (healthBar && healthBar.gameObject.activeInHierarchy)
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / maxHealth, 5 * Time.deltaTime);
    }

    public void EnemyDeath()
    {
        myUnit.anim.SetTrigger("Death");
        healthBar.gameObject.SetActive(false);
        Invoke("Destruction", corpseTime);
        GameManager.instance.EnemyKilled();
    }

    public void Destruction()
    {
        GameObject fx = Instantiate(deathFX, hitFX.transform.position, deathFX.transform.rotation);
        Destroy(fx, 2f);

        Destroy(gameObject);
    }
}
