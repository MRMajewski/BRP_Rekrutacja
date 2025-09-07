using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PopUpView : MonoBehaviour
{
    #region Variables
    [Header("Pop Up Elements")] public Text LabelText;
    public Text MessageText;
    public Button ConfirmButton;
    public Button CancelButton;

    public IUIViewWithSelectionStack parentView;

    #endregion

    #region Popup Logic
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

        EventSystem.current.SetSelectedGameObject(ConfirmButton.gameObject);
    }

    private void ClearPopUp()
    {
        LabelText.text = "";
        MessageText.text = "";
        ConfirmButton.onClick.RemoveAllListeners();
        CancelButton.onClick.RemoveAllListeners();
    }

    private void ClosePopUp()
    {
        Destroy(gameObject);
    }
    #endregion
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