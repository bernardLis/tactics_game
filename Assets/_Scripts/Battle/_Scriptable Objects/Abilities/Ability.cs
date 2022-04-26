using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor;

public enum AbilityType { Attack, Heal, Push, Buff, Utility, Create }

public abstract class Ability : BaseScriptableObject
{
    [HideInInspector] public string ReferenceID;

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
    public StatType MultiplerStat;
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

    [HideInInspector] public GameObject CharacterGameObject;
    protected FaceDirectionUI _faceDirectionUI;
    protected CharacterRendererManager _characterRendererManager;

    protected AudioSource _audioSource;
    protected Highlighter _highlighter;
    protected BattleCharacterController _battleCharacterController;

    Vector3 middleOfTargeting;

    // called from editor using table data
    public virtual void Create(Dictionary<string, object> item, StatModifier statModifier, Status status)
    {
        ReferenceID = item["ReferenceID"].ToString();
        Description = item["Description"].ToString();
        AbilityType = (AbilityType)System.Enum.Parse(typeof(AbilityType), item["AbilityType"].ToString());
        MultiplerStat = (StatType)System.Enum.Parse(typeof(StatType), item["MultiplierStat"].ToString());
        WeaponType = (WeaponType)System.Enum.Parse(typeof(WeaponType), item["WeaponType"].ToString());
        Projectile = (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/Prefabs/{item["Projectile"]}.prefab", typeof(GameObject));
        Icon = (Sprite)AssetDatabase.LoadAssetAtPath($"Assets/Sprites/Ability/{item["Icon"]}", typeof(Sprite));
        Sound = (AudioClip)AssetDatabase.LoadAssetAtPath($"Assets/Sounds/Ability/{item["Sound"]}", typeof(AudioClip));
        BasePower = int.Parse(item["BasePower"].ToString());
        ManaCost = int.Parse(item["ManaCost"].ToString());
        AreaOfEffect = int.Parse(item["AreaOfEffect"].ToString());
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
        _faceDirectionUI = self.GetComponent<FaceDirectionUI>();
        _characterRendererManager = self.GetComponentInChildren<CharacterRendererManager>();

        _highlighter = BattleManager.Instance.GetComponent<Highlighter>();
        _battleCharacterController = BattleCharacterController.Instance;
        _audioSource = AudioManager.Instance.GetComponent<AudioSource>();
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
        if (CharacterGameObject.CompareTag(Tags.Player))
            _battleCharacterController.UpdateCharacterState(CharacterState.SelectingInteractionTarget);

        await _highlighter.HighlightAbilityRange(this);
    }

    public virtual async Task HighlightAreaOfEffect(Vector3 middlePos)
    {
        if (CharacterGameObject.CompareTag(Tags.Player))
            _battleCharacterController.UpdateCharacterState(CharacterState.ConfirmingInteraction);

        middleOfTargeting = middlePos;
        await _highlighter.HighlightAbilityAOE(this, middlePos);
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

        foreach (WorldTile t in targetTiles)
        {
            // check if there is an object there and try to attack it
            Vector3 pos = t.GetMiddleOfTile();

            // TODO: sound
            _audioSource.clip = Sound;
            _audioSource.Play();

            await AbilityLogic(pos);
        }
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
}
