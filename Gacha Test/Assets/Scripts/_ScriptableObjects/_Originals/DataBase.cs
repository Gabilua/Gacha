using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Database", menuName = "Database" )]
public class DataBase : ScriptableObject, ISerializationCallbackReceiver
{
    public Weapon[] Weapons;
    public Dictionary<Weapon, int> GetWeaponId = new Dictionary<Weapon, int>();
    public Dictionary<int, Weapon> GetWeapon = new Dictionary<int, Weapon>();

    public Artifact[] Artifacts;
    public Dictionary<Artifact, int> GetArtifactId = new Dictionary<Artifact, int>();
    public Dictionary<int, Artifact> GetArtifact = new Dictionary<int, Artifact>();

    public Consumable[] Consumables;
    public Dictionary<Consumable, int> GetConsumableId = new Dictionary<Consumable, int>();
    public Dictionary<int, Consumable> GetConsumable = new Dictionary<int, Consumable>();

    public Character[] Characters;
    public Dictionary<Character, int> GetCharacterId = new Dictionary<Character, int>();
    public Dictionary<int, Character> GetCharacter = new Dictionary<int, Character>();

    public void OnAfterDeserialize()
    {
        GetWeaponId = new Dictionary<Weapon, int>();
        GetWeapon = new Dictionary<int, Weapon>();
        for(int i = 0; i < Weapons.Length; i++)
        {
            GetWeaponId.Add(Weapons[i], i);
            GetWeapon.Add(i, Weapons[i]);
        }

        GetArtifactId = new Dictionary<Artifact, int>();
        GetArtifact = new Dictionary<int, Artifact>();
        for (int i = 0; i < Artifacts.Length; i++)
        {
            GetArtifactId.Add(Artifacts[i], i);
            GetArtifact.Add(i, Artifacts[i]);
        }

        GetConsumableId = new Dictionary<Consumable, int>();
        GetConsumable = new Dictionary<int, Consumable>();
        for (int i = 0; i < Consumables.Length; i++)
        {
            GetConsumableId.Add(Consumables[i], i);
            GetConsumable.Add(i, Consumables[i]);
        }

        GetCharacterId = new Dictionary<Character, int>();
        GetCharacter = new Dictionary<int, Character>();
        for (int i = 0; i < Characters.Length; i++)
        {
            GetCharacterId.Add(Characters[i], i);
            GetCharacter.Add(i, Characters[i]);
        }
    }

    public void OnBeforeSerialize() { }

    [ContextMenu("Set ID")]
    public void SetID()
    {
        for(int i = 0; i < Weapons.Length; i++)
            Weapons[i].ID = i;

        for (int i = 0; i < Artifacts.Length; i++)
            Artifacts[i].ID = i;
    }
}
