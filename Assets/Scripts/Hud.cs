using UnityEngine;
using TMPro;

public class Hud : MonoBehaviour {
    
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI coinText;

    private float elapsedTime;

    private int coins;

    private bool isRunning;

    private void Start() {
        elapsedTime = 0f;
        coins = 0;
        isRunning = true;
    }

    private void Update() {
        if (isRunning) {
            elapsedTime += Time.deltaTime;
            UpdateTimerText();
            UpdateCoins();
        }
    }

    public string UpdateTimerText() {
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100F) % 100F);
        string time = $"Time: {minutes:00}:{seconds:00}:{milliseconds:00}";
        timerText.text = time;
        return time;
    }

    private void UpdateCoins() {
        coins = GameObject.FindWithTag("Player").GetComponent<PlayerController>().GetCoins();
        coinText.text = $"Coins: {coins}";
    }

    public void StartStopwatch() {
        isRunning = true;
    }

    public void StopStopwatch() {
        isRunning = false;
    }

    public void ResetStopwatch() {
        elapsedTime = 0f;
        UpdateTimerText();
    }

    public int GetTime() {
        return (int)(elapsedTime * 1000);
    }
}