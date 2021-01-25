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
    [SerializeField] GameObject partyCharacterUIPrefab, characterScreenRosterElementPrefab, partyScreenRosterElementPrefab, damageDisplayPrefab, lootListElementPrefab, inventorySlotPrefab;
    public RectTransform partyCharacters;

    [Header("Combat HUD")]
    [SerializeField] GameObject combatHUD;
    [SerializeField] TextMeshProUGUI missionDescription, missionProgress, innRestCost;
    [SerializeField] Image missionIcon;

    [Header("Menu Screens")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject missionsTab, loadingScreen, homeCheckScreen, progressCheckScreen, innScreen, characterScreen, partyScreen, mapScreen;
    [SerializeField] GameObject homeButton, innRestButton, mapTravelButton;
    [SerializeField] TextMeshProUGUI missionTabTownName, innScreenTownName;
    [SerializeField] TextMeshProUGUI royalsRewardDisplay;
    [SerializeField] TextMeshProUGUI stardustRewardDisplay;
    public RectTransform missionList, characterScreenRoster, partyScreenRoster, lootList;
    [SerializeField] Animator characterScreenThumbDisplay;
    [SerializeField] GameObject[] characterScreenAvatars;
    [SerializeField] GameObject[] characterScreenSessionContent;
    [SerializeField] TextMeshProUGUI[] characterScreenCharStats;
    [SerializeField] TextMeshProUGUI[] characterScreenWpnStats;
    [SerializeField] Image characterScreenStarDisplay;
    [SerializeField] Image weaponScreenStarDisplay;
    [SerializeField] GameObject[] partyScreenAvatars;
    [SerializeField] TextMeshProUGUI[] partyScreenAvatarNames;
    [SerializeField] GameObject[] partyScreenAvatarSlotCam;
    [SerializeField] RectTransform[] partyScreenDisplaySlots;
    [SerializeField] int[] temporaryPartyComposition;
    [SerializeField] MapIcon[] mapIcons;
    [SerializeField] TextMeshProUGUI mapTownName, mapTownDescription;
    [SerializeField] RectTransform characterScreenInventoryHolder;
    [SerializeField] GameObject characterInventorySection;
    [SerializeField] InventoryWeaponPanel _inventoryWeaponPanel;

    [Header("Configurations")]
    public Color[] elementColors;

    float staminaBarFadeTimer;
    RectTransform currentDraggingElement;
    Town currentMapIconTownSelected;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    private void Start()
    {
        PopulatePartyUI();
        PopulateCharacterScreenRoster();
        PopulatePartyScreenRoster();
    }

    private void Update()
    {
        if (currentDraggingElement != null)
            currentDraggingElement.position = Input.mousePosition;
    }

    void ButtonManagement()
    {
        if (GameManager.instance.isHome)
            homeButton.SetActive(false);
        else
            homeButton.SetActive(true);
    }

    public void PopulatePartyUI()
    {
        foreach (Transform child in partyCharacters)
        {
            Destroy(child.gameObject);
        }

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
        innScreenTownName.text = GameManager.instance.missions.currentTown.townName+" Inn";

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

        Character activeCharacter = GameManager.instance.player.combat.activeCharacterInfo;
        UpdateCharacterScreenAvatar(System.Array.IndexOf(GameManager.instance.player.combat.characterInfo, activeCharacter));
        UpdateCharacterScreenSection(0);
        UpdateCharacterWeaponIventory(activeCharacter.weaponType);
    }

    public void ToggleMapScreen(bool state)
    {
        foreach (var icon in mapIcons)
        {
            if (icon.myTown.unlocked)
                icon.gameObject.SetActive(true);
            else
                icon.gameObject.SetActive(false);
        }

        currentMapIconTownSelected = null;
        UpdateMapScreenInfo(GameManager.instance.missions.currentTown);

        mapScreen.SetActive(state);
    }

    public void UpdateMapScreenInfo(Town town)
    {
        currentMapIconTownSelected = town;

        if (town == GameManager.instance.missions.currentTown)
            mapTravelButton.SetActive(false);
        else
            mapTravelButton.SetActive(true);

        mapTownName.text = town.townName;
        mapTownDescription.text = town.townDescription;
    }

    public void ChooseMapIcon()
    {
        GameManager.instance.TownChange(currentMapIconTownSelected);
        ToggleMapScreen(false);
    }

    public void TogglePartyScreen(bool state)
    {
        for (int i = 0; i < temporaryPartyComposition.Length; i++)
        {
            if (GameManager.instance.player.combat.currentParty[i] == null)
                temporaryPartyComposition[i] = -1;
            else
                temporaryPartyComposition[i] = GameManager.instance.player.combat.currentParty[i].ID;
        }

        UpdatePartyScreenAvatarDisplay();
        partyScreen.SetActive(state);
        ToggleMainMenu(false);
    }

    public void ToggleProgressCheckScreen(bool state)
    {
        UpdateRewardScreen();

        progressCheckScreen.SetActive(state);
    }

    public void ToggleMissionsTab(bool state)
    {
        missionTabTownName.text = GameManager.instance.missions.currentTown.townName;
        missionsTab.SetActive(state);
    }

    public void StartMission()
    {
        StartCoroutine("LoadingScreen", 2f);
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
            case 1:
                {
                    int remainingWaves = GameManager.instance.currentMission.waveNumber - GameManager.instance.remainingWaves;
                    missionProgress.text = remainingWaves.ToString() + "/" + GameManager.instance.currentMission.waveNumber.ToString();
                }
                break;
        }
    }

    public IEnumerator LoadingScreen(float loadingTime)
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
        if (!GameManager.instance.isHome && !combatHUD.activeInHierarchy)
            combatHUD.SetActive(true);

        if (!state && characterScreen.activeInHierarchy)
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

            ToggleCharacterInventorySection(false);
        }
        else if (i == 1)
        {
            foreach (var avatar in characterScreenAvatars)
            {
                if (avatar.activeInHierarchy)
                    avatar.GetComponent<EquipmentManager>().Unsheath();
            }

            ToggleCharacterInventorySection(true);
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
        characterScreenCharStats[0].text = GameManager.instance.player.combat.characterInfo[i].characterName.ToString();
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
        characterScreenThumbDisplay.SetFloat("WeaponType", (int)GameManager.instance.player.combat.characterInfo[i].weaponType);
        characterScreenAvatars[i].GetComponent<EquipmentManager>().EquipmentSetup(i);

        foreach (var avatar in characterScreenAvatars)
        {
            avatar.gameObject.SetActive(false);
        }

        characterScreenAvatars[i].SetActive(true);

        UpdadeCharacterScreenSectionContent(i);
    }

    public void UpdateCharacterWeaponIventory(WeaponType p_weaponType) => _inventoryWeaponPanel.UpdateCharacterInfo(p_weaponType);

    void PopulateCharacterScreenRoster()
    {
        foreach (Transform child in characterScreenRoster)
        {
            Destroy(child.gameObject);
        }

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

    void PopulatePartyScreenRoster()
    {
        foreach (Transform child in partyScreenRoster)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < GameManager.instance.player.GetComponent<PlayerCombat>().characterInfo.Length; i++)
        {
            if (GameManager.instance.player.GetComponent<PlayerCombat>().characterInfo[i].level > 0)
            {
                NewPartyScreenRosterElement(GameManager.instance.player.GetComponent<PlayerCombat>().characterInfo[i]);
            }
        }
    }

    public void NewPartyScreenRosterElement(Character character)
    {
        GameObject ui = Instantiate(partyScreenRosterElementPrefab, partyScreenRoster);
        ui.GetComponent<PartyScreenRosterElement>().myCharacter = character;
        ui.GetComponent<PartyScreenRosterElement>().SetupElement();
    }

    public void AttachElementToCursor(GameObject element)
    {
        DettachElementFromCursor();

        currentDraggingElement = element.GetComponent<RectTransform>();
    }

    public void DettachElementFromCursor()
    {
        if (currentDraggingElement != null)
            Destroy(currentDraggingElement.gameObject);
    }

    public void DropRosterElementOnDisplaySlot(int id)
    {
        //Runs through all UI character slots
        for (int i = 0; i < partyScreenDisplaySlots.Length; i++)
        {
            //Checks if the dragged icon is being dropped on top of any UI slot
            if (GetWorldSapceRect(partyScreenDisplaySlots[i]).Contains(GetWorldSapceRect(currentDraggingElement).position))
            {
                //Runs through the temporaryParty array of character IDs
                for (int j = 0; j < temporaryPartyComposition.Length; j++)
                {
                    //Checks if the dropped character ID is already present on any temporaryParty slots
                    if (temporaryPartyComposition[j] == id)
                    {
                        //Clears up the previous slot containing the character
                        temporaryPartyComposition[j] = -1;

                        //Checks if the repeated character is being dropped on top of an occupied slot
                        if (temporaryPartyComposition[i] != -1)
                        {
                            //Places character present on picked slot to the slot previously occupied by the repeated character
                            temporaryPartyComposition[j] = temporaryPartyComposition[i];
                        }
                    }                    
                }

                //Places picked character on chosen slot and updates the temporaryParty array's position with its ID
                temporaryPartyComposition[i] = id;

                UpdatePartyScreenAvatarDisplay();
            }
        }
    }

    Rect GetWorldSapceRect(RectTransform rt)
    {
        var r = rt.rect;
        r.center = rt.TransformPoint(r.center);
        r.size = rt.TransformVector(r.size);
        return r;
    }

    void UpdatePartyScreenAvatarDisplay()
    {
        for (int i = 0; i < temporaryPartyComposition.Length; i++)
        {
            PlaceCameraOnSpecifiedCharacter(temporaryPartyComposition[i], i);
        }
    }

    public void NewLootListElement(int amount, string name)
    {
        GameObject ui = Instantiate(lootListElementPrefab, lootList);
        ui.GetComponent<LootDisplayUI>().SetupLootUI(amount, name);
    }

    public void PlaceCameraOnSpecifiedCharacter(int avatarIndex, int slotIndex)
    {      
        partyScreenAvatarSlotCam[slotIndex].transform.position = partyScreenAvatars[avatarIndex+1].transform.position;

        if (avatarIndex < 0)
            partyScreenAvatarNames[slotIndex].text = " ";
        else
            partyScreenAvatarNames[slotIndex].text = GameManager.instance.player.combat.characterInfo[avatarIndex].characterName;
    }

    public void Deploy()
    {
        GameManager.instance.DeployParty(temporaryPartyComposition);
        TogglePartyScreen(false);
    }

    void ToggleCharacterInventorySection(bool state)
    {
        characterInventorySection.SetActive(state);
    }

    public void NewCharacterScreenInventorySlot(Weapon weapon)
    {
        GameObject ui = Instantiate(inventorySlotPrefab, characterScreenInventoryHolder);
        ui.GetComponent<InventorySlot>().myWeapon = weapon;
        ui.GetComponent<InventorySlot>().SetupSlot();
    }
}
