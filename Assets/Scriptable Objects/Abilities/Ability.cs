using UnityEngine;
using System.Threading.Tasks;

public enum AbilityType { ATTACK, DEFEND, HEAL, MOVE }

public abstract class Ability : ScriptableObject
{
    public string aName = "New Ability";
    public string aDescription = "New Description";

    public AbilityType aType;
    public GameObject aProjectile; // TODO: is this a correct impementation, should it be a Scriptable Object?

    public Sprite aIcon;
    public AudioClip aSound;
    public int value;
    public int range;
    public int manaCost;

    [Header("Highlight")]
    public bool canTargetSelf;
    public bool canTargetDiagonally;
    public Color highlightColor;

    protected AudioSource audioSource;

    GameObject characterGameObject;
    protected Highlighter highlighter;
    CharacterStats myStats;

    public virtual void Initialize(GameObject obj)
    {
        characterGameObject = obj;
        highlighter = GameManager.instance.GetComponent<Highlighter>();
        myStats = obj.GetComponent<CharacterStats>();
        audioSource = AudioScript.instance.GetComponent<AudioSource>();
    }
    public virtual async Task HighlightTargetable()
    {
        await highlighter.HighlightTiles(characterGameObject.transform.position, range,
                       highlightColor, canTargetDiagonally, canTargetSelf);

    }
    public virtual async Task<bool> TriggerAbility(GameObject target)
    {
        // meant to be overwritten;
        await Task.Yield(); // just to get rid of errors;
        return false; // just to get rid of errors;
    }
}
