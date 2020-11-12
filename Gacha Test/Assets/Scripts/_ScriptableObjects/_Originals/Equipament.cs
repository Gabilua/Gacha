using UnityEngine;
using UnityEngine.Analytics;
using Random = UnityEngine.Random;

public enum Attribute { MaxHp, DefFlat, DefPercentage, AtkFlat, AtkPercentage, Skill, CritRate, CritDmg, Recharge }

public abstract class Equipament : ScriptableObject
{
    public int ID;
    public string equipamentName;
    public int level;
    public int currentExperience;
    public float stars = 1;

    [SerializeField] protected float maxHP;
    [SerializeField] protected float defFlat;
    [SerializeField] protected float defPercentage;
    [SerializeField] protected float atkFlat;
    [SerializeField] protected float atkPercentage;
    [SerializeField] protected float skill;
    [SerializeField] protected float critRate;
    [SerializeField] protected float critDmg;
    [SerializeField] protected float recharge;

    public float MaxHp => maxHP * attribute[(int)Attribute.MaxHp];
    public float DefFlat => defFlat * attribute[(int)Attribute.DefFlat];
    public float DefPercentage => defPercentage * attribute[(int)Attribute.DefPercentage];
    public float AtkFlat => atkFlat * attribute[(int)Attribute.AtkFlat];
    public float AtkPercentage => atkPercentage * attribute[(int)Attribute.AtkPercentage];
    public float Skill => skill * attribute[(int)Attribute.Skill];
    public float CritRate => critRate * attribute[(int)Attribute.CritRate];
    public float CritDmg => critDmg * attribute[(int)Attribute.CritDmg];
    public float Recharge => recharge * attribute[(int)Attribute.Recharge];

    public int[] attribute;
    protected int[] _attributeBonusOrder;

    public abstract void _Init_();

    public abstract void LevelUp();

    protected void RandomizeBonusOrder()
    {
        if (_attributeBonusOrder == null)
            return;

        int[] _attributeOrder = new int[attribute.Length];
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
}