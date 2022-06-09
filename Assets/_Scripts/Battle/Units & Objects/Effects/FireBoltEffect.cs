using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class FireBoltEffect : Effect
{
    [SerializeField] GameObject _bolt;
    [SerializeField] GameObject _smoke;
    [SerializeField] Color _targetColor;


    List<GameObject> _tempObjects = new();


    // I just want the bolt to be instantiated up high and fall on the highlighted tiles (yeah, multiple if the hightlight is multiple)
    public override async Task Play(Ability ability, Vector3 targetPos)
    {
        BoardManager bm = BoardManager.Instance;
        Light2D globalLight = bm.GlobalLight;
        Color startColor = globalLight.color;

        DOTween.To(() => globalLight.color, x => globalLight.color = x, _targetColor, 0.5f).SetTarget(globalLight);
        await Task.Delay(500);

        HighlightManager highlightManager = HighlightManager.Instance;
        foreach (WorldTile tile in highlightManager.HighlightedTiles)
        {
            Vector3 tilePosition = tile.GetMiddleOfTile();
            GameObject temp = new GameObject("temp");
            temp.transform.position = tilePosition;

            Vector3 skyPos = new Vector3(tilePosition.x + Random.Range(1f, 2f), tilePosition.y + 20f);
            GameObject bolt = Instantiate(_bolt, skyPos, Quaternion.identity);
            // look at the target; // forward makes it dissapear // right does not look right
            bolt.transform.right = tilePosition - skyPos; // https://answers.unity.com/questions/585035/lookat-2d-equivalent-.html
            bolt.transform.DOMove(tilePosition, 0.4f).OnComplete(() =>
            {
                bolt.SetActive(false);
                Destroy(Instantiate(_smoke, bolt.transform.position, Quaternion.identity), 0.5f);
            });
            await Task.Delay(500);

            _tempObjects.Add(bolt);
            _tempObjects.Add(temp);
        }

        DOTween.To(() => globalLight.color, x => globalLight.color = x, startColor, 0.5f).SetTarget(globalLight);
        await Task.Delay(500);
        CleanUp();
    }

    void CleanUp()
    {
        for (int i = _tempObjects.Count - 1; i >= 0; i--)
            Destroy(_tempObjects[i]);
    }
}
