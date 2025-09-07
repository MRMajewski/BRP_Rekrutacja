using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private int currentScore = 0;
    private float textPunchValue = 1.25f;
    private void OnEnable()
    {
        GameEvents.EnemyKilled += OnEnemyKilled;
    }

    private void OnDisable()
    {
        GameEvents.EnemyKilled -= OnEnemyKilled;
    }

    public void InitScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }

    private void OnEnemyKilled(IEnemy enemy)
    {
        int points = enemy.GetBasePoints();
        textPunchValue = 1.25f;

        if (enemy.GetWeakness() == enemy.GetKillingAttackType())
        {
            points = Mathf.RoundToInt(points * 1.5f);
            textPunchValue = 1.5f;
        }

        currentScore += points;
        UpdateScoreUI();

        TweensManager.Instance.PlayPunchEffect(scoreText, textPunchValue);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.SetText($"Score: {currentScore}");
        }
    }

    public void AddPoints(int points)
    {
        currentScore += points;
        UpdateScoreUI();
    }
}
