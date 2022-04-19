using System.Threading.Tasks;
using UnityEngine;
public interface IPushable<Vector3, T, Y>
{
    public Task GetPushed(Vector3 dir, T attacker, Y ability);

    public Task MoveToPosition(Vector3 finalPos, float time);
    public Task CheckCollision(Y ability, Collider2D col);
    public Task CollideWithCharacter(Y ability, Collider2D col);
    public Task CollideWithIndestructible(Y ability, Collider2D col);
    public Task CollideWithDestructible(Y ability, Collider2D col);

}
