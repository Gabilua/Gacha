using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public GameObject currentWeapon, currentShield;
    public Transform[] activeEquipment, idleEquipment;

    public void Sheath()
    {
        currentWeapon.transform.position = idleEquipment[0].position;
        currentWeapon.transform.rotation = idleEquipment[0].rotation;

        currentWeapon.transform.parent = idleEquipment[0];

        if (currentShield)
        {
            currentShield.transform.position = idleEquipment[1].position;
            currentShield.transform.rotation = idleEquipment[1].rotation;
            currentShield.transform.parent = idleEquipment[1];
        }
    }

    public void Unsheath()
    {
        currentWeapon.transform.position = activeEquipment[0].position;
        currentWeapon.transform.rotation = activeEquipment[0].rotation;

        currentWeapon.transform.parent = activeEquipment[0];

        if (currentShield)
        {
            currentShield.transform.position = activeEquipment[1].position;
            currentShield.transform.rotation = activeEquipment[1].rotation;

            currentShield.transform.parent = activeEquipment[1];
        }
    }
}
