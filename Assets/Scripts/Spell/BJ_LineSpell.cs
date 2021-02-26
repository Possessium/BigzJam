using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJ_LineSpell : BJ_Spell
{
    private void Update()
    {
        transform.position += new Vector3(transform.forward.x, 0, transform.forward.z) * .01f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<BJ_Player>())
        {
            collision.transform.GetComponent<BJ_Enemy>().Hit(damage);
        }
        else
            Destroy(this.gameObject);
    }

}
