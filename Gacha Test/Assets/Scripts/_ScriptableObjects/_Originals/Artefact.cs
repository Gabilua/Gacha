using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Artefact", menuName = "Artefacts")]
public class Artefact : Equipament
{
    [SerializeField] private Vector2 _maxHpBounds;
    [SerializeField] private Vector2 _defFlatBounds;
    [SerializeField] private Vector2 _defPercentageBounds;
    [SerializeField] private Vector2 _atkFlatBounds;
    [SerializeField] private Vector2 _atkPercentagepBounds;
    [SerializeField] private Vector2 _skillBounds;
    [SerializeField] private Vector2 _critRateBounds;
    [SerializeField] private Vector2 _critDmgBounds;
    [SerializeField] private Vector2 _rechargeBounds;

    public override void _Init_()
    {
        _attribute = new float[Enum.GetValues(typeof(Attribute)).Length];
        _attributeBonusOrder = new int[_attribute.Length];

        RandomizeBonusOrder();

        for(int i = 0; i < stars; i++)
            _attribute[_attributeBonusOrder[i]] = 1;

        maxHP = Random.Range(_maxHpBounds.x, _maxHpBounds.y);
        defFlat = Random.Range(_defFlatBounds.x, _defFlatBounds.y);
        defPercentage = Random.Range(_defPercentageBounds.x, _defPercentageBounds.y);
        atkFlat = Random.Range(_atkFlatBounds.x, _atkFlatBounds.y);
        atkPercentage = Random.Range(_atkPercentagepBounds.x, _atkPercentagepBounds.y);
        skill = Random.Range(_skillBounds.x, _skillBounds.y);
        critRate = Random.Range(_critRateBounds.x, _critRateBounds.y);
        critDmg = Random.Range(_critDmgBounds.x, _critDmgBounds.y);
        recharge = Random.Range(_rechargeBounds.x, _rechargeBounds.y);
    }

    public override void LevelUp()
    {
        throw new NotImplementedException();
    }
}
