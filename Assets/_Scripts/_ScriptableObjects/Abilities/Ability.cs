using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor;

public enum AbilityType { Attack, Heal, Push, Buff, Create, AttackCreate }

public abstract class Ability : BaseScriptableObject
{
    [Tooltip("Used for saving and character creation")]
    public string ReferenceID;

    [Header("Base Characterstics")]
    public string Description = "New Description";
    public Sprite Icon;
    public Sound Sound;
    public int BasePower;
    public int ManaCost;
    public AbilityType AbilityType;
    public WeaponType WeaponType; // abilities have weapons that can use them


    [Header("AOE Characterstics")]
    [Tooltip("0 is one tile, 1 is a cross.")]
    public int AreaOfEffect;
    [Tooltip("Range determines how long the line is.")]
    public bool LineAreaOfEffect;

    [Header("Other Characterstics")]
    public bool SpellcastAnimation;
    public StatModifier StatModifier;
    public Status Status;
    public GameObject Projectile;
    public GameObject AbilityEffect;

    [Header("Highlight")]
    public int Range;
    public bool CanTargetSelf;
    public bool CanTargetDiagonally;
    public Color HighlightColor;

    [Header("Shop")]
    public int Price = 1;

    [HideInInspector] public GameObject CharacterGameObject;
    protected CharacterStats _stats;
    protected FaceDirectionUI _faceDirectionUI;
    protected CharacterRendererManager _characterRendererManager;

    protected AudioSource _audioSource;
    protected HighlightManager _highlighter;
    protected BattleCharacterController _battleCharacterController;
    protected BattleUI _battleUI;

    Vector3 _middleOfTargeting;

    List<CharacterSelection> affectedCharacters = new();


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
        await _highlighter.HighlightAbilityRange(this);
    }

    public virtual async Task HighlightAreaOfEffect(Vector3 middlePos)
    {
        if (CharacterGameObject.CompareTag(Tags.Player))
            _battleCharacterController.UpdateCharacterState(CharacterState.ConfirmingInteraction);

        _middleOfTargeting = middlePos;
        if (LineAreaOfEffect)
            await _highlighter.HighlightAbilityLineAOE(this, middlePos);
        else
            await _highlighter.HighlightAbilityAOE(this, middlePos);
    }

    public virtual async Task TriggerAbility(List<WorldTile> tiles)
    {
        // copy the list for safety
        List<WorldTile> targetTiles = new(tiles);

        // targeting self, you should be able to choose what direction to face
        Collider2D[] cols = Physics2D.OverlapCircleAll(_middleOfTargeting, 0.2f);
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

    public virtual int CalculateInteractionResult(CharacterStats attacker, CharacterStats defender, bool isRetaliation = false)
    {
        int damage = (BasePower + attacker.Power.GetValue() - defender.Armor.GetValue());

        // bonus for face dir
        if (AbilityType == AbilityType.Attack)
        {
            float bonusDamagePercent = CalculateBonusDamagePercent(attacker, defender, isRetaliation);
            damage += Mathf.FloorToInt(damage * bonusDamagePercent);
        }

        // -1 coz it is attack and has to be negative... TODO: this is very imperfect.
        return -1 * damage;
    }

    float CalculateBonusDamagePercent(CharacterStats attacker, CharacterStats defender, bool isRetaliation)
    {
        int attackDir = defender.CalculateAttackDir(attacker.gameObject.transform.position);
        if (isRetaliation)
            attackDir = 2;

        if (attackDir == 2)
            return 0;
        if (attackDir == 1)
            return 0.25f;
        if (attackDir == 0)
            return 0.50f;

        return 0;
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