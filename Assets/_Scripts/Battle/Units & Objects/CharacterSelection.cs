using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using Pathfinding;

public class CharacterSelection : MonoBehaviour
{
    protected Highlighter _highlighter;
    protected TurnManager _turnManager;
    protected SingleNodeBlocker _blocker;


    // https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
    protected Tilemap _tilemap;
    protected WorldTile _tile;

    // local
    protected CharacterStats _myStats;
    SpriteRenderer[] _spriteRenderers;
    Color _grayOutColor;

    public bool HasFinishedTurn { get; protected set; }

    public virtual void Awake()
    {
        _highlighter = Highlighter.Instance;
        _turnManager = TurnManager.Instance;
        _blocker = GetComponent<SingleNodeBlocker>();

        _tilemap = BattleManager.Instance.GetComponent<TileManager>().Tilemap;

        _myStats = GetComponent<CharacterStats>();
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        _grayOutColor = Helpers.GetColor("gray");

        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
    }

    protected void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.PlayerTurn)
            HandlePlayerTurn();
        if (state == BattleState.EnemyTurn)
            HandleEnemyTurn();
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
    }

    protected virtual void HandlePlayerTurn()
    {
        // meant to be overwritten
    }
    protected virtual void HandleEnemyTurn()
    {
        // meant to be overwritten
    }

    public virtual void FinishCharacterTurn()
    {
        _myStats.SetAttacker(false);
        SetHasFinishedTurn(true);
        GrayOutCharacter();
    }

    public void GrayOutCharacter()
    {
        foreach (SpriteRenderer rend in _spriteRenderers)
            if (rend != null)
                rend.DOColor(_grayOutColor, 1f);
    }

    protected void ReturnCharacterColor()
    {
        // shieet that looks way better than changing it right away;
        foreach (SpriteRenderer rend in _spriteRenderers)
            if (rend != null)
                rend.DOColor(Color.white, 2f);
    }

    public void SetHasFinishedTurn(bool has) { HasFinishedTurn = has; }

    public bool WillTakeTurn()
    {
        if (_myStats.IsStunned)
            return false;

        return true;
    }

    /* A* node blocker*/

    public void ActivateSingleNodeBlocker()
    {
        _blocker.manager = FindObjectOfType<BlockManager>();
        _blocker.BlockAtCurrentPosition();
    }


}
