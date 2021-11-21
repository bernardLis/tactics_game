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
        // play animation TODO: add defend animation
        await characterRendererManager.SpellcastAnimation(dir);

        // add armor
        myStats.armor.AddModifier(value);
        myStats.UseMana(manaCost);

        return true;
    }
}
