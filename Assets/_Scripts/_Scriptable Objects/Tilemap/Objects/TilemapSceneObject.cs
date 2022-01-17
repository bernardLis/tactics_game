using UnityEngine;

public enum TilemapObjectPlacement { Left, Top, Right, Bottom, Any }


[CreateAssetMenu(menuName = "Tilemap/SceneObject")]
public class TilemapSceneObject : TilemapObject
{
    public Vector2Int accessSize; // when it comes to scene, it's tiles that are a joint
    public float offsetX;
    public float offsetY;

    public TilemapObjectPlacement placement;
}
