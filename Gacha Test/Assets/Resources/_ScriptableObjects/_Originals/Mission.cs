using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Mission", menuName = "Missions")]
public class Mission : ScriptableObject
{
    [Header("General")]
    public int missionType;
    public string missionTitle;
    public string missionDescription;
    public Sprite icon;
    public Color iconColor;

    [Header("Level Configuration")]
    public Vector2 tileAmountInterval;
    public int gridSize;

    [Header("Enemy Configuration")]
    public GameObject[] mobGroups;
    public int mobAmount;
    public int waveNumber;

    [Header("Reward Configuration")]
    public Vector2 royalsAmountInterval;
    public Vector2 stardustAmountInterval;
    public Vector2 expAmountInterval;
}
