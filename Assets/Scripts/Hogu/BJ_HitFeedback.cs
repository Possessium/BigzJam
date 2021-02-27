using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJ_HitFeedback : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DamageFeedback());
    }
    IEnumerator DamageFeedback()
    {
        bool _right = transform.localPosition.x < -2;
        while (true)
        {
            transform.position += new Vector3(_right ? 3f : -3f, 0, .5f) * Time.deltaTime * 4;

            if (_right && transform.localPosition.x > 2)
                _right = false;
            if (!_right && transform.localPosition.x < -2)
                _right = true;

            if (transform.localPosition.y > 5)
            {
                Destroy(this.gameObject);
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
