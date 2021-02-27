using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/CreateEnemyData", order = 1)]
public class BJ_EnemyData : ScriptableObject
{
    public LayerMask PlayerLayer;
    public LayerMask AllyLayer;

    public int CacHealth = 100;
    public int DistHealth = 100;
    public int HealHealth = 100;

    public float DetectionRange = 10;

    public float CacDistanceMaxHit = 1;
    public float DistRangeMaxHit = 10;
    public float HealRangeMaxHeal = 7.5f;

    public int CacValue = 100;
    public int CacFatValue = 250;
    public int DistValue = 175;
    public int HealValue = 300;

    public float CooldownDuration = 2;
}
