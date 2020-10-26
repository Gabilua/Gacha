using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Town", menuName = "Towns")]
public class Town : ScriptableObject
{
    [Header("General")]
    public int townID;
    public string townName;
    public string[] townAreas;
    public Mission[] townMissions;
}
