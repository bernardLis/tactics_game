using UnityEngine;
using System.Threading.Tasks;

public enum AbilityType { Attack, Heal, Move, Buff, Utility }

public abstract class Ability : BaseScriptableObject
{
    [Header("Characteristics")]
    public string Description = "New Description";
    public Sprite Icon;
    public AudioClip Sound;
    [Tooltip("Base strength of attack / heal")]
    public int BasePower; // TODO: better name? AbilityStrength? Ability..? 
    public int ManaCost;
    [Tooltip("0 is one tile, 1 is a cross")]
    public int AreaOfEffect;
    public AbilityType AbilityType;
    public WeaponType WeaponType; // abilities have weapons that can use them
    public GameObject Projectile; // TODO: is this a correct impementation, should it be a Scriptable Object?
    
    [Header("Abilities can add modifiers and statuses on interaction")]
    public StatModifier StatModifier;
    public Status Status;

    [Header("Highlight")]
    public int Range;
    public bool CanTargetSelf;
    public bool CanTargetDiagonally;
    public Color HighlightColor;

    protected AudioSource _audioSource;
    protected GameObject _characterGameObject;
    protected Highlighter _highlighter;
    protected BattleCharacterController _battleCharacterController;

    public virtual void Initialize(GameObject self)
    {
        _characterGameObject = self;
        _highlighter = BattleManager.instance.GetComponent<Highlighter>();
        _battleCharacterController = BattleCharacterController.instance;
        _audioSource = AudioScript.instance.GetComponent<AudioSource>();
    }

    // TODO: I am not certain whether this is correct.
    public virtual bool CanHit(GameObject self, GameObject target)
    {
        int manDist = Helpers.GetManhattanDistance(self.transform.position, target.transform.position);
        return manDist <= Range;
    }

    public virtual async Task HighlightTargetable(GameObject self)
    {
        if (_characterGameObject.CompareTag("Player"))
            _battleCharacterController.UpdateCharacterState(CharacterState.SelectingInteractionTarget);

        if (Range == 0)
            _highlighter.HighlightSingle(self.transform.position, HighlightColor);
        else
            await _highlighter.HighlightTiles(_characterGameObject.transform.position, Range,
                       HighlightColor, CanTargetDiagonally, CanTargetSelf);
    }

    public virtual async Task HighlightAreaOfEffect(Vector3 middlePos)
    {
        if (_characterGameObject.CompareTag("Player"))
            _battleCharacterController.UpdateCharacterState(CharacterState.ConfirmingInteraction);

        if (AreaOfEffect == 0)
            _highlighter.HighlightSingle(middlePos, HighlightColor);
        else
            await _highlighter.HighlightTiles(middlePos, AreaOfEffect, HighlightColor, true, CanTargetSelf);
    }

    public virtual async Task<bool> TriggerAbility(GameObject target)
    {
        _audioSource.clip = Sound;
        _audioSource.Play();

        await Task.Yield(); // just to get rid of errors;
        return true;
    }
}
