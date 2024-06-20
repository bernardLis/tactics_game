﻿using System.Collections;
using UnityEngine;

namespace Lis.Units.Attack
{
    public class AttackControllerWhirlwind : AttackController
    {
        [SerializeField] GameObject _effect;
        readonly float _radius = 5f;

        public override IEnumerator AttackCoroutine()
        {
            while (CurrentCooldown > 0) yield return new WaitForSeconds(0.1f);
            BaseAttack();

            Animator.SetTrigger(AnimSpecialAttack);
            AudioManager.CreateSound()
                .WithSound(Attack.Sound)
                .WithPosition(transform.position)
                .Play();
            _effect.SetActive(true);
            foreach (UnitController be in GetOpponentsInRadius(_radius))
                StartCoroutine(be.GetHit(Attack));

            Invoke(nameof(CleanUp), 2f);
        }

        void CleanUp()
        {
            _effect.SetActive(false);
        }
    }
}