using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nodeData : MonoBehaviour
{   
    public bool isSmall, isMedium, isLarge;
    public bool isCorridor, isRoom, isCap;

    public bool canConnectToSmall, canConnectToMedium, canConnectToLarge;
    public bool canConnectToCorridor, canConnectToRoom, canConnectToCap;

    public bool connectedWithAnotherNode = false;
}
