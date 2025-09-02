using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryView : UiView
{
    [Header("Inventory Elements")] [SerializeField]
    private SoulInformation SoulItemPlaceHolder;

    [SerializeField] private Text Description;
    [SerializeField] private Text Name;
    [SerializeField] private Image Avatar;
    [SerializeField] private Button UseButton;
    [SerializeField] private Button DestroyButton;

    private RectTransform _contentParent;
    private GameObject _currentSelectedGameObject;
    private SoulInformation _currentSoulInformation;

    [SerializeField]
    private List<SoulInformation> soulsList = new List<SoulInformation>();

    public override void Awake()
    {
        base.Awake();
        _contentParent = (RectTransform)SoulItemPlaceHolder.transform.parent;
        InitializeInventoryItems();
    }

    private void InitializeInventoryItems()
    {
        soulsList.Clear();
        soulsList.TrimExcess();

        for (int i = 0, j = SoulController.Instance.Souls.Count; i < j; i++)
        {
            SoulInformation newSoul = Instantiate(SoulItemPlaceHolder.gameObject, _contentParent).GetComponent<SoulInformation>();
            newSoul.SetSoulItem(SoulController.Instance.Souls[i], () => SoulItem_OnClick(newSoul));
            soulsList.Add(newSoul);
        }

        SoulItemPlaceHolder.gameObject.SetActive(false);

        firstSelected = soulsList.First().gameObject;

        SetupGridNavigation();
    }

    private void OnEnable()
    {
        ClearSoulInformation();
    }

    private void ClearSoulInformation()
    {
        Description.text = "";
        Name.text = "";
        Avatar.sprite = null;
        SetupUseButton(false);
        SetupDestroyButton(false);
        _currentSelectedGameObject = null;
        _currentSoulInformation = null;
    }

    public void SoulItem_OnClick(SoulInformation soulInformation)
    {
        _currentSoulInformation = soulInformation;
        _currentSelectedGameObject = soulInformation.gameObject;
        SetupSoulInformation(soulInformation.soulItem);
        SelectActionButton(soulInformation);

    }

    private void SelectActionButton(SoulInformation soulInformation)
    {
        if (soulInformation.soulItem.CanBeUsed)
        {
            EventSystem.current.SetSelectedGameObject(UseButton.gameObject);
        }
        else if (soulInformation.soulItem.CanBeDestroyed)
        {
            EventSystem.current.SetSelectedGameObject(DestroyButton.gameObject);
        }

    }

    private void SetupSoulInformation(SoulItem soulItem)
    {
        Description.text = soulItem.Description;
        Name.text = soulItem.Name;
        Avatar.sprite = soulItem.Avatar;
        SetupUseButton(soulItem.CanBeUsed);
        SetupDestroyButton(soulItem.CanBeDestroyed);
        SetupActionButtonsNavigation(soulItem);
    }
    private void SetupActionButtonsNavigation(SoulItem soulItem)
    {

        bool useActive = soulItem.CanBeUsed;
        bool destroyActive = soulItem.CanBeDestroyed;


        Navigation useNav = new Navigation { mode = Navigation.Mode.None };
        Navigation destroyNav = new Navigation { mode = Navigation.Mode.None };

        if (useActive && destroyActive)
        {

            useNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnRight = DestroyButton
            };

            destroyNav = new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnLeft = UseButton
            };
        }
        else if (useActive && !destroyActive)
        {
            useNav = new Navigation { mode = Navigation.Mode.None };
        }
        else if (!useActive && destroyActive)
        {
            destroyNav = new Navigation { mode = Navigation.Mode.None };
        }
        UseButton.navigation = useNav;
        DestroyButton.navigation = destroyNav;

    }
    private void SelectElement(int index)
    {

    }

    private void CantUseCurrentSoul()
    {
        PopUpInformation popUpInfo = new PopUpInformation { DisableOnConfirm = true, UseOneButton = true, Header = "CAN'T USE", Message = "THIS SOUL CANNOT BE USED IN THIS LOCALIZATION" };
        GUIController.Instance.ShowPopUpMessage(popUpInfo);
    }

    private void UseCurrentSoul(bool canUse)
    {
        if (!canUse)
        {
            CantUseCurrentSoul();
        }
        else
        {
            //USE SOUL
            Destroy(_currentSelectedGameObject);
            ClearSoulInformation();
        }
    }

    private void DestroyCurrentSoul()
    {
        Destroy(_currentSelectedGameObject);
        ClearSoulInformation();
    }

    private void SetupUseButton(bool active)
    {
        UseButton.onClick.RemoveAllListeners();
        if (active)
        {
            bool isInCorrectLocalization = GameControlller.Instance.IsCurrentLocalization(_currentSoulInformation.soulItem.UsableInLocalization);

            if (!isInCorrectLocalization)
            {
                UseButton.interactable = false;
                return;
            }


            PopUpInformation popUpInfo = new PopUpInformation
            {
                DisableOnConfirm = isInCorrectLocalization,
                UseOneButton = false,
                Header = "USE ITEM",
                Message = "Are you sure you want to USE: " + _currentSoulInformation.soulItem.Name + " ?",
                Confirm_OnClick = () => UseCurrentSoul(isInCorrectLocalization)
            };
            UseButton.onClick.AddListener(() => GUIController.Instance.ShowPopUpMessage(popUpInfo));
            UseButton.interactable = true;
        }
        UseButton.gameObject.SetActive(active);


    }
    private void SetupGridNavigation()
    {
        int columns = 3;

        for (int i = 0; i < soulsList.Count; i++)
        {
            Button button = soulsList[i].GetComponent<Button>();
            if (button == null) continue;

            Navigation nav = new Navigation
            {
                mode = Navigation.Mode.Explicit
            };

            int rowIndex = i / columns;
            int columnIndex = i % columns;

            if (columnIndex > 0)
                nav.selectOnLeft = soulsList[i - 1].GetComponent<Button>();

            if (columnIndex < columns - 1 && i + 1 < soulsList.Count)
                nav.selectOnRight = soulsList[i + 1].GetComponent<Button>();

            if (rowIndex > 0)
                nav.selectOnUp = soulsList[i - columns].GetComponent<Button>();

            if (i + columns < soulsList.Count)
                nav.selectOnDown = soulsList[i + columns].GetComponent<Button>();

            button.navigation = nav;
        }
    }

    private void SetupDestroyButton(bool active)
    {
        DestroyButton.onClick.RemoveAllListeners();
        if (active)
        {
            PopUpInformation popUpInfo = new PopUpInformation
            {
                DisableOnConfirm = true,
                UseOneButton = false,
                Header = "DESTROY ITEM",
                Message = "Are you sure you want to DESTROY: " + Name.text + " ?",
                Confirm_OnClick = () => DestroyCurrentSoul()
            };
            DestroyButton.onClick.AddListener(() => GUIController.Instance.ShowPopUpMessage(popUpInfo));
        }

        DestroyButton.gameObject.SetActive(active);
    }
}