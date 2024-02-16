using System.Collections;
using UnityEngine;

namespace Lis
{
    public class BattleBud : BattleCreatureRanged
    {
        [SerializeField] GameObject _effect;
        GameObject _effectInstance;

        protected override IEnumerator PathToOpponent()
        {
            yield return ManageCreatureAbility();
            yield return base.PathToOpponent();
        }

        protected override IEnumerator CreatureAbility()
        {
            yield return base.CreatureAbility();

            // teleport
            _effectInstance = Instantiate(_effect, transform.position, Quaternion.identity);
            Vector3 point = ClosestPositionWithClearLos();
            transform.position = point;

            Invoke(nameof(CleanUp), 2f);
        }

        void CleanUp()
        {
            if (_effectInstance != null)
                Destroy(_effectInstance);
        }
    }
}