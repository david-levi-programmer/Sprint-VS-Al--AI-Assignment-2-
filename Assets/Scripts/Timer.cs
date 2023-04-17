using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText;
    public TMP_Text resultsText;
    public float currentTime;
    float finalTime;
    public bool timerOn;
    public int lapLimit; //By making this public, I can give different tracks different lap limits

    private static Timer instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("There should be only ONE Time/Lap Manager in a scene.");
        }

        instance = this;
    }

    public static Timer GetInstance()
    {
        return instance;
    }

    void Start()
    {
        timerOn = true;
        resultsText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (timerOn)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            finalTime = currentTime;
            DisplayResults(finalTime);
        }

        DisplayTime(currentTime);
    }

    void DisplayTime(float time)
    {
        time += 1;
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void DisplayResults(float time)
    {
        time += 1;
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        resultsText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        resultsText.gameObject.SetActive(true);
    }
}