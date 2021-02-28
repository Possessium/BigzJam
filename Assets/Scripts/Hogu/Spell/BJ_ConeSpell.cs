using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJ_ConeSpell : BJ_Spell
{
    [SerializeField] float range;
    [SerializeField] float angle;

    protected void Start()
    {
        base.Start();

        Collider[] _hits = Physics.OverlapSphere(transform.position, range, enemyLayer);
        
        foreach (Collider _hit in _hits)
        {
            if(Vector3.Angle(transform.position, _hit.transform.position) < angle)
            {
                if (_hit.GetComponent<BJ_Enemy>())
                {
                    _hit.GetComponent<BJ_Enemy>().Hit(damage);
                    // slow
                }
            }
        }

        StartCoroutine(DelayDestroy());
    }

    IEnumerator DelayDestroy()
    {
        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
    }
}
