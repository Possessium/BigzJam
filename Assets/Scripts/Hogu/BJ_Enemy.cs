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
    Vector3 _pos = Vector3.zero;

    public int Health { get; private set; } = 100;

    Transform targetPlayer;
    BJ_Enemy targetAlly;

    bool playerFound = false;
    bool allyFound = false;
    bool canHit = true;
    bool canMove = false;

    void Start()
    {
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
        _pos = new Vector3(Random.Range(movementBounds.min.x, movementBounds.max.x), 0, Random.Range(movementBounds.min.z, movementBounds.max.z));
        agent.SetDestination(_pos);
    }

    private void OnDrawGizmos()
    {
        if(!playerFound)
        {
            // bounds
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(movementBounds.center, movementBounds.size);

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
        if (!canMove)
        {
            agent.isStopped = true;
            return;
        }
        else
            agent.isStopped = false;


        healthImage.transform.parent.GetComponent<RectTransform>().eulerAngles = renderer.transform.eulerAngles = new Vector3(90, -transform.rotation.y, 0);

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

        if (Vector3.Distance(_pos, new Vector3(transform.position.x, 0, transform.position.z)) < 1)
        {
            _pos = new Vector3(Random.Range(movementBounds.min.x, movementBounds.max.x), 0, Random.Range(movementBounds.min.z, movementBounds.max.z));
            agent.SetDestination(_pos);
        }
    }

    void PlayerFoundBehaviour()
    {
        switch (enemyType)
        {
            case EnemyType.Cac:

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
                    _pos = targetPlayer.position;
                    _pos.y = 0;
                }

                else
                {
                    _pos = transform.position;
                    if (canHit)
                    {
                        targetPlayer.GetComponent<BJ_Player>().Hit();
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
                    _pos = new Vector3(Random.Range(movementBounds.min.x, movementBounds.max.x), 0, Random.Range(movementBounds.min.z, movementBounds.max.z));
                }
                break;
        }

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
            _pos.y = 0;
            if (canHit)
            {
                // HEAL
                Debug.Log("SOIIIIN");
                StartCoroutine(CoolDown());
            }
        }
        agent.SetDestination(_pos);

    }

    public void Hit(int _dmg)
    {
        Health -= _dmg;
        healthImage.fillAmount = Health < 0 ? 0 : ((float)Health / (enemyType == EnemyType.Cac ? data.CacHealth : enemyType == EnemyType.CacFat ? data.CacHealth * 2 : enemyType == EnemyType.Dist ? data.DistHealth : data.HealHealth));
        if (Health <= 0)
            Death();
        GameObject _damage = Instantiate(healthFeedback, healthImage.transform.parent);
        _damage.transform.eulerAngles = new Vector3(90, 0, 0);
        _damage.GetComponent<TMPro.TMP_Text>().SetText(_dmg.ToString());
    }

    void Death()
    {
        Destroy(this.gameObject);
        BJ_GameManager.I.AddScore(enemyType == EnemyType.Cac ? data.CacValue : enemyType == EnemyType.CacFat ? data.CacFatValue : enemyType == EnemyType.Dist ? data.DistValue : data.HealValue);
    }

    enum EnemyType
    {
        Cac,
        CacFat,
        Dist,
        Heal
    }
}

/*
 * 
 * 
 * mouvement random l�ger
 *      circlecast autour de lui pour d�tection du joueur
 *          if found && !heal follow player & no more circlecast
 *          else wait random temps puis loop back
 *          
 *      if heal
 *          circlecast autour de lui pour d�tection de ses alli�s
 *              check de la vie des trouv�s
 *                  si trouv� low go to him & heal with cooldown
 *              if found player flees
 *          
 * onMoveToPlayer
 *      switch ennemyType
 *          
 *          c�c - if close enough hit then wait before next hit (still move)
 *          
 *          dist - if range OK & no obstacle hit then wait before next hit (still move)
 *                  if player too close flees
 *          
 *          
 *      
 * 
 * 
 * 
 * 
 * 
 */