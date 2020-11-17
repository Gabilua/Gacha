using System.Collections.Generic;
using UnityEngine;

public class CollectionsDictionary : MonoBehaviour
{
    public Dictionary<CollectionName, CollectionValue> collections { get; private set; }
    
    public void Awake()
    {
        collections = new Dictionary<CollectionName, CollectionValue>();

        DataBase dataBase = Resources.Load<DataBase>("Database");

        for (int i = 0; i < dataBase.Weapons.Length; i++)
        {
            if (!collections.ContainsKey(dataBase.Weapons[i].collection))
                collections.Add(dataBase.Weapons[i].collection, new CollectionValue());

            collections[dataBase.Weapons[i].collection].AddDrop(dataBase.Weapons[i]);
        }

        for (int i = 0; i < dataBase.Characters.Length; i++)
        {
            if (!collections.ContainsKey(dataBase.Characters[i].collection))
                collections.Add(dataBase.Characters[i].collection, new CollectionValue());

            collections[dataBase.Characters[i].collection].AddDrop(dataBase.Characters[i]);
        }

        /*foreach(KeyValuePair<CollectionName, CollectionValue> entry in collections)
        {
            Debug.Log(entry.Key);
            entry.Value.PrintValues();
        }*/
    }
}

public class CollectionValue
{
    public List<ScriptableObject>[] dropsByStars;

    public CollectionValue()
    {
        dropsByStars = new List<ScriptableObject>[5];
        for (int i = 0; i < 5; i++)
            dropsByStars[i] = new List<ScriptableObject>();
    }

    public void AddDrop(ScriptableObject p_scriptableObject)
    {
        int stars;
        if (p_scriptableObject.GetType() == typeof(Weapon))
        {
            Weapon weapon = (Weapon)p_scriptableObject;
            stars = weapon.stars;
        }
        else if (p_scriptableObject.GetType() == typeof(Character))
        {
            Character character = (Character)p_scriptableObject;
            stars = character.stars;
        }
        else
            return;

        dropsByStars[stars - 1].Add(p_scriptableObject);
    }

    public void PrintValues()
    {
        for(int i = 0; i < dropsByStars.Length; i++)
        {
            Debug.Log(i);
            for(int j = 0; j < dropsByStars[i].Count; j++)
            {
                Debug.Log(dropsByStars[i][j]);
            }
        }
    }
}
