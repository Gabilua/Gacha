using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LootDisplayUI : MonoBehaviour
{
    public TextMeshProUGUI description;
    [SerializeField] float duration;

    private void OnEnable()
    {
        Destroy(gameObject, duration);
    }

    public void SetupLootUI(int amount, string name)
    {
        if (amount > 1)
            description.text = amount + " " + name;
        else
            description.text = name;
    }
}
