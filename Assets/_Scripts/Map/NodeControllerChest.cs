using UnityEngine;
using DG.Tweening;
using Lis.Core;

namespace Lis.Map
{
    public class NodeControllerChest : NodeController
    {
        [SerializeField] Sound _chestOpenSound;
        [SerializeField] Transform _chest;

        protected override void ResolveNode()
        {
            base.ResolveNode();
            if (!Node.IsVisited) ShowChest();
        }

        void ShowChest()
        {
            _chest.DOMoveY(1.5f, 1).SetEase(Ease.InOutSine);
            _chest.DOLocalRotate(new(0, -180, 0), 1).OnComplete(() =>
            {
                AudioManager.Instance.CreateSound().WithSound(_chestOpenSound)
                    .WithPosition(transform.position).Play();

                _chest.GetComponent<ChestController>().OpenChest();
            });
        }
    }
}