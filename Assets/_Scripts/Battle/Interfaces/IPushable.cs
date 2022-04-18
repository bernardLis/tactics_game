using System.Threading.Tasks;
using UnityEngine;
public interface IPushable<Vector3, T, Y>
{
    public Task GetPushed(Vector3 dir, T attacker, Y ability);

    public Task MoveToPosition(Vector3 finalPos, float time);
    public async Task CheckCollision(Y ability, Collider2D col)
    {
        // nothing to collide with = being pushed into empty space
        if (col == null)
            return;

        // player/enemy get damaged  and are moved back to their starting position
        // character colliders are children
        if (col.CompareTag(Tags.PlayerCollider) || col.transform.gameObject.CompareTag(Tags.EnemyCollider))
            await CollideWithCharacter(ability, col);

        // character bounces back from being pushed into obstacle (and takes damage)
        if (col.CompareTag(Tags.Obstacle) || col.CompareTag(Tags.BoundCollider))
            await CollideWithIndestructible(ability, col);

        // character destroys boulder when they are pushed into it
        if (col.CompareTag(Tags.PushableObstacle))
            await CollideWithDestructible(ability, col);
    }

    public Task CollideWithCharacter(Y ability, Collider2D col);
    public Task CollideWithIndestructible(Y ability, Collider2D col);
    public Task CollideWithDestructible(Y ability, Collider2D col);

}
