using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText;
    public float currentTime;
    public bool timerOn = false;
    public int lapLimit;
    //TODO - Create instance so that player and rival can get track's lap limit

    void Start()
    {
        timerOn = true;
    }

    void Update()
    {
        if (timerOn)
        {
            currentTime += Time.deltaTime;
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
}