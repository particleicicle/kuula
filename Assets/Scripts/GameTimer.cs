using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Assign in Inspector (optional)
    private float elapsedTime = 0f;
    private bool isRunning = true;

    void Update()
    {
        if (isRunning) {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay() {      
        if(timerText != null) {
            timerText.text = GameManager.Instance.enableTimer ? TimeString : string.Empty;
        }         
    }

    string TimeString
    {
        get 
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // Call this when the game ends
    public void StopTimer()
    {
        isRunning = false;
        Debug.Log("Total Time: " + elapsedTime + " seconds");
        GameManager.Instance.levelCompletionTimes.Add(TimeString);
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
