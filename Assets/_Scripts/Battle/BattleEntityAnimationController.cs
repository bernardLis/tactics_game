using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEntityAnimationController : MonoBehaviour
{

    BattleEntity _entity;
    Animator _animator;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Initialize(BattleEntity entity)
    {
        _entity = entity;

        _entity.OnAttack += Attack;
        _entity.OnDamageTaken += DamageTaken;

        _entity.OnCelebrate += Jump;
        _entity.OnDeath += Death;
    }


    void Attack(BattleEntity be)
    {
        _animator.Play("Attack");
    }

    void Jump(BattleEntity be)
    {
        _animator.Play("Celebrate");
    }

    void DamageTaken(BattleEntity be)
    {
        _animator.Play("Take Damage");
    }

    void Death(BattleEntity be)
    {
        _animator.Play("Die");
    }
}
