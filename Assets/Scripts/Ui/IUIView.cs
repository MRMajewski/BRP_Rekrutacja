using UnityEngine;
using UnityEngine.UI;

public interface IUIView
{

    void OnPanelActivated();

    void OnPanelDeactivated();

    bool GetCloseOnCancel();

    GameObject GetFirstSelected();
}
