using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJ_NextFloor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BJ_Player>())
            BJ_GameManager.I.NextFloor();
    }
}
