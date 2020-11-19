using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] GameObject[] GFX;
    [SerializeField] ParticleSystem openFX;
    [SerializeField] GameObject deathFX;
    [SerializeField] float[] chestLevelRarity;
    [SerializeField] int[] moraRewardPerLevel;
    [Range(1,4)]
    public int chestLevel;

    public void RollRandomChestLevel()
    {
        chestLevel = Random.Range(1, 5);    
    }

    public void RollChestLevelAboveThisRarity(float rarity)
    {
        if (rarity <= chestLevelRarity[0])
            chestLevel = 1;
        else if (rarity > chestLevelRarity[0] && rarity <= chestLevelRarity[1])
            chestLevel = 2;
        else if (rarity > chestLevelRarity[1] && rarity <= chestLevelRarity[2])
            chestLevel = 3;
        else if (rarity > chestLevelRarity[2] && rarity <= chestLevelRarity[3])
            chestLevel = 4;
    }

    private void OnEnable()
    {
        foreach (var variation in GFX)
        {
            variation.SetActive(false);
        }

        GFX[chestLevel - 1].SetActive(true);
    }

    void Open()
    {
        openFX.Play();
        GameObject fx = Instantiate(deathFX, transform.position, transform.rotation);
        Destroy(fx, 6f);

        Animator[] anims = GetComponentsInChildren<Animator>();

        foreach (var anim in anims)
        {
            if(anim.gameObject.activeInHierarchy)
                anim.SetTrigger("Open");

             GameManager.instance.StartCoroutine("EarnRoyalsDelayed", moraRewardPerLevel[chestLevel - 1]);
        }

        Destroy(gameObject, 4.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Damage")
        {
            Open();            
        }
    }
}
