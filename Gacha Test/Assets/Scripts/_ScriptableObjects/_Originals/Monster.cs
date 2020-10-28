using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Monster", menuName = "Monsters")]
public class Monster : ScriptableObject
{
    public string monsterName;
    public int ID;
    public int element;

    public int level;
    public int currentExperience;
    public float maxHP;
    public float def;
    public float atk;
    public float skill;
    public float critRate;
    public float critDmg;
    public float[] baseAtkMultipliers;

    public GameObject baseAtkProjectile;
    public float baseAtkRate;
    public float aggroRange;
    public float engageDistance;
    public float moveSpeed;
    public float corpseTime;
}
