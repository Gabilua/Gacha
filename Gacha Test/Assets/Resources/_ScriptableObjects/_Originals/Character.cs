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
    public WeaponType weaponType;
    public int stars = 1;
    public CollectionName collection = CollectionName.Default;

    public Weapon currentWeapon;
    public Artifact currentType1Artifact;
    public Artifact currentType2Artifact;
    public Artifact currentType3Artifact;
    public Artifact currentType4Artifact;
    public Artifact currentType5Artifact;

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

    public Weapon startWeapon;
    public GameObject baseAtkProjectile;
    public Avatar animatorAvatar;
    public float baseAtkCD;
    public Sprite icon;
}

