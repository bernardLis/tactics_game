using UnityEngine;
using System.Threading.Tasks;

public class PushableObstacle : Obstacle, IPushable<Vector3, GameObject, Ability>, IUITextDisplayable, ICreatable<Vector3, Ability>
{
    // global
    BattleManager _battleManager;

    // summon (falling) boulder
    [SerializeField] GameObject _shadow;

    // push
    BoxCollider2D _selfCollider;
    Vector3 _finalPos;
    CharacterStats _targetStats;
    int _damage = 50;

    // display info
    string _displayText = "Boulder, you can move if you know the technique.";

    // Start is called before the first frame update
    void Start()
    {
        _battleManager = BattleManager.Instance;
        _selfCollider = GetComponent<BoxCollider2D>();
    }

    public async Task Initialize(Vector3 pos, Ability ability)
    {
        await Fall(pos);

        AstarPath.active.Scan();

        _selfCollider = GetComponent<BoxCollider2D>();
        _selfCollider.enabled = false;

        Collider2D col = Physics2D.OverlapCircle(pos, 0.2f);

        await CheckCollision(ability, col);

        if (_selfCollider != null)
            _selfCollider.enabled = true;

        FindObjectOfType<BoardManager>().AddEnvObject(transform);
    }

    public async Task Fall(Vector3 pos)
    {
        _shadow.SetActive(true);
        SpriteRenderer shadowRenderer = _shadow.GetComponent<SpriteRenderer>();
        _shadow.transform.SetParent(null, true);
        _shadow.transform.position = pos;

        float randX = Random.Range(3f, 7f);
        float randY = Random.Range(3f, 7f);
        if (Random.Range(0, 2) == 0)
            randX *= -1;

        Vector3 startPos = new Vector3(pos.x + randX, pos.y + randY, pos.z);
        transform.position = startPos;

        float timeToReachTarget = 1f;
        float t = 0;
        while (Vector2.Distance(pos, transform.position) > 0.01f)
        {
            t += Time.deltaTime / timeToReachTarget;
            transform.position = Vector2.Lerp(startPos, pos, t);

            shadowRenderer.color = new Color(0f, 0f, 0f, t);
            _shadow.transform.localScale = Vector3.one * (1 - t);

            await Task.Yield();
        }

        Destroy(_shadow);
    }

    public async Task GetPushed(Vector3 dir, GameObject attacker, Ability ability)
    {
        _finalPos = transform.position + dir;

        _selfCollider.enabled = false;
        Collider2D col = Physics2D.OverlapCircle(_finalPos, 0.2f);

        await MoveToPosition(_finalPos, 0.5f);
        await CheckCollision(ability, col);

        if (_selfCollider != null)
            _selfCollider.enabled = true;
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
        await DestroySelf();
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
        Animator anim = GetComponent<Animator>();
        if (anim != null)
            anim.Play("Stone Breaking");
        // TODO: waiting for animation to finish... too hard for now.
        // I think the animation is too short, I don't get it when I ask anim - I get new state
        await Task.Delay(500);

        Destroy(gameObject);
    }


    public string DisplayText() { return _displayText; }

}
