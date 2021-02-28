using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJ_EnemyFire : MonoBehaviour
{
    private void Start()
    {
        transform.eulerAngles = new Vector3(90, transform.eulerAngles.y, transform.eulerAngles.z);
    }

    private void Update()
    {
        transform.position += transform.up * .3f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.GetComponent<BJ_Player>())
        {
            collision.transform.GetComponent<BJ_Player>().Hit();
            Destroy(this.gameObject);
        }
        if (!collision.transform.GetComponent<BJ_Enemy>() && collision.transform.name != "Plane")
            Destroy(this.gameObject);
    }
}
