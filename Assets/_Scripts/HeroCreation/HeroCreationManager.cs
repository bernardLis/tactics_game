using DG.Tweening;
using Lis.Core;
using Lis.Core.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lis.HeroCreation
{
    public class HeroCreationManager : Singleton<HeroCreationManager>
    {
        const string _ussCommonTextVeryLarge = "common__text-very-large";
        const string _ussCommonTextLarge = "common__text-large";
        const string _ussCommonButtonArrow = "common__button-arrow";
        const string _ussCommonButton = "common__button";

        const string _ussClassName = "hero-creation__";
        const string _ussNameField = _ussClassName + "name-field";
        const string _ussIcon = _ussClassName + "icon";
        const string _ussRotateLeft = _ussClassName + "icon-rotate-left";
        const string _ussRotateRight = _ussClassName + "icon-rotate-right";
        const string _ussFace = _ussClassName + "icon-face";
        const string _ussBody = _ussClassName + "icon-body";

        GameManager _gameManager;
        CameraManager _cameraManager;

        [SerializeField] ItemDisplayer _femaleHero;
        [SerializeField] ItemDisplayer _maleHero;

        VisualHero _currentVisualHero;
        ItemDisplayer _currentHero;

        public VisualElement Root;
        VisualElement _visualOptionContainer;
        ScrollView _customizationScrollView;
        VisualElement _buttonContainer;

        TextField _nameField;
        Label _currentBodyLabel;

        protected override void Awake()
        {
            base.Awake();

            Root = GetComponent<UIDocument>().rootVisualElement;
            _visualOptionContainer = Root.Q<VisualElement>("visualOptionContainer");
            _customizationScrollView = Root.Q<ScrollView>("customizationScrollView");
            _buttonContainer = Root.Q<VisualElement>("buttonContainer");
        }

        void Start()
        {
            _gameManager = GameManager.Instance;

            _cameraManager = GetComponent<CameraManager>();
            _cameraManager.LookAtDefault();

            AddTitle();

            AddNameField();
            AddBodyItems(_customizationScrollView);
            InitializeBodies();

            AddUiButtons();
            ResolveCurrentHero();
        }

        void ResolveCurrentHero()
        {
            if (_gameManager.CurrentVisualHero != null)
                LoadHero(_gameManager.CurrentVisualHero);
            else
                ResetCurrentHero();
        }

        void LoadHero(VisualHero hero)
        {
            _currentVisualHero = hero;
            _nameField.value = hero.Name;
            if (hero.BodyType == 0 && _currentHero == _maleHero)
                ChangeBody();
            if (hero.BodyType == 1 && _currentHero == _femaleHero)
                ChangeBody();

            _currentHero.SetVisualHero(hero);
        }

        void ResetCurrentHero()
        {
            _currentVisualHero = ScriptableObject.CreateInstance<VisualHero>();
            _currentVisualHero.Initialize();
            if (_currentVisualHero.BodyType == 0 && _currentHero == _maleHero)
                ChangeBody();
            if (_currentVisualHero.BodyType == 1 && _currentHero == _femaleHero)
                ChangeBody();

            _nameField.value = "Tavski";
            _femaleHero.SetVisualHero(_currentVisualHero);
            _maleHero.SetVisualHero(_currentVisualHero);
        }

        void AddNameField()
        {
            _nameField = new();
            _nameField.value = "Tavski";
            _nameField.maxLength = 20;
            _nameField.AddToClassList(_ussNameField);
            _nameField.AddToClassList(_ussCommonTextLarge);
            _customizationScrollView.Add(_nameField);

            _nameField.RegisterValueChangedCallback((_) => _currentVisualHero.Name = _nameField.value);
        }

        void AddBodyItems(VisualElement parent)
        {
            VisualElement container = new();
            container.AddToClassList("item-selector-element__main");
            parent.Add(container);

            MyButton previousButton = new("<", _ussCommonButtonArrow, ChangeBody);
            MyButton nextButton = new(">", _ussCommonButtonArrow, ChangeBody);
            container.Add(previousButton);

            _currentBodyLabel = new("Body Type 0");
            _currentBodyLabel.style.fontSize = 26;
            container.Add(_currentBodyLabel);

            container.Add(nextButton);
        }

        void InitializeBodies()
        {
            _femaleHero.Initialize(_gameManager.UnitDatabase.GetAllFemaleHeroOutfits(), _currentVisualHero);
            _maleHero.Initialize(_gameManager.UnitDatabase.GetAllMaleHeroOutfits(), _currentVisualHero);

            _currentHero = _femaleHero;
            _femaleHero.Activate();
        }

        void ChangeBody()
        {
            if (_currentHero == _femaleHero)
            {
                _femaleHero.Deactivate();
                _maleHero.Activate();
                _currentBodyLabel.text = "Body Type 1";
                _currentHero = _maleHero;
                _currentVisualHero.BodyType = 1;

                return;
            }

            _maleHero.Deactivate();
            _femaleHero.Activate();
            _currentHero = _femaleHero;
            _currentVisualHero.BodyType = 0;

            _currentBodyLabel.text = "Body Type 0";
        }

        void AddUiButtons()
        {
            MyButton zoomOnHeadButton = new("", _ussIcon, () => _cameraManager.LookAtHead());
            MyButton zoomOnBodyButton = new("", _ussIcon, () => _cameraManager.LookAtDefault());
            MyButton rotateHeroLeft = new("", _ussIcon, RotateLeft);
            MyButton rotateHeroRight = new("", _ussIcon, RotateRight);

            zoomOnHeadButton.AddToClassList(_ussFace);
            zoomOnBodyButton.AddToClassList(_ussBody);
            rotateHeroLeft.AddToClassList(_ussRotateLeft);
            rotateHeroRight.AddToClassList(_ussRotateRight);

            _buttonContainer.Add(zoomOnHeadButton);
            _buttonContainer.Add(zoomOnBodyButton);

            VisualElement container = new();
            container.style.flexDirection = FlexDirection.Row;

            container.Add(rotateHeroLeft);
            container.Add(rotateHeroRight);
            _buttonContainer.Add(container);

            MyButton playButton = new("Play", _ussCommonButton, PlayGame);
            _buttonContainer.Add(playButton);
            MyButton backButton = new("Back", _ussCommonButton, GoBack);
            _buttonContainer.Add(backButton);
        }

        void RotateLeft()
        {
            Vector3 newRot = _currentHero.transform.localRotation.eulerAngles + new Vector3(0, 45, 0);
            _currentHero.transform.DOLocalRotate(newRot, 0.3f);
        }

        void RotateRight()
        {
            Vector3 newRot = _currentHero.transform.localRotation.eulerAngles + new Vector3(0, -45, 0);
            _currentHero.transform.DOLocalRotate(newRot, 0.3f);
        }

        void AddTitle()
        {
            Label title = new("Hero Customization");
            title.AddToClassList(_ussCommonTextVeryLarge);
            _visualOptionContainer.Insert(0, title);
            _visualOptionContainer.Insert(1, new HorizontalSpacerElement());
        }

        void PlayGame()
        {
            _gameManager.AddCurrentVisualHero();
            _gameManager.StartGame();
        }

        void GoBack()
        {
            _gameManager.LoadScene(Scenes.MainMenu);
        }
    }
}