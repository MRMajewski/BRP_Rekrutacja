using UnityEngine;

public class GUIController : MonoBehaviour
{

    #region singleton

    public static GUIController Instance;
    private PopUpView currentPopUp;
    private void Awake()
    {
        DisableOnStartObject.SetActive(false);
        Instance = this;
    }

    #endregion

    [SerializeField] private GameObject DisableOnStartObject;
    [SerializeField] private RectTransform ViewsParent;
    [SerializeField] private GameObject InGameGuiObject;
    [SerializeField] private PopUpView PopUp;
    [SerializeField] private PopUpScreenBlocker ScreenBlocker;

    private void Start()
    {
        if (ScreenBlocker) ScreenBlocker.InitBlocker();
    }

    private void ActiveInGameGUI(bool active)
    {
        InGameGuiObject.SetActive(active);
    }

    public void ShowPopUpMessage(PopUpInformation popUpInfo, IUIPanelWithSelectionStack parent = null)
    {
        //PopUpView newPopUp = Instantiate(PopUp, ViewsParent) as PopUpView;
        //newPopUp.ActivePopUpView(popUpInfo);
        PopUpView newPopUp = Instantiate(PopUp, ViewsParent);
        newPopUp.parentView = parent;
        newPopUp.ActivePopUpView(popUpInfo);
        //PopUpView newPopUp = Instantiate(PopUp, ViewsParent);
        //newPopUp.ActivePopUpView(popUpInfo);
        currentPopUp = newPopUp;
    }

    public void ClosePopUp(PopUpView popUpView)
    {
        if (popUpView != null)
        {
            Destroy(popUpView.gameObject);
            if (currentPopUp == popUpView)
                currentPopUp = null;
        }
    }

    public void CloseCurrentPopUp()
    {
        if (currentPopUp != null)
        {
            Destroy(currentPopUp.gameObject);
            currentPopUp = null;
        }
    }
    public void ActiveScreenBlocker(bool active, PopUpView popUpView)
    {
        if (active) ScreenBlocker.AddPopUpView(popUpView);
        else ScreenBlocker.RemovePopUpView(popUpView);
    }


    #region IN GAME GUI Clicks

    public void InGameGUIButton_OnClick(UiView viewToActive)
    {
        viewToActive.ActiveView(() => ActiveInGameGUI(true));

        ActiveInGameGUI(false);
        GameControlller.Instance.IsPaused = true;
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }
    
    #endregion
}