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
        _entity.OnStartedMoving += Move;

        _entity.OnAttack += Attack;
        _entity.OnDamageTaken += DamageTaken;

        _entity.OnCelebrate += Jump;
        _entity.OnDeath += Death;
    }


    void Idle(BattleEntity be)
    {
        ResetAll();
    }

    void ResetAll()
    {
        _animator.SetBool("Move Forward", false);
    }

    void Move(BattleEntity be)
    {
        _animator.SetBool("Move Forward", true);
    }


    void Attack(BattleEntity be)
    {
        _animator.SetTrigger("Attack");
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
        _animator.Play("Die");
    }
}
