using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJ_Spell : MonoBehaviour
{
    [SerializeField] ParticleSystem fx;
    [SerializeField] Animator animator;
    [SerializeField] protected int damage;
    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] SpellType spellType;
    public SpellType TypeSpell { get { return spellType; } } 
    protected void Start()
    {
        if(fx)
            Instantiate(fx, transform.position, transform.rotation);
    }


}

public enum SpellType
{
    Line,
    Wall,
    Cone,
    Area,
    Nova
}