using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameplayInputController : MonoBehaviour
{
    [SerializeField] private EnemiesController enemiesController;

  //  [SerializeField] private GUI

    private int _currentEnemyIndex = 0;


    private PadControls inputActions;

    private void Awake()
    {
        inputActions = new PadControls();
    }

    private void OnEnable()
    {
        // Subskrypcje
        inputActions.Gameplay.Options.performed += OnPause;
        inputActions.Gameplay.Inventory.performed += OnInventory;
        inputActions.Gameplay.Cancel.performed += OnCancel;
        inputActions.Gameplay.Confirm.performed += OnConfirm;
        inputActions.Gameplay.Enable();
    }

    private void OnDisable()
    {
        // Odsubskrybowanie
        inputActions.Gameplay.Options.performed -= OnPause;
        inputActions.Gameplay.Inventory.performed -= OnInventory;
        inputActions.Gameplay.Cancel.performed -= OnCancel;
        inputActions.Gameplay.Confirm.performed -= OnConfirm;

        inputActions.Gameplay.Disable();
    }
    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        //if (EventSystem.current.currentSelectedGameObject.TryGetComponent( out Button actionButton))
        //{
        //    actionButton.onClick.Invoke();
            
        //}
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        Debug.Log("Options pressed");
        GameControlller.Instance.IsPaused = true;
        GameControlller.Instance.UIPanelController.OpenPausePanel();
        SwitchToUIInput();
    }

    private void OnInventory(InputAction.CallbackContext ctx)
    {
        Debug.Log("Inventory pressed");
        GameControlller.Instance.IsPaused = true;
        GameControlller.Instance.UIPanelController.OpenInventoryPanel();
        SwitchToUIInput();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        Debug.Log("Cancel pressed");
        // TODO: logika cofania / zamykania paneli

        if (enemiesController.ActiveEnemies[0].IsActionPanelOpen)
        {
            enemiesController.ActiveEnemies[0].CancelCombatWithEnemy();
        }
    }


    public void SwitchToUIInput()
    {
        inputActions.Gameplay.Disable();
        inputActions.UI.Enable();
    }

    public void SwitchToGameplayInput()
    {
        inputActions.UI.Disable();
        inputActions.Gameplay.Enable();
    }

    public void ReturnFromUI()
    {
        SwitchToGameplayInput();

        if (enemiesController.ActiveEnemies[0].IsActionPanelOpen)
        {
            EventSystem.current.SetSelectedGameObject(enemiesController.ActiveEnemies[0].GetBowButton().gameObject);
        }
        else
            EventSystem.current.SetSelectedGameObject(enemiesController.ActiveEnemies[0].GetCombatButton().gameObject);
    }


    public void FocusOnFirstEnemy()
    {
        FocusEnemy(enemiesController.ActiveEnemies[0]);
    }

    private void FocusEnemy(SoulEnemy enemy)
    {
        if (enemy == null) return;
        EventSystem.current.SetSelectedGameObject(enemy.GetCombatButton().gameObject);
    }

    public void FocusNextEnemy()
    {
        if (enemiesController.ActiveEnemies.Count == 0) return;

        _currentEnemyIndex = (_currentEnemyIndex + 1) % enemiesController.ActiveEnemies.Count;
        FocusEnemy(enemiesController.ActiveEnemies[_currentEnemyIndex]);
    }

    public void FocusPreviousEnemy()
    {
        if (enemiesController.ActiveEnemies.Count == 0) return;

        _currentEnemyIndex--;
        if (_currentEnemyIndex < 0) _currentEnemyIndex = enemiesController.ActiveEnemies.Count - 1;

        FocusEnemy(enemiesController.ActiveEnemies[_currentEnemyIndex]);
    }
}
