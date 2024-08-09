using System.Collections.Generic;
using Lis.Core;
using UnityEngine;

namespace Lis.Map
{
    [CreateAssetMenu(menuName = "ScriptableObject/Map/Map")]
    public class Map : BaseScriptableObject
    {
        [HideInInspector] public List<MapRow> MapRows = new();

        public void Initialize()
        {
            MapRows.Clear();

            int rowCount = Random.Range(8, 11);
            for (int i = 0; i < rowCount; i++)
            {
                MapRow mr = CreateInstance<MapRow>();

                int count = Random.Range(2, 4);
                if (i == 0) count = 1;
                if (i == rowCount - 1) count = 1;

                mr.Initialize(count, i);
                MapRows.Add(mr);
            }
        }
    }
}