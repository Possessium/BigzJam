using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJ_AreaSpell : BJ_Spell
{
    [SerializeField] float duration;
    [SerializeField] float cooldown;
    [SerializeField] float range;

    float timer = 0;
    float hitTimer = 0;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 255, 255, .5f);
        Gizmos.DrawSphere(transform.position, range);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > duration)
            Destroy(this.gameObject);

        cooldown += Time.deltaTime;

        if (hitTimer > cooldown)
        {
            hitTimer = 0;

            Collider[] _hits = Physics.OverlapSphere(transform.position, range, enemyLayer);

            foreach (Collider _hit in _hits)
            {
                if (_hit.GetComponent<BJ_Enemy>())
                {
                    _hit.GetComponent<BJ_Enemy>().Hit(damage);
                }
            }

        }
    }
}
