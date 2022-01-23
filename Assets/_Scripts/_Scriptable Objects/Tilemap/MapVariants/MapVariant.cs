using UnityEngine;

public enum MapType { None, Circle, River, Lake, Hourglass }

[CreateAssetMenu(menuName = "Tilemap/MapVariant")]
public class MapVariant : BaseScriptableObject
{
    public string variantName;
    [Tooltip("min, max")]
    public Vector2 obstaclePercent;
    [Tooltip("min, max")]
    public Vector2 terrainIrregularitiesPercent;
    [Tooltip("min, max")]
    public Vector2 trapPercent;
    public MapType mapType;
}
