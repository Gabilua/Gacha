using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretHolder : MonoBehaviour
{
    [SerializeField] float secretChance;
    [SerializeField] GameObject secret, disguise;

    [SerializeField] Chest myChest;
    [SerializeField] bool isChest;

    private void OnEnable()
    {
        ResetSecret();
        RollSecret();
    }

    private void ResetSecret()
    {
        disguise.SetActive(true);
        secret.SetActive(false);
    }

    void RollSecret()
    {
        if((Random.value*100) <= secretChance)
        {
            if (isChest)
                myChest.RollRandomChestLevel();

            disguise.SetActive(false);
            secret.SetActive(true);
        }
    }
    
}
