using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UIElements;
using UnityEngine.Rendering.Universal;
using Pathfinding;

public class AsuraStrikeEffect : Effect
{
    [SerializeField] Sprite[] _kanjis;
    GameObject[] _kanjiDisplayers;
    Camera _cam;

    VisualElement _root;
    VisualElement _container;
    Light2D _globalLight;
    float _startIntensity;

    AILerp _aiLerp;
    Vector3 _newPos;

    // Start is called before the first frame update
    public override async Task Play(Ability ability, Vector3 targetPos)
    {
        // screen shake 
        _cam = Helpers.Camera;
        _cam.GetComponent<CameraManager>().Shake();

        // lower the light intensity for the effect
        BoardManager bm = BoardManager.Instance;
        _globalLight = bm.GlobalLight;
        _startIntensity = _globalLight.intensity;
        float targetIntensity = 0.8f;

        DOTween.To(() => _globalLight.intensity, x => _globalLight.intensity = x, targetIntensity, 0.2f);
        await Task.Delay(200);

        _root = BattleUI.Instance.Root;
        _container = new();
        _root.Add(_container);
        _container.style.flexDirection = FlexDirection.Row;
        _container.style.position = Position.Absolute;
        _container.style.top = Length.Percent(50);
        _container.style.height = 250;
        _container.style.width = Length.Percent(100);
        _container.style.justifyContent = Justify.Center;

        for (int i = 0; i < _kanjis.Length; i++)
        {
            if (i == _kanjis.Length / 2)
                MoveCharacter(ability, targetPos);

            VisualElement kanjiElement = new();
            kanjiElement.style.width = 200;
            kanjiElement.style.height = 200;
            kanjiElement.style.marginLeft = 30;
            kanjiElement.style.marginRight = 30;
            kanjiElement.style.backgroundImage = _kanjis[i].texture;
            _container.Add(kanjiElement);
            await Task.Delay(200);

        }
        Invoke("Cleanup", 0.5f);
    }

    void MoveCharacter(Ability ability, Vector3 pos)
    {
        Vector3 dir = pos - ability.CharacterGameObject.transform.position;
        _newPos = ability.CharacterGameObject.transform.position + dir * 2;
        // TODO:need to check whether the position is free if not just move to the characters pos
        Collider2D[] cols = Physics2D.OverlapCircleAll(_newPos, 0.2f);
        Debug.Log($"cols: {cols.Length}");
        if (cols.Length != 0)
            _newPos = pos;
        _aiLerp = ability.CharacterGameObject.GetComponent<AILerp>();
        _aiLerp.destination = _newPos;
        _aiLerp.canMove = false;
        ability.CharacterGameObject.transform.DOMove(_newPos, 0.5f);
        //aiLerp.SetPath(null);
    }

    async Task Cleanup()
    {
        await Task.Delay(1000);
        _root.Remove(_container);
        _aiLerp.Teleport(_newPos);
        _aiLerp.canMove = true;
        DOTween.To(() => _globalLight.intensity, x => _globalLight.intensity = x, _startIntensity, 0.5f);
        await Task.Delay(50);

        DestroySelf();
    }
}
