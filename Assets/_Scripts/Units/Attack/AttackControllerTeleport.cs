﻿using System.Collections;
using Lis.Battle;
using Lis.Battle.Arena;
using UnityEngine;

namespace Lis.Units.Attack
{
    public class AttackControllerTeleport : AttackController
    {
        ArenaController _arenaController;

        [SerializeField] GameObject _effect;
        GameObject _effectInstance;

        public override void Initialize(UnitController unitController, Attack attack)
        {
            base.Initialize(unitController, attack);
            _arenaController = ArenaController.Instance;
            _effectInstance = Instantiate(_effect, transform.position, Quaternion.identity,
                BattleManager.Instance.EntityHolder);
        }

        public override IEnumerator AttackCoroutine()
        {
            while (CurrentCooldown > 0) yield return new WaitForSeconds(0.1f);
            BaseAttack();

            _effectInstance.transform.position = transform.position;
            _effectInstance.SetActive(true);

            UnitController.transform.position = _arenaController.GetRandomPositionInArena();

            Animator.SetTrigger(AnimSpecialAttack);
            AudioManager.PlaySfx(Attack.Sound, transform.position);

            Invoke(nameof(CleanUp), 3f);
        }

        void CleanUp()
        {
            _effectInstance.SetActive(false);
        }
    }
}