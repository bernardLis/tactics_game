using System.Collections;
using Lis.Core;
using UnityEngine;

namespace Lis.Units.Hero.Ability
{
    public class ObjectController : MonoBehaviour
    {
       protected AudioManager AudioManager;
        protected Ability Ability;

        public virtual void Initialize(Ability ability)
        {
            AudioManager = AudioManager.Instance;

            Ability = ability;
            Ability.OnLevelUp += OnAbilityLevelUp;
        }

        protected virtual void OnAbilityLevelUp()
        {
            // override
        }

        void OnDestroy() => Ability.OnLevelUp -= OnAbilityLevelUp;

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
    }
}