using UnityEngine;

public class AttackPosition
{
    public GameObject target;
    public WorldTile tile;
    public int attackDirection;

    public AttackPosition(GameObject _target, WorldTile _tile, int _attackDirection)
    {
        target = _target;
        tile = _tile;
        attackDirection = _attackDirection;
    }
}
