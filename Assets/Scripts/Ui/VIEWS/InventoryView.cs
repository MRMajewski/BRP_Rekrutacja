using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryView : UiView, IUIViewWithSelectionStack
{
    #region Variables & References
    [Header("Inventory Elements")]
    [SerializeField] private SoulInformation soulItemPlaceholder;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text nameText;
    [SerializeField] private Image avatarImage;
    [SerializeField] private Button useButton;
    [SerializeField] private Button destroyButton;
    [SerializeField] private InputActionReference cancelAction;

    private RectTransform contentParent;
    private SoulInformation currentSoulInfo;
    private GameObject currentSelected;

    private readonly Stack<SelectionObjectInStack> selectionStack = new Stack<SelectionObjectInStack>();
    private readonly List<SoulInformation> soulsList = new List<SoulInformation>();
    #endregion

    #region Initialization & Setup
    private void Awake()
    {
        contentParent = (RectTransform)soulItemPlaceholder.transform.parent;
        InitializeInventoryItems();
    }

    private void OnEnable()
    {
        ClearSoulInfo();
    }

    private void InitializeInventoryItems()
    {
        soulsList.Clear();

        foreach (var soul in SoulController.Instance.Souls)
        {
            var newSoul = Instantiate(soulItemPlaceholder.gameObject, contentParent)
                .GetComponent<SoulInformation>();

            newSoul.SetSoulItem(soul, () => OnSoulClicked(newSoul));
            soulsList.Add(newSoul);
        }

        soulItemPlaceholder.gameObject.SetActive(false);
        firstSelected = soulsList.FirstOrDefault()?.gameObject;

        SetupGridNavigation();
    }

    private void SetupGridNavigation()
    {
        int columns = 3;

        for (int i = 0; i < soulsList.Count; i++)
        {
            var button = soulsList[i].GetComponent<Button>();
            if (button == null) continue;

            var nav = new Navigation { mode = Navigation.Mode.Explicit };
            int row = i / columns;
            int col = i % columns;

            if (col > 0) nav.selectOnLeft = soulsList[i - 1].GetComponent<Button>();
            if (col < columns - 1 && i + 1 < soulsList.Count) nav.selectOnRight = soulsList[i + 1].GetComponent<Button>();
            if (row > 0) nav.selectOnUp = soulsList[i - columns].GetComponent<Button>();
            if (i + columns < soulsList.Count) nav.selectOnDown = soulsList[i + columns].GetComponent<Button>();

            button.navigation = nav;
        }
    }

    private void SetupActionButtonsNavigation(SoulItem soul)
    {
        var useNav = new Navigation { mode = Navigation.Mode.None };
        var destroyNav = new Navigation { mode = Navigation.Mode.None };

        if (soul.CanBeUsed && soul.CanBeDestroyed)
        {
            useNav = new Navigation { mode = Navigation.Mode.Explicit, selectOnRight = destroyButton };
            destroyNav = new Navigation { mode = Navigation.Mode.Explicit, selectOnLeft = useButton };
        }

        useButton.navigation = useNav;
        destroyButton.navigation = destroyNav;
    }
    #endregion

    #region Actions & UI Logic
    private void OnSoulClicked(SoulInformation soulInfo)
    {
        currentSoulInfo = soulInfo;
        currentSelected = soulInfo.gameObject;

        ShowSoulInfo(soulInfo.soulItem);

        PushToSelectionStack(soulInfo.gameObject, () =>
        {
            if (soulInfo.gameObject == null)
                Debug.Log("Selection canceled");
        });

        SelectActionButton(soulInfo);
    }

    private void ShowSoulInfo(SoulItem soul)
    {
        descriptionText.text = soul.Description;
        nameText.text = soul.Name;
        avatarImage.sprite = soul.Avatar;

        SetupUseButton(soul.CanBeUsed);
        SetupDestroyButton(soul.CanBeDestroyed);
        SetupActionButtonsNavigation(soul);
    }

    private void ClearSoulInfo()
    {
        descriptionText.text = "";
        nameText.text = "";
        avatarImage.sprite = null;
        SetupUseButton(false);
        SetupDestroyButton(false);
        currentSelected = null;
        currentSoulInfo = null;
    }

    private void SetupUseButton(bool active)
    {
        useButton.onClick.RemoveAllListeners();
        useButton.gameObject.SetActive(active);
        if (!active) return;

        bool isCorrect = GameControlller.Instance.IsCurrentLocalization(currentSoulInfo.soulItem.UsableInLocalization);
        useButton.interactable = isCorrect;

        useButton.onClick.AddListener(() =>
        {
            if (isCorrect) UseSoul();
            else ShowCantUsePopup();
        });
    }

    private void SetupDestroyButton(bool active)
    {
        destroyButton.onClick.RemoveAllListeners();
        destroyButton.gameObject.SetActive(active);
        if (!active) return;

        destroyButton.onClick.AddListener(() =>
        {
            PushToSelectionStack(destroyButton.gameObject, () =>
            {
                SetSelection(destroyButton.gameObject);
                GUIController.Instance.CloseCurrentPopUp();
            });

            ShowDestroyPopup();
        });
    }

    private void SelectActionButton(SoulInformation soulInfo)
    {
        if (soulInfo.soulItem.CanBeUsed && useButton.interactable)
            SetSelection(useButton.gameObject);
        else if (soulInfo.soulItem.CanBeDestroyed && destroyButton.interactable)
            SetSelection(destroyButton.gameObject);
    }

    private void UseSoul()
    {
        soulsList.Remove(currentSoulInfo);
        soulsList.TrimExcess();
        SetupGridNavigation();

        Destroy(currentSelected);
        currentSelected = null;
        currentSoulInfo = null;
        ClearSoulInfo();

        SelectFallbackAfterRemoval();
        GameControlller.Instance.ScoreManager.AddPoints(75);
    }

    private void DestroySoul()
    {
        soulsList.Remove(currentSoulInfo);
        soulsList.TrimExcess();
        SetupGridNavigation();

        Destroy(currentSelected);
        ClearSoulInfo();

        var fallback = GetDefaultSelection();
        if (fallback != null)
            EventSystem.current.SetSelectedGameObject(fallback);
        else
            UIViewController.Instance.CloseCurrentPanel();
    }

    private void SelectFallbackAfterRemoval()
    {
        var fallback = GetDefaultSelection();
        if (fallback != null)
            EventSystem.current.SetSelectedGameObject(fallback);
        else
            UIViewController.Instance.CloseCurrentPanel();
    }

    private void ShowCantUsePopup()
    {
        GUIController.Instance.ShowPopUpMessage(new PopUpInformation
        {
            Header = "CAN'T USE",
            Message = "This soul can't be used here",
            UseOneButton = true,
            DisableOnConfirm = true,
            Confirm = ()=> TryHandleCancel()
        }, this);
    }

    private void ShowDestroyPopup()
    {
        GUIController.Instance.ShowPopUpMessage(new PopUpInformation
        {
            Header = "DESTROY ITEM",
            Message = $"Destroy {nameText.text}?",
            UseOneButton = false,
            Confirm = () =>
            {
                TryHandleCancel();
                DestroySoul();
            },
            Cancel = () => TryHandleCancel()
        }, this);
    }
    #endregion

    #region Selection Stack
    public void PushToSelectionStack(GameObject target, Action onCancel)
    {
        if (target == null) return;

        selectionStack.Push(new SelectionObjectInStack(target, onCancel));
        LogSelectionStack($"PushToSelectionStack -> {target.name}");
    }

    private void SetSelection(GameObject target)
    {
        if (target != null)
            EventSystem.current.SetSelectedGameObject(target);
    }

    public GameObject GetDefaultSelection()
    {
        return soulsList.FirstOrDefault()?.gameObject;
    }

    public bool TryHandleCancel()
    {
        while (selectionStack.Count > 0)
        {
            var current = selectionStack.Pop();
            if (current.SelectedObject == null) continue;

            current.OnCancel?.Invoke();

            if (current.SelectedObject != null && current.SelectedObject.activeInHierarchy)
            {
                EventSystem.current.SetSelectedGameObject(current.SelectedObject);
                return true;
            }
        }
        return false;
    }

    public void ClearStack() => selectionStack.Clear();
    public Stack<SelectionObjectInStack> GetSelectionStack() => selectionStack;

    private void LogSelectionStack(string context)
    {
        string topName = selectionStack.Count > 0 && selectionStack.Peek().SelectedObject != null
            ? selectionStack.Peek().SelectedObject.name
            : "NULL";

        Debug.Log($"[{context}] Stack count: {selectionStack.Count}, Top: {topName}");
    }
    #endregion
}
