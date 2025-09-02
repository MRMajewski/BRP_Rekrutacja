using System;
using UnityEngine;
using UnityEngine.UI;

public class UiView : MonoBehaviour, IUIPanel
{

    [Header("UI VIEW elements")] [SerializeField]
    private bool UnpauseOnClose = false;

    [SerializeField] private bool CloseOnNewView = true;
    [SerializeField] private Button BackButon;

    [SerializeField] protected GameObject firstSelected;

    private UiView _parentView;


    public virtual void Awake()
    {
        BackButon.onClick.AddListener(() => DisableView_OnClick(this));
    }

    public void ActiveView_OnClick(UiView viewToActive)
    {
        viewToActive.SetParentView(this);
        viewToActive.ActiveView();
        this.ActiveView(!CloseOnNewView);
    }

    private void DisableView_OnClick(UiView viewToDisable)
    {
        viewToDisable.DisableView();
    }

    public void DestroyView_OnClick(UiView viewToDisable)
    {
        viewToDisable.DestroyView();
    }

    public void SetParentView(UiView parentView)
    {
        _parentView = parentView;
    }

    public void ActiveView(bool active)
    {
        this.gameObject.SetActive(active);
    }

    public void ActiveView(Action onBackButtonAction = null)
    {
        if (onBackButtonAction != null) BackButon.onClick.AddListener(() => onBackButtonAction());

        if (!gameObject.activeSelf) 
            this.ActiveView(true);
    }

    public void DisableView()
    {
        if (_parentView != null)
        {
            _parentView.ActiveView();
        }
        
        if (UnpauseOnClose) GameControlller.Instance.IsPaused = false;

        this.ActiveView(false);
    }

    public void DestroyView()
    {
        if (_parentView != null)
        {
            _parentView.ActiveView();
        }

        Destroy(this.gameObject);
    }

    public void DisableBackButton()
    {
        BackButon.gameObject.SetActive(false);
    }

    public Button GetBackButton()
    {
        return BackButon;
    }

    public void OnPanelActivated()
    {
       ActiveView_OnClick(this);
    }

    public void OnPanelDeactivated()
    {
        DisableView_OnClick(this);
    }

    public string GetPanelName()
    {
        throw new NotImplementedException();
    }

    public GameObject GetFirstSelected()
    {
        return firstSelected;
    }
}