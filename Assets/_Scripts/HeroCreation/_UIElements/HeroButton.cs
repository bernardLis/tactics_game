using System;
using Lis.Core;
using UnityEngine.UIElements;

namespace Lis.HeroCreation
{
    public class HeroButton : VisualElement
    {
        const string _ussCommonButton = "common__button";
        const string _ussCommonRemoveButton = "common__remove-button";

        readonly GameManager _gameManager;

        VisualHero _visualHero;
        readonly MyButton _button;
        readonly MyButton _removeButton;

        public event Action OnSelected;

        public HeroButton(VisualHero visualHero)
        {
            _gameManager = GameManager.Instance;
            _visualHero = visualHero;

            _button = new(visualHero.Name, _ussCommonButton, SelectHero);
            _removeButton = new("x", _ussCommonRemoveButton, RemoveHero);
            _removeButton.style.position = Position.Absolute;
            _removeButton.style.right = 0;
            _removeButton.style.top = 0;

            Add(_button);
            Add(_removeButton);
        }

        void SelectHero()
        {
            _gameManager.SelectHero(_visualHero);
            _gameManager.StartGame();
            OnSelected?.Invoke();
        }

        void RemoveHero()
        {
            _gameManager.RemoveVisualHero(_visualHero);
            RemoveFromHierarchy();
        }
    }
}