using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;
using Pathfinding;

public class CharacterSelection : MonoBehaviour
{
    protected HighlightManager _highlighter;
    protected TurnManager _turnManager;
    protected SingleNodeBlocker _blocker;


    // https://medium.com/@allencoded/unity-tilemaps-and-storing-individual-tile-data-8b95d87e9f32
    protected Tilemap _tilemap;
    protected WorldTile _tile;

    // local
    protected CharacterStats _myStats;
    SpriteRenderer[] _spriteRenderers;
    Color _grayOutColor;

    [SerializeField] SelectionArrow _selectionArrow;

    public bool HasFinishedTurn { get; protected set; }

    public virtual void Awake()
    {
        _highlighter = HighlightManager.Instance;
        _turnManager = TurnManager.Instance;
        _blocker = GetComponent<SingleNodeBlocker>();
        _blocker.manager = FindObjectOfType<BlockManager>();

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
        _blocker.BlockAtCurrentPosition();
    }

    public void DeactivateSingleNodeBlocker()
    {
        _blocker.Unblock();
    }

    public void ToggleSelectionArrow(bool isActive, Color? color = null)
    {
        // https://stackoverflow.com/questions/2804395/c-sharp-4-0-can-i-use-a-color-as-an-optional-parameter-with-a-default-value
        Color c = color ?? Color.white;
        _selectionArrow.gameObject.SetActive(isActive);
        _selectionArrow.GetComponent<SpriteRenderer>().color = c;
    }

}
