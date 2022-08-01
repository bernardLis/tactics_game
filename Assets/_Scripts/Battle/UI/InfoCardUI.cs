using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using UnityEngine.Tilemaps;
using System.Threading.Tasks;

public class InfoCardUI : Singleton<InfoCardUI>
{
    // global
    CharacterUI _characterUI;
    BattleCharacterController _battleCharacterController;
    MovePointController _movePointController;

    // tilemap
    Tilemap _tilemap;
    WorldTile _tile;

    // tile info
    VisualElement _tileInfoCard;
    Label _tileInfoText;

    // character card
    GameObject _displayedCharacter;
    VisualElement _characterCard;
    CharacterCardVisual _characterCardVisual;

    // interaction summary
    VisualElement _interactionSummary;

    Label _attackLabel;
    VisualElement _attackDamageGroup;

    VisualElement _retaliationSummary;
    Label _retaliationDamageValue;

    // animate card left right on show/hide
    float _cardShowValue = 0f;
    float _cardHideValue = -100f;
    string _hideTileInfoTweenID = "hideTileInfoTweenID";

    protected override void Awake()
    {
        base.Awake();

        // getting ui elements
        var root = GetComponent<UIDocument>().rootVisualElement;

        // tile info
        _tileInfoCard = root.Q<VisualElement>("tileInfoCard");
        _tileInfoText = root.Q<Label>("tileInfoText");

        // character card
        _characterCard = root.Q<VisualElement>("characterCard");
        _characterCard.style.left = Length.Percent(_cardHideValue);

        // interaction summary
        _interactionSummary = root.Q<VisualElement>("interactionSummary");
        _interactionSummary.style.alignSelf = Align.FlexStart; // idk where which button it is in gui

        _attackLabel = root.Q<Label>("attackLabel");
        _attackDamageGroup = root.Q<VisualElement>("attackDamageGroup");

        _retaliationSummary = root.Q<VisualElement>("retaliationSummary");
        _retaliationDamageValue = root.Q<Label>("retaliationDamageValue");
    }

    void Start()
    {
        // This is our Dictionary of tiles
        _tilemap = BattleManager.Instance.GetComponent<TileManager>().Tilemap;

        _characterUI = CharacterUI.Instance;
        _battleCharacterController = BattleCharacterController.Instance;
        _movePointController = MovePointController.Instance;

        TurnManager.OnBattleStateChanged += TurnManager_OnBattleStateChanged;
        MovePointController.OnMove += MovePointController_OnMove;
        BattleCharacterController.OnCharacterStateChanged += BattleCharacterController_OnCharacterStateChange;
    }

    void OnDestroy()
    {
        TurnManager.OnBattleStateChanged -= TurnManager_OnBattleStateChanged;
        MovePointController.OnMove -= MovePointController_OnMove;
        BattleCharacterController.OnCharacterStateChanged -= BattleCharacterController_OnCharacterStateChange;
    }

    void TurnManager_OnBattleStateChanged(BattleState state)
    {
        if (state == BattleState.Won || state == BattleState.Lost)
            HideAll();
    }

    void MovePointController_OnMove(Vector3 pos)
    {
        ShowTileInfo(pos);
        ResolveCharacterCard(pos);
        ResolveInteractionSummary(pos);
    }

    void BattleCharacterController_OnCharacterStateChange(CharacterState state)
    {
        if (state == CharacterState.None)
            HandleCharacterStateNone();
        if (state == CharacterState.Selected)
            HandleCharacterSelected();
        if (state == CharacterState.SelectingInteractionTarget)
            HandleCharacterSelectingInteractionTarget();
        if (state == CharacterState.SelectingFaceDir)
            return;
        if (state == CharacterState.ConfirmingInteraction)
            return;
    }

    void HandleCharacterStateNone()
    {
        ResolveInteractionSummary(_movePointController.transform.position);
    }

    void HandleCharacterSelected()
    {
        HideCharacterCard();
        ResolveInteractionSummary(_movePointController.transform.position);
        ShowTileInfo(_movePointController.transform.position);
    }

    void HandleCharacterSelectingInteractionTarget()
    {
        ResolveCharacterCard(_movePointController.transform.position);
        ResolveInteractionSummary(_movePointController.transform.position);
    }

