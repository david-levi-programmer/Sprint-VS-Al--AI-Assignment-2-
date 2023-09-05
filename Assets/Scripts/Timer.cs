using UnityEngine;
using TMPro; //This enables the text elements to be used by the code

public class Timer : MonoBehaviour //This clock does more than just keep track of time
{
    //v all the HUD elements
    public TMP_Text timerText;
    public TMP_Text resultsText;
    public TMP_Text countdownText;
    public float currentTime; //this is the timer that will be counting up over time
    float finalTime; //this is what time it is when the race is over
    //the only situation in which the timer is off is when the race is over
    public bool timerOn; //therefore, this variable can be used by other scripts to indicate that the race has ended
    public int lapLimit; //By making this public, I can give different tracks different lap limits
    public int countdownLoop; //How many times it's counted down until the race begins

    private static Timer instance; //so that other scripts can communicate with this one

    private void Awake() //Do this when loading into the scene
    {
        if (instance != null) //check if there's more than one Timer in the scene...
        {
            Debug.LogWarning("There should be only ONE Time/Lap Manager in a scene.");
        }

        instance = this;
    }

    public static Timer GetInstance() //called by the other scripts to communicate with each other
    {
        return instance;
    }

    void Start() //Do this on the first frame of gameplay
    {
        timerOn = true; //the race has begun, start the timer
        resultsText.gameObject.SetActive(false); //the race just started, there shouldn't be results yet
        timerText.gameObject.SetActive(false); //and thus, no final time
    }

    void Update() //what happens every frame
    {
        if (countdownLoop != 3) //until the starting countdown is over
        {
            Countdown(); //don't do anything BUT the starting countdown
        }
        else if (currentTime > 1 && countdownLoop == 3) //when the race timer starts
        {
            countdownText.gameObject.SetActive(false); //don't show the countdown
        }
        else if (countdownLoop == 3) //when the countdown is over
        {
            timerText.gameObject.SetActive(true); //bring the clock on screen
            countdownText.text = "GO!"; //and yell 'GOOOOOO!'
        }

        if (timerOn) //when the timer is on, count it up
        {
            currentTime += Time.deltaTime;
        }
        else //when the timer is turned off, the race has ended...
        {
            finalTime = currentTime; //...and we have our final time
            DisplayResults(finalTime); //display the results
        }

        DisplayTime(currentTime); //when none of the above is happening, show the current time
    }

    void DisplayTime(float time) //showing the player's current time isn't as simple as just putting it on-screen
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
        resultsText.gameObject.SetActive(true); //show the results menu while you're at it
    }

    private void Countdown() //the starting countdown
    {
        if (currentTime > 1) //this is so the countdown changes with every second
        {
            countdownLoop += 1; //start the countdown!
            currentTime = 0; //this is so it doesn't affect the player's time when the race starts
        }
        //v doing all this for every second was easier than trying to figure out how to get the timer
        //to actually go down instead of up
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
            currentTime = 0; //reset the timer when the countdown's done to properly keep track of the player's time
        }
    }
}