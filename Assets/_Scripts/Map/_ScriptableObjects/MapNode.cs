using Lis.Core;
using UnityEngine;

namespace Lis.Map
{
    [CreateAssetMenu(menuName = "ScriptableObject/Map/MapNode")]
    public class MapNode : BaseScriptableObject
    {
        public Vector3 MapPosition;
        public bool IsVisited;

        public Arena.Arena Arena;

        // TODO: possibly cutscene

        public void Initialize(Vector3 pos, int row)
        {
            MapPosition = pos;
            if (row == 0) IsVisited = true;
        }
    }
}