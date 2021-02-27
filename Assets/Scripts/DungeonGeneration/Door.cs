using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Door : MonoBehaviour
{
    [SerializeField] GameObject obstacle, collider;

    public GameObject Obstacle { get { return obstacle; } }
    public GameObject Collider { get { return collider; } }
    public enum DoorType
    {
        left, right, top, bottom
    }


    public DoorType doorType;
}
