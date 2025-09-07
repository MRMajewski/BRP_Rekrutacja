using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static GameplayInputController;

public class UIViewController : MonoBehaviour
{
    [SerializeField] private InputActionReference cancelAction;
    [SerializeField] private List<UIPanelData> panelsList;

    private Stack<Action> cancelStack = new Stack<Action>();
    private readonly Stack<IUIView> panelStack = new Stack<IUIView>();

    #region Initialization
    private void OnEnable()
    {
        if (cancelAction != null)
            cancelAction.action.performed += OnCancel;
    }

    private void OnDisable()
    {
        if (cancelAction != null)
            cancelAction.action.performed -= OnCancel;
    }
    #endregion

    #region Singleton
    private static UIViewController _instance;

    public static UIViewController Instance
    {
        get
        {
            if (_instance == null) _instance = FindFirstObjectByType<UIViewController>();
            return _instance;
        }
        set => _instance = value;
    }
    #endregion

    #region Panel Management
    public void OpenPanel(IUIView panel)
    {
        if (panel == null) return;

        if (panelStack.Count > 0)
            panelStack.Peek().OnPanelDeactivated();

        panelStack.Push(panel);
        panel.OnPanelActivated();

        FocusFirst(panel);
    }

    public void OpenPanelWithoutDeactivatePreviousPanel(IUIView panel)
    {

        panelStack.Push(panel);
        panel.OnPanelActivated();

        FocusFirst(panel);
    }

    public void OpenPanel(UiView panel)
    {
        OpenPanel((IUIView)panel);
    }

    public void OpenPanelWithoutDeactivatePreviousPanel(UiView panel)
    {
        OpenPanelWithoutDeactivatePreviousPanel((IUIView)panel);
    }

    public void OpenInventoryPanel()
    {
        var panelData = panelsList.Find(p => p.Name == "Inventory");
        if (panelData != null)
            OpenPanel(panelData.UIPanel);
    }

    public void OpenPausePanel()
    {
        var panelData = panelsList.Find(p => p.Name == "Pause");
        if (panelData != null)
            OpenPanel(panelData.UIPanel);
    }

    public void CloseCurrentPanel()
    {
        if (panelStack.Count == 0) return;

        var top = panelStack.Pop();
        top.OnPanelDeactivated();

        if (panelStack.Count > 0)
        {
            var previous = panelStack.Peek();
            previous.OnPanelActivated();
            FocusFirst(previous);
        }
        else
        {
            CloseAllPanels();
        }
    }

    private void FocusFirst(IUIView panel)
    {
        if (EventSystem.current == null || panel == null) return;

        var first = panel.GetFirstSelected();
        EventSystem.current.SetSelectedGameObject(null);
        if (first != null)
            EventSystem.current.SetSelectedGameObject(first);
    }
    #endregion

    #region Cancel Handling
    private void OnCancel(InputAction.CallbackContext _)
    {

        if (GameControlller.Instance.GameplayInput.CurrentMode != InputMode.UI)
            return;

        var currentPanel = GetCurrentPanel();

        if (currentPanel is IUIViewWithSelectionStack panelWithStack)
        {
            if (panelWithStack.TryHandleCancel())
                return;
        }

        CloseCurrentPanel();

        IUIView GetCurrentPanel()
        {
            return panelStack.Count > 0 ? panelStack.Peek() : null;
        }
    }

    public void PushCancelAction(Action action)
    {
        cancelStack.Push(action);
    }
    public void PopCancelAction()
    {
        if (cancelStack.Count > 0)
            cancelStack.Pop();
    }

    public void HandleCancel()
    {
        if (cancelStack.Count > 0)
        {
            var action = cancelStack.Pop();
            action?.Invoke();
        }
        else
        {
            CloseCurrentPanel();
        }
    }
    public void ClearCancelStack()
    {
        cancelStack.Clear();
    }
    public void CloseAllPanels()
    {
        while (panelStack.Count > 0)
            panelStack.Pop().OnPanelDeactivated();

        GameControlller.Instance.GameplayInput.ReturnFromUI();
    }

    #endregion
}

[Serializable]
public class UIPanelData
{
    public string Name;
    [SerializeField] private MonoBehaviour panelObject;
    public IUIView UIPanel => panelObject as IUIView;
}

