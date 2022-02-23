using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Character/Enemy")]
public class EnemyCharacter : Character
{
    string[] _names = new string[10]
    {
        "Liam",
        "Noah",
        "Oliver",
        "Elijah",
        "William",
        "James",
        "Benjamin",
        "Lucas",
        "Henry",
        "Alexander"
    };

    public Sprite[] Portraits;
    
    public Brain EnemyBrain;

    public EquipmentDatabase EquipmentDatabase;

    public override void Initialize(GameObject obj)
    {
        // randomized name and portrait
        CharacterName = _names[Random.Range(0, _names.Length)];
        Portrait = Portraits[Random.Range(0, Portraits.Length)];
        // TODO: understand reflections and get it done in a nice loop
        // enemy needs a body
        if (Body == null)
            Body = RandomizeEq(EquipmentSlot.Body);

        // TODO: need to differentiate between a piece of eq left blank on purpose and not
        // TODO: it would be fun if eq had influence on enemy stats
        // so you start fighting with enemies in rugs and end fighting enemies in golden armors
        // but that's for another day;
        if (Feet == null && Random.value > 0.5)
            Feet = RandomizeEq(EquipmentSlot.Feet);
        if (Hair == null && Random.value > 0.5)
            Hair = RandomizeEq(EquipmentSlot.Hair);
        if (Hands == null && Random.value > 0.5)
            Hands = RandomizeEq(EquipmentSlot.Hands);
        if (Helmet == null && Random.value > 0.5)
            Helmet = RandomizeEq(EquipmentSlot.Helmet);
        if (Legs == null && Random.value > 0.5)
            Legs = RandomizeEq(EquipmentSlot.Legs);
        if (Torso == null && Random.value > 0.5)
            Torso = RandomizeEq(EquipmentSlot.Torso);
        if (Shield == null && Random.value > 0.5)
            Shield = RandomizeEq(EquipmentSlot.Shield);
        if (Weapon == null)
            Weapon = (Weapon)RandomizeEq(EquipmentSlot.Weapon);

        base.Initialize(obj);
        var clone = Instantiate(EnemyBrain);
        clone.Initialize(obj);
        //enemyBrain = obj.AddComponent(typeof(EnemyAI)) as EnemyAI;
    }

    Equipment RandomizeEq(EquipmentSlot slot)
    {
        // TODO: this could be made smarter, right?
        if (slot == EquipmentSlot.Body)
            return EquipmentDatabase.AllBodies[Random.Range(0, EquipmentDatabase.AllBodies.Count)];
        if (slot == EquipmentSlot.Feet)
            return EquipmentDatabase.AllFeet[Random.Range(0, EquipmentDatabase.AllFeet.Count)];
        if (slot == EquipmentSlot.Hair)
            return EquipmentDatabase.AllHair[Random.Range(0, EquipmentDatabase.AllHair.Count)];
        if (slot == EquipmentSlot.Hands)
            return EquipmentDatabase.AllHands[Random.Range(0, EquipmentDatabase.AllHands.Count)];
        if (slot == EquipmentSlot.Helmet)
            return EquipmentDatabase.AllHelmets[Random.Range(0, EquipmentDatabase.AllHelmets.Count)];
        if (slot == EquipmentSlot.Legs)
            return EquipmentDatabase.AllLegs[Random.Range(0, EquipmentDatabase.AllLegs.Count)];
        if (slot == EquipmentSlot.Torso)
            return EquipmentDatabase.AllTorsos[Random.Range(0, EquipmentDatabase.AllTorsos.Count)];
        if (slot == EquipmentSlot.Shield)
            return EquipmentDatabase.AllShields[Random.Range(0, EquipmentDatabase.AllShields.Count)];
        if (slot == EquipmentSlot.Weapon)
            return EquipmentDatabase.AllWeapons[Random.Range(0, EquipmentDatabase.AllWeapons.Count)];

        return null;
    }


}
