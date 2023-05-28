using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleEntityCanvas : MonoBehaviour
{
    BattleEntity _entity;


    // Start is called before the first frame update
    void Start()
    {
        _entity = transform.parent.GetComponent<BattleEntity>();
    }


}
