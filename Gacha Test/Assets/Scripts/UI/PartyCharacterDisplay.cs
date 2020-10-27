using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PartyCharacterDisplay : MonoBehaviour
{
    public Character myCharacter;
    public TextMeshProUGUI id;
    public Image ultDisplay;
    public Image icon;

    private void Start()
    {
        id.text = myCharacter.ID.ToString();
        icon.sprite = myCharacter.icon;
        ultDisplay.color = UIManager.instance.elementColors[myCharacter.element];
    }

    public void Click()
    {
        UIManager.instance.NewCharacterPartyUIElement(GameManager.instance.player.GetComponent<PlayerCombat>().activeCharacterInfo);
        GameManager.instance.player.GetComponent<PlayerCombat>().ChangeActiveCharacter(myCharacter.ID);
        Destroy(gameObject);
    }
}
