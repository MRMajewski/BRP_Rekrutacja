using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class PopUpView : MonoBehaviour
{
   // public GameObject PopUpScreenBlocker;
    [Header("Pop Up Elements")] public Text LabelText;
    public Text MessageText;
    public Button ConfirmButton;
    public Button CancelButton;

    public IUIPanelWithSelectionStack parentView;

    //public override void Awake()
    //{
    //    GetBackButton().onClick.AddListener(() => DestroyView_OnClick(this));
    //}

    private void OnEnable()
    {
        GUIController.Instance.ActiveScreenBlocker(true, this);
    }

    private void OnDisable()
    {
        GUIController.Instance.ActiveScreenBlocker(false, this);
    }
    //public override void OnPanelActivated()
    //{
    //    gameObject.SetActive(true);
    //    GUIController.Instance.ActiveScreenBlocker(true, this);
    //}

    //public override void OnPanelDeactivated()
    //{
    //  //  this.OnPanelDeactivated();
    //  //  UIPanelController.Instance.CloseCurrentPanel();
    //    //test!
    //    //gameObject.SetActive(false);
    //    //GUIController.Instance.ActiveScreenBlocker(false, this);
    //    DestroyView();
    //}
    private void OnCancel(InputAction.CallbackContext ctx)
    {
        //var selected = EventSystem.current.currentSelectedGameObject;

        //// Jeśli zaznaczony jest Use albo Destroy → wracamy do SoulInformation
        //if (selected == UseButton.gameObject || selected == DestroyButton.gameObject)
        //{
        //    if (_currentSoulInformation != null)
        //    {
        //        EventSystem.current.SetSelectedGameObject(_currentSoulInformation.gameObject);
        //        closePanelOnCancel = true;
        //    }
        //}
        //else
        //{
        //    UIPanelController.Instance.CloseCurrentPanel();
        //}
    }
    //public override GameObject GetFirstSelected()
    //{
    //    return YesButton != null ? YesButton.gameObject : null;
    //}
    public void ActivePopUpView(PopUpInformation popUpInfo)
    {
        ClearPopUp();

        LabelText.text = popUpInfo.Header;
        MessageText.text = popUpInfo.Message;

        // Domyślne zaznaczenie -> idzie do parenta
        if (parentView != null)
        {
            parentView.SelectObject(CancelButton.gameObject, ClosePopUp);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(CancelButton.gameObject);
        }

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
    }
    //private void RegisterCancelForCurrent(GameObject current)
    //{
    //    //  var panelController = FindObjectOfType<UIPanelController>();
    //    UIPanelController.Instance.PushCancelAction(() =>
    //    {
    //        if (parentView.GetSelectionStack().Count > 0)
    //        {
    //            var previous = parentView.GetSelectionStack().Pop();
    //            EventSystem.current.SetSelectedGameObject(previous);
    //        }
    //    });
    //}
    private void ClearPopUp()
    {
        LabelText.text = "";
        MessageText.text = "";
        ConfirmButton.onClick.RemoveAllListeners();
    }

    private void ClosePopUp()
    {
     //   controller.ToggleScreenBlocker(false, this);
        Destroy(gameObject);
    }

    public Stack<GameObject> GetSelectionStack()
    {
        throw new NotImplementedException();
    }

    //void IUIPanelWithSelectionStack.RegisterCancelForCurrent(GameObject current)
    //{
    //    RegisterCancelForCurrent(current);
    //}
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