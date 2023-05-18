using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    //all the HUD elements
    public TMP_Text timerText;
    public TMP_Text resultsText;
    public TMP_Text countdownText;
    public float currentTime; //this is the timer that will be counting up over time
    float finalTime; //this is what time it is when the race is over
    //the only situation in which the timer is off is when the race is over
    public bool timerOn; //therefore, this variable can be used by other scripts to indicate that the race has ended
    public int lapLimit; //By making this public, I can give different tracks different lap limits
    public int countdownLoop; //How many times it's counted down until GO! time

    private static Timer instance; //so that other scripts can communicate with this one

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
        timerOn = true; //the race has begun, start the timer
        resultsText.gameObject.SetActive(false); //the race just started, there shouldn't be results yet
        timerText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (countdownLoop != 3)
        {
            Countdown();
        }
        else if (currentTime > 1 && countdownLoop == 3)
        {
            countdownText.gameObject.SetActive(false);
        }
        else if (countdownLoop == 3)
        {
            timerText.gameObject.SetActive(true);
            countdownText.text = "GO!";
        }

        if (timerOn) //when the timer is on, count it up
        {
            currentTime += Time.deltaTime;
        }
        else //when it is off, the race has ended...
        {
            finalTime = currentTime; //...and we have our final time
            DisplayResults(finalTime); //display the results
        }

        DisplayTime(currentTime);
    }

    void DisplayTime(float time)
    {
        time += 1; //by default, Unity doesn't tell time in seconds and minutes
        //so we have to convert a float variable (a decimal number) into the proper format...
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        //...then tell Unity how to display it
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    
    private void DisplayResults(float time)
    {
        //this is only separate because reusing the above function would put it in the top corner of the screen
        //instead of in the middle of it
        time += 1;
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);
        resultsText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        resultsText.gameObject.SetActive(true);
    }

    private void Countdown()
    {
        if (currentTime > 1)
        {
            countdownLoop += 1;
            currentTime = 0;
        }
        else if (countdownLoop == 0)
        {
            countdownText.text = "3";
        }
        else if (countdownLoop == 1)
        {
            countdownText.text = "2";
        }
        else if (countdownLoop == 2)
        {
            countdownText.text = "1";
        }
        else if (countdownLoop > 2)
        {
            currentTime = 0;
        }
    }
}