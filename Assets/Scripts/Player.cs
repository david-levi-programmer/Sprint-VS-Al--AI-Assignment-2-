using TMPro; //This enables the text elements to be used by the code
using UnityEngine;

public class Player : MonoBehaviour
{
    //'public' means the variable can be changed in the unity editor
    //the values set in this script are the variable's default values
    public float speed = 10.0f; //how fast you can MOVE
    public float turnspeed = 50.0f; //how fast you can TURN
    public int carrotSupply = 3; //There's no getting these back, so use them wisely

    private int lap; //The AI has their own lap variable though theirs is public
    bool messageShown; //whether the final lap message has appeared

    public GameObject carrot; //the carrot the player can drop
    GameObject rival; //So that this script can tell the AI script what's up
    Transform nose; //So that you can see which direction you're facing in top-down view

    //all the HUD elements
    public TMP_Text lapText;
    public TMP_Text finalLapText;
    public TMP_Text victoryText;
    public TMP_Text carrotText;
    public TMP_Text AILapText;
    public GameObject resultsMenu;
    public GameObject deathMenu;

    //You don't need 'FINAL LAP!!!' flashing in your face all the time
    float messageTimer; //How long messages like 'FINAL LAP!' is on-screen
    public float messageTimeLimit; //how long until the message disappears

    private static Player instance; //so that other scripts can communicate with this one

    public Camera playerCam; //the default behind-the-back camera
    public Camera overheadCamera; //the top-down view that can be switched to

    public bool paused = false; //of course it's not paused at the start

    public static Player GetInstance() //called by the other scripts to communicate with each other
    {
        return instance;
    }

    private void Start() //Do this on the first frame of gameplay
    {
        rival = GameObject.FindGameObjectWithTag("Rival"); //find the AI rival so that this script can talk to his
        nose = this.gameObject.transform.GetChild(0); //get the child of the player object, in this case the nose
        nose.gameObject.SetActive(false); //this should only be visible in top-down view
        //the race just started, don't show all v this yet!
        finalLapText.gameObject.SetActive(false);
        victoryText.gameObject.SetActive(false);
        AILapText.gameObject.SetActive(false);
        playerCam.enabled = true;
        overheadCamera.enabled = false;
        resultsMenu.SetActive(false);
        messageShown = false; //you haven't been informed it's the final lap yet
        instance = this; //the instance is this script so other scripts can communicate with it
    }

