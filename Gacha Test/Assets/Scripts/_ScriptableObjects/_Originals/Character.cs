using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters")]
public class Character : ScriptableObject
{
    public string characterName;
    public int ID;
    public int element;
    public int weaponType;

    public int level;
    public int currentExperience;
    public float currentHealth;
    public float maxHP;
    public int def;
    public int atk;
    public int skill;
    public float critRate;
    public float critDmg;
    public float recharge;
    public float[] baseAtkMultipliers;

    public GameObject baseAtkProjectile;
    public Avatar animatorAvatar;
    public float baseAtkCD;
    public Sprite icon;
}

