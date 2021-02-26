using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum DoorType
    {
        left1, left2, right1, right2, top1, top2, bottom1, bottom2
    }

    public DoorType doorType;
}
