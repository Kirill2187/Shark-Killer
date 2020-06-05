using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private HungerBar bar;
    public TextMeshProUGUI bestScoreText;
    public int damagePerBomb = 20;
    public GameObject endGamePanel;

    private int hunger;
    public GameObject hungerBar;

    public AnimationCurve hungerDecreaseTime;
    public int hungerPerBird = 5;

    public int hungerPerFish = 1;

    [HideInInspector] public bool isGameFinished;
    [HideInInspector] public bool isGamePaused;
    private float lastHungerDecreaseTime;
    public int maxHunger = 100;
    private PlayerMovement movementController;

    public GameObject player;

    public int score;
    public ScoreCounter scoreCounter;

    public TextMeshProUGUI scoreText;

    private void Start()
    {
        hunger = maxHunger;
        bar = hungerBar.GetComponent<HungerBar>();
        bar.SetHunger(hunger, maxHunger);
        lastHungerDecreaseTime = Time.time;
        movementController = player.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if (isGamePaused) return;
        if (hunger <= 0 && !isGameFinished)
        {
            isGameFinished = true;
            EndGame();
            return;
        }

        if (Time.time > lastHungerDecreaseTime +
            hungerDecreaseTime.Evaluate(Time.timeSinceLevelLoad) / (movementController.IsBoosted() ? 3f : 1.5f))
        {
            hunger--;
            UpdateHunger();

            lastHungerDecreaseTime = Time.time;
        }
    }

    private void UpdateHunger()
    {
        if (hunger > maxHunger) hunger = maxHunger;
        bar.SetHunger(hunger, maxHunger);
    }

    public void EatFish()
    {
        if (hunger <= 0) return;
        hunger += hungerPerFish;
        AddScores(hungerPerFish);
        UpdateHunger();
    }

    public void EatBird()
    {
        if (hunger <= 0) return;
        hunger += hungerPerBird;
        AddScores(hungerPerBird);
        UpdateHunger();
    }

    public void FillHunger()
    {
        if (hunger <= 0) return;
        hunger = maxHunger;
        UpdateHunger();
    }

    public void BombDamage()
    {
        hunger -= damagePerBomb;
        UpdateHunger();
    }

    private void EndGame()
    {
        AudioManager.instance.Play("GameOver");
        Camera.main.GetComponent<CameraMovement>().speed = 0f;
        Destroy(player.GetComponent<GravityController>());
        var body = player.GetComponent<Rigidbody2D>();
        body.gravityScale = 0.3f;
        body.angularDrag = 0.8f;
        body.drag = 0.8f;

        player.GetComponent<PlayerMovement>().isDead = true;

        Invoke("ShowEndGamePanel", 3f);
    }

    public void AddScores(int add)
    {
        score += add;
        scoreCounter.UpdateScore(score);
    }

    private void ShowEndGamePanel()
    {
        var currentBestScore = PlayerPrefs.GetInt("bestScore", 0);
        if (score > currentBestScore)
        {
            currentBestScore = score;
            PlayerPrefs.SetInt("bestScore", currentBestScore);
            PlayerPrefs.Save();
        }

        scoreText.text = "Your score: " + score;
        bestScoreText.text = "Your best score:\n" + currentBestScore;
        endGamePanel.SetActive(true);
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0;
    }

    public void ContinueGame()
    {
        isGamePaused = false;
        Time.timeScale = 1f;
    }
}