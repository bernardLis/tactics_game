using Lis.Battle;
using Lis.Battle.Arena;
using Lis.Battle.Fight;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis
{
    public class HeroArenaLeaveController : MonoBehaviour
    {
        [SerializeField] GameObject _effect;
        ArenaManager _arenaManager;
        BoxCollider _collider;

        void Start()
        {
            BattleInitializer.Instance.OnBattleInitialized += OnBattleInitialized;
            _collider = GetComponent<BoxCollider>();

            _arenaManager = ArenaManager.Instance;
        }

        void OnTriggerExit(Collider other)
        {
            // if hero leaves, fight is active and hero is in the arena - make collider not trigger (hero can't come back)
            if (FightManager.IsFightActive == false) return;
            if (other.TryGetComponent(out HeroController heroController))
                if (_arenaManager.IsPositionInArena(heroController.transform.position))
                {
                    heroController.StartAllAbilities();
                    heroController.Hero.Speed.ApplyBonusValueChange(-3);
                    _collider.isTrigger = false;
                    _effect.SetActive(true);
                }
        }

        void OnBattleInitialized()
        {
            FightManager.Instance.OnFightEnded += OnFightEnded;
        }

        void OnFightEnded()
        {
            _collider.isTrigger = true;
            _effect.SetActive(false);
        }
    }
}