using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor;

public enum AbilityType { Attack, Heal, Push, Buff, Utility, Create, AttackCreate }

public abstract class Ability : BaseScriptableObject
{
    [Tooltip("Used for saving and character creation")]
    public string ReferenceID;

    [Header("Characteristics")]
    public string Description = "New Description";
    public Sprite Icon;
    public Sound Sound;

    [Tooltip("Base strength of attack / heal")]
    public int BasePower; // TODO: better name? AbilityStrength? Ability..?
    public int ManaCost;

    [Tooltip("0 is one tile, 1 is a cross")]
    public int AreaOfEffect;
    public bool LineAreaOfEffect;
    public AbilityType AbilityType;
    public StatType MultiplerStat;
    public WeaponType WeaponType; // abilities have weapons that can use them
    public GameObject Projectile; // TODO: is this a correct impementation, should it be a Scriptable Object?
    public GameObject AbilityEffect;
    public bool SpellcastAnimation;

    [Header("Abilities can add modifiers and statuses on interaction")]
    public StatModifier StatModifier;
    public Status Status;

    [Header("Highlight")]
    public int Range;
    public bool CanTargetSelf;
    public bool CanTargetDiagonally;
    public Color HighlightColor;


    [HideInInspector] public GameObject CharacterGameObject;
    protected CharacterStats _stats;
    protected FaceDirectionUI _faceDirectionUI;
    protected CharacterRendererManager _characterRendererManager;

    protected AudioSource _audioSource;
    protected HighlightManager _highlighter;
    protected BattleCharacterController _battleCharacterController;
    protected BattleUI _battleUI;

    Vector3 middleOfTargeting;

    List<CharacterSelection> affectedCharacters = new();

