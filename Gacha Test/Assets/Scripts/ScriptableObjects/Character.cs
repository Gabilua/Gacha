using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters")]
public class Character : ScriptableObject
{
    public string characterName;
    public int level;
    public int currentExperience;
    public float maxHP;
    public float def;
    public float atk;
    public float skill;
    public float critRate;
    public float critDmg;
    public float recharge;
}
