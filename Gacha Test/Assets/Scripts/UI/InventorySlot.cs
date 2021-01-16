using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public Weapon myWeapon;
    public TextMeshProUGUI level;
    public Image rarityBG;
    public Image starDisplay;
    public Image weaponIcon;

    [SerializeField] Color[] rarities;
    [SerializeField] Vector2[] starPos;

    public void SetupSlot()
    {
        level.text = "Lv." + myWeapon.level;

        starDisplay.fillAmount = myWeapon.stars/5;
        rarityBG.color = rarities[myWeapon.stars - 1];

        //weaponIcon.sprite = myWeapon.inventoryIcon;

        starDisplay.rectTransform.position = starPos[myWeapon.stars - 1];
    }
}
