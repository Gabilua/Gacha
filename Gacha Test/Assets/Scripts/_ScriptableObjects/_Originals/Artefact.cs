using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Artefact", menuName = "Artefacts")]
public class Artefact : Equipament
{
    [SerializeField] private Vector2 _bounds;

    public override void _Init_()
    {
        _attribute = new float[Enum.GetValues(typeof(Attribute)).Length];
        _attributeBonusOrder = new int[stars];

        RandomizeBonusOrder();

        for(int i = 0; i < _attribute.Length; i++)
        {
            int j = 0;
            for(; j < _attributeBonusOrder.Length; j++)
            {
                if (i == _attributeBonusOrder[j])
                {
                    _attribute[i] = Random.Range(_bounds.x, _bounds.y);
                    break;
                }     
            }
        }

        SetValues();
    }
}
