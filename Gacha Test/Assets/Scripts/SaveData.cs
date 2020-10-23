using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Save", menuName = "Saves")]
public class SaveData : ScriptableObject
{
    public int stardust;
    public int royals;

    public Character[] allCharacters;
    public Character[] allWeapons;
}
