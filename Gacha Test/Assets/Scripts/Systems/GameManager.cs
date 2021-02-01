using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    public MissionManager missions;
    public PlayerController player;
    public Inventory playerInventory;
    public SaveData save;
    [SerializeField] Transform generatedLevel, townLevel, spawnedMobGroups, playerSpawn;
    [SerializeField] Vector3[] gridTilePositions;
    [SerializeField] Vector3[] generatedTilePositions;
    int lastDir = -1;
    [SerializeField] GameObject currentTownLevel, tileGenerator;
    public Camera cam;

    [Header("Mission Generation")]
    [SerializeField] Mission[] availableMissions;
    public Mission currentMission;
    public Area currentMissionArea;
    public GameObject currentMissionUiElement;
    public int missionTileAmount;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerInventory.Save();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            playerInventory.ClearInventory();
        }
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
        missions.ResetMissions();
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
       // GenerateLevel();
        GenerateGridLevel();
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

    void GenerateGridLevel()
    {
        int r = Random.Range(0, currentMission.gridSize);
        tileGenerator.transform.position = new Vector3(tileGenerator.transform.position.x + (r * currentMissionArea.tileSize), tileGenerator.transform.position.y, tileGenerator.transform.position.z);

        generatedTilePositions = new Vector3[currentMission.gridSize * currentMission.gridSize];
        gridTilePositions = new Vector3[generatedTilePositions.Length];

        generatedTilePositions[0] = tileGenerator.transform.position;
        playerSpawn.position = generatedTilePositions[0];

        GameObject tile = Instantiate(currentMissionArea.gridTile, tileGenerator.transform.position, transform.rotation);
        tile.transform.SetParent(generatedLevel);

        for (int i = 0; i < generatedTilePositions.Length; i++)
        {
            if (levelGenerated)
                break;
            else
            {
                float d = Random.value*100;
                int dir = 1;

                if (d <= 5)
                    dir = 2;
                else
                    dir = Random.Range(0,2);

                MoveGridGeneration(dir);

                if (i > 0)
                    generatedTilePositions[i] = tileGenerator.transform.position;

                if(i < generatedLevel.childCount-1)
                {
                    if (lastDir == 0)
                    {
                        generatedLevel.GetChild(i).GetComponent<GridTile>().rightOpen = true;
                        generatedLevel.GetChild(i + 1).GetComponent<GridTile>().leftOpen = true;

                    }
                    if (lastDir == 1)
                    {
                        generatedLevel.GetChild(i).GetComponent<GridTile>().leftOpen = true;
                        generatedLevel.GetChild(i + 1).GetComponent<GridTile>().rightOpen = true;
                    }
                    if (lastDir == 2)
                    {
                        generatedLevel.GetChild(i).GetComponent<GridTile>().downOpen = true;
                        generatedLevel.GetChild(i + 1).GetComponent<GridTile>().upOpen = true;
                    }

                    generatedLevel.GetChild(i).GetComponent<GridTile>().SetupOpenings();
                    generatedLevel.GetChild(i + 1).GetComponent<GridTile>().SetupOpenings();
                }
            }
        }

        if (levelGenerated)
        {
            FillNonCritical();
        }
    }

    void FillNonCritical()
    {   /*
        for (int i = 0; i < gridTilePositions.Length; i++)
        {
            if(Math.Abs(transform.position.z - (currentMissionArea.tileSize*i)) <= (currentMissionArea.tileSize * (currentMission.gridSize - 1)))
            {
                gridTilePositions[i] = (gridTilePositions[i].x, transform.position.y, transform.position.z - (currentMissionArea.tileSize * i));
            }
            if (transform.position.x + (currentMissionArea.tileSize * i) <= (currentMissionArea.tileSize * (currentMission.gridSize - 1)))
            {
                gridTilePositions[i] = (transform.position.x + (currentMissionArea.tileSize * i), transform.position.y, gridTilePositions[i].z);
            }
        }

        
        GameObject nonCriticalTile = Instantiate(currentMissionArea.gridTile, position, transform.rotation);
        nonCriticalTile.transform.SetParent(generatedLevel);
        nonCriticalTile.GetComponent<GridTile>().ShuffleOpenings();
        */
    }

    void MoveGridGeneration(int dir)
    {
        //go right
        if(dir == 0)
        {
            if (lastDir == 1)
            {
                GoDown();
            }
            else
            {
                GoRight();
            }            
        }
        //go left
        if (dir == 1)
        {
            if (lastDir == 0)
            {
                GoDown();
            }
            else
            {
                GoLeft();
            }
        }
        //go down
        if (dir == 2)
        {
            GoDown();
        }  

        if (!levelGenerated)
        {
            GameObject tile = Instantiate(currentMissionArea.gridTile, tileGenerator.transform.position, transform.rotation);
            tile.transform.SetParent(generatedLevel);
        }
    }

    void GoRight()
    {
        if (tileGenerator.transform.position.x + currentMissionArea.tileSize <= (currentMissionArea.tileSize * (currentMission.gridSize - 1)))
        {
            Vector3 pos = new Vector3(tileGenerator.transform.position.x + currentMissionArea.tileSize, tileGenerator.transform.position.y, tileGenerator.transform.position.z);
            Vector3 tilePos = pos;
            tileGenerator.transform.position = tilePos;

            lastDir = 0;
        }
        else
        {
            GoDown();
        }
    }

    void GoLeft()
    {
        if (tileGenerator.transform.position.x - currentMissionArea.tileSize > 0)
        {
            Vector3 pos = new Vector3(tileGenerator.transform.position.x - currentMissionArea.tileSize, tileGenerator.transform.position.y, tileGenerator.transform.position.z);
            Vector3 tilePos = pos;
            tileGenerator.transform.position = tilePos;

            lastDir = 1;
        }
        else
        {
            GoDown();
        }
    }

     void GoDown()
    {
        if (Math.Abs(tileGenerator.transform.position.z - currentMissionArea.tileSize) <= (currentMissionArea.tileSize * (currentMission.gridSize - 1)))
        {
            Vector3 tilePos = new Vector3(tileGenerator.transform.position.x, tileGenerator.transform.position.y, tileGenerator.transform.position.z - currentMissionArea.tileSize);
            tileGenerator.transform.position = tilePos;
        }
        else
            levelGenerated = true;

        lastDir = 2;
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

    #region InventoryManagement
    public void GetBannerDrop(ScriptableObject p_equipament)
    {
        Equipament equipament = p_equipament as Equipament;

        Type type = p_equipament.GetType();
        if (type.Equals(typeof(Weapon)))
        {
            Weapon weapon = new Weapon();
            weapon = equipament as Weapon;
            weapon._Init_();
            playerInventory.AddWeapon(weapon);
        }
        else if (type.Equals(typeof(Artifact)))
        {
            Artifact artifact = new Artifact();
            artifact = equipament as Artifact;
            artifact._Init_();
            playerInventory.AddArtifact(artifact);
        }
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
