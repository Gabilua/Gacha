﻿using System.Collections;
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
    public float stars;

    public int level;
    public int currentExperience;
    public float currenthP;
    public float maxHP;
    public int atk;
    public int def;
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

