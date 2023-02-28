using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NameDisplay : MonoBehaviour
{
    BattleEntity _entity;
    TextMeshProUGUI _textMesh;
    // Start is called before the first frame update
    void Start()
    {
        _entity = transform.root.GetComponent<BattleEntity>();
        _textMesh = GetComponent<TextMeshProUGUI>();
        _textMesh.text = _entity.name;
    }
}
