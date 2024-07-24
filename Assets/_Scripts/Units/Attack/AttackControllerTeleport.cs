using System.Collections;
using Lis.Arena;
using Lis.Arena.Fight;
using UnityEngine;

namespace Lis.Units.Attack
{
    public class AttackControllerTeleport : AttackController
    {
        [SerializeField] GameObject _effect;
        ArenaController _arenaController;
        GameObject _effectInstance;

        public override void Initialize(UnitController unitController, Attack attack)
        {
            base.Initialize(unitController, attack);
            _arenaController = ArenaController.Instance;
            _effectInstance = Instantiate(_effect, transform.position, Quaternion.identity,
                FightManager.Instance.EffectHolder);
        }

        public override IEnumerator AttackCoroutine()
        {
            while (CurrentCooldown > 0) yield return new WaitForSeconds(0.1f);
            BaseAttack();

            _effectInstance.transform.position = transform.position;
            _effectInstance.SetActive(true);

            UnitController.transform.position = _arenaController.GetRandomPositionInArena();

            Animator.SetTrigger(AnimSpecialAttack);
            AudioManager.CreateSound()
                .WithSound(Attack.Sound)
                .WithPosition(transform.position)
                .Play();

            Invoke(nameof(CleanUp), 3f);
        }

        void CleanUp()
        {
            _effectInstance.SetActive(false);
        }
    }
}