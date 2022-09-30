using System;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PushableObstacle : Creatable, IPushable<Vector3, GameObject, Ability>, ICreatable<Vector3, Ability, string>
{
    // global
    BattleManager _battleManager;

    // summon (falling) boulder
    [SerializeField] GameObject _shadow;
    [SerializeField] GameObject _poofEffect;

    // push
    Vector3 _finalPos;
    protected CharacterStats _targetStats;
    protected int _damage = 50;

    // display info
    string _displayText = "Boulder. You can push it with an ability.";

    string _lightFlickerTweenId = "_lightFlickerTweenId";

    public event Action OnPushed;

    // Start is called before the first frame update
    void Start()
    {
        _battleManager = BattleManager.Instance;
        _selfCollider = GetComponent<BoxCollider2D>();
        FlickerLight();
    }

    void FlickerLight()
    {
        Light2D l = GetComponentInChildren<Light2D>();
        if (l == null)
            return;
        DOTween.To(() => l.intensity, x => l.intensity = x, 0.2f, 4f).SetLoops(-1, LoopType.Yoyo).SetId(_lightFlickerTweenId);
    }

    public override async Task Initialize(Vector3 pos, Ability ability, string tag = "")
    {
        await Fall(pos);

        AstarPath.active.Scan();

        _selfCollider = GetComponent<BoxCollider2D>();
        _selfCollider.enabled = false;

        await CheckCollision(ability);

        if (_selfCollider != null)
            _selfCollider.enabled = true;

        BoardManager bm = FindObjectOfType<BoardManager>();
        if (bm)
            bm.AddEnvObject(transform);

        FlickerLight();
    }

    public async Task Fall(Vector3 pos)
    {
        SpriteRenderer boulderRenderer = GetComponent<SpriteRenderer>();
        boulderRenderer.sortingOrder = 99;

        _finalPos = pos;

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
        Destroy(Instantiate(_poofEffect, transform.position, Quaternion.identity), 1f);
        Destroy(_shadow);
        AudioManager.Instance.PlaySFX("StoneBreaking", transform.position);

        boulderRenderer.sortingOrder = 50;
    }

    public async Task GetPushed(Vector3 dir, GameObject attacker, Ability ability)
    {
        _finalPos = transform.position + dir;

        _selfCollider.enabled = false;

        OnPushed?.Invoke();

        await MoveToPosition(_finalPos, 0.5f);
        await CheckCollision(ability);

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

    public async Task CheckCollision(Ability ability)
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(_finalPos, 0.2f);

        foreach (Collider2D c in cols)
        {
            // player/enemy get damaged  and are moved back to their starting position
            // character colliders are children
            if (c.CompareTag(Tags.Player) || c.CompareTag(Tags.Enemy))
            {
                await CollideWithCharacter(ability, c);
                continue;
            }

            // character bounces back from being pushed into obstacle (and takes damage)
            if (c.CompareTag(Tags.Obstacle) || c.CompareTag(Tags.BoundCollider))
            {
                await CollideWithIndestructible(ability, c);
                continue;
            }

            // both boulders are destoryed
            if (c.CompareTag(Tags.PushableObstacle))
            {
                await CollideWithDestructible(ability, c);
                continue;
            }

            if (c.CompareTag(Tags.FireOnTile))
            {
                await CollideWithFire(ability, c);
                continue;
            }

            if (c.CompareTag(Tags.WaterOnTile))
            {
                await CollideWithWater(ability, c);
                continue;
            }
        }
        // TODO: nastiness that works.
        Invoke("ScanAstar", 1f);
    }

    void ScanAstar()
    {
        Debug.Log($"Scanning astar");
        AstarPath.active.Scan();
    }

    public virtual async Task CollideWithCharacter(Ability ability, Collider2D col)
    {
        _targetStats = col.GetComponent<CharacterStats>();
        await _targetStats.TakeDamageFinal(_damage);
        await DestroySelf();
    }

    public async Task CollideWithIndestructible(Ability ability, Collider2D col)
    {
        await DestroySelf();
    }

    public async Task CollideWithDestructible(Ability ability, Collider2D col)
    {
        await DestroySelf(false, false);

        if (col.TryGetComponent(out PushableObstacle pushable))
            await pushable.DestroySelf();
    }

    public async Task CollideWithFire(Ability ability, Collider2D col)
    {
        if (col.TryGetComponent(out FireOnTile fireOnTile))
            await fireOnTile.DestroySelf();
    }

    public async Task CollideWithWater(Ability ability, Collider2D col)
    {
        if (col.TryGetComponent(out WaterOnTile waterOnTile))
            await waterOnTile.DestroySelf();
    }

    public override async Task DestroySelf(bool playEffects = true, bool scanAstar = true)
    {

        _selfCollider.enabled = false;
        if (scanAstar)
            ScanAstar();

        transform.DOKill();
        if (playEffects == false)
        {
            Destroy(Instantiate(_poofEffect, transform.position, Quaternion.identity), 1f);
            AudioManager.Instance.PlaySFX("StoneBreaking", transform.position);
            Animator anim = GetComponent<Animator>();
            Debug.Log($"anim {anim}");
            if (anim != null)
            {
                Debug.Log($"animnot null ");

                anim.Play("Stone Breaking");

            }
            // TODO: waiting for animation to finish... too hard for now.
            // I think the animation is too short, I don't get it when I ask anim - I get new state
            await Task.Delay(500);
        }

        Destroy(gameObject);
    }


    public override VisualElement DisplayText() { return new Label(_displayText); }

    public override string GetCreatedObjectDescription() { return "Boulder."; }
}
