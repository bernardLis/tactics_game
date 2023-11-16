using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Boss")]
public class Boss : EntityMovement
{
    [Header("Attacks")]
    public List<BossAttack> Attacks = new List<BossAttack>();

}
