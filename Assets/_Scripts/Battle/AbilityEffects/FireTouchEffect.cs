using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class FireTouchEffect : AbilityEffect
{
    [SerializeField] GameObject _effectGameObject;

    public override async Task Play(Ability ability, Vector3 targetPos)
    {
        Debug.Log($"playing effect");
        HighlightManager highlightManager = HighlightManager.Instance;
        foreach (WorldTile tile in highlightManager.HighlightedTiles)
        {
            Vector3 tilePosition = tile.GetMiddleOfTile();
            Destroy(Instantiate(_effectGameObject, tilePosition, Quaternion.identity), 1f);
        }
        await Task.Delay(1);
    }

}