    /* tile info */
    public void ShowTileInfo(Vector3 pos)
    {
        _tileInfoCard.Clear();

        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        foreach (Collider2D c in cols)
        {
            if (c.CompareTag(Tags.BoundCollider))
                _tileInfoCard.Add(new Label("Impassable map bounds."));
            if (c.TryGetComponent(out IUITextDisplayable uiText))
            {
                VisualElement l = uiText.DisplayText();
                l.style.whiteSpace = WhiteSpace.Normal;
                _tileInfoCard.Add(l);
            }
        }

        // hide/show the whole panel
        if (_tileInfoCard.childCount == 0)
        {
            HideTileInfo();
            return;
        }

        _tileInfoCard.style.display = DisplayStyle.Flex;

        DOTween.Pause(_hideTileInfoTweenID);

        DOTween.To(() => _tileInfoCard.style.left.value.value, x => _tileInfoCard.style.left = Length.Percent(x), _cardShowValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    public void HideTileInfo()
    {
        DOTween.To(() => _tileInfoCard.style.left.value.value, x => _tileInfoCard.style.left = Length.Percent(x), _cardHideValue, 0.5f)
               .SetEase(Ease.InOutSine)
               .OnComplete(() => DisplayNone(_tileInfoCard))
               .SetId(_hideTileInfoTweenID);
    }

    void ResolveCharacterCard(Vector3 pos)
    {
        // show character card if there is character standing there
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        foreach (Collider2D c in cols)
            if (_battleCharacterController.SelectedCharacter != c.gameObject
                && c.TryGetComponent(out CharacterStats stats))
            {
                ShowCharacterCard(stats);
                return;
            }

        // hide if it is something else
        if (TurnManager.BattleState == BattleState.Deployment) // show card is called from deployment controller
            return;

        HideCharacterCard();
    }

    /* character card */
    public void ShowCharacterCard(CharacterStats stats)
    {
        if (stats == null)
            return;
        if (_displayedCharacter == stats.gameObject)
            return;

        _displayedCharacter = stats.gameObject;
        _characterCard.Clear();
        stats.OnCharacterDeath += OnCharacterDeath;

        _characterCardVisual = new(stats);
        _characterCard.Add(_characterCardVisual);

        // show the card
        _characterCard.style.display = DisplayStyle.Flex;
        DOTween.To(() => _characterCard.style.left.value.value, x => _characterCard.style.left = Length.Percent(x), _cardShowValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    void OnCharacterDeath(GameObject g)
    {
        HideCharacterCard();
    }

    public void HideCharacterCard()
    {
        if (_displayedCharacter != null)
            _displayedCharacter.GetComponent<CharacterStats>().OnCharacterDeath -= OnCharacterDeath;

        _displayedCharacter = null;
        DOTween.To(() => _characterCard.style.left.value.value, x => _characterCard.style.left = Length.Percent(x), _cardHideValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    void ResolveInteractionSummary(Vector3 pos)
    {
        HideInteractionSummary();

        if (_battleCharacterController.SelectedAbility == null)
            return;
        Ability selectedAbility = _battleCharacterController.SelectedAbility;

        // don't show interaction summary if not in range of interaction
        Vector3Int tilePos = _tilemap.WorldToCell(pos);
        if (!TileManager.Tiles.TryGetValue(tilePos, out _tile))
            return;
        if (!_tile.WithinRange)
            return;

        // check if there is a character standing there
        Collider2D[] cols = Physics2D.OverlapCircleAll(pos, 0.2f);
        foreach (Collider2D c in cols)
            if (c.TryGetComponent(out CharacterStats stats))
            {
                CharacterStats attacker = _battleCharacterController.SelectedCharacter.GetComponent<CharacterStats>();
                CharacterStats defender = c.GetComponent<CharacterStats>();

                ShowInteractionSummary(attacker, defender, selectedAbility);
            }
    }

    public void ShowInteractionSummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        _interactionSummary.style.display = DisplayStyle.Flex;
        DOTween.To(() => _interactionSummary.style.left.value.value, x => _interactionSummary.style.left = Length.Percent(x), _cardShowValue, 0.5f)
               .SetEase(Ease.InOutSine);

        _characterUI.HideHealthChange();
        DisplayNone(_retaliationSummary); // flexing it when it is necessary
        _attackDamageGroup.Clear();

        // different labels and UI for heal / attack
        if (ability.AbilityType == AbilityType.Attack)
            HandleAttackAbilitySummary(attacker, defender, ability);

        if (ability.AbilityType == AbilityType.Push)
            HandlePushAbilitySummary(attacker, defender, ability);

        if (ability.AbilityType == AbilityType.Heal)
            HandleHealAbilitySummary(attacker, defender, ability);

        if (ability.AbilityType == AbilityType.Buff)
            HandleBuffAbilitySummary(attacker, defender, ability);

        if (ability.AbilityType == AbilityType.Create)
            HandleCreateAbilitySummary(attacker, defender, ability);

        if (ability.AbilityType == AbilityType.AttackCreate)
            HandleAttackCreateAbilitySummary(attacker, defender, ability);
    }

    async void HandleAttackAbilitySummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        _attackLabel.text = "Attack";

        await Task.Delay(100);
        int attackValue = ability.CalculateInteractionResult(attacker, defender);

        if (defender.IsShielded)
            attackValue = 0;

        TextWithTooltip value = new("Damage: " + (-1 * attackValue), "Attacking from the back +50% dmg, from the side +25% dmg");
        // color if bonus attack
        if (attackValue != 0)
        {
            int attackDir = defender.CalculateAttackDir(attacker.gameObject.transform.position);
            // side attack 1, face to face 2, from the back 0, 
            if (attackDir == 0)
                value.style.backgroundColor = Color.red;
            if (attackDir == 1)
                value.style.backgroundColor = new Color(1f, 0.55f, 0f);
        }

        _attackDamageGroup.Add(value);
        HandleStatusesAbilitySummary(ability);

        // self dmg
        if (attacker.gameObject == defender.gameObject)
            _characterUI.ShowHealthChange(attackValue);

        ShowHealthChange(defender, attackValue);
        ShowRetaliationSummary(attacker, defender, ability);
    }

    void HandlePushAbilitySummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        _attackLabel.text = "Push";

        int attackValue = ability.CalculateInteractionResult(attacker, defender);
        Label value = new("Damage: " + (-1 * attackValue)); // it looks weird when it is negative.
        _attackDamageGroup.Add(value);
        HandleStatusesAbilitySummary(ability);

        ShowHealthChange(defender, attackValue);
    }

    void HandleHealAbilitySummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        _attackLabel.text = "Heal";
        int healValue = ability.CalculateInteractionResult(attacker, defender, false);
        Label value = new("Heal: " + healValue); // it looks weird when it is negative.
        _attackDamageGroup.Add(value);
        HandleStatusesAbilitySummary(ability);

        if (attacker.gameObject == defender.gameObject)
            _characterUI.ShowHealthChange(healValue);

        ShowHealthChange(defender, healValue);
    }

    void HandleBuffAbilitySummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        _attackLabel.text = "Buff";

        HandleStatusesAbilitySummary(ability);
    }

    void HandleCreateAbilitySummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        _attackLabel.text = "Create";
        CreateAbility a = (CreateAbility)ability;
        if (a.CreatedObject.TryGetComponent(out IUITextDisplayable uiText))
        {
            Label value = new("Create: " + a.CreatedObject.GetComponent<Creatable>().GetCreatedObjectDescription());
            value.style.whiteSpace = WhiteSpace.Normal;
            _attackDamageGroup.Add(value);
        }
    }

    void HandleAttackCreateAbilitySummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        _attackLabel.text = "Attack & Create";
        AttackCreateAbility a = (AttackCreateAbility)ability;

        int attackValue = ability.CalculateInteractionResult(attacker, defender);
        Label value = new("Damage: " + (-1 * attackValue)); // it looks weird when it is negative.
        _attackDamageGroup.Add(value);

        // handle created object
        Creatable creatable = a.CreatedObject.GetComponent<Creatable>();
        Label createdObjectDesc = new("Creates: " + creatable.GetCreatedObjectDescription());
        _attackDamageGroup.Add(createdObjectDesc);

        if (creatable.Status == null)
            return;
        ModifierVisual mElement = new ModifierVisual(a.CreatedObject.GetComponent<Creatable>().Status);
        _attackDamageGroup.Add(mElement);
    }

    void HandleStatusesAbilitySummary(Ability ability)
    {
        if (ability.StatModifier != null)
        {
            ModifierVisual mElement = new ModifierVisual(ability.StatModifier);
            _attackDamageGroup.Add(mElement);
        }

        if (ability.Status != null)
        {
            ModifierVisual mElement = new ModifierVisual(ability.Status);
            _attackDamageGroup.Add(mElement);
        }
    }

    void ShowRetaliationSummary(CharacterStats attacker, CharacterStats defender, Ability ability)
    {
        // retaliation only if there is an ability that character can retaliate with
        Ability retaliationAbility = defender.GetRetaliationAbility();
        if (retaliationAbility == null)
            return;

        bool willRetaliate = defender.WillRetaliate(attacker.gameObject);
        bool canRetaliate = retaliationAbility.CanHit(defender.gameObject, attacker.gameObject);
        if (!willRetaliate || !canRetaliate)
            return;

        // show change in attackers health after they get retaliated on
        _retaliationSummary.style.display = DisplayStyle.Flex;

        int relatiationResult = retaliationAbility.CalculateInteractionResult(defender, attacker, true); // correct defender, attacker
        _retaliationDamageValue.text = "" + (-1 * relatiationResult);

        _characterUI.ShowHealthChange(relatiationResult);
    }

    public void HideInteractionSummary()
    {
        _attackDamageGroup.Clear();
        DOTween.To(() => _interactionSummary.style.left.value.value, x => _interactionSummary.style.left = Length.Percent(x), _cardHideValue, 0.5f)
               .SetEase(Ease.InOutSine);
    }

    void ShowHealthChange(CharacterStats stats, int val)
    {
        _characterCardVisual.HealthBar.DisplayInteractionResult(stats.MaxHealth.GetValue(),
                                                                stats.CurrentHealth,
                                                                val);
    }

    public void ShowManaChange(CharacterStats stats, int val)
    {
        _characterCardVisual.ManaBar.DisplayInteractionResult(stats.MaxMana.GetValue(),
                                                                stats.CurrentMana,
                                                                val);
    }

    void DisplayNone(VisualElement el) { el.style.display = DisplayStyle.None; }

    void HideAll()
    {
        HideCharacterCard();
        HideTileInfo();
        HideInteractionSummary();
    }
}
