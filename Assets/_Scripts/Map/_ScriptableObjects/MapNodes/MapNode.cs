using Lis.Core;
using UnityEngine;

namespace Lis.Map
{
    [CreateAssetMenu(menuName = "ScriptableObject/Map/Map Node")]

    public class MapNode : BaseScriptableObject
    {
        public GameObject NodePrefab;

        public Vector3 MapPosition;
        public bool IsVisited;
        public int Row;

        public virtual void Initialize(Vector3 pos, int row)
        {
            MapPosition = pos;
            Row = row;
            if (row == 0) IsVisited = true;
        }
    }
}