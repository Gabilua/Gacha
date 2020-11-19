using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public Town currentTown;
    [SerializeField] GameObject missionListElement;

    private void Start()
    {
        ResetMissions();
    }

    public void ResetMissions()
    {
        DeleteExistingMissions();
        GenerateMissions(5);
    }

    void DeleteExistingMissions()
    {
        foreach (Transform child in UIManager.instance.missionList)
        {
            Destroy(child.gameObject);
        }
    }

    public void GenerateMissions(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject element = Instantiate(missionListElement, UIManager.instance.missionList);
            element.GetComponent<MissionListElement>().SetupMissionUI();
        }
    }
}
