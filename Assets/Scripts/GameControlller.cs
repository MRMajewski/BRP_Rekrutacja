using UnityEngine;

public enum GameLocalization
{
    SWAMPS,
    DUNGEON,
    CASTLE,
    CITY,
    TOWER
}

public class GameControlller : MonoBehaviour
{

    #region Singleton

    private static GameControlller _instance;

    public static GameControlller Instance
    {
        get
        {
            if (_instance == null) _instance = FindFirstObjectByType<GameControlller>();
            return _instance;
        }
        set => _instance = value;
    }

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    [SerializeField] private GameLocalization currentGameLocalization;

    [SerializeField] private EnemiesController enemiesController;

    [SerializeField] private UIPanelController uiPanelController;

    [SerializeField] private GameplayInputController gameplayInput;
    [SerializeField] private ScoreManager scoreManager;
 
    public GameLocalization CurrentGameLocalization
    {
        get => currentGameLocalization;

        set => currentGameLocalization = value;
    }

   public EnemiesController EnemiesController { get => enemiesController; }
   public UIPanelController UIPanelController { get => uiPanelController; }
   public GameplayInputController GameplayInput { get => gameplayInput; }
    public ScoreManager ScoreManager => scoreManager;
    private bool _isPaused;

    public bool IsPaused
    {

        get => _isPaused;
        set
        {
            _isPaused = value;
            Time.timeScale = _isPaused ? 0f : 1f;
        }
    }

    public bool IsCurrentLocalization(GameLocalization localization)
    {
        return CurrentGameLocalization == localization;
    }


    public void Start()
    {
        InitGame();
    }

    public void InitGame()
    {
        IsPaused = false;
        enemiesController.InitializeEnemies();
        gameplayInput.FocusOnFirstEnemy();
        scoreManager.InitScore();
     
    }
}