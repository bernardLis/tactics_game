using Lis.Core;
using UnityEngine;

namespace Lis.Map
{
    [CreateAssetMenu(menuName = "ScriptableObject/Map/Map Node Fight")]
    public class MapNodeFight : MapNode
    {
        public Arena.Arena Arena;

        public override void Initialize(Vector3 pos, int row)
        {
            base.Initialize(pos, row);

            Arena = GameManager.Instance.GameDatabase.GetArena();
        }
    }
}