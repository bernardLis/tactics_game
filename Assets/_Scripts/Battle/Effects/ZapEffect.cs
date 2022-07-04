using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class ZapEffect : Effect
{
    public override async Task Play(Ability ability, Vector3 targetPos)
    {
        ElectricLineController controller = GetComponent<ElectricLineController>();
        List<WorldTile> highlightedTiles = HighlightManager.Instance.HighlightedTiles;

        Vector3 characterPos = ability.CharacterGameObject.transform.position;
        controller.Electrify(characterPos);

        for (int i = 0; i < highlightedTiles.Count; i++)
        {
            controller.AddPosition(highlightedTiles[i].GetMiddleOfTile());
            await Task.Delay(50);
        }

        await Task.Delay(500);

        DestroySelf();
    }

}
