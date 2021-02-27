using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BJ_SpellItem : MonoBehaviour
{
    [SerializeField] SpriteRenderer rend;
    [SerializeField] Sprite lineSprite;
    [SerializeField] Sprite wallSprite;
    [SerializeField] Sprite novaSprite;
    [SerializeField] Sprite coneSprite;
    [SerializeField] Sprite areaSprite;

    [SerializeField] SpellType typeSpell;
    public SpellType TypeSpell { get { return typeSpell; } }

    private void Start()
    {
        typeSpell = (SpellType)Random.Range(0, 5);
        transform.position = new Vector3(transform.position.x, 70, transform.position.z);
        SwitchType(TypeSpell);
    }

    public void SwitchType(SpellType _t)
    {
        typeSpell = _t;

        switch (typeSpell)
        {
            case SpellType.Line:
                rend.sprite = lineSprite;

                rend.color = Color.yellow;
                break;
            case SpellType.Wall:
                rend.sprite = wallSprite;

                rend.color = Color.grey;
                break;
            case SpellType.Cone:
                rend.sprite = coneSprite;

                rend.color = Color.blue;
                break;
            case SpellType.Area:
                rend.sprite = areaSprite;

                rend.color = Color.red;
                break;
            case SpellType.Nova:
                rend.sprite = novaSprite;

                rend.color = Color.cyan;
                break;
        }
    }
}
