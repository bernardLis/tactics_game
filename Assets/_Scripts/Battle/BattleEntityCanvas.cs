using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleEntityCanvas : MonoBehaviour
{
    BattleEntity _entity;

    [SerializeField] Image _elementImage;

    // Start is called before the first frame update
    void Start()
    {
        _entity = transform.parent.GetComponent<BattleEntity>();
        _elementImage.sprite = _entity.ArmyEntity.Element.Icon;
    }


}
