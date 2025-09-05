using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventoryView : UiView, IUIPanelWithSelectionStack
{
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

    public override void Awake()
    {
        base.Awake();
        contentParent = (RectTransform)soulItemPlaceholder.transform.parent;
        InitializeInventoryItems();
    }

    private void OnEnable()
    {
        ClearSoulInfo();
        if (cancelAction != null)
            cancelAction.action.performed += OnCancel;
    }

    private void OnDisable()
    {
        if (cancelAction != null)
            cancelAction.action.performed -= OnCancel;
    }

    // -----------------------------
    // INIT
    // -----------------------------
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

    // -----------------------------
    // SELECT OBJECT AND ADD IT TO STACK
    // -----------------------------
    public void SelectObject(GameObject target, Action onCancel)
    {
        if (target == null) return;

        selectionStack.Push(new SelectionObjectInStack(target, onCancel));
        EventSystem.current.SetSelectedGameObject(target);
    }

    public GameObject GetDefaultSelection()
    {
        return soulsList.FirstOrDefault()?.gameObject;
    }

    public bool TryHandleCancel()
    {
        var fallback = GetDefaultSelection();

        if (selectionStack.Count == 0)
        {
            if (fallback != null)
                EventSystem.current.SetSelectedGameObject(fallback);
            return false;
        }

        var current = selectionStack.Pop();
        current.OnCancel?.Invoke();

        GameObject target = null;
        if (selectionStack.Count > 0)
        {
            var previous = selectionStack.Peek();
            if (previous.SelectedObject != null && previous.SelectedObject.activeInHierarchy)
            {
                target = previous.SelectedObject;
            }
            else
            {
                target = fallback;
            }
        }
        else
        {
            target = fallback;
        }

        if (target != null)
            EventSystem.current.SetSelectedGameObject(target);

        return true;
    }
    public void ClearStack() => selectionStack.Clear();
    public Stack<SelectionObjectInStack> GetSelectionStack() => selectionStack;

    // -----------------------------
    // ON SOUL CLICKED
    // -----------------------------
    private void OnSoulClicked(SoulInformation soulInfo)
    {
        currentSoulInfo = soulInfo;
        currentSelected = soulInfo.gameObject;

        ShowSoulInfo(soulInfo.soulItem);

        SelectObject(soulInfo.gameObject, () =>
        {
            Debug.Log($"Cancel on {soulInfo.soulItem.Name}");
            ClearSoulInfo();
        });

        SelectActionButton(soulInfo);
    }

    private void SelectActionButton(SoulInformation soulInfo)
    {
        if (soulInfo.soulItem.CanBeUsed && useButton.interactable)
        {
            SelectObject(useButton.gameObject, () =>
                EventSystem.current.SetSelectedGameObject(soulInfo.gameObject));
        }
        else if (soulInfo.soulItem.CanBeDestroyed && destroyButton.interactable)
        {
            SelectObject(destroyButton.gameObject, () =>
                EventSystem.current.SetSelectedGameObject(soulInfo.gameObject));
        }
    }

    // -----------------------------
    // Cancel
    // -----------------------------
    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (!TryHandleCancel())
            UIPanelController.Instance.CloseCurrentPanel();
    }



    // -----------------------------
    // UI Helpers
    // -----------------------------
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

        var isCorrect = GameControlller.Instance.IsCurrentLocalization(currentSoulInfo.soulItem.UsableInLocalization);
        useButton.interactable = isCorrect;

        useButton.onClick.AddListener(() =>
        {
            if (isCorrect) UseSoul(); else ShowCantUsePopup();
        });
    }

    private void SetupDestroyButton(bool active)
    {
        destroyButton.onClick.RemoveAllListeners();
        destroyButton.gameObject.SetActive(active);

        if (!active) return;

        destroyButton.onClick.AddListener(() => ShowDestroyPopup());
    }

    // -----------------------------
    // ACTIONS
    // -----------------------------
    private void UseSoul()
    {

        soulsList.Remove(currentSoulInfo);
        soulsList.TrimExcess();
        SetupGridNavigation();


        Destroy(currentSelected);

        ClearSoulInfo();
        TryHandleCancel();

      //  if (!TryHandleCancel())
       //     UIPanelController.Instance.CloseCurrentPanel();
    }

    private void DestroySoul()
    {
        soulsList.Remove(currentSoulInfo);
        soulsList.TrimExcess();
        SetupGridNavigation();

        Destroy(currentSelected);
        ClearSoulInfo();
        TryHandleCancel();

      //  if (!TryHandleCancel())
      //     UIPanelController.Instance.CloseCurrentPanel();
    }

    private void ShowCantUsePopup()
    {
        GUIController.Instance.ShowPopUpMessage(new PopUpInformation
        {
            Header = "CAN'T USE",
            Message = "This soul can't be used here",
            UseOneButton = true,
            DisableOnConfirm = true,
            Confirm = UseSoul,
            Cancel = () => {
                TryHandleCancel();
            }
        }, this);
    }

    private void ShowDestroyPopup()
    {
        GUIController.Instance.ShowPopUpMessage(new PopUpInformation
        {
            Header = "DESTROY ITEM",
            Message = $"Destroy {nameText.text}?",
            UseOneButton = false,
            Confirm = DestroySoul,
            Cancel = () => { TryHandleCancel(); 
            }
        }, this);
    }

    // -----------------------------
    // GRID NAV
    // -----------------------------
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
}
