using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterScreenRosterElement : MonoBehaviour
{
    public Character myCharacter;
    public TextMeshProUGUI id;
    public Image icon;

    public void SetupElement()
    {
        id.text = myCharacter.ID.ToString();
        icon.sprite = myCharacter.icon;
    }

    public void Click()
    {
        UIManager.instance.UpdateCharacterScreenAvatar(myCharacter.ID);
        UIManager.instance.UpdateCharacterScreenSection(0);
        UIManager.instance.UpdateCharacterWeaponIventory(myCharacter.weaponType);
    }
}
