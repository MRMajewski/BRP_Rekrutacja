using System;
using UnityEngine;
using UnityEngine.UI;

public class UiView : MonoBehaviour, IUIView
{
    #region Variables
    [Header("UI VIEW Elements")]
    [SerializeField] protected GameObject firstSelected;
    [SerializeField] private bool UnpauseOnClose = false;
    [SerializeField] private bool closePanelOnCancel = true;
    #endregion


    #region View Logic
    public void ActiveView(bool active)
    {
        gameObject.SetActive(active);
    }

    public void DisableView()
    {
        ActiveView(false);

        if (UnpauseOnClose)
            GameControlller.Instance.IsPaused = false;
    }
    #endregion

    #region IUIView Implementation
    public virtual void OnPanelActivated()
    {
        ActiveView(true);
    }

    public virtual void OnPanelDeactivated()
    {
        DisableView();
    }

    public virtual GameObject GetFirstSelected() => firstSelected;

    public bool GetCloseOnCancel() => closePanelOnCancel;
    #endregion
}
