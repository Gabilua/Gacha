using UnityEngine;

public class Consumable : ScriptableObject
{
    protected int id;

    protected int amount;

    public void GetConsumable() => amount++;

    public virtual void UseConsumable() => amount--;
}