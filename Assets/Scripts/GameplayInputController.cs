using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameplayInputController : MonoBehaviour
{
    #region Variables
    [SerializeField] private EnemiesController enemiesController;
   
    private PadControls inputActions;
    public PadControls InputActions { get => inputActions; }
    public enum InputMode { Gameplay, UI }

    [SerializeField]
    public InputMode CurrentMode { get; private set; } = InputMode.Gameplay;

    private float navigateCooldown = 0.25f; 
    private float lastNavigateTime = 0f;
    private int currentSlotIndex = 0;

    #endregion
    #region Initialization
    private void Awake()
    {
        inputActions = new PadControls();
    }

    private void OnEnable()
    {
        SwitchToGameplayInput();

        inputActions.Gameplay.Options.performed += OnPause;
        inputActions.Gameplay.Inventory.performed += OnInventory;
        inputActions.Gameplay.Cancel.performed += OnCancel;
        inputActions.Gameplay.Confirm.performed += OnConfirm;
        inputActions.Gameplay.Navigate.performed += OnNavigate;

        inputActions.Gameplay.Enable();
    }

    private void OnDisable()
    {
        inputActions.Gameplay.Options.performed -= OnPause;
        inputActions.Gameplay.Inventory.performed -= OnInventory;
        inputActions.Gameplay.Cancel.performed -= OnCancel;
        inputActions.Gameplay.Confirm.performed -= OnConfirm;
        inputActions.Gameplay.Navigate.performed -= OnNavigate;

        inputActions.Gameplay.Disable();
    }

    #endregion
    #region Input Handlers
    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return; 

        if (selected.TryGetComponent(out Button actionButton))
        {
            actionButton.onClick.Invoke();
        }
    }
    private void OnCancel(InputAction.CallbackContext ctx)
    {
        Debug.Log("Cancel pressed");

        var enemies = GetOccupiedEnemies();
        if (enemies.Count == 0) return;

        SoulEnemy currentEnemy = enemies[currentSlotIndex];
        if (currentEnemy.IsActionPanelOpen)
        {
            currentEnemy.CancelCombatWithEnemy();
        }

    }
    private List<SoulEnemy> GetOccupiedEnemies()
    {
        return enemiesController.GetActiveEnemies();
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

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        Vector2 dir = ctx.ReadValue<Vector2>();

        if (Time.time - lastNavigateTime < navigateCooldown) return;

        if (dir.x > 0.5f)
        {
            FocusNextSlot();
            lastNavigateTime = Time.time;
        }
        else if (dir.x < -0.5f)
        {
            FocusPreviousSlot();
            lastNavigateTime = Time.time;
        }
    }

    #endregion

    #region Enemy Focus & Navigation
    private List<SoulEnemySlot> GetAllSlots()
    {
        return enemiesController.GetAllSoulEnemySlots();
    }

    private SoulEnemySlot GetCurrentSlot()
    {
        var slots = GetAllSlots();
        if (slots.Count == 0) return null;
        return slots[currentSlotIndex];
    }
    public void FocusNextSlot()
    {
        var slots = GetAllSlots();
        if (slots.Count == 0) return;

        if (GetCurrentSlot().enemy != null && !GetCurrentSlot().enemy.IsActionPanelOpen)
        {
            currentSlotIndex = (currentSlotIndex + 1) % slots.Count;
            FocusSlot(slots[currentSlotIndex]);
        }
    }
    public void FocusPreviousSlot()
    {
        var slots = GetAllSlots();
        if (slots.Count == 0) return;

        if (GetCurrentSlot().enemy != null && !GetCurrentSlot().enemy.IsActionPanelOpen)
        {
            currentSlotIndex--;
            if (currentSlotIndex < 0) currentSlotIndex = slots.Count - 1;
            FocusSlot(slots[currentSlotIndex]);
        }
    }

    private void FocusSlot(SoulEnemySlot slot)
    {
        if (slot == null || slot.enemy == null) return;

        FocusEnemy(slot);
    }

    public void FocusOnFirstEnemy()
    {
        var enemies = GetOccupiedEnemies();
        if (enemies.Count == 0) return;

        currentSlotIndex = 0;
        FocusEnemy(GetAllSlots()[currentSlotIndex]);
    }

    public void FocusEnemy(SoulEnemySlot slot)
    {
        if (slot == null) return;

        EventSystem.current.SetSelectedGameObject(slot.enemy.GetCombatButton().gameObject);
        CameraController.Instance.MoveToPosition(slot.cameraPos);
    }

    public void FocusEnemy(SoulEnemy slot)
    {
        if (slot == null) return;

        var enemies = GetOccupiedEnemies();

        EventSystem.current.SetSelectedGameObject(slot.GetCombatButton().gameObject);
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

        SoulEnemy currentEnemy = enemies[currentSlotIndex];
        EventSystem.current.SetSelectedGameObject(
            currentEnemy.IsActionPanelOpen ?
            currentEnemy.GetBowButton().gameObject :
            currentEnemy.GetCombatButton().gameObject
        );

    }
    #endregion
}
