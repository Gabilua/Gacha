using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    public MissionManager missions;
    public PlayerController player;
    public Inventory playerInventory;
    public SaveData save;
    [SerializeField] Transform generatedLevel, townLevel, spawnedMobGroups, playerSpawn;
    [SerializeField] GameObject currentTownLevel;
    public Camera cam;

    [Header("Mission Generation")]
    [SerializeField] Mission[] availableMissions;
    public Mission currentMission;
    public Area currentMissionArea;
    public GameObject currentMissionUiElement;
    public float missionTileAmount;
    public int missionEnemyAmount;

    [HideInInspector] public int currentMissionRoyalsReward;
    [HideInInspector] public int currentMissionStardustReward;
    [HideInInspector] public int currentMissionExpReward;

    public bool isHome;
    bool levelGenerated;

    //Mission Parameters
    public bool missionSuccess;

    //-- Exterminate --
     public int remainingEnemies;
    //-- Exterminate --
     public int remainingWaves;

    #region Unity Methods
    private void Awake()
    {
        if (!instance)
            instance = this;

        playerInventory.Load();
    }

    private void OnEnable()
    {
        /*for(int i = 0; i < 10; i++)
        {
            playerInventory.AddRandomConsumable();
            playerInventory.AddRandomWeapon();
            playerInventory.AddRandomArtifact();
        }
        */
    }

    private void Update()
    {

    }

    private void OnApplicationQuit()
    {
        playerInventory.Save();
        playerInventory.ClearInventory();
    }
    #endregion

    #region General
    public IEnumerator HitStop(int i)
    {
        if(i >= 0)
        {
            Time.timeScale = 0;

            switch (i)
            {
                case 0:
                    yield return new WaitForSecondsRealtime(0.15f);
                    break;
                case 1:
                    yield return new WaitForSecondsRealtime(0.25f);
                    break;
                case 2:
                    yield return new WaitForSecondsRealtime(0.2f);
                    break;
                case 3:
                    yield return new WaitForSecondsRealtime(0.175f);
                    break;
                case 4:
                    yield return new WaitForSecondsRealtime(0.12f);
                    break;
            }

            Time.timeScale = 1;
        }
        else
            yield return new WaitForSecondsRealtime(0.15f);
    }
    public void DeployParty(int[] partyCharacterIDs)
    {
        for (int i = 0; i < player.combat.currentParty.Length; i++)
        {
            if (partyCharacterIDs[i] == -1)
                player.combat.currentParty[i] = null;
            else
                player.combat.currentParty[i] = player.combat.characterInfo[partyCharacterIDs[i]];
        }

        player.combat.ChangeActiveCharacter(player.combat.currentParty[0].ID);

        UIManager.instance.PopulatePartyUI();
        SaveCurrentParty();
    }
    public void TownChange(Town town)
    {
        DestroyTownLevel();
        UIManager.instance.StartCoroutine("LoadingScreen", 3f);
        missions.currentTown = town;
        CreateTownLevel();
        player.Spawn(playerSpawn);
    }
    void CreateTownLevel()
    {
        currentTownLevel = Instantiate(missions.currentTown.townLevel, townLevel.position, townLevel.rotation);
        currentTownLevel.transform.SetParent(townLevel);
    }
    void DestroyTownLevel()
    {
        if (currentTownLevel != null)
            Destroy(currentTownLevel);
    }
    #endregion

    #region TownManagement
    public int InnRestCost()
    {
        int restCost = 0;

        for (int i = 0; i < player.combat.currentParty.Length; i++)
        {
            if (player.combat.currentParty[i] != null)
                restCost += Mathf.FloorToInt(player.combat.currentParty[i].maxHP - player.combat.currentParty[i].currenthP);
        }

        restCost /= 2;
        return restCost;
    }

    public void InnRestHeal()
    {
        EarnRoyals(-InnRestCost());
        player.combat.FullPartyMaxHeal();
        UIManager.instance.ToggleInnScreen(false);

    }
    #endregion

    #region MissionManagement
    public IEnumerator StartMission(int i)
    {
        UIManager.instance.ToggleMissionsTab(false);
        yield return new WaitForSeconds(0.5f);
        SetupMission(i);
    }
    void SetupMission(int i)
    {
        currentMission = availableMissions[i];
        DestroyTownLevel();
        GenerateLevel();
        SetupMissionParameters();       

        UIManager.instance.StartMission();
        UIManager.instance.ToggleCombatHUD(true);
        UIManager.instance.UpdateHealthBar(player.combat.currentHealth, player.combat.maxHealth);

        isHome = false;
        player.isIdle = false;
        player.Spawn(playerSpawn);
    }
    public int CalculateRoyalRewards(Mission mission)
    {
        return currentMissionRoyalsReward = Mathf.FloorToInt((Random.Range(Mathf.FloorToInt(mission.royalsAmountInterval.x), Mathf.FloorToInt(mission.royalsAmountInterval.y))) * missions.currentTown.difficultyLevel);
    }
    public int CalculateStardustRewards(Mission mission)
    {
       return currentMissionStardustReward = Mathf.FloorToInt(Random.Range(Mathf.FloorToInt(mission.stardustAmountInterval.x), Mathf.FloorToInt(mission.stardustAmountInterval.y))*missions.currentTown.difficultyLevel);
    }
    public Area ChooseAreaForMissionBasedOnCurrentTown()
    {
        return missions.currentTown.townAreas[Random.Range(0, missions.currentTown.townAreas.Length)];
    }
    public Mission ChooseMissionTypeBasedOnCurrentTown()
    {
        return missions.currentTown.townMissions[Random.Range(0, missions.currentTown.townMissions.Length)];
    }
    public void EndMission()
    {
        UIManager.instance.StartCoroutine("LoadingScreen", 2f);
        UIManager.instance.ToggleHomeCheckScreen(false);
        UIManager.instance.ToggleProgressCheckScreen(false);

        if (missionSuccess)
        {
            Rewards();
            Destroy(currentMissionUiElement);
        }

        DestroyLevel();
        CreateTownLevel();

        UIManager.instance.ToggleMainMenu(false);
        UIManager.instance.ToggleCombatHUD(false);
       
        isHome = true;
        player.isIdle = true;
        player.Spawn(playerSpawn);
    }
    void Rewards()
    {
        StartCoroutine("EarnRoyalsDelayed", currentMissionRoyalsReward);
        StartCoroutine("EarnStardustDelayed", currentMissionStardustReward);
    }
    void GenerateLevel()
    {
        if(currentMission.missionType == 0)
        {
            missionTileAmount = Random.Range(Mathf.FloorToInt(currentMission.tileAmountInterval.x), Mathf.FloorToInt(currentMission.tileAmountInterval.y + Mathf.CeilToInt(missions.currentTown.difficultyLevel)));

            for (int i = 0; i < missionTileAmount; i++)
            {
                Vector3 tilePos = new Vector3((i * currentMissionArea.tileLenght) + transform.position.x, transform.position.y, transform.position.z);

                if (i == 0)
                {
                    GameObject tile = Instantiate(currentMissionArea.startTile, tilePos, transform.rotation);
                    tile.transform.SetParent(generatedLevel);
                }
                else if (i == (missionTileAmount - 1))
                {
                    GameObject tile = Instantiate(currentMissionArea.endTile, tilePos, transform.rotation);
                    tile.transform.SetParent(generatedLevel);

                    generatedLevel.GetComponent<NavMeshSurface>().BuildNavMesh();
                    levelGenerated = true;
                }
                else
                {
                    int r = Random.Range(0, currentMissionArea.tilePool.Length);
                    GameObject tile = Instantiate(currentMissionArea.tilePool[r], tilePos, transform.rotation);
                    tile.transform.SetParent(generatedLevel);
                }
            }

            if (levelGenerated)
                SpawnEnemies();
        }
        else if(currentMission.missionType == 1)
        {
            GameObject tile = Instantiate(currentMissionArea.defenseTile, transform.position, transform.rotation);
            tile.transform.SetParent(generatedLevel);

            generatedLevel.GetComponent<NavMeshSurface>().BuildNavMesh();
            SpawnEnemies();
        }
    }
    void SpawnEnemies()
    {
        if(currentMission.missionType == 0)
        {
            float currentLevelLenght = (missionTileAmount * currentMissionArea.tileLenght)-20;
            int missionMobAmount = Random.Range(currentMission.mobAmount + Mathf.FloorToInt(missions.currentTown.difficultyLevel), currentMission.mobAmount + Mathf.CeilToInt(missions.currentTown.difficultyLevel));

            for (int i = 0; i < missionMobAmount; i++)
            {
                Vector3 mobPos = new Vector3((i*(currentLevelLenght/ missionMobAmount)) +transform.position.x, transform.position.y, transform.position.z);

                int r = Random.Range(0, currentMission.mobGroups.Length);
                GameObject mob = Instantiate(currentMission.mobGroups[r], mobPos, transform.rotation);
                mob.transform.SetParent(spawnedMobGroups);
            }

            missionEnemyAmount = spawnedMobGroups.GetComponentsInChildren<EnemyController>().Length;
        }
        else if(currentMission.missionType == 1)
        {
           if(missionEnemyAmount <= 0)
            {
                Vector3 enemySpawnPosition = new Vector3(transform.position.x + 35, transform.position.y, transform.position.z);
                int r = Random.Range(0, currentMission.mobGroups.Length);

                GameObject mob = Instantiate(currentMission.mobGroups[r], enemySpawnPosition, transform.rotation);
                mob.transform.SetParent(spawnedMobGroups);

                remainingEnemies += spawnedMobGroups.GetComponentsInChildren<EnemyController>().Length;

                foreach (var enemy in spawnedMobGroups.GetComponentsInChildren<EnemyController>())
                {
                    enemy.objectiveTarget = player.transform;
                    enemy.hasObjective = true;
                }
            }
        }
    }
    void DestroyLevel()
    {
        foreach (Transform child in generatedLevel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in spawnedMobGroups.transform)
        {
            Destroy(child.gameObject);
        }

        ResetMissionParameters();
    }
    void ResetMissionParameters()
    {
        levelGenerated = false;
        missionTileAmount = 0;
        missionEnemyAmount = 0;
        currentMissionExpReward = 0;
        currentMissionRoyalsReward = 0;
        currentMissionStardustReward = 0;
        currentMissionUiElement = null;
        currentMission = null;

        missionSuccess = false;
    }
    void SetupMissionParameters()
    {
        switch (currentMission.missionType)
        {
            case 0:
                {
                    remainingEnemies = missionEnemyAmount;
                }
                break;
            case 1:
                {
                    remainingWaves = currentMission.waveNumber-1;
                }
                break;
        }
    }
    void CheckMissionProgress()
    {
        switch (currentMission.missionType)
        {
            case 0:
                {
                    if (remainingEnemies <= 0)
                        missionSuccess = true;
                }
                break;
            case 1:
                {
                    if (remainingWaves <= 0 && remainingEnemies <= 0)
                        missionSuccess = true;
                }
                break;
        }
    }
    public void EnemyKilled()
    {
        switch (currentMission.missionType)
        {
            case 0:
                {
                    remainingEnemies--;
                    UIManager.instance.UpdateMissionParameters();
                }
                break;
            case 1:
                {
                    remainingEnemies--;

                    if (remainingEnemies <= 0)
                    {
                        if (remainingWaves > 0)
                        {
                            Invoke("SpawnEnemies", Random.Range(3, 6));
                            remainingWaves--;
                        }
                    }

                    UIManager.instance.UpdateMissionParameters();
                }
                break;
        }

        CheckMissionProgress();
    }
    #endregion

    #region WritingToSave
    void SaveCurrentParty()
    {
        save.currentParty = player.combat.currentParty;
    }
    public IEnumerator EarnStardustDelayed(int amount)
    {
        yield return new WaitForSeconds(1.5f);
        EarnStardust(amount);
    }

    void EarnStardust(int amount)
    {
        save.stardust += amount;

        if (amount > 0)
            UIManager.instance.NewLootListElement(amount, "Stardust");
    }

    public IEnumerator EarnRoyalsDelayed(int amount)
    {
        yield return new WaitForSeconds(1.5f);
        EarnRoyals(amount);
    }

    void EarnRoyals(int amount)
    {
        save.royals += amount;

        if (amount > 0)
            UIManager.instance.NewLootListElement(amount, "Royals");
    }
    void EarnCharacterExperience(int i, int amount)
    {
        save.allCharacters[i].currentExperience += amount;
    }
    void LevelUpCharacter(int i)
    {
        save.allCharacters[i].level++;
    }
    void EarnWeaponExperience(int i, int amount)
    {
        save.allWeapons[i].currentExperience += amount;
    }
    void LevelUpWeapon(int i)
    {
        save.allWeapons[i].level++;
    }
    #endregion
}
