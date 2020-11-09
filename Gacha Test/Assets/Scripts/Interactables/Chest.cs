using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] GameObject[] GFX;
    public int chestLevel;

    private void OnEnable()
    {
        foreach (var variation in GFX)
        {
            variation.SetActive(false);
        }

        GFX[chestLevel - 1].SetActive(true);
    }
}