    // called from editor using table data
    public virtual void Create(Dictionary<string, object> item, StatModifier statModifier, Status status)
    {
        ReferenceID = item["ReferenceID"].ToString();
        Description = item["Description"].ToString();
        AbilityType = (AbilityType)System.Enum.Parse(typeof(AbilityType), item["AbilityType"].ToString());
        MultiplerStat = (StatType)System.Enum.Parse(typeof(StatType), item["MultiplierStat"].ToString());
        WeaponType = (WeaponType)System.Enum.Parse(typeof(WeaponType), item["WeaponType"].ToString());
        Projectile = (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/Prefabs/Battle/Projectiles/{item["Projectile"]}.prefab", typeof(GameObject));
        AbilityEffect = (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/Prefabs/Battle/Effects/AbilityEffects/{item["AbilityEffect"]}.prefab", typeof(GameObject));
        Icon = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/Sprites/Ability/{item["Icon"]}", typeof(Sprite));
        Sound = (Sound)AssetDatabase.LoadAssetAtPath($"Assets/_Scripts/General/Sounds/{item["Sound"]}.asset", typeof(Sound));
        BasePower = int.Parse(item["BasePower"].ToString());
        ManaCost = int.Parse(item["ManaCost"].ToString());
        AreaOfEffect = int.Parse(item["AreaOfEffect"].ToString());
        LineAreaOfEffect = item["LineAreaOfEffect"].ToString() == "TRUE" ? true : false;
        StatModifier = statModifier;
        Status = status;
        Range = int.Parse(item["Range"].ToString());
        CanTargetSelf = item["CanTargetSelf"].ToString() == "TRUE" ? true : false;
        CanTargetDiagonally = item["CanTargetDiagonally"].ToString() == "TRUE" ? true : false;
        HighlightColor = Utility.HexToColor(item["HighlightColor"].ToString());
    }

    public virtual void Initialize(GameObject self)
    {
        CharacterGameObject = self;
        _stats = self.GetComponent<CharacterStats>();
        _faceDirectionUI = self.GetComponent<FaceDirectionUI>();
        _characterRendererManager = self.GetComponentInChildren<CharacterRendererManager>();

        _highlighter = BattleManager.Instance.GetComponent<HighlightManager>();
        _battleCharacterController = BattleCharacterController.Instance;
        _audioSource = AudioManager.Instance.GetComponent<AudioSource>();
        _battleUI = BattleUI.Instance;
    }

    // TODO: I am not certain whether this is correct.
    // It's only used for retaliation so it is alright
    // it does not need to take 'pathfinding' into consideration.
    public virtual bool CanHit(GameObject self, GameObject target)
    {
        int manDist = Helpers.GetManhattanDistance(self.transform.position, target.transform.position);
        return manDist <= Range;
    }

    public virtual async Task HighlightTargetable(GameObject self)
    {
        ClearAffectedCharacters();

        if (CharacterGameObject.CompareTag(Tags.Player))
            _battleCharacterController.UpdateCharacterState(CharacterState.SelectingInteractionTarget);

        await _highlighter.HighlightAbilityRange(this);
    }

    public virtual async Task HighlightAreaOfEffect(Vector3 middlePos)
    {
        if (CharacterGameObject.CompareTag(Tags.Player))
            _battleCharacterController.UpdateCharacterState(CharacterState.ConfirmingInteraction);

        middleOfTargeting = middlePos;
        if (LineAreaOfEffect)
            await _highlighter.HighlightAbilityLineAOE(this, middlePos);
        else
            await _highlighter.HighlightAbilityAOE(this, middlePos);

        MarkAffectedCharacters();
    }

    public virtual async Task TriggerAbility(List<WorldTile> tiles)
    {
        // copy the list for safety
        List<WorldTile> targetTiles = new(tiles);

        // targeting self, you should be able to choose what direction to face
        Collider2D[] cols = Physics2D.OverlapCircleAll(middleOfTargeting, 0.2f);
        foreach (Collider2D c in cols)
            if (c.gameObject == CharacterGameObject && CharacterGameObject.CompareTag(Tags.Player))
                if (!await PlayerFaceDirSelection()) // allows to break out from selecing face direction TODO: it does not work
                    return;
        
        if (Sound != null)
            AudioManager.Instance.PlaySFX(Sound, CharacterGameObject.transform.position); // TODO: is that a correct place for sound

        foreach (WorldTile t in targetTiles)
        {
            // check if there is an object there and try to attack it
            Vector3 pos = t.GetMiddleOfTile();
            await AbilityLogic(pos);
        }


        ClearAffectedCharacters();
    }

    public virtual bool IsTargetViable(GameObject target)
    {
        // meant to be overwritten
        return false;// to get rid of errors;
    }

    public virtual async Task AbilityLogic(Vector3 pos)
    {
        // meant to be overwritten
        await Task.Yield(); // to get rid of errors;
    }

    public virtual int CalculateInteractionResult(CharacterStats attacker, CharacterStats defender)
    {
        int multiplierValue = 0;
        foreach (Stat s in attacker.Stats)
            if (s.Type == MultiplerStat)
                multiplierValue = s.GetValue();

        // -1 coz it is attack and has to be negative... TODO: this is very imperfect.
        return -1 * (BasePower + multiplierValue - defender.Armor.GetValue());
    }

    protected async Task<bool> PlayerFaceDirSelection()
    {
        Vector2 dir = Vector2.zero;
        if (_faceDirectionUI != null)
            dir = await _faceDirectionUI.PickDirection();

        // TODO: is that correct? facedir returns vector2.zero when it's broken out of
        if (dir == Vector2.zero)
            return false;

        _characterRendererManager.Face(dir.normalized);

        return true;
    }

    void MarkAffectedCharacters()
    {
        affectedCharacters.Clear();
        foreach (WorldTile tile in _highlighter.HighlightedTiles)
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(tile.GetMiddleOfTile(), 0.2f);
            foreach (Collider2D c in cols)
                if (c.TryGetComponent(out CharacterSelection selection))
                {
                    selection.ToggleSelectionArrow(true, HighlightColor);
                    affectedCharacters.Add(selection);
                }
        }
    }

    void ClearAffectedCharacters()
    {
        foreach (CharacterSelection selection in affectedCharacters)
        {
            if (selection == null)
                return;

            selection.ToggleSelectionArrow(false);
            if (selection.gameObject == _battleCharacterController.SelectedCharacter)
                selection.ToggleSelectionArrow(true, Color.white);
        }
    }

    /* UI Helpers */
    public string GetAOEDescription()
    {
        if (LineAreaOfEffect)
            return $"{Range} tiles in a line";

        if (AreaOfEffect == 0)
            return "1 tile";

        if (AreaOfEffect == 1)
            return "5 tiles in a cross";

        return $"Yes, scale: {AreaOfEffect}";
    }
}