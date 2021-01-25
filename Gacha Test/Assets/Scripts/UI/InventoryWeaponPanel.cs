using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryWeaponPanel : MonoBehaviour
{
    [SerializeField] private Inventory playerInventory;

    [SerializeField] private GameObject _inventorySlotPrefab;

    public void UpdateCharacterInfo(WeaponType p_weaponType)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        List<EquipamentSlot> weapons = playerInventory.GetWeaponsByType(p_weaponType);

        foreach(EquipamentSlot weapon in weapons)
        {
            InventorySlot inventorySlot = Instantiate(_inventorySlotPrefab, transform).GetComponent<InventorySlot>();

            inventorySlot.myWeapon = (Weapon) weapon.equipament;
            inventorySlot.level.text = "Lv." + weapon.level;
            inventorySlot.starDisplay.fillAmount = weapon.equipament.stars / 5.0f;
        }
    }
}
