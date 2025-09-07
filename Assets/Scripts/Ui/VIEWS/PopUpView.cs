using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopUpView : MonoBehaviour
{
    [Header("Pop Up Elements")] public Text LabelText;
    public Text MessageText;
    public Button ConfirmButton;
    public Button CancelButton;

    public IUIPanelWithSelectionStack parentView;

    private void OnEnable()
    {
        GUIController.Instance.ActiveScreenBlocker(true, this);
    }

    private void OnDisable()
    {
        GUIController.Instance.ActiveScreenBlocker(false, this);
    }

    public void ActivePopUpView(PopUpInformation popUpInfo)
    {
        ClearPopUp();

        LabelText.text = popUpInfo.Header;
        MessageText.text = popUpInfo.Message;

        if (popUpInfo.UseOneButton)
        {
            ConfirmButton.GetComponentInChildren<Text>().text = "OK";
            CancelButton.gameObject.SetActive(false);
        }
        else
        {
            CancelButton.gameObject.SetActive(true);
            CancelButton.onClick.AddListener(() =>
            {
                popUpInfo.Cancel?.Invoke();
                ClosePopUp();
            });
        }

        ConfirmButton.onClick.AddListener(() =>
        {
            popUpInfo.Confirm?.Invoke();
            ClosePopUp();
        });

        EventSystem.current.SetSelectedGameObject(CancelButton.gameObject);
    }

    private void ClearPopUp()
    {
        LabelText.text = "";
        MessageText.text = "";
        ConfirmButton.onClick.RemoveAllListeners();
    }

    private void ClosePopUp()
    {
        Destroy(gameObject);
    }

}

public struct PopUpInformation
{
    public bool UseOneButton;
    public bool DisableOnConfirm;
    public string Header;
    public string Message;
    public Action Confirm;
    public Action Cancel;
}