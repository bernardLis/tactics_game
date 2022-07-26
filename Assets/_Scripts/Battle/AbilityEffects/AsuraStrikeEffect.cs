using UnityEngine;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine.UIElements;
using Pathfinding;

public class AsuraStrikeEffect : AbilityEffect
{
    LightManager _lightManager;
    AudioManager _audioManager;

    [SerializeField] Sprite[] _kanjis;
    Camera _cam;

    VisualElement _root;
    VisualElement _container;

    AILerp _aiLerp;
    Vector3 _newPos;

    // Start is called before the first frame update
    public override async Task Play(Ability ability, Vector3 targetPos)
    {
        _lightManager = LightManager.Instance;

        _audioManager = AudioManager.Instance;

        // lower the light intensity for the effect
        _lightManager.ChangeGlobalLightIntensity(0.8f, 0.2f);
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

            VisualElement kanjiElement = new();
            kanjiElement.transform.scale = Vector3.one * 0.1f;
            kanjiElement.style.width = 200;
            kanjiElement.style.height = 200;
            DOTween.To(x => kanjiElement.transform.scale = x * Vector3.one, 0, 1, 0.5f);

            kanjiElement.style.marginLeft = 30;
            kanjiElement.style.marginRight = 30;
            kanjiElement.style.backgroundImage = _kanjis[i].texture;
            _container.Add(kanjiElement);
            _audioManager.PlaySFX("Bang", transform.position);

            await Task.Delay(400);
        }
        // screen shake 
        _cam = Helpers.Camera;
        _cam.GetComponent<CameraManager>().Shake();
        _audioManager.PlaySFX("AsuraStrike", transform.position);

        MoveCharacter(ability, targetPos);

        Invoke("Cleanup", 1.5f);
    }

    void MoveCharacter(Ability ability, Vector3 pos)
    {
        Vector3 dir = (pos - ability.CharacterGameObject.transform.position).normalized;
        _newPos = ability.CharacterGameObject.transform.position + dir * 2;
        // need to check whether the position is free if not just move to the characters pos
        Collider2D[] cols = Physics2D.OverlapCircleAll(_newPos, 0.2f);
        if (cols.Length != 0)
            _newPos = pos;
        _aiLerp = ability.CharacterGameObject.GetComponent<AILerp>();
        _aiLerp.destination = _newPos;
        _aiLerp.canMove = false;
        ability.CharacterGameObject.transform.DOMove(_newPos, 0.5f);
    }

    async Task Cleanup()
    {
        _root.Remove(_container);
        _aiLerp.Teleport(_newPos);
        _aiLerp.canMove = true;
        _lightManager.ResetGlobalLightIntensity(0.5f);

        await Task.Delay(50);

        DestroySelf();
    }
}
