using System;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Attribute { MaxHp, DefFlat, DefPercentage, AtkFlat, AtkPercentage, Skill, CritRate, CritDmg, Recharge }

public abstract class Equipament : ScriptableObject
{
    public string equipamentName;
    public int level;
    public int currentExperience;
    public int stars = 1;

    public float maxHP = 1;
    public float defFlat = 1;
    public float defPercentage = 1;
    public float atkFlat = 1;
    public float atkPercentage = 1;
    public float skill = 1;
    public float critRate = 1;
    public float critDmg = 1;
    public float recharge = 1;

    protected float[] _attribute;
    protected int[] _attributeBonusOrder;

    public abstract void _Init_();

    protected void RandomizeBonusOrder()
    {
        if (_attributeBonusOrder == null)
            return;

        int[] _attributeOrder = new int[_attribute.Length];
        for (int i = 0; i < _attributeOrder.Length; i++)
            _attributeOrder[i] = i;

        for (int i = 0; i < 2 * _attributeOrder.Length; i++)
        {
            int rand1 = Random.Range(0, _attributeOrder.Length);
            int rand2 = Random.Range(0, _attributeOrder.Length);

            int aux = _attributeOrder[rand1];
            _attributeOrder[rand1] = _attributeOrder[rand2];
            _attributeOrder[rand2] = aux;
        }

        for (int i = 0; i < _attributeBonusOrder.Length; i++)
            _attributeBonusOrder[i] = _attributeOrder[i];
    }

    protected void SetValues()
    {
        maxHP *= _attribute[(int)Attribute.MaxHp];
        defFlat *= _attribute[(int)Attribute.DefFlat];
        defPercentage *= _attribute[(int)Attribute.DefPercentage];
        atkFlat *= _attribute[(int)Attribute.AtkFlat];
        atkPercentage *= _attribute[(int)Attribute.AtkPercentage];
        skill *= _attribute[(int)Attribute.Skill];
        critRate *= _attribute[(int)Attribute.CritRate];
        critDmg *= _attribute[(int)Attribute.CritDmg];
        recharge *= _attribute[(int)Attribute.Recharge];
    }
}
