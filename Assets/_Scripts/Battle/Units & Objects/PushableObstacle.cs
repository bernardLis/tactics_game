using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PushableObstacle : Obstacle, IPushable<Vector3, GameObject, Ability>, IUITextDisplayable
{
    // global
    BattleManager _battleManager;

    // push
    [SerializeField] AudioClip stoneCracking;
    Vector3 _finalPos;
    CharacterStats _targetStats;
    int _damage = 50;

    // display info
    string _displayText = "Boulder, you can move if you know the technique.";

    // Start is called before the first frame update
    void Start()
    {
        _battleManager = BattleManager.Instance;
    }

    public async Task GetPushed(Vector3 dir, GameObject attacker, Ability ability)
    {
        _finalPos = transform.position + dir;

        BoxCollider2D selfCollider = transform.GetComponentInChildren<BoxCollider2D>();
        selfCollider.enabled = false;
        Collider2D col = Physics2D.OverlapCircle(_finalPos, 0.2f);

        if (col == null)
            selfCollider.enabled = true;

        await MoveToPosition(_finalPos, 0.5f);
        await CheckCollision(ability, col);

        if (selfCollider != null)
            selfCollider.enabled = true;
    }

    public async Task MoveToPosition(Vector3 finalPos, float time)
    {
        Vector3 startingPos = transform.position;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }

        _battleManager.SnapToGrid(transform);
    }

    public async Task CheckCollision(Ability ability, Collider2D col)
    {
        // nothing to collide with = being pushed into empty space
        if (col == null)
            return;

        // player/enemy get damaged  and are moved back to their starting position
        // character colliders are children
        if (col.CompareTag(Tags.Player) || col.transform.gameObject.CompareTag(Tags.Enemy))
            await CollideWithCharacter(ability, col);

        // character bounces back from being pushed into obstacle (and takes damage)
        if (col.CompareTag(Tags.Obstacle) || col.CompareTag(Tags.BoundCollider))
            await CollideWithIndestructible(ability, col);

        // character destroys boulder when they are pushed into it
        if (col.CompareTag(Tags.PushableObstacle))
            await CollideWithDestructible(ability, col);
    }

    public async Task CollideWithCharacter(Ability ability, Collider2D col)
    {
        _targetStats = col.GetComponent<CharacterStats>();
        await _targetStats.TakeDamageNoDodgeNoRetaliation(_damage);

        Destroy(gameObject);
    }

    public async Task CollideWithIndestructible(Ability ability, Collider2D col)
    {
        await DestroySelf();
    }

    public async Task CollideWithDestructible(Ability ability, Collider2D col)
    {
        await DestroySelf();
    }

    async Task DestroySelf()
    {
        AudioManager.Instance.PlaySound("Stone Breaking");
        Animator anim = GetComponentInChildren<Animator>();
        if (anim != null)
            anim.Play("Stone Breaking");
        // TODO: waiting for animation to finish... too hard for now.
        // I think the animation is too short, I don't get it when I ask anim - I get new state
        await Task.Delay(500);

        // TODO: add some sound and visual
        Destroy(gameObject);
    }


    public string DisplayText() { return _displayText; }

}
