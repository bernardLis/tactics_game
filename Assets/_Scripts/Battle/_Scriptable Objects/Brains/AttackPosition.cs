using UnityEngine;

public class AttackPosition
{
    public GameObject Target;
    public WorldTile Tile;
    public int AttackDirection;

    public AttackPosition(GameObject target, WorldTile tile, int attackDirection)
    {
        Target = target;
        Tile = tile;
        AttackDirection = attackDirection;
    }
}
