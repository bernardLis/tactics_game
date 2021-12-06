using UnityEngine;
using System.Threading.Tasks;

public class DefendTriggerable : MonoBehaviour
{
    CharacterStats myStats;
    CharacterRendererManager characterRendererManager;
    FaceDirectionUI faceDirectionUI;

    void Awake()
    {
        myStats = GetComponent<CharacterStats>();
        characterRendererManager = GetComponentInChildren<CharacterRendererManager>();
        faceDirectionUI = GetComponent<FaceDirectionUI>();
    }

    public async Task<bool> Defend(int value, int manaCost)
    {
        Vector2 dir = await faceDirectionUI.PickDirection();

        // TODO: is that correct, facedir returns vector2.zero when it's broken out of
        if (dir == Vector2.zero)
            return false;

        // play animation TODO: add defend animation
        await characterRendererManager.SpellcastAnimation();

        // add armor
        myStats.armor.AddModifier(value);
        myStats.UseMana(manaCost);

        return true;
    }
}
