using System.Collections;
using DG.Tweening;
using Lis.Core;
using Lis.Map.MapNodes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lis.Map
{
    public class NodeControllerShop : NodeController
    {
        [SerializeField] Transform _coin;
        MyButton _showShopButton;

        protected override void ResolveNode()
        {
            base.ResolveNode();
            _showShopButton = new("Shop", "common__button", () => ShowShop());

            MapManager.ButtonContainer.Insert(0, _showShopButton);

            if (Node.IsVisited) return;
            StartCoroutine(ShowShopCoroutine());
            Node.NodeCompleted();
        }

        IEnumerator ShowShopCoroutine()
        {
            _coin.DOKill();

            _coin.DOMoveY(1.5f, 1f).SetEase(Ease.InOutSine);
            _coin.DOLocalRotate(new(90, 360, 0), 1f);

            yield return new WaitForSeconds(1f);

            ShopScreen ss = ShowShop();
            ss.OnHide += () =>
            {
                _coin.DOScale(0f, 0.5f).SetEase(Ease.InOutBack)
                    .OnComplete(() => { Icon.gameObject.SetActive(true); });
            };
        }

        ShopScreen ShowShop()
        {
            ShopScreen shop = new();
            shop.InitializeShop((MapNodeShop)Node);
            return shop;
        }

        public override void LeaveNode()
        {
            base.LeaveNode();
            _showShopButton.RemoveFromHierarchy();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if (this != PlayerController.CurrentNode) return;
            ShowShop();
        }


        public override void SetAvailable()
        {
            base.SetAvailable();

            _coin.DOLocalRotate(new(90, 360, 0), 3f, RotateMode.FastBeyond360)
                .SetEase(Ease.InOutBack)
                .SetLoops(-1, LoopType.Yoyo)
                .SetDelay(Random.Range(0.1f, 0.5f));
        }

        public override void SetUnavailable()
        {
            base.SetUnavailable();
            if (this == PlayerController.CurrentNode) return;

            _coin.DOKill();
            Icon.gameObject.SetActive(true);
            Gfx.DOScale(0f, 0.5f).SetEase(Ease.InOutBack);
        }
    }
}