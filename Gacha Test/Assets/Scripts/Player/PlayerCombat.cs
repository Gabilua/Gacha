using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : CombatManager
{
    [SerializeField] PlayerController player;
    [SerializeField] EquipmentManager equipment;
    [SerializeField] Transform avatarHolder;
    [SerializeField] CharacterAvatar[] characterAvatars;
    [SerializeField] Character[] characterInfo;

    public Character[] currentParty;
    [SerializeField] CharacterAvatar activeCharacterAvatar;
    public Character activeCharacterInfo;

    private void Start()
    {
        characterAvatars = avatarHolder.GetComponentsInChildren<CharacterAvatar>();
        ChangeActiveCharacter(3);
    }

    void Update()
    {
        UIManager.instance.UpdateHealthBar(currentHealth, maxHealth);
    }

    public void ChangeActiveCharacter(int i)
    {
        foreach (var avatar in characterAvatars)
        {
            avatar.gameObject.SetActive(false);
        }

        activeCharacterAvatar = characterAvatars[i];
        activeCharacterInfo = characterInfo[i];

        activeCharacterAvatar.gameObject.SetActive(true);

        SetupCharacterAvatar();
    }

    void SetupCharacterAvatar()
    {
        equipment.activeEquipment[0] = activeCharacterAvatar.activeEquipment[0];
        equipment.activeEquipment[1] = activeCharacterAvatar.activeEquipment[1];
        equipment.idleEquipment[0] = activeCharacterAvatar.idleEquipment[0];
        equipment.idleEquipment[1] = activeCharacterAvatar.idleEquipment[1];

        player.anim.avatar = activeCharacterInfo.animatorAvatar;

        if (player.isIdle)
            equipment.Sheath();
        else
            equipment.Unsheath();
    }
}
