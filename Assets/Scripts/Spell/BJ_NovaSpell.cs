using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJ_NovaSpell : BJ_Spell
{
    [SerializeField] float duration;
    [SerializeField] float cooldown;

    float timer = 0;
    float hitTimer = 0;


    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > duration)
            Destroy(this.gameObject);

        cooldown += Time.deltaTime;

        if(hitTimer > cooldown)
        {
            hitTimer = 0;

            Collider[] _hits = Physics.OverlapSphere(transform.position, 5, enemyLayer);

            foreach (Collider _hit in _hits)
            {
                if (_hit.GetComponent<BJ_Enemy>())
                {
                    // + Freeze
                }
            }

        }

    }
}
