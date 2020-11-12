using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]
public class Weapon : Equipament
{
    public int ID;
    public int weaponType;
    public GameObject inGameGFX;

    public override void _Init_()
    {
        attribute = new int[Enum.GetValues(typeof(Attribute)).Length];
        _attributeBonusOrder = new int[2];

        RandomizeBonusOrder();

        attribute[_attributeBonusOrder[0]] = 1;
        attribute[_attributeBonusOrder[1]] = 1;
    }

    public override void LevelUp()
    {
        throw new NotImplementedException();
    }
}
