using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GachaBanner : MonoBehaviour
{
    [SerializeField] private CollectionsDictionary _collectionsDictionary;

    [SerializeField] private float[] starRarity;
    [SerializeField] private CollectionName[] collections;
    [SerializeField] private float[] collectionRarity;

    [SerializeField] private SpecialDrop[] specialDrops;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ClickBanner);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        Debug.Log(GetDrop());
    }

    public void ClickBanner() => Debug.Log(GetDrop());

    public ScriptableObject GetDrop()
    {
        float raritySum = 0;

        for (int i = 0; i < specialDrops.Length; i++)
            raritySum += specialDrops[i].rarity;

        if (raritySum > 1.0f)
            Debug.LogError("Rarity sum can't be over 1");

        float percentageRoll = Random.value;

        if(percentageRoll < raritySum)
        {
            int rollIndex = 0;
            for (int i = 0; i < specialDrops.Length; i++)
                if (specialDrops[i].rarity >= percentageRoll)
                {
                    rollIndex = i;
                    break;
                }

            return specialDrops[rollIndex].GetRandom();
        }
        else
            return RollCollection();

    }

    private ScriptableObject RollCollection()
    {
        float percentageRoll = Random.value;
        int stars = 0;
        
        for(int i = 0; i < starRarity.Length; i++)
            if(starRarity[i] >= percentageRoll)
            {
                stars = i;
                break;
            }

        percentageRoll = Random.value;
        int collectionIndex = 0;

        for (int i = 0; i < collectionRarity.Length; i++)
            if (collectionRarity[i] >= percentageRoll)
            {
                collectionIndex = i;
                break;
            }

        int roll = Random.Range(0, _collectionsDictionary.collections[collections[collectionIndex]].dropsByStars[stars].Count);
        
        return _collectionsDictionary.collections[collections[collectionIndex]].dropsByStars[stars][roll];
    }
}

[Serializable]
public struct SpecialDrop
{
    public float rarity;
    public ScriptableObject[] specialDropPrefabs;

    public ScriptableObject GetRandom() => specialDropPrefabs[Random.Range(0, specialDropPrefabs.Length)];
}