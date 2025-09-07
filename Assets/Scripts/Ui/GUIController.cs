using UnityEngine;

public class GUIController : MonoBehaviour
{

    #region Variables
    [SerializeField] private GameObject DisableOnStartObject;
    [SerializeField] private RectTransform ViewsParent;
    [SerializeField] private GameObject InGameGuiObject;
    [SerializeField] private PopUpView PopUp;
    [SerializeField] private PopUpScreenBlocker ScreenBlocker;

    #endregion
    #region singleton
    public static GUIController Instance;
    private PopUpView currentPopUp;
    private void Awake()
    {
        DisableOnStartObject.SetActive(false);
        Instance = this;
    }
    #endregion

    #region GUI Logic
    private void Start()
    {
        if (ScreenBlocker) ScreenBlocker.InitBlocker();
    }

    private void ActiveInGameGUI(bool active)
    {
        InGameGuiObject.SetActive(active);
    }

    public void ShowPopUpMessage(PopUpInformation popUpInfo, IUIViewWithSelectionStack parent = null)
    {
        PopUpView newPopUp = Instantiate(PopUp, ViewsParent);
        newPopUp.parentView = parent;
        newPopUp.ActivePopUpView(popUpInfo);
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
    #endregion


    #region IN GAME GUI Clicks

    public void InGameGUIButton_OnClick(UiView viewToActive)
    {
        viewToActive.ActiveView(true);
        ActiveInGameGUI(false);
        GameControlller.Instance.IsPaused = true;
    }

    public void ButtonQuit()
    {
        Application.Quit();
    }
    
    #endregion
}