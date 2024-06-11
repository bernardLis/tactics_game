using System.Collections;
using Lis.Battle;
using Lis.Core;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class ObjectController : MonoBehaviour
    {
        protected Ability Ability;
        protected AudioManager AudioManager;

        protected AudioSource AudioSource;
        protected BattleManager BattleManager;

        void OnDestroy()
        {
            Ability.OnLevelUp -= OnAbilityLevelUp;
        }

        public virtual void Initialize(Ability ability)
        {
            AudioManager = AudioManager.Instance;

            Ability = ability;
            Ability.OnLevelUp += OnAbilityLevelUp;

            BattleManager = BattleManager.Instance;
            BattleManager.OnGamePaused += OnGamePaused;
            BattleManager.OnGameResumed += OnGameResumed;
        }

        protected virtual void OnGamePaused()
        {
            if (AudioSource != null)
                AudioSource.Pause();
        }

        protected virtual void OnGameResumed()
        {
            if (AudioSource != null)
                AudioSource.UnPause();
        }


        protected virtual void OnAbilityLevelUp()
        {
            // override
        }

        public virtual void Execute(Vector3 pos, Quaternion rot)
        {
            Transform t = transform;
            t.localPosition = pos;
            t.localRotation = rot;
            gameObject.SetActive(true);
            StartCoroutine(ExecuteCoroutine());
        }

        protected virtual IEnumerator ExecuteCoroutine()
        {
            yield return null;
        }

        public virtual void DisableSelf()
        {
            gameObject.SetActive(false);
        }
    }
}