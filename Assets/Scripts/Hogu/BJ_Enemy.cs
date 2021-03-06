using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BJ_Enemy : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] NavMeshData navMesh;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask allyLayer;
    [SerializeField] Bounds movementBounds;
    [SerializeField] EnemyType enemyType;
    [SerializeField] SpriteRenderer renderer;
    [SerializeField] BJ_EnemyData data;
    [SerializeField] Image healthImage;
    [SerializeField] GameObject healthFeedback;
    [SerializeField] float scale;
    [SerializeField] Animator animator;
    [SerializeField] Vector3 _pos = Vector3.zero;
    [SerializeField] GameObject freeze;

    [SerializeField] GameObject fireObject;

    public int Health { get; private set; } = 100;

    Transform targetPlayer;
    BJ_Enemy targetAlly;

    bool playerFound = false;
    bool allyFound = false;
    bool canHit = true;
    [SerializeField] bool canMove = false;
    bool started = false;

    public void Init()
    {
        started = true;
        switch (enemyType)
        {
            case EnemyType.Cac:
                Health = data.CacHealth;
                break;
            case EnemyType.Dist:
                Health = data.DistHealth;
                break;
            case EnemyType.Heal:
                Health = data.HealHealth;
                break;
            case EnemyType.CacFat:
                Health = data.CacHealth * 2;
                break;
        }

        //renderer = GetComponentInChildren<SpriteRenderer>();
        //renderer.color = enemyType == EnemyType.Cac ? Color.red : enemyType == EnemyType.Dist ? Color.blue : Color.yellow;
        _pos = new Vector3(transform.parent.position.x, 0, transform.parent.position.z) + new Vector3(Random.Range(movementBounds.min.x, movementBounds.max.x), transform.position.y, Random.Range(movementBounds.min.z, movementBounds.max.z));

        _pos.y = transform.position.y;
        agent.SetDestination(_pos);
    }

    private void OnDrawGizmos()
    {
        if(!playerFound)
        {
            // bounds
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(new Vector3(transform.parent.position.x, 0, transform.parent.position.z) + movementBounds.center, movementBounds.size);

            // next pos
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(_pos, .5f);
            
            // detection range
            Gizmos.color = new Color(255, 255, 0, .2f);
            Gizmos.DrawSphere(transform.position, data.DetectionRange);
        }
    }

    void Update()
    {
        if (!started)
            return;

        if (!canMove)
        {
            agent.isStopped = true;
            return;
        }
        else
            agent.isStopped = false;


        healthImage.transform.parent.GetComponent<RectTransform>().eulerAngles = freeze.transform.eulerAngles = renderer.transform.eulerAngles = new Vector3(90, -transform.rotation.y, 0);

        if (playerFound)
        {
            PlayerFoundBehaviour();
        }

        else
        {
            if (enemyType == EnemyType.Heal && !allyFound)
                LookForAlly();
            else if (enemyType == EnemyType.Heal && allyFound)
                AllyFoundBehaviour();
            
            LookForPlayer();
        }
    }

    public void SetMove(bool _s)
    {
        if (!started)
            return;
        canMove = _s;
    }

    void LookForPlayer()
    {
        Collider[] _hits = new Collider[1];
        if (Physics.OverlapSphereNonAlloc(transform.position, data.DetectionRange, _hits, playerLayer) > 0)
        {
            targetPlayer = _hits[0].transform;
            playerFound = true;
            return;
        }

        
        if (Vector3.Distance(new Vector3(_pos.x, 0, _pos.z), new Vector3(transform.position.x, 0, transform.position.z)) < 1)
        {
            _pos = new Vector3(transform.parent.position.x, 0, transform.parent.position.z) + new Vector3(Random.Range(movementBounds.min.x, movementBounds.max.x), transform.position.y, Random.Range(movementBounds.min.z, movementBounds.max.z));

            _pos.y = transform.position.y;
            agent.SetDestination(_pos);
        }
    }

    void PlayerFoundBehaviour()
    {
        switch (enemyType)
        {
            case EnemyType.Cac:
            case EnemyType.CacFat:

                _pos = targetPlayer.transform.position;
                if (Vector3.Distance(new Vector3(targetPlayer.transform.position.x, 0, targetPlayer.transform.position.z), new Vector3(transform.position.x, 0, transform.position.z)) < data.CacDistanceMaxHit)
                {
                    if(canHit)
                    {
                        targetPlayer.GetComponent<BJ_Player>().Hit();
                        StartCoroutine(CoolDown());
                    }
                }
                break;

            case EnemyType.Dist:

                if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(targetPlayer.transform.position.x, 0, targetPlayer.transform.position.z)) > data.DistRangeMaxHit)
                {
                    _pos = new Vector3(targetPlayer.position.x, transform.position.y, targetPlayer.transform.position.z);
                    //_pos.y = 0;
                }

                else
                {
                    _pos = transform.position;
                    if (canHit)
                    {
                        Instantiate(fireObject, transform.position, Quaternion.LookRotation(new Vector3(targetPlayer.position.x, transform.position.y, targetPlayer.position.z) - transform.position));
                        StartCoroutine(CoolDown());
                    }
                }


                break;

            case EnemyType.Heal:
                _pos = transform.position + (transform.position - targetPlayer.transform.position);
                _pos.y = 0;
                if (Vector3.Distance(_pos, new Vector3(transform.position.x, 0, transform.position.z)) > data.DetectionRange + 1)
                {
                    playerFound = false;
                    _pos = new Vector3(transform.parent.position.x, 0, transform.parent.position.z) + new Vector3(Random.Range(movementBounds.min.x, movementBounds.max.x), 0, Random.Range(movementBounds.min.z, movementBounds.max.z));
                }
                break;
        }
        _pos.y = transform.position.y;
        agent.SetDestination(_pos);
    }

    IEnumerator CoolDown()
    {
        animator.SetTrigger("Attack");
        canHit = false;
        yield return new WaitForSeconds(data.CooldownDuration);
        canHit = true;
    }


    void LookForAlly()
    {
        Collider[] _hits = new Collider[5];
        if (Physics.OverlapSphereNonAlloc(transform.position, 10, _hits, allyLayer) > 0)
        {
            foreach (Collider _hit in _hits)
            {
                if(_hit && _hit.GetComponent<BJ_Enemy>() && _hit.GetComponent<BJ_Enemy>() != this && _hit.GetComponent<BJ_Enemy>().Health < 100)
                {
                    targetAlly = _hit.GetComponent<BJ_Enemy>();
                    allyFound = true;
                    return;
                }
            }
        }
    }

    void AllyFoundBehaviour()
    {
        _pos = targetAlly.transform.position;
        _pos.y = 0;
        if (Vector3.Distance(_pos, new Vector3(transform.position.x, 0, transform.position.z)) < data.HealRangeMaxHeal)
        {
            _pos = transform.position;
            //_pos.y = 0;
            if (canHit)
            {
                targetAlly.Hit(-20);
                Debug.Log("SOIIIIN");
                StartCoroutine(CoolDown());
            }
        }

        _pos.y = transform.position.y;
        agent.SetDestination(_pos);

    }

    public void Hit(int _dmg)
    {
        if (!started)
            return;

        Health -= _dmg;
        if (Health > (enemyType == EnemyType.Cac ? data.CacHealth : enemyType == EnemyType.CacFat ? data.CacHealth * 2 : enemyType == EnemyType.Dist ? data.DistHealth : data.HealHealth))
            Health = enemyType == EnemyType.Cac ? data.CacHealth : enemyType == EnemyType.CacFat ? data.CacHealth * 2 : enemyType == EnemyType.Dist ? data.DistHealth : data.HealHealth;
        healthImage.fillAmount = Health < 0 ? 0 : ((float)Health / (enemyType == EnemyType.Cac ? data.CacHealth : enemyType == EnemyType.CacFat ? data.CacHealth * 2 : enemyType == EnemyType.Dist ? data.DistHealth : data.HealHealth));
        if (Health <= 0)
            Death();
        if(_dmg > 0)
        {
            GameObject _damage = Instantiate(healthFeedback, healthImage.transform.parent);
            _damage.transform.eulerAngles = new Vector3(90, 0, 0);
            _damage.GetComponent<TMPro.TMP_Text>().SetText(_dmg.ToString());
        }
    }

    void Death()
    {
        Destroy(this.gameObject);
        BJ_GameManager.I.AddScore(enemyType == EnemyType.Cac ? data.CacValue : enemyType == EnemyType.CacFat ? data.CacFatValue : enemyType == EnemyType.Dist ? data.DistValue : data.HealValue);
    }

    public void SetFreeze(bool _s)
    {
        canMove = !_s;
        freeze.SetActive(_s);
    }

    enum EnemyType
    {
        Cac,
        CacFat,
        Dist,
        Heal
    }
}
