using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionListElement : MonoBehaviour
{
    public Mission myMission;

    public Image icon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public TextMeshProUGUI royalReward;
    public TextMeshProUGUI stardustReward;
    public TextMeshProUGUI expReward;

    public int calculatedRoyalsReward;
    public int calculatedStardustReward;

    public Area missionArea;

    public void ClickElement()
    {
        GameManager.instance.currentMissionArea = missionArea;
        GameManager.instance.StartCoroutine("StartMission", myMission.missionType);

        GameManager.instance.currentMissionRoyalsReward = calculatedRoyalsReward;
        GameManager.instance.currentMissionStardustReward = calculatedStardustReward;

        GameManager.instance.currentMissionUiElement = gameObject;
    }

    public void SetupMissionUI()
    {
        myMission = GameManager.instance.ChooseMissionTypeBasedOnCurrentTown();
        missionArea = GameManager.instance.ChooseAreaForMissionBasedOnCurrentTown();
        icon.sprite = myMission.icon;
        icon.color = myMission.iconColor;
        title.text = missionArea.areaName + " - "+ myMission.missionTitle;
        description.text = myMission.missionDescription;

        calculatedRoyalsReward = GameManager.instance.CalculateRoyalRewards(myMission);
        calculatedStardustReward = GameManager.instance.CalculateStardustRewards(myMission);

        royalReward.text = calculatedRoyalsReward.ToString();
        stardustReward.text = calculatedStardustReward.ToString();
        //expReward.text = myMission.missionTitle;
    }
}
