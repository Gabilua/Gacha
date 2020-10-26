using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionManager : MonoBehaviour
{
    public Town currentTown;
    [SerializeField] RectTransform missionList;
    [SerializeField] GameObject missionListElement;

    private void Start()
    {
        DeleteExistingMissions();
        GenerateMissions(6);
    }

    void DeleteExistingMissions()
    {
        foreach (Transform child in missionList)
        {
            Destroy(child.gameObject);
        }
    }

    void GenerateMissions(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject element = Instantiate(missionListElement, missionList);
            element.GetComponent<MissionListElement>().SetupMissionUI(currentTown.townAreas[0]);
        }
    }
}
