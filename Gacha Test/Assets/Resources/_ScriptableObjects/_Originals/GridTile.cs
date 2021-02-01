using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTile : MonoBehaviour
{
    [SerializeField] GameObject upOpening, downOpening, leftOpening, rightOpening;
    public bool upOpen, downOpen, leftOpen, rightOpen;

    public void ShuffleOpenings()
    {
        if (Random.value > 0.5f)
            upOpening.SetActive(false);
        if (Random.value > 0.5f)
            upOpening.SetActive(false);
        if (Random.value > 0.5f)
            upOpening.SetActive(false);
        if (Random.value > 0.5f)
            upOpening.SetActive(false);
    }

    public void SetupOpenings()
    {
        if (upOpen)
            upOpening.SetActive(false);
        if (downOpen)
            downOpening.SetActive(false);
        if (leftOpen)
            leftOpening.SetActive(false);
        if (rightOpen)
            rightOpening.SetActive(false);
    }
}
