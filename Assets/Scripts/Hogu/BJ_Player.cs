using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class BJ_Player : MonoBehaviour
{
    [SerializeField] int maxHealth = 5;
    [SerializeField] int health = 5;
    [SerializeField] SpellType spell1;
    [SerializeField] SpellType spell2;
    [SerializeField] GameObject aimObject;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] SpriteRenderer rend;
    [SerializeField] Animator animator;
    [SerializeField] float noHitDuration;

    #region Spells

    [SerializeField] GameObject lineSpell = null;
    [SerializeField] GameObject coneSpell = null;
    [SerializeField] GameObject novaSpell = null;
    [SerializeField] GameObject areaSpell = null;
    [SerializeField] GameObject wallSpell = null;

    [SerializeField] float lineCooldown;
    [SerializeField] float novaCooldown;
    [SerializeField] float coneCooldown;
    [SerializeField] float wallCooldown;
    [SerializeField] float areaCooldown;

    [SerializeField] bool canLine = true;
    [SerializeField] bool canNova = true;
    [SerializeField] bool canCone = true;
    [SerializeField] bool canWall = true;
    [SerializeField] bool canArea = true;

    #endregion

    [SerializeField] GameObject healthSource;
    [SerializeField] List<GameObject> healths;
    [SerializeField] GameObject ui;


    [SerializeField] bool canMove = false;

    Vector2 movement;

    private void Start()
    {
        InitHealth();
    }

    private void Update()
    {
        if (!canMove)
            return;

        RaycastHit _hit;
        if (Physics.Raycast(transform.position, new Vector3(movement.x, 0, 0), out _hit, 1, wallLayer))
            movement.x = 0;

        if (Physics.Raycast(transform.position, new Vector3(0, 0, movement.y), out _hit, 1, wallLayer))
            movement.y = 0;

        transform.position += new Vector3(movement.x, 0, movement.y) * .1f;
    }

    public void SetMove(bool _state)
    {
        canMove = _state;
    }
    void InitHealth()
    {
        health = maxHealth;

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject _go = Instantiate(healthSource, ui.transform);
            healths.Add(_go);
            _go.GetComponent<RectTransform>().anchoredPosition = new Vector3((healthSource.GetComponent<RectTransform>().rect.width * i) + i * 20 + 20, -20, 0);
        }
    }

    public void Move(InputAction.CallbackContext _ctx)
    {
        movement = _ctx.ReadValue<Vector2>();

        animator.SetBool("Moving", movement.magnitude > 0);

        rend.flipX = movement.x < 0;
    }

    public void Aim(InputAction.CallbackContext _ctx)
    {
        if (_ctx.ReadValue<Vector2>() == Vector2.zero)
            return;
        aimObject.transform.position = transform.position + (new Vector3(_ctx.ReadValue<Vector2>().normalized.x, 0, _ctx.ReadValue<Vector2>().normalized.y)) * 5;
    }

    public void Fire1(InputAction.CallbackContext _ctx)
    {
        FireSpell(spell1);
    }
    public void Fire2(InputAction.CallbackContext _ctx)
    {
        FireSpell(spell2);
    }

    void FireSpell(SpellType _t)
    {
        if (!canMove)
            return;

        Quaternion _rotation = Quaternion.LookRotation(new Vector3(aimObject.transform.position.x, 0, aimObject.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z));
        switch (_t)
        {
            case SpellType.Line:
                if (canLine)
                {
                    StartCoroutine(CoolDown(_t));
                    Instantiate(lineSpell, transform.position, _rotation);
                }
                break;
            case SpellType.Wall:
                if (canWall)
                {
                    StartCoroutine(CoolDown(_t));
                    Instantiate(wallSpell, aimObject.transform.position, _rotation);
                }
                break;
            case SpellType.Cone:
                if (canCone)
                {
                    StartCoroutine(CoolDown(_t));
                    Instantiate(coneSpell, transform.position, _rotation);
                }
                break;
            case SpellType.Area:
                if (canArea)
                {
                    StartCoroutine(CoolDown(_t));
                    Instantiate(areaSpell, aimObject.transform.position, Quaternion.identity);
                }
                break;
            case SpellType.Nova:
                if (canNova)
                {
                    StartCoroutine(CoolDown(_t));
                    Instantiate(novaSpell, aimObject.transform.position, Quaternion.identity);
                }
                break;
        }
    }

    IEnumerator CoolDown(SpellType _t)
    {
        animator.SetTrigger("Attack");
        switch (_t)
        {
            case SpellType.Line:
                canLine = false;
                yield return new WaitForSeconds(lineCooldown);
                canLine = true;
                break;
            case SpellType.Wall:
                canWall = false;
                yield return new WaitForSeconds(wallCooldown);
                canWall = true;
                break;
            case SpellType.Cone:
                canCone = false;
                yield return new WaitForSeconds(coneCooldown);
                canCone = true;
                break;
            case SpellType.Area:
                canArea = false;
                yield return new WaitForSeconds(areaCooldown);
                canArea = true;
                break;
            case SpellType.Nova:
                canNova = false;
                yield return new WaitForSeconds(novaCooldown);
                canNova = true;
                break;
        }
    }

    public void SwitchSpell1(InputAction.CallbackContext _ctx)
    {
        SwitchSpell(true);
    }

    public void SwitchSpell2(InputAction.CallbackContext _ctx)
    {
        SwitchSpell(false);
    }

    void SwitchSpell(bool _1)
    {
        // Choper le spell au sol (si non return)
        BJ_SpellItem _s = null;

        BJ_SpellItem[] _items = FindObjectsOfType<BJ_SpellItem>();

        for (int i = 0; i < _items.Length; i++)
        {
            if(Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(_items[i].transform.position.x, 0, _items[i].transform.position.z)) < 2)
            {
                _s = _items[i];
                break;
            }
        }

        if (!_s)
            return;

        SpellType _t = _s.TypeSpell;

        if(_1)
        {
            _s.SwitchType(spell1);
            spell1 = _t;
            StartCoroutine(CoolDown(spell1));
        }

        else
        {
            _s.SwitchType(spell2);
            spell2 = _t;
            StartCoroutine(CoolDown(spell2));
        }
    }

    Coroutine hitBlink;

    public void Hit()
    {
        if (!canMove || hitBlink != null)
            return;

        health--;
        if(health <= 0)
        {
            Death();
            return;
        }

        hitBlink = StartCoroutine(Blink());

        healths[health].transform.GetChild(0).gameObject.SetActive(false);
    }

    IEnumerator Blink()
    {

        float _t = 0;
        
        while(_t < noHitDuration)
        {
            _t += Time.deltaTime + .1f;
            rend.enabled = !rend.enabled;
            yield return new WaitForSeconds(.1f);
        }

        rend.enabled = true;
        hitBlink = null;
    }

    void Death()
    {
        animator.SetTrigger("Death");
        healths[0].transform.GetChild(0).gameObject.SetActive(false);
        canMove = false;
    }

}

/*
 * 
 * onmovement
 *      check la direction, if wall don't
 *      
 * public Hit(int dmg)
 * 
 * onFire
 *      call le spell
 * 
 * onChangeSpell
 *      check si un spell nearby
 *          échanger si oui
 *              
 * death
 * 
 * 
 */