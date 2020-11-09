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
    [HideInInspector] public int remainingEnemies;

    #region Unity Methods
    private void Awake()
    {
        if (!instance)
            instance = this;
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

    #endregion

    #region TownManagement
    public int InnRestCost()
    {
        int restCost = 0;

        for (int i = 0; i < player.combat.currentParty.Length; i++)
        {
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
        GenerateLevel();
        SetupMissionParameters();       

        UIManager.instance.StartMission();
        UIManager.instance.ToggleCombatHUD(true);
        UIManager.instance.UpdateHealthBar(player.combat.currentHealth, player.combat.maxHealth);

        if (currentTownLevel != null)
            Destroy(currentTownLevel);    

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
    public void EndMission()
    {
        UIManager.instance.StartCoroutine("LoadingScreen");
        UIManager.instance.ToggleHomeCheckScreen(false);
        UIManager.instance.ToggleProgressCheckScreen(false);

        if (missionSuccess)
        {
            Rewards();
            Destroy(currentMissionUiElement);
        }

        DestroyLevel();

        UIManager.instance.ToggleMainMenu(false);
        UIManager.instance.ToggleCombatHUD(false);

        if (currentTownLevel == null)
            currentTownLevel = Instantiate(missions.currentTown.townLevel, townLevel.position, townLevel.rotation);

        isHome = true;
        player.isIdle = true;
        player.Spawn(playerSpawn);
    }
    void Rewards()
    {       
        EarnRoyals(currentMissionRoyalsReward);
        EarnStardust(currentMissionStardustReward);
    }
    void GenerateLevel()
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
            else if(i == (missionTileAmount - 1))
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
        }

        CheckMissionProgress();
    }
    #endregion

    #region WritingToSave
    void SaveCurrentParty()
    {
        save.currentParty = player.combat.currentParty;
    }

    void EarnStardust(int amount)
    {
        save.stardust += amount;
    }
    void EarnRoyals(int amount)
    {
        save.royals += amount;
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
