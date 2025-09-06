using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    private int currentScore = 0;

    private void OnEnable()
    {
        GameEvents.EnemyKilled += OnEnemyKilled;
    }

    private void OnDisable()
    {
        GameEvents.EnemyKilled -= OnEnemyKilled;
    }

    private void OnEnemyKilled(IEnemy enemy)
    {
        int points = enemy.GetBasePoints();

        // bonus 50%, jeœli trafiony s³aboœci¹
        if (enemy.GetWeakness() == enemy.GetKillingAttackType())
        {
            points = Mathf.RoundToInt(points * 1.5f);
        }

        currentScore += points;
        UpdateScoreUI();
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
