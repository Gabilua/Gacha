﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Area", menuName = "Areas")]
public class Area : ScriptableObject
{
    [Header("General")]
    public int areaID;
    public string areaName;
    public GameObject[] tilePool;
    public GameObject startTile, endTile;
    public float tileLenght;
}
