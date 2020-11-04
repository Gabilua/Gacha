using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("General HUD")]
    [SerializeField] GameObject generalHUD;
    [SerializeField] Image staminaBar;
    [SerializeField] Image healthBar;
    [SerializeField] TextMeshProUGUI healthValue;
    [SerializeField] TextMeshProUGUI currentCharacterLevelDisplay;
    [SerializeField] TextMeshProUGUI royalsCounter;
    [SerializeField] TextMeshProUGUI stardustCounter;
    [SerializeField] GameObject partyCharacterUIPrefab, characterScreenRosterElementPrefab, damageDisplayPrefab;
    public RectTransform partyCharacters;

    [Header("Combat HUD")]
    [SerializeField] GameObject combatHUD;
    [SerializeField] TextMeshProUGUI missionDescription, missionProgress, innRestCost;
    [SerializeField] Image missionIcon;

    [Header("Menu Screens")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject missionsTab, loadingScreen, homeCheckScreen, progressCheckScreen, innScreen, characterScreen;
    [SerializeField] GameObject homeButton, innRestButton;
    [SerializeField] TextMeshProUGUI royalsRewardDisplay;
    [SerializeField] TextMeshProUGUI stardustRewardDisplay;
    public RectTransform missionList, characterScreenRoster;
    [SerializeField] Animator characterScreenThumbDisplay;
    [SerializeField] GameObject[] characterScreenAvatars;
    [SerializeField] GameObject[] characterScreenSessionContent;
    [SerializeField] TextMeshProUGUI[] characterScreenCharStats;
    [SerializeField] TextMeshProUGUI[] characterScreenWpnStats;
    [SerializeField] Image characterScreenStarDisplay;
    [SerializeField] Image weaponScreenStarDisplay;

    [Header("Configurations")]
    [SerializeField] float loadingTime;
    public Color[] elementColors;

    float staminaBarFadeTimer;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    private void Start()
    {
        PopulatePartyUI();
        PopulateCharacterScreenRoster();
    }

    void ButtonManagement()
    {
        if (GameManager.instance.isHome)
            homeButton.SetActive(false);
        else
            homeButton.SetActive(true);
    }

    void PopulatePartyUI()
    {
        for (int i = 0; i < GameManager.instance.player.GetComponent<PlayerCombat>().currentParty.Length; i++)
        {
            if(GameManager.instance.player.GetComponent<PlayerCombat>().currentParty[i] != null && GameManager.instance.player.GetComponent<PlayerCombat>().currentParty[i] != GameManager.instance.player.GetComponent<PlayerCombat>().activeCharacterInfo)
            {
                NewCharacterPartyUIElement(GameManager.instance.player.GetComponent<PlayerCombat>().currentParty[i]);
            }
        }
    }

    public void NewCharacterPartyUIElement(Character character)
    {
        GameObject ui = Instantiate(partyCharacterUIPrefab, partyCharacters);
        ui.GetComponent<PartyCharacterDisplay>().myCharacter = character;
        ui.GetComponent<PartyCharacterDisplay>().SetupElement();
    }

    public void SpawnDamageDisplay(Vector3 where, float amount)
    {
        GameObject ui = Instantiate(damageDisplayPrefab, where, damageDisplayPrefab.transform.rotation);
        ui.GetComponentInChildren<TextMeshProUGUI>().text = amount.ToString("F0");
        Destroy(ui, 2f);
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentValue / maxValue, 10 * Time.deltaTime);
        healthValue.text = currentValue.ToString("F0")+" / "+maxValue.ToString("F0");
    }

    public void UpdateStaminaBar(float currentValue, float maxValue)
    {
        staminaBar.fillAmount = Mathf.Lerp(staminaBar.fillAmount, currentValue / maxValue, 10 * Time.deltaTime);

        if (staminaBar.fillAmount >= 1)
        {
            staminaBarFadeTimer += Time.deltaTime;

            if(staminaBarFadeTimer >= 1.5f)
            {
                staminaBar.transform.parent.gameObject.SetActive(false);
            }
        }
        else
        {
            staminaBarFadeTimer = 0;
            staminaBar.transform.parent.gameObject.SetActive(true);
        }
    }

    public void UpdateCurrentCharacterLevelDisplay(int characterLevel)
    {
        currentCharacterLevelDisplay.text = "Lv."+characterLevel.ToString();
    }

    public void ToggleInnScreen(bool state)
    {
        innRestCost.text = GameManager.instance.InnRestCost().ToString();

        if (GameManager.instance.InnRestCost() > 0)
        {
            if (GameManager.instance.InnRestCost() <= GameManager.instance.save.royals)
                innRestButton.SetActive(true);
            else
                innRestButton.SetActive(false);
        }
        else
            innRestButton.SetActive(false);

        innScreen.SetActive(state);
    }

    public void ToggleHomeCheckScreen(bool state)
    {
        homeCheckScreen.SetActive(state);
    }

    public void ToggleCharacterScreen()
    {
        characterScreen.SetActive(!characterScreen.activeInHierarchy);

        UpdateCharacterScreenAvatar(System.Array.IndexOf(GameManager.instance.player.combat.characterInfo, GameManager.instance.player.combat.activeCharacterInfo));
        UpdateCharacterScreenSection(0);
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

        if (characterScreen.activeInHierarchy)
            characterScreen.SetActive(false);

        generalHUD.SetActive(!state);
        mainMenu.SetActive(state);

        royalsCounter.text = GameManager.instance.save.royals.ToString();
        stardustCounter.text = GameManager.instance.save.stardust.ToString();

        ButtonManagement();
    } 

    public void UpdateCharacterScreenSection(int i)
    {
        characterScreenThumbDisplay.SetInteger("CurrentScreen", i);

        if (i == 0)
        {
            foreach (var avatar in characterScreenAvatars)
            {
                if (avatar.activeInHierarchy)
                    avatar.GetComponent<EquipmentManager>().Sheath();
            }
        }
        else if (i == 1)
        {
            foreach (var avatar in characterScreenAvatars)
            {
                if (avatar.activeInHierarchy)
                    avatar.GetComponent<EquipmentManager>().Unsheath();
            }
        }

        foreach (var content in characterScreenSessionContent)
        {
            content.SetActive(false);
        }

        characterScreenSessionContent[i].SetActive(true);
    }

    public void UpdadeCharacterScreenSectionContent(int i)
    {
        // update character stats
        characterScreenCharStats[0].text = GameManager.instance.player.combat.characterInfo[i].name.ToString();
        characterScreenCharStats[1].text = "Lv."+GameManager.instance.player.combat.characterInfo[i].level.ToString()+"/90";
        characterScreenStarDisplay.fillAmount = GameManager.instance.player.combat.characterInfo[i].stars/5;
        characterScreenCharStats[2].text = GameManager.instance.player.combat.characterInfo[i].maxHP.ToString();
        characterScreenCharStats[3].text = GameManager.instance.player.combat.characterInfo[i].atk.ToString();
        characterScreenCharStats[4].text = GameManager.instance.player.combat.characterInfo[i].def.ToString();
        characterScreenCharStats[5].text = GameManager.instance.player.combat.characterInfo[i].skill.ToString();
        characterScreenCharStats[6].text = GameManager.instance.player.combat.characterInfo[i].critRate.ToString();
        characterScreenCharStats[7].text = GameManager.instance.player.combat.characterInfo[i].critDmg.ToString();
        characterScreenCharStats[8].text = GameManager.instance.player.combat.characterInfo[i].recharge.ToString();


        // update weapon stats
        characterScreenWpnStats[0].text = GameManager.instance.player.combat.characterInfo[i].currentWeapon.name;
        characterScreenWpnStats[1].text = GameManager.instance.player.combat.characterInfo[i].currentWeapon.level.ToString();
        weaponScreenStarDisplay.fillAmount = GameManager.instance.player.combat.characterInfo[i].currentWeapon.stars / 5f;
        characterScreenWpnStats[2].text = "MainStat";
        characterScreenWpnStats[3].text = "SubStat";
    }

    public void UpdateCharacterScreenAvatar(int i)
    {
        characterScreenThumbDisplay.avatar = GameManager.instance.player.combat.characterInfo[i].animatorAvatar;
        characterScreenThumbDisplay.SetFloat("WeaponType", GameManager.instance.player.combat.characterInfo[i].weaponType);
        characterScreenAvatars[i].GetComponent<EquipmentManager>().EquipmentSetup(i);

        foreach (var avatar in characterScreenAvatars)
        {
            avatar.gameObject.SetActive(false);
        }

        characterScreenAvatars[i].SetActive(true);

        UpdadeCharacterScreenSectionContent(i);
    }

    void PopulateCharacterScreenRoster()
    {
        for (int i = 0; i < GameManager.instance.player.GetComponent<PlayerCombat>().characterInfo.Length; i++)
        {
            if (GameManager.instance.player.GetComponent<PlayerCombat>().characterInfo[i].level > 0)
            {
                NewCharacterScreenRosterElement(GameManager.instance.player.GetComponent<PlayerCombat>().characterInfo[i]);
            }
        }
    }

    public void NewCharacterScreenRosterElement(Character character)
    {
        GameObject ui = Instantiate(characterScreenRosterElementPrefab, characterScreenRoster);
        ui.GetComponent<CharacterScreenRosterElement>().myCharacter = character;
        ui.GetComponent<CharacterScreenRosterElement>().SetupElement();
    }
}
