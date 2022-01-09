using UnityEngine;
using System.Threading.Tasks;

public enum AbilityType { Attack, Heal, Move, Buff }

public abstract class Ability : BaseScriptableObject
{
    public string aName = "New Ability";
    public string aDescription = "New Description";

    public AbilityType aType;
    public WeaponType weaponType; // abilities have weapons that can use them
    public GameObject aProjectile; // TODO: is this a correct impementation, should it be a Scriptable Object?

    public Sprite aIcon;
    public AudioClip aSound;
    public int value;
    public int manaCost;
    [Tooltip("1 is one tile, 2 is a cross")]
    public int areaOfEffect;

    [Header("Abilities can add modifiers and statuses on interaction")]
    public StatModifier statModifier;
    public Status status;

    [Header("Highlight")]
    public int range;
    public bool canTargetSelf;
    public bool canTargetDiagonally;
    public Color highlightColor;

    protected AudioSource audioSource;

    protected GameObject characterGameObject;
    protected Highlighter highlighter;
    protected BattleCharacterController battleCharacterController;

    public virtual void Initialize(GameObject _self)
    {
        characterGameObject = _self;
        highlighter = GameManager.instance.GetComponent<Highlighter>();
        battleCharacterController = BattleCharacterController.instance;
        audioSource = AudioScript.instance.GetComponent<AudioSource>();
    }

    // TODO: I am not certain whether this is correct.
    public virtual bool CanHit(GameObject _self, GameObject _target)
    {
        int manDist = Helpers.GetManhattanDistance(_self.transform.position, _target.transform.position);
        return manDist <= range;
    }

    public virtual async Task HighlightTargetable(GameObject _self)
    {
        if (characterGameObject.CompareTag("Player"))
            battleCharacterController.UpdateCharacterState(CharacterState.SelectingInteractionTarget);

        if (range == 0)
            highlighter.HighlightSingle(_self.transform.position, highlightColor);
        else
            await highlighter.HighlightTiles(characterGameObject.transform.position, range,
                       highlightColor, canTargetDiagonally, canTargetSelf);
    }

    public virtual async Task HighlightAreaOfEffect(Vector3 _middle)
    {
        if (characterGameObject.CompareTag("Player"))
            battleCharacterController.UpdateCharacterState(CharacterState.ConfirmingInteraction);

        if (areaOfEffect == 0)
            highlighter.HighlightSingle(_middle, highlightColor);
        else
            await highlighter.HighlightTiles(_middle, areaOfEffect, highlightColor, true, canTargetSelf);
    }

    public virtual async Task<bool> TriggerAbility(GameObject _target)
    {
        audioSource.clip = aSound;
        audioSource.Play();

        await Task.Yield(); // just to get rid of errors;
        return true;
    }


}
