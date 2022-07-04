using UnityEngine;

public enum MapType { None, Circle, River, Lake, Hourglass }

[CreateAssetMenu(menuName = "ScriptableObject/Tilemap/MapVariant")]
public class MapVariant : BaseScriptableObject
{
    public string VariantName;
    [Tooltip("min, max")]
    public Vector2 ObstaclePercent;
    [Tooltip("min, max")]
    public Vector2 TerrainIrregularitiesPercent;
    [Tooltip("min, max")]
    public MapType MapType;
}
