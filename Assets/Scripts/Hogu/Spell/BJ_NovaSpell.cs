using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJ_NovaSpell : BJ_Spell
{
    [SerializeField] float duration;
    [SerializeField] float cooldown;
    [SerializeField] float range;
    List<BJ_Enemy> hitEnemies = new List<BJ_Enemy>();

    float timer = 0;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 0, 0, .5f);
        Gizmos.DrawSphere(transform.position, range);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > duration)
            Destroy(this.gameObject);

        Collider[] _hits = Physics.OverlapSphere(transform.position, range, enemyLayer);

        foreach (Collider _hit in _hits)
        {
            if (_hit.GetComponent<BJ_Enemy>() && !hitEnemies.Contains(_hit.GetComponent<BJ_Enemy>()))
            {
                _hit.GetComponent<BJ_Enemy>().SetFreeze(true);
                hitEnemies.Add(_hit.GetComponent<BJ_Enemy>());
            }
        }
    }

    private void OnDestroy()
    {
        foreach (BJ_Enemy _e in hitEnemies)
        {
            if(_e)
                _e.SetFreeze(false);
        }
    }
}
