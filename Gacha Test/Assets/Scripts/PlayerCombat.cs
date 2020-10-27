using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : CombatManager
{
    [SerializeField] PlayerController player;

    void Update()
    {
        UIManager.instance.UpdateHealthBar(currentHealth, maxHealth);
    }
}
