using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Threading.Tasks;

public class JourneyNodeBehaviour : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public JourneyNode journeyNode;
    JourneyMapManager journeyMapManager;
    SpriteRenderer sr;

    Vector3 originalScale;

    public void Initialize(JourneyNode _jn)
    {
        journeyNode = _jn;
        journeyMapManager = JourneyMapManager.instance;

        sr = GetComponentInChildren<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    public void AnimateAvailableNode()
    {
        float duration = 1f;
        transform.DOScale(originalScale * 1.5f, duration).SetLoops(-1, LoopType.Yoyo);
        sr.DOColor(Color.black, duration).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopAnimating()
    {
        transform.DOKill();
        transform.localScale = originalScale;

        sr.DOKill();
        sr.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!journeyMapManager.availableNodes.Contains(journeyNode))
            transform.DOScale(originalScale * 1.2f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!journeyMapManager.availableNodes.Contains(journeyNode))
            transform.DOScale(originalScale, 1f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        journeyMapManager.NodeClick(this);
    }

    public void DrawCircle()
    {

        Animator anim = GetComponentInChildren<Animator>();
        anim.enabled = true;
        /*
        int segments = 128;


        LineRenderer lr = gameObject.AddComponent<LineRenderer>();
        Color c1 = new Color(0.5f, 0.5f, 0.5f, 1);
        lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
        lr.startColor = c1;
        lr.endColor = c1;
        lr.startWidth = 0.5f;
        lr.endWidth = 0.5f;
        lr.positionCount = (segments + 1);
        lr.useWorldSpace = false;

        
        lr.positionCount = (segments + 1);
        lr.useWorldSpace = false;
            
        await DrawPoints(lr, segments);
        */
    }

    async Task DrawPoints(LineRenderer _lr, int _segments)
    {
        float radius = 4f;
        float deltaTheta = (float)(2.0 * Mathf.PI) / _segments;
        float theta = 0f;
        for (int i = 0; i < _segments + 1; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, 0, z);
            _lr.SetPosition(i, pos);
            theta += deltaTheta;
            await Task.Delay(10);
        }


        /*
        float xRadius = 5f;
        float yRadius = 5f;

        float x;
        float y;
        float z;

        float angle = 20f;

        for (int i = 0; i < (_segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xRadius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * yRadius;

            _lr.SetPosition(i, new Vector3(x, y, 0));

            angle += (360f / _segments);

            await Task.Delay(10);
        }
        */
    }
}
