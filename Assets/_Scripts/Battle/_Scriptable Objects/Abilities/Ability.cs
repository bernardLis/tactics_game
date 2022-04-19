using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor;

public enum AbilityType { Attack, Heal, Push, Buff, Utility }

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

    protected AudioSource _audioSource;
    public GameObject _characterGameObject;
    protected Highlighter _highlighter;
    protected BattleCharacterController _battleCharacterController;
    BattleUI _battleUI;

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
        _characterGameObject = self;
        _highlighter = BattleManager.Instance.GetComponent<Highlighter>();
        _battleCharacterController = BattleCharacterController.Instance;
        _audioSource = AudioManager.Instance.GetComponent<AudioSource>();
        _battleUI = BattleUI.Instance;
    }

    // TODO: I am not certain whether this is correct.
    public virtual bool CanHit(GameObject self, GameObject target)
    {
        int manDist = Helpers.GetManhattanDistance(self.transform.position, target.transform.position);
        return manDist <= Range;
    }

    public virtual async Task HighlightTargetable(GameObject self)
    {
        if (_characterGameObject.CompareTag(Tags.Player))
            _battleCharacterController.UpdateCharacterState(CharacterState.SelectingInteractionTarget);

        await _highlighter.HighlightAbilityRange(this);
    }

    public virtual async Task HighlightAreaOfEffect(Vector3 middlePos)
    {
        if (_characterGameObject.CompareTag(Tags.Player))
            _battleCharacterController.UpdateCharacterState(CharacterState.ConfirmingInteraction);

        await _highlighter.HighlightAbilityAOE(this, middlePos);
    }

    public virtual async Task<bool> TriggerAbility(GameObject target)
    {
        _audioSource.clip = Sound;
        _audioSource.Play();

        string abilityName = Helpers.ParseScriptableObjectCloneName(this.name);
        _battleUI.DisplayBattleLog($"{_characterGameObject.name} uses {abilityName} on {target.name} .");

        await Task.Yield(); // just to get rid of errors;
        return true;
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
}
