using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Battle/Boss Attack")]
public class BossAttack : BaseScriptableObject
{
    public Sprite Icon;
    public string Description;
    public int CooldownSeconds;

    [Header("Battle GameObjects")]
    public GameObject BossAttackManagerPrefab;
    [HideInInspector] public BattleBossAttack BattleBossAttack;

    public void Initialize(BattleBoss bb)
    {
        BattleBossAttack = Instantiate(BossAttackManagerPrefab, bb.transform).GetComponent<BattleBossAttack>();
        BattleBossAttack.Initialize(bb);
    }
}
