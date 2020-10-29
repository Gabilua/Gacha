﻿using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapons")]
public class Weapon : Equipament
{
    public override void _Init_()
    {
        _attribute = new float[Enum.GetValues(typeof(Attribute)).Length];
        _attributeBonusOrder = new int[2];

        RandomizeBonusOrder();

        for(int i = 0; i < _attribute.Length; i++)
        {
            int j = 0;
            for(; j < _attributeBonusOrder.Length; j++)
            {
                if (i == _attributeBonusOrder[j])
                {
                    _attribute[i] = 1;
                    break;
                }        
            }
        }

        SetValues();
    }
}
