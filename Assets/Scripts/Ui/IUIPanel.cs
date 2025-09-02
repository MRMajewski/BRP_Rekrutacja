using UnityEngine;
using UnityEngine.UI;

public interface IUIPanel
{

    void OnPanelActivated();

    void OnPanelDeactivated();

    string GetPanelName();

    GameObject GetFirstSelected();
}
