using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("General")]
    [SerializeField] GameObject homeButton;
    [SerializeField] GameObject combatHUD, generalHUD, mainMenu, missionsTab, loadingScreen, homeCheckScreen, progressCheckScreen;
    [SerializeField] RectTransform missionList;
    [SerializeField] TextMeshProUGUI missionDescription, missionProgress;
    [SerializeField] Image missionIcon;
    [SerializeField] TextMeshProUGUI royalsCounter;
    [SerializeField] TextMeshProUGUI stardustCounter;
    [SerializeField] TextMeshProUGUI royalsRewardDisplay;
    [SerializeField] TextMeshProUGUI stardustRewardDisplay;

    [Header("Combat")]
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI healthValue;

    [Header("Configurations")]
    [SerializeField] float loadingTime;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    void ButtonManagement()
    {
        if (GameManager.instance.isHome)
            homeButton.SetActive(false);
        else
            homeButton.SetActive(true);
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentValue / maxValue, 5 * Time.deltaTime);
        healthValue.text = currentValue.ToString("F0")+" / "+maxValue.ToString("F0");
    }

    public void ToggleHomeCheckScreen(bool state)
    {
        homeCheckScreen.SetActive(state);
    }

    public void ToggleProgressCheckScreen(bool state)
    {
        UpdateRewardScreen();

        progressCheckScreen.SetActive(state);
    }

    public void ToggleMissionsTab(bool state)
    {
        missionsTab.SetActive(state);
    }

    public void StartMission()
    {
        StartCoroutine("LoadingScreen");
        ToggleMissionsTab(false);

        UpdateMissionParameters();
    }

    public void UpdateMissionParameters()
    {
        missionDescription.text = GameManager.instance.currentMission.missionDescription;
        missionIcon.sprite = GameManager.instance.currentMission.icon;

        switch (GameManager.instance.currentMission.missionType)
        {
            case 0:
                {
                    int remainingEnemies = GameManager.instance.missionEnemyAmount - GameManager.instance.remainingEnemies;
                    missionProgress.text = remainingEnemies.ToString()+"/" + GameManager.instance.missionEnemyAmount.ToString();
                }
                break;
        }
    }

    public IEnumerator LoadingScreen()
    {
        loadingScreen.SetActive(true);
        yield return new WaitForSeconds(loadingTime);
        loadingScreen.SetActive(false);
    }

    public void UpdateRewardScreen()
    {
        royalsRewardDisplay.text = GameManager.instance.currentMissionRoyalsReward.ToString();
        stardustRewardDisplay.text = GameManager.instance.currentMissionStardustReward.ToString();
    }

    public void ToggleCombatHUD(bool state)
    {
        combatHUD.SetActive(state);
    }

    public void ToggleMainMenu(bool state)
    {
        if (!GameManager.instance.isHome)
            combatHUD.SetActive(!state);

        generalHUD.SetActive(!state);
        mainMenu.SetActive(state);

        royalsCounter.text = GameManager.instance.save.royals.ToString();
        stardustCounter.text = GameManager.instance.save.stardust.ToString();

        ButtonManagement();
    } 
}
