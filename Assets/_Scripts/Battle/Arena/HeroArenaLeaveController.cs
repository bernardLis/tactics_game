using Lis.Battle;
using Lis.Battle.Arena;
using Lis.Battle.Fight;
using Lis.Units.Hero;
using UnityEngine;

namespace Lis
{
    public class HeroArenaLeaveController : MonoBehaviour
    {
        ArenaManager _arenaManager;
        BoxCollider _collider;
        [SerializeField] GameObject _effect;

        void Start()
        {
            BattleInitializer.Instance.OnBattleInitialized += OnBattleInitialized;
            _collider = GetComponent<BoxCollider>();

            _arenaManager = ArenaManager.Instance;
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

        void OnTriggerExit(Collider other)
        {
            Debug.Log("on trigger exit");
            // if hero leaves, fight is active and hero is in the arena - make collider not trigger (hero can't come back)
            if (FightManager.IsFightActive == false) return;
            if (other.TryGetComponent(out HeroController heroController))
                if (_arenaManager.IsPositionInArena(heroController.transform.position))
                {
                    Debug.Log("hero in arena");
                    heroController.StartAllAbilities();
                    heroController.Hero.Speed.ApplyBonusValueChange(-3);
                    _collider.isTrigger = false;
                    _effect.SetActive(true);
                }
        }
    }
}