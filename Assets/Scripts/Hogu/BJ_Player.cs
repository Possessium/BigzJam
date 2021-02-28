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

    [SerializeField] Image spell1Front;
    [SerializeField] Image spell2Front;

    [SerializeField] Image spell1Back;
    [SerializeField] Image spell2Back;

    [SerializeField] Sprite lineSpellSpriteBack;
    [SerializeField] Sprite coneSpellSpriteBack;
    [SerializeField] Sprite novaSpellSpriteBack;
    [SerializeField] Sprite areaSpellSpriteBack;
    [SerializeField] Sprite wallSpellSpriteBack;

    [SerializeField] Sprite lineSpellSpriteFront;
    [SerializeField] Sprite coneSpellSpriteFront;
    [SerializeField] Sprite novaSpellSpriteFront;
    [SerializeField] Sprite areaSpellSpriteFront;
    [SerializeField] Sprite wallSpellSpriteFront;

    #endregion

    #region Sounds
    [SerializeField] AudioClip lineSound;
    [SerializeField] AudioClip coneSound;
    [SerializeField] AudioClip novaSound;
    [SerializeField] AudioClip areaSound;
    [SerializeField] AudioClip wallSound;
    [SerializeField] AudioClip dedSound;
    [SerializeField] AudioClip oofSound;
    [SerializeField] AudioClip switchSound;

    [SerializeField] AudioSource soundSource;
    #endregion

    [SerializeField] GameObject healthSource;
    [SerializeField] List<GameObject> healths;
    [SerializeField] GameObject ui;


    [SerializeField] bool canMove = false;

    Vector2 movement;

    private void Start()
    {
        InitHealth();

        spell1 = (SpellType)Random.Range(0, 5);
        spell2 = (SpellType)Random.Range(0, 5);

        spell1Back.sprite = spell1 == SpellType.Area ? areaSpellSpriteBack : spell1 == SpellType.Cone ? coneSpellSpriteBack : spell1 == SpellType.Line ? lineSpellSpriteBack : spell1 == SpellType.Nova ? novaSpellSpriteBack : wallSpellSpriteBack;
        spell1Front.sprite = spell1 == SpellType.Area ? areaSpellSpriteFront : spell1 == SpellType.Cone ? coneSpellSpriteFront : spell1 == SpellType.Line ? lineSpellSpriteFront : spell1 == SpellType.Nova ? novaSpellSpriteFront : wallSpellSpriteFront;

        spell2Back.sprite = spell2 == SpellType.Area ? areaSpellSpriteBack : spell2 == SpellType.Cone ? coneSpellSpriteBack : spell2 == SpellType.Line ? lineSpellSpriteBack : spell2 == SpellType.Nova ? novaSpellSpriteBack : wallSpellSpriteBack;
        spell2Front.sprite = spell2 == SpellType.Area ? areaSpellSpriteFront : spell2 == SpellType.Cone ? coneSpellSpriteFront : spell2 == SpellType.Line ? lineSpellSpriteFront : spell2 == SpellType.Nova ? novaSpellSpriteFront : wallSpellSpriteFront;
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

        transform.position += new Vector3(movement.x, 0, movement.y) * .2f;
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
                    if(lineSound)
                        soundSource.PlayOneShot(lineSound);
                    StartCoroutine(CoolDown(_t));
                    Instantiate(lineSpell, transform.position + transform.up, _rotation);
                }
                break;
            case SpellType.Wall:
                if (canWall)
                {
                    if(wallSound)
                        soundSource.PlayOneShot(wallSound);
                    StartCoroutine(CoolDown(_t));
                    Instantiate(wallSpell, aimObject.transform.position, Quaternion.identity);
                }
                break;
            case SpellType.Cone:
                if (canCone)
                {
                    if(coneSound)
                        soundSource.PlayOneShot(coneSound);
                    StartCoroutine(CoolDown(_t));
                    Instantiate(coneSpell, transform.position, _rotation);
                }
                break;
            case SpellType.Area:
                if (canArea)
                {
                    if(areaSound)
                        soundSource.PlayOneShot(areaSound);
                    StartCoroutine(CoolDown(_t));
                    Instantiate(areaSpell, aimObject.transform.position, Quaternion.identity);
                }
                break;
            case SpellType.Nova:
                if (canNova)
                {
                    if(novaSound)
                        soundSource.PlayOneShot(novaSound);
                    StartCoroutine(CoolDown(_t));
                    Instantiate(novaSpell, aimObject.transform.position, Quaternion.identity);
                }
                break;
        }
    }

    IEnumerator CoolDown(SpellType _t)
    {
        float _time = 0;

        animator.SetTrigger("Attack");
        while (true)
        {
            _time += Time.deltaTime;
            switch (_t)
            {
                case SpellType.Line:
                    canLine = false;
                    if (_time > lineCooldown)
                    {
                        if (spell1 == _t)
                            spell1Front.fillAmount = 1;
                        if (spell2 == _t)
                            spell1Front.fillAmount = 1;
                        canLine = true;
                        yield break;
                    }
                    break;
                case SpellType.Wall:
                    canWall = false;
                    if (_time > wallCooldown)
                    {
                        if (spell1 == _t)
                            spell1Front.fillAmount = 1;
                        if (spell2 == _t)
                            spell1Front.fillAmount = 1;
                        canWall = true;
                        yield break;
                    }
                    break;
                case SpellType.Cone:
                    canCone = false;
                    if (_time > coneCooldown)
                    {
                        if (spell1 == _t)
                            spell1Front.fillAmount = 1;
                        if (spell2 == _t)
                            spell1Front.fillAmount = 1;
                        canCone = true;
                        yield break;
                    }
                    break;
                case SpellType.Area:
                    canArea = false;
                    if (_time > areaCooldown)
                    {
                        if (spell1 == _t)
                            spell1Front.fillAmount = 1;
                        if (spell2 == _t)
                            spell1Front.fillAmount = 1;
                        canArea = true;
                        yield break;
                    }
                    break;
                case SpellType.Nova:
                    canNova = false;
                    if (_time > novaCooldown)
                    {
                        if (spell1 == _t)
                            spell1Front.fillAmount = 1;
                        if (spell2 == _t)
                            spell1Front.fillAmount = 1;
                        canNova = true;
                        yield break;
                    }
                    break;
            }
            if (spell1 == _t)
                spell1Front.fillAmount = _time / (spell1 == SpellType.Area ? areaCooldown : spell1 == SpellType.Cone ? coneCooldown : spell1 == SpellType.Line ? lineCooldown : spell1 == SpellType.Nova ? novaCooldown : wallCooldown);
            if (spell2 == _t)
                spell2Front.fillAmount = _time / (spell2 == SpellType.Area ? areaCooldown : spell2 == SpellType.Cone ? coneCooldown : spell2 == SpellType.Line ? lineCooldown : spell2 == SpellType.Nova ? novaCooldown : wallCooldown);
            yield return new WaitForEndOfFrame();
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

        if(switchSound)
            soundSource.PlayOneShot(switchSound);

        SpellType _t = _s.TypeSpell;

        if(_1)
        {
            _s.SwitchType(spell1);
            spell1 = _t;

            spell1Back.sprite = spell1 == SpellType.Area ? areaSpellSpriteBack : spell1 == SpellType.Cone ? coneSpellSpriteBack : spell1 == SpellType.Line ? lineSpellSpriteBack : spell1 == SpellType.Nova ? novaSpellSpriteBack : wallSpellSpriteBack;
            spell1Front.sprite = spell1 == SpellType.Area ? areaSpellSpriteFront : spell1 == SpellType.Cone ? coneSpellSpriteFront : spell1 == SpellType.Line ? lineSpellSpriteFront : spell1 == SpellType.Nova ? novaSpellSpriteFront : wallSpellSpriteFront;

            StartCoroutine(CoolDown(spell1));
        }

        else
        {
            _s.SwitchType(spell2);
            spell2 = _t;

            spell2Back.sprite = spell2 == SpellType.Area ? areaSpellSpriteBack : spell2 == SpellType.Cone ? coneSpellSpriteBack : spell2 == SpellType.Line ? lineSpellSpriteBack : spell2 == SpellType.Nova ? novaSpellSpriteBack : wallSpellSpriteBack;
            spell2Front.sprite = spell2 == SpellType.Area ? areaSpellSpriteFront : spell2 == SpellType.Cone ? coneSpellSpriteFront : spell2 == SpellType.Line ? lineSpellSpriteFront : spell2 == SpellType.Nova ? novaSpellSpriteFront : wallSpellSpriteFront;

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

        if(oofSound)
            soundSource.PlayOneShot(oofSound);

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
        if(dedSound)
            soundSource.PlayOneShot(dedSound);

        animator.SetTrigger("Death");
        healths[0].transform.GetChild(0).gameObject.SetActive(false);
        canMove = false;
        BJ_GameManager.I.EndGame();
    }

    public void Respawn()
    {
        transform.position = new Vector3(0, 75, 0);
        health = maxHealth;
        animator.SetTrigger("Respawn");
    }

}
