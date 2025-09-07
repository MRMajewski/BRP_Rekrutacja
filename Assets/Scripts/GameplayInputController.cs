using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameplayInputController : MonoBehaviour
{
    [SerializeField] private EnemiesController enemiesController;

    private int currentSlotIndex = 0;

    private PadControls inputActions;
    public PadControls InputActions { get => inputActions; }
    public enum InputMode { Gameplay, UI }

    [SerializeField]
    public InputMode CurrentMode { get; private set; } = InputMode.Gameplay;

    private float navigateCooldown = 0.25f; // czas między zmianami slotów
    private float lastNavigateTime = 0f;



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
        inputActions.Gameplay.Navigate.performed += OnNavigate; // <- jedna akcja Vector2

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
    private void OnConfirm(InputAction.CallbackContext ctx)
    {
        var selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return; 

        if (selected.TryGetComponent(out Button actionButton))
        {
            actionButton.onClick.Invoke();
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

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        Vector2 dir = ctx.ReadValue<Vector2>();

        // Sprawdzamy cooldown
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

        // jeśli obecny enemy nie ma otwartego panelu akcji
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
        //EventSystem.current.SetSelectedGameObject(slot.enemy.GetCombatButton().gameObject);
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

        //var enemies = GetOccupiedEnemies();
        //currentSlotIndex = enemies.IndexOf(enemy);

       // GetAllSlots()[currentSlotIndex].cameraPos;

        EventSystem.current.SetSelectedGameObject(slot.enemy.GetCombatButton().gameObject);
        CameraController.Instance.MoveToPosition(slot.cameraPos);
    }

    public void FocusEnemy(SoulEnemy slot)
    {
        if (slot == null) return;

        var enemies = GetOccupiedEnemies();
       // currentSlotIndex = enemies.IndexOf(enemy);

        // GetAllSlots()[currentSlotIndex].cameraPos;

        EventSystem.current.SetSelectedGameObject(slot.GetCombatButton().gameObject);
      //  CameraController.Instance.MoveToPosition(slot.cameraPos);
    }
}
