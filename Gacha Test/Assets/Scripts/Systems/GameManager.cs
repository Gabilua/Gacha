using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    public PlayerController player;
    public SaveData save;
    [SerializeField] Transform generatedLevel, spawnedMobGroups, playerSpawn;
    [SerializeField] GameObject townLevel;
    public Camera cam;

    [Header("Mission Generation")]
    [SerializeField] Mission[] availableMissions;
    public Mission currentMission;
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
        townLevel.SetActive(false);

        isHome = false;
        player.isIdle = false;
        player.Spawn(playerSpawn);
    }
    public int CalculateRoyalRewards(Mission mission)
    {
        return currentMissionRoyalsReward = Random.Range(Mathf.FloorToInt(mission.royalsAmountInterval.x), Mathf.FloorToInt(mission.royalsAmountInterval.y));
    }
    public int CalculateStardustRewards(Mission mission)
    {
       return currentMissionStardustReward = Random.Range(Mathf.FloorToInt(mission.stardustAmountInterval.x), Mathf.FloorToInt(mission.stardustAmountInterval.y));
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
        townLevel.SetActive(true);

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
        missionTileAmount = Random.Range(Mathf.FloorToInt(currentMission.tileAmountInterval.x), Mathf.FloorToInt(currentMission.tileAmountInterval.y));

        for (int i = 0; i < missionTileAmount; i++)
        {
            Vector3 tilePos = new Vector3((i * currentMission.tileLenght) + transform.position.x, transform.position.y, transform.position.z);

            if (i == 0)
            {
                GameObject tile = Instantiate(currentMission.startTile, tilePos, transform.rotation);
                tile.transform.SetParent(generatedLevel);
            }
            else if(i == (missionTileAmount - 1))
            {
                GameObject tile = Instantiate(currentMission.endTile, tilePos, transform.rotation);
                tile.transform.SetParent(generatedLevel);

                generatedLevel.GetComponent<NavMeshSurface>().BuildNavMesh();
                levelGenerated = true;
            }
            else
            {
                int r = Random.Range(0, currentMission.tilePool.Length); 
                GameObject tile = Instantiate(currentMission.tilePool[r], tilePos, transform.rotation);
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
            float currentLevelLenght = (missionTileAmount * currentMission.tileLenght)-20;

            for (int i = 0; i < currentMission.mobAmount; i++)
            {
                Vector3 mobPos = new Vector3((i*(currentLevelLenght/currentMission.mobAmount)) +transform.position.x, transform.position.y, transform.position.z);

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
