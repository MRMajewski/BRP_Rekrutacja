using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SoulEnemy : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject InteractionPanelObject;
    [SerializeField] private GameObject ActionsPanelObject;
    [SerializeField] private SpriteRenderer EnemySpriteRenderer;

    private SpawnPoint _enemyPosition;

    [Header("Button References")]
    [SerializeField] private Button combatButton;
    [Header("Button References")]
    [SerializeField] private Button bowButton;
    [Header("Button References")]
    [SerializeField] private Button swordButton;

    public Button GetCombatButton() => combatButton;
    public Button GetBowButton() => bowButton;
    public Button GetSwordButton() => swordButton;

    private bool isActionPanelOpen = false;

    public bool IsActionPanelOpen {  get { return isActionPanelOpen; } }
    public void SetupEnemy(Sprite sprite, SpawnPoint spawnPoint)
    {
        EnemySpriteRenderer.sprite = sprite;
        _enemyPosition = spawnPoint;
        gameObject.SetActive(true);
    }

    public SpawnPoint GetEnemyPosition()
    {
        return _enemyPosition;
    }

    public GameObject GetEnemyObject()
    {
        return this.gameObject;
    }

    private void ActiveCombatWithEnemy()
    {
        ActiveInteractionPanel(false);
        ActiveActionPanel(true);

        EventSystem.current.SetSelectedGameObject(bowButton.gameObject);
    }

    public void CancelCombatWithEnemy()
    {
        ActiveInteractionPanel(true);
        ActiveActionPanel(false);

        EventSystem.current.SetSelectedGameObject(combatButton.gameObject);
    }


    private void ActiveInteractionPanel(bool active)
    {
        InteractionPanelObject.SetActive(active);
    }

    private void ActiveActionPanel(bool active)
    {
        isActionPanelOpen = active;
        ActionsPanelObject.SetActive(active);
    }

    private void UseBow()
    {
        // USE BOW
        GameEvents.EnemyKilled?.Invoke(this);
    }


    private void UseSword()
    {
        GameEvents.EnemyKilled?.Invoke(this);
        // USE SWORD
    }

    #region OnClicks
    [ContextMenu("click combat")]
    public void Combat_OnClick()
    {
        ActiveCombatWithEnemy();
    }
    [ContextMenu("use bow")]
    public void Bow_OnClick()
    {
        UseBow();
    }
    [ContextMenu("use sword")]
    public void Sword_OnClick()
    {
        UseSword();
    }

    #endregion
}


public interface IEnemy
{
    SpawnPoint GetEnemyPosition();
    GameObject GetEnemyObject();
}
