using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory")]
public class Inventory : ScriptableObject, ISerializationCallbackReceiver
{
    public string savePath;
    private DataBase dataBase;

    public List<ConsumableSlot> ConsumableContainer = new List<ConsumableSlot>();
    public List<EquipamentSlot> WeaponContainer = new List<EquipamentSlot>();
    public List<EquipamentSlot> ArtifactContainer = new List<EquipamentSlot>();

    private void OnEnable()
    {
        dataBase = Resources.Load<DataBase>("Database");
    }

    public void AddConsumable (Consumable p_consumable, int p_amount)
    {
        for(int i = 0; i < ConsumableContainer.Count; i++)
        {
            if(ConsumableContainer[i].item == p_consumable)
            {
                ConsumableContainer[i].AddAmount(p_amount);
                return;
            }
        }

        ConsumableContainer.Add(new ConsumableSlot(dataBase.GetConsumableId[p_consumable], p_consumable, p_amount));
    }

    public void AddWeapon(Weapon p_weapon) => WeaponContainer.Add(new EquipamentSlot(p_weapon));

    public void AddArtifact(Artifact p_artifact) => ArtifactContainer.Add(new EquipamentSlot(p_artifact));

    public void Save()
    {
        string saveData = JsonUtility.ToJson(this, true);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));

        bf.Serialize(file, saveData);
        file.Close();
    }

    public void Load()
    {
        if(File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            file.Close();
        }
    }

    public void OnAfterDeserialize()
    {
        for(int i = 0; i < ConsumableContainer.Count; i++)
            ConsumableContainer[i].item = dataBase.GetConsumable[ConsumableContainer[i].ID];

        for (int i = 0; i < WeaponContainer.Count; i++)
            WeaponContainer[i].equipament = dataBase.GetWeapon[WeaponContainer[i].ID];

        for (int i = 0; i < ArtifactContainer.Count; i++)
            ArtifactContainer[i].equipament = dataBase.GetWeapon[ArtifactContainer[i].ID];
    }

    public void OnBeforeSerialize() { }
   
    public void ClearInventory()
    {
        ConsumableContainer.Clear();
        WeaponContainer.Clear();
        ArtifactContainer.Clear();
    }

    public void AddRandomConsumable()
    {
        Consumable aux = dataBase.Consumables[Random.Range(0, dataBase.Consumables.Length)];
        AddConsumable(aux, 1);
    }
    
    public void AddRandomWeapon()
    {
        Weapon aux = dataBase.Weapons[Random.Range(0, dataBase.Weapons.Length)];
        aux._Init_();
        AddWeapon(aux);
    }

    public void AddRandomArtifact()
    {
        Artifact aux = dataBase.Artifacts[Random.Range(0, dataBase.Artifacts.Length)];
        aux._Init_();
        AddArtifact(aux);
    }

    public List<EquipamentSlot> GetWeaponsByType(WeaponType p_weaponType)
    {
        List<EquipamentSlot> result = new List<EquipamentSlot>();

        foreach(EquipamentSlot weapon in WeaponContainer)
        {
            if ((weapon.equipament as Weapon).weaponType == p_weaponType)
                result.Add(weapon);
        }

        return result;
    }
}

[Serializable]
public class ConsumableSlot
{
    public int ID;
    public Consumable item;
    public int amount;

    public ConsumableSlot(int p_id, Consumable p_consumable, int p_amount)
    {
        ID = p_id;
        item = p_consumable;
        amount = p_amount;
    }

    public void AddAmount(int p_value)
    {
        amount += p_value;
    }
}

[Serializable]
public class EquipamentSlot
{
    public int ID;
    public Equipament equipament;
    public int level;
    public int[] attribute;

    public EquipamentSlot(Equipament p_equipament)
    {
        ID = p_equipament.ID;
        equipament = p_equipament;
        level = p_equipament.level;
        attribute = p_equipament.attribute;
    }
}