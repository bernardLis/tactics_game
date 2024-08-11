using Lis.Core;
using UnityEngine;

namespace Lis.Map.MapNodes
{
    [CreateAssetMenu(menuName = "ScriptableObject/Map/Map Node")]

    public class MapNode : BaseScriptableObject
    {
        public string NodeName;

        public GameObject NodePrefab;

        public Vector3 MapPosition;
        public bool IsVisited;
        public int Row;

       protected GameManager GameManager;

        public virtual void Initialize(Vector3 pos, int row)
        {
            GameManager = GameManager.Instance;

            MapPosition = pos;
            Row = row;
            if (row == 0) IsVisited = true;
        }
    }
}