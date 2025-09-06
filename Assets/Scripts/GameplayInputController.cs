using System.Collections.Generic;
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
    public PadControls InputActions { get => inputActions; }
    public enum InputMode { Gameplay, UI }

    [SerializeField]
    public InputMode CurrentMode { get; private set; } = InputMode.Gameplay;

    private void Awake()
    {
        inputActions = new PadControls();

    }

    private void OnEnable()
    {
        SwitchToGameplayInput();
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
        if (EventSystem.current.currentSelectedGameObject.TryGetComponent(out Button actionButton))
        {
            actionButton.onClick.Invoke();
            return;
        }
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

        var enemies = GetOccupiedEnemies();
        if (enemies.Count == 0) return;

        SoulEnemy currentEnemy = enemies[_currentEnemyIndex];
        if (currentEnemy.IsActionPanelOpen)
        {
            currentEnemy.CancelCombatWithEnemy();
        }
        //Debug.Log("Cancel pressed");
        //// TODO: logika cofania / zamykania paneli

        //if (enemiesController.ActiveEnemies[0].IsActionPanelOpen)
        //{
        //    enemiesController.ActiveEnemies[0].CancelCombatWithEnemy();
        //}
    }
    private List<SoulEnemy> GetOccupiedEnemies()
    {
        return enemiesController.GetActiveEnemies();
    }
    public void SwitchToUIInput()
    {
        inputActions.Gameplay.Disable();
        inputActions.UI.Enable();
        CurrentMode = InputMode.UI;
        Debug.Log("Przełączono na UI");
    }

    public void SwitchToGameplayInput()
    {
        inputActions.UI.Disable();
        inputActions.Gameplay.Enable();
        CurrentMode = InputMode.Gameplay;
        Debug.Log("Przełączono na Gameplay");
    }

    public void ReturnFromUI()
    {
        SwitchToGameplayInput();

        var enemies = GetOccupiedEnemies();
        if (enemies.Count == 0) return;

        SoulEnemy currentEnemy = enemies[_currentEnemyIndex];
        EventSystem.current.SetSelectedGameObject(
            currentEnemy.IsActionPanelOpen ?
            currentEnemy.GetBowButton().gameObject :
            currentEnemy.GetCombatButton().gameObject
        );

        //if (enemiesController.ActiveEnemies[0].IsActionPanelOpen)
        //{
        //    EventSystem.current.SetSelectedGameObject(enemiesController.ActiveEnemies[0].GetBowButton().gameObject);
        //}
        //else
        //    EventSystem.current.SetSelectedGameObject(enemiesController.ActiveEnemies[0].GetCombatButton().gameObject);



    }


    public void FocusOnFirstEnemy()
    {
        var enemies = GetOccupiedEnemies();
        if (enemies.Count == 0) return;

        _currentEnemyIndex = 0;
        FocusEnemy(enemies[_currentEnemyIndex]);
      //  FocusEnemy(enemiesController.ActiveEnemies[0]);
    }
    private void OnEnemySpawned(SoulEnemy enemy)
    {
        if (enemy == null) return;

        var enemies = GetOccupiedEnemies();
        _currentEnemyIndex = enemies.IndexOf(enemy);
        FocusEnemy(enemy);
    }
    public void FocusEnemy(SoulEnemy enemy)
    {
        if (enemy == null) return;

        var enemies = GetOccupiedEnemies();
        _currentEnemyIndex = enemies.IndexOf(enemy);

        EventSystem.current.SetSelectedGameObject(enemy.GetCombatButton().gameObject);
        // FocusEnemy(enemy);
        //   if (enemy == null) return;
        //  EventSystem.current.SetSelectedGameObject(enemy.GetCombatButton().gameObject);
    }
    public void FocusNextEnemy()
    {
        var enemies = GetOccupiedEnemies();
        if (enemies.Count == 0) return;

        _currentEnemyIndex = (_currentEnemyIndex + 1) % enemies.Count;
        FocusEnemy(enemies[_currentEnemyIndex]);
    }

    public void FocusPreviousEnemy()
    {
        var enemies = GetOccupiedEnemies();
        if (enemies.Count == 0) return;

        _currentEnemyIndex--;
        if (_currentEnemyIndex < 0) _currentEnemyIndex = enemies.Count - 1;

        FocusEnemy(enemies[_currentEnemyIndex]);
    }
   
}
