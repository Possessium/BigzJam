using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BJ_Player : MonoBehaviour
{
    [SerializeField] int health = 100;
    [SerializeField] SpellType spell1;
    [SerializeField] SpellType spell2;
    [SerializeField] GameObject aimObject;
    [SerializeField] LayerMask wallLayer;

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

    Vector2 movement;

    private void Update()
    {
        RaycastHit _hit;
        if (Physics.Raycast(transform.position, new Vector3(movement.x, 0, 0), out _hit, 1, wallLayer))
            movement.x = 0;

        if (Physics.Raycast(transform.position, new Vector3(0, 0, movement.y), out _hit, 1, wallLayer))
            movement.y = 0;

        transform.position += new Vector3(movement.x, 0, movement.y) * .1f;
    }


    public void Move(InputAction.CallbackContext _ctx)
    {
        movement = _ctx.ReadValue<Vector2>();
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

    public void SwitchSpell(InputAction.CallbackContext _ctx)
    {

    }

    public void Hit(int _dmg)
    {

    }

    void Death()
    {

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