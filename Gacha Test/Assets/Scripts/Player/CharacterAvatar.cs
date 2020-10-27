﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAvatar : MonoBehaviour
{
    public Transform[] activeEquipment, idleEquipment;
    public GameObject currentWeapon, currentShield;
    public Collider[] baseAtkCols;
    public Transform baseAtkProjectileBarrel;
    public ParticleSystem[] atkFX;
}
