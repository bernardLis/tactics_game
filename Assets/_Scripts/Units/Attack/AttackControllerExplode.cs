﻿using System.Collections;
using UnityEngine;

namespace Lis.Units.Attack
{
    public class AttackControllerExplode : AttackController
    {
        [SerializeField] GameObject _effect;
        readonly float _explosionRadius = 5f;

        public override IEnumerator AttackCoroutine()
        {
            while (CurrentCooldown > 0) yield return new WaitForSeconds(0.1f);
            BaseAttack();

            int countDown = 3;
            while (countDown > 0)
            {
                Animator.SetTrigger(AnimSpecialAttack);
                UnitController.DisplayFloatingText($"Explode in: {countDown}", Color.red);
                countDown--;
                yield return new WaitForSeconds(1f);
            }

            AudioManager.CreateSound()
                .WithSound(Attack.Sound)
                .WithPosition(transform.position)
                .Play();

            _effect.SetActive(true);
            foreach (UnitController be in GetOpponentsInRadius(_explosionRadius))
                StartCoroutine(be.GetHit(Attack));

            Invoke(nameof(CleanUp), 2f);
        }

        void CleanUp()
        {
            _effect.SetActive(false);
        }
    }
}