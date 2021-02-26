using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJ_WallSpell : BJ_Spell
{
    [SerializeField] float duration;

    protected void Start()
    {
        base.Start();
        StartCoroutine(Duration());
    }

    IEnumerator Duration()
    {
        yield return new WaitForSeconds(duration);
        Destroy(this.gameObject);
    }
}
