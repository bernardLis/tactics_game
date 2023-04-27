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
        _entity.OnIdle += Idle;
        _entity.OnStartedWalking += Walk;

        _entity.OnAttack += Attack;
        _entity.OnDamageTaken += DamageTaken;

        _entity.OnCelebrate += Jump;
        _entity.OnDeath += Death;

    }

    void ResetAll()
    {
        _animator.SetBool("Run Forward", false);
    }

    void Walk(BattleEntity be)
    {
        _animator.SetBool("Run Forward", true);
    }

    void Idle(BattleEntity be)
    {
        ResetAll();
    }

    void Attack(BattleEntity be)
    {
        _animator.SetTrigger("Stab Attack");
    }

    void Jump(BattleEntity be)
    {
        _animator.SetTrigger("Jump");
    }

    void DamageTaken(BattleEntity be)
    {
        _animator.SetTrigger("Take Damage");
    }

    void Death(BattleEntity be)
    {
        _animator.SetTrigger("Die");
    }
}
