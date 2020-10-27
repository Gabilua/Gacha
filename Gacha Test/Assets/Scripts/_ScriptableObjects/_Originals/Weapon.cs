using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public int level;
    public int currentExperience;
    public int subStatNumber;
    public float maxHP;
    public float def;
    public float atk;
    public float skill;
    public float critRate;
    public float critDmg;
    public float recharge;
}
