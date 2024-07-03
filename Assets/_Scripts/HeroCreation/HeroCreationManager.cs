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

        [SerializeField] GameObject _femalePillar;
        [SerializeField] GameObject _malePillar;

        [SerializeField] GameObject _femaleExplosion;
        [SerializeField] GameObject _maleExplosion;

        [SerializeField] Sound _explosionSound;

        [SerializeField] ItemSetter _femaleHero;
        [SerializeField] ItemSetter _maleHero;

        VisualHero _femaleVisualHero;
        VisualHero _maleVisualHero;

        VisualHero _currentVisualHero;

        ItemSetter _currentHero;

        public VisualElement Root;
        VisualElement _bodySelectionContainer;
        VisualElement _visualOptionContainer;
        ScrollView _customizationScrollView;
        VisualElement _buttonContainer;

        TextField _nameField;
        Label _currentBodyLabel;

        protected override void Awake()
        {
            base.Awake();

            Root = GetComponent<UIDocument>().rootVisualElement;
            _bodySelectionContainer = Root.Q<VisualElement>("bodySelectionContainer");
            _visualOptionContainer = Root.Q<VisualElement>("visualOptionContainer");
            _customizationScrollView = Root.Q<ScrollView>("customizationScrollView");
            _buttonContainer = Root.Q<VisualElement>("buttonContainer");
        }

        void Start()
        {
            _gameManager = GameManager.Instance;

            _cameraManager = GetComponent<CameraManager>();

            AddTitle();

            AddNameField();
            InitializeBodies();

            HandleBodySelectionScreen();

            AddUiButtons();
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

        void InitializeBodies()
        {
            _femaleVisualHero = ScriptableObject.CreateInstance<VisualHero>();
            _maleVisualHero = ScriptableObject.CreateInstance<VisualHero>();
            _femaleVisualHero.Initialize(0);
            _maleVisualHero.Initialize(1);

            _femaleHero.Initialize(_gameManager.UnitDatabase.GetAllFemaleHeroOutfits(), _femaleVisualHero);
            _maleHero.Initialize(_gameManager.UnitDatabase.GetAllMaleHeroOutfits(), _maleVisualHero);

            _femaleHero.Activate();
            _maleHero.Activate();

            _currentHero = _femaleHero;
        }

        void HandleBodySelectionScreen()
        {
            VisualElement femaleBodyContainer = Root.Q<VisualElement>("femaleBodySelectorContainer");
            VisualElement maleBodyContainer = Root.Q<VisualElement>("maleBodySelectorContainer");

            femaleBodyContainer.RegisterCallback<PointerUpEvent>(SelectFemale);
            maleBodyContainer.RegisterCallback<PointerUpEvent>(SelectMale);
        }

        void SelectFemale(PointerUpEvent evt)
        {
            DefaultSelect();
            _cameraManager.SelectBodyType(0);
            _maleExplosion.SetActive(true);
            AudioManager.Instance.CreateSound()
                .WithSound(_explosionSound)
                .WithPosition(_maleHero.transform.position)
                .Play();
            _maleHero.Deactivate();
            _currentHero = _femaleHero;
        }

        void SelectMale(PointerUpEvent evt)
        {
            DefaultSelect();
            _cameraManager.SelectBodyType(1);
            _femaleExplosion.SetActive(true);
            AudioManager.Instance.CreateSound()
                .WithSound(_explosionSound)
                .WithPosition(_femaleHero.transform.position)
                .Play();

            _femaleHero.Deactivate();
            _currentHero = _maleHero;
        }

        void DefaultSelect()
        {
            _bodySelectionContainer.style.display = DisplayStyle.None;

            _buttonContainer.style.display = DisplayStyle.Flex;
            _visualOptionContainer.style.display = DisplayStyle.Flex;
            _buttonContainer.style.opacity = 0;
            _visualOptionContainer.style.opacity = 0;

            DOTween.To(x => _buttonContainer.style.opacity = x,
                _buttonContainer.style.opacity.value, 1, 0.5f).SetDelay(0.5f);
            DOTween.To(x => _visualOptionContainer.style.opacity = x,
                _visualOptionContainer.style.opacity.value, 1, 0.5f).SetDelay(0.5f);
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