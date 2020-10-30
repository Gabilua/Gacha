using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]
public class Weapon : Equipament
{
    public override void _Init_()
    {
        _attribute = new float[Enum.GetValues(typeof(Attribute)).Length];
        _attributeBonusOrder = new int[2];

        RandomizeBonusOrder();

        _attribute[_attributeBonusOrder[0]] = 1;
        _attribute[_attributeBonusOrder[1]] = 1;
    }

    public override void LevelUp()
    {
        throw new NotImplementedException();
    }
}
