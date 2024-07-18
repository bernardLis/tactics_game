using System;
using DG.Tweening;
using Lis.Map;
using Shapes;
using UnityEngine;

namespace Lis
{
    public class MapPathDrawer : MonoBehaviour
    {
        Line _line;

        string _tweenId;

        public void Initialize(NodeController startNode, NodeController endNode)
        {
            _line = GetComponent<Line>();
            _line.Start = startNode.transform.position;
            _line.End = endNode.transform.position;
            _line.gameObject.SetActive(false);

            _tweenId = Guid.NewGuid().ToString();
        }

        public void Activate()
        {
            _line.gameObject.SetActive(true);

            DOTween.To(x => _line.DashOffset = x, 0, 1, 1)
                .SetLoops(-1, LoopType.Restart)
                .SetEase(Ease.InOutSine)
                .SetId(_tweenId);
        }

        public void Deactivate()
        {
            DOTween.Kill(_tweenId);

            _line.gameObject.SetActive(false);
        }
    }
}