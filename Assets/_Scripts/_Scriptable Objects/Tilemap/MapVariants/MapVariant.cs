using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapType {None, Circle, River, Lake }

[CreateAssetMenu(menuName = "Tilemap/MapVariant")]
public class MapVariant : BaseScriptableObject
{
    public string variantName;
    [Tooltip("min, max")]
    public Vector2 obstaclePercent;
    [Tooltip("min, max")]
    public Vector2 terrainIrregularitiesPercent;
    public MapType mapType;
}