    void FixedUpdate() //what happens every frame
    {
        if (Timer.GetInstance().countdownLoop > 2) //when the start countdown is over
        {
            //v bring the HUD on-screen and update it
            lapText.gameObject.SetActive(true);
            carrotText.gameObject.SetActive(true);
            rival.GetComponent<COMRacer>().staminaText.gameObject.SetActive(true);
            rival.GetComponent<COMRacer>().moodText.gameObject.SetActive(true);

            //v Player movement
            float horiz = Input.GetAxis("Horizontal"); // < > arrows or A and D
            float vert = Input.GetAxis("Vertical"); // v ^ arrows or W and S
            float rotation = horiz * turnspeed * Time.deltaTime; //when the 'horiz' input is given
            float translation = vert * speed * Time.deltaTime; //when the 'vert' input is given
            transform.Rotate(0, rotation, 0); //steering
            transform.Translate(0, 0, translation); //actually moving

            //press Space to drop a carrot, but if you don't have any, you can't do it
            if (Input.GetKeyDown(KeyCode.Space) && carrotSupply > 0) //GetKeyDown seems to work best with VSync on
            {
                Instantiate(carrot, transform.position, carrot.transform.rotation); //spawn the carrot
                rival.GetComponent<COMRacer>().GetCarrot(carrot.transform.position); //give the AI a friendly heads up
                carrotSupply -= 1; //that's one carrot gone from your supply and there's no getting them back
            }

            //v what lap you and the AI are on and how many you've got left
            lapText.text = "Lap " + lap + "/" + Timer.GetInstance().lapLimit;
            AILapText.text = "Al Lap " + rival.GetComponent<COMRacer>().lap + "/" + Timer.GetInstance().lapLimit;
            carrotText.text = carrotSupply + " carrots left"; //how many carrots you have left

            if (lap > Timer.GetInstance().lapLimit) //FINISH
            {
                victoryText.gameObject.SetActive(true); //YOU WIN
                paused = true; //Pause the game so nothing that can happen after the race ends
                Timer.GetInstance().timerOn = false; //The race is over, you can stop the timer
                resultsMenu.SetActive(true); //Rematch? Back to menu?
            }
            else if (lap == Timer.GetInstance().lapLimit) //Yeah! The final lap!
            {
                if (!messageShown) //When the message is shown, don't show it again
                {
                    finalLapText.gameObject.SetActive(true); //Show the message
                    messageTimer += Time.deltaTime; //Start the timer
                    if (messageTimer > messageTimeLimit) //Go away when the time limit's up
                    {
                        finalLapText.gameObject.SetActive(false); //Hide the message
                        messageTimer = 0; //in case there's anything else shown
                        messageShown = true; //so that the script knows the message has been shown
                    }
                }
            }

            if (Input.GetKey(KeyCode.P) && !paused) //You can't pause what is already paused
            {
                //v Gives the menu script it's cue to pause and bring up the menu
                MenuFunctions.GetInstance().PauseGame();
            }

            if (paused)
            {
                Time.timeScale = 0; //Freeze everything that's not menu-related
                //Hide the in-game HUD elements while paused
                lapText.gameObject.SetActive(false);
                AILapText.gameObject.SetActive(false);
                carrotText.gameObject.SetActive(false);
                rival.GetComponent<COMRacer>().staminaText.gameObject.SetActive(false);
                rival.GetComponent<COMRacer>().moodText.gameObject.SetActive(false);
                Timer.GetInstance().timerText.gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.M) && playerCam.enabled) //Press P in normal view
            {
                playerCam.enabled = false; //Switch off behind-the-back cam...
                overheadCamera.enabled = true; //...and turn on overhead camera
                nose.gameObject.SetActive(true); //Show which direction you're facing
                AILapText.gameObject.SetActive(true); //Show what lap the AI is on, so as to show everything
            }
            else if (Input.GetKeyDown(KeyCode.M) && !playerCam.enabled) //Press P again...
            {
                playerCam.enabled = true; //...and go back to over-the-shoulder
                overheadCamera.enabled = false; //Switch off the overhead camera
                nose.gameObject.SetActive(false); //You won't be needing this anymore
                AILapText.gameObject.SetActive(false); //Hide what lap the AI is on
            }
        }
    }

    public void UnPause() //Called by MenuFunctions upon clicking 'resume'
    {
        paused = false; //No longer paused...
        Time.timeScale = 1; //...so unfreeze everything
        //v Show the HUD again now that it's unpaused and hide the pause menu
        lapText.gameObject.SetActive(true);
        carrotText.gameObject.SetActive(true);
        rival.GetComponent<COMRacer>().staminaText.gameObject.SetActive(true);
        rival.GetComponent<COMRacer>().moodText.gameObject.SetActive(true);
        Timer.GetInstance().timerText.gameObject.SetActive(true);
        MenuFunctions.GetInstance().mainMenu.SetActive(false);

        if (!playerCam.enabled)
        {
            AILapText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider collider) //when you've crossed the finish line...
    {
        if (collider.gameObject.CompareTag("Finish")) //...check the tag to confirm it's the finish line...
        {
            lap += 1; //...and increase the lap count
        }

        if (collider.gameObject.CompareTag("Fall Out"))
        {
            Debug.Log("FALL OUT");
            deathMenu.SetActive(true);
            Time.timeScale = 0;
        }
    }
}