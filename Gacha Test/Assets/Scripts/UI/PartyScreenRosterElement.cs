using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PartyScreenRosterElement : MonoBehaviour
{
    public GameObject dragVersionPrefab;
    public Character myCharacter;
    public TextMeshProUGUI id;
    public Image icon;

    public void SetupElement()
    {
        id.text = myCharacter.ID.ToString();
        icon.sprite = myCharacter.icon;
    }

    public void Drag()
    {
        GameObject iconCopy = Instantiate(dragVersionPrefab, transform);
        iconCopy.GetComponent<Image>().sprite = myCharacter.icon;
        UIManager.instance.AttachElementToCursor(iconCopy);
    }

    public void Drop()
    {
        UIManager.instance.DettachElementFromCursor();
        UIManager.instance.DropRosterElementOnDisplaySlot(myCharacter.ID);
    }
}
