using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapIcon : MonoBehaviour
{
    public Town myTown;

    public void Click()
    {
        UIManager.instance.UpdateMapScreenInfo(myTown);
    }
}
