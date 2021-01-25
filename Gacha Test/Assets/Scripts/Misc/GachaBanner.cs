using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GachaBanner : MonoBehaviour
{
    [SerializeField] private CollectionsDictionary _collectionsDictionary;

    [SerializeField] private SpecialDrop[] specialDrops;
    [SerializeField] private float[] starRarity;
    [SerializeField] private CollectionRoll[] _collections;

    private Button _button;
    private GameManager _gameManager;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ClickBanner);
    }

    private void Start()
    {
        _gameManager = GameManager.instance;
    }

    public void ClickBanner() => Debug.Log(GetDrop());

    public ScriptableObject GetDrop()
    {
        float raritySum = 0;

        for (int i = 0; i < specialDrops.Length; i++)
            raritySum += specialDrops[i].rarity;

        float percentageRoll = Random.value;

        if(percentageRoll < raritySum)
        {
            int rollIndex = 0;
            float percentageOffSet = 0;

            for (int i = 0; i < specialDrops.Length; i++)
            {
                if (specialDrops[i].rarity + percentageOffSet >= percentageRoll)
                {
                    rollIndex = i;
                    break;
                }
                else
                    percentageOffSet += specialDrops[i].rarity;
            }

            return specialDrops[rollIndex].GetRandom();
        }
        else
            return RollCollection();
    }

    private ScriptableObject RollCollection()
    {
        float percentageRoll = Random.value;
        float percentageOffSet = 0;
       
        int starsIndex = 0; 
        for(int i = 0; i < starRarity.Length; i++)
        {
            if (starRarity[i] + percentageOffSet >= percentageRoll)
            {
                starsIndex = i;
                break;
            }
            else
                percentageOffSet += starRarity[i];
        }

        percentageRoll = Random.value;
        percentageOffSet = 0;

        int collectionIndex = 0;
        for (int i = 0; i < _collections.Length; i++)
        {
            if (_collections[i].rarity + percentageOffSet >= percentageRoll)
            {
                collectionIndex = i;
                break;
            }
            else
                percentageOffSet += _collections[i].rarity;
        }

        int roll = Random.Range(0, _collectionsDictionary.collections[_collections[collectionIndex].collectionName].dropsByStars[starsIndex].Count);

        GameManager.instance.GetBannerDrop(_collectionsDictionary.collections[_collections[collectionIndex].collectionName].dropsByStars[starsIndex][roll]);

        return _collectionsDictionary.collections[_collections[collectionIndex].collectionName].dropsByStars[starsIndex][roll];
    }
}

[Serializable]
public struct SpecialDrop
{
    public float rarity;
    public ScriptableObject[] specialDropPrefabs;

    public ScriptableObject GetRandom() => specialDropPrefabs[Random.Range(0, specialDropPrefabs.Length)];
}

[Serializable]
public struct CollectionRoll
{
    public CollectionName collectionName;
    public float rarity;
}