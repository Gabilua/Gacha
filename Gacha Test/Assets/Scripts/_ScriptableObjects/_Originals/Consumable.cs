using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Consumable")]
public class Consumable : ScriptableObject
{
    public int ID;

    [TextArea(15, 20)]
    public string _description;
}