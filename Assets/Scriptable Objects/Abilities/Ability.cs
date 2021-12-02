using UnityEngine;
using System.Threading.Tasks;

public enum AbilityType { Attack, Defend, Heal, Move }

public abstract class Ability : ScriptableObject
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
    public int areaOfEffect; // 1 is one tile, 2 is a cross

    [Header("Highlight")]
    public int range;
    public bool canTargetSelf;
    public bool canTargetDiagonally;
    public Color highlightColor;

    protected AudioSource audioSource;

    GameObject characterGameObject;
    protected Highlighter highlighter;
    protected BattleCharacterController battleCharacterController;

    public virtual void Initialize(GameObject obj)
    {
        characterGameObject = obj;
        highlighter = GameManager.instance.GetComponent<Highlighter>();
        battleCharacterController = BattleCharacterController.instance;
        audioSource = AudioScript.instance.GetComponent<AudioSource>();
    }

    // TODO: this is wrong. BUUUT... I use it only for retaliation, where it is 'mostly' correct.
    public virtual bool CanHit(GameObject _self, GameObject _target)
    {
        // manhattan distance to see whether we are in range
        int manDistance = Mathf.FloorToInt(Mathf.Abs(_self.transform.position.x - _target.transform.position.x)
                                         + Mathf.Abs(_self.transform.position.y - _target.transform.position.y));

        if (manDistance > range)
            return false;

        return true;
    }
    public virtual async Task HighlightTargetable()
    {
        battleCharacterController.UpdateCharacterState(CharacterState.SelectingInteractionTarget);

        await highlighter.HighlightTiles(characterGameObject.transform.position, range,
                       highlightColor, canTargetDiagonally, canTargetSelf);

    }

    public virtual async Task HighlightAreaOfEffect(Vector3 _middle)
    {
        battleCharacterController.UpdateCharacterState(CharacterState.ConfirmingInteraction);
        if (areaOfEffect == 0)
            highlighter.HighlightSingle(_middle, highlightColor);
        else
            await highlighter.HighlightTiles(_middle, areaOfEffect, highlightColor, true, canTargetSelf);
    }

    public virtual async Task<bool> TriggerAbility(GameObject target)
    {
        // meant to be overwritten;
        await Task.Yield(); // just to get rid of errors;
        return false; // just to get rid of errors;
    }
}
