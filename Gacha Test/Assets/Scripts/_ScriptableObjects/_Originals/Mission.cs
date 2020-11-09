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

    [Header("Level Configuration")]
    public Vector2 tileAmountInterval;

    [Header("Enemy Configuration")]
    public GameObject[] mobGroups;
    public int mobAmount;

    [Header("Reward Configuration")]
    public Vector2 royalsAmountInterval;
    public Vector2 stardustAmountInterval;
    public Vector2 expAmountInterval;
}
