using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10.0f; //how fast you can MOVE
    public float turnspeed = 50.0f; //how fast you can TURN
    public int carrotSupply = 3; //There's no getting these back, so use them wisely

    private int lap; //The AI has their own lap variable though theirs is public
    bool messageShown; //whether the final lap message has appeared

    public GameObject carrot; //the carrot the player can drop
    GameObject rival; //So that this script can tell the AI script what's up

    //all the HUD elements
    public TMP_Text lapText;
    public TMP_Text finalLapText;
    public TMP_Text victoryText;
    public TMP_Text carrotText;
    public GameObject resultsMenu;

    //You don't need 'FINAL LAP!!!' flashing in your face all the time
    float messageTimer; //How long stuff like 'FINAL LAP!' is on-screen
    public float timeLimit; //how long until the message disappears

    private static Player instance; //so that other scripts can communicate with this one

    public Camera playerCam;
    public Camera overheadCamera;

    public bool paused = false; //of course it's not paused at the start

    public static Player GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        rival = GameObject.FindGameObjectWithTag("Rival"); //find the AI rival so you can talk to him
        //the race just started, don't show all this yet!
        finalLapText.gameObject.SetActive(false);
        victoryText.gameObject.SetActive(false);
        playerCam.gameObject.SetActive(true);
        overheadCamera.gameObject.SetActive(false);
        resultsMenu.SetActive(false);
        messageShown = false; //you haven't been informed it's the final lap yet
        instance = this;
    }

    void FixedUpdate()
    {
        if (Timer.GetInstance().countdownLoop > 2)
        {
            lapText.gameObject.SetActive(true);
            carrotText.gameObject.SetActive(true);
            rival.GetComponent<COMRacer>().staminaText.gameObject.SetActive(true);
            rival.GetComponent<COMRacer>().moodText.gameObject.SetActive(true);

            float horiz = Input.GetAxis("Horizontal"); // < > arrows or A and D
            float vert = Input.GetAxis("Vertical"); // v ^ arrows or W and S
            float rotation = horiz * turnspeed * Time.deltaTime;
            float translation = vert * speed * Time.deltaTime;
            transform.Rotate(0, rotation, 0); //steering
            transform.Translate(0, 0, translation); //actually moving

            //press Space to drop a carrot, but if you don't have any, you can't do it
            if (Input.GetKeyDown(KeyCode.Space) && carrotSupply > 0) //How hard does it want the player to hit the button???
            {
                Instantiate(carrot, transform.position, carrot.transform.rotation); //spawn the carrot
                rival.GetComponent<COMRacer>().GetCarrot(carrot.transform.position); //give the AI a friendly heads up
                carrotSupply -= 1; //that's one carrot gone from your supply and there's no getting them back
            }

            //what lap you're on and how many you've got left
            lapText.text = "Lap " + lap + "/" + Timer.GetInstance().lapLimit;
            carrotText.text = carrotSupply + " carrots left"; //how many carrots you've got

            if (lap > Timer.GetInstance().lapLimit) //You've crossed the finish line. Huzzah!
            {
                victoryText.gameObject.SetActive(true); //YOU WIN!!!
                paused = true; //Pause the game so nothing WEIRD happens after the race ends
                Timer.GetInstance().timerOn = false; //The race is over, you can stop the timer
                resultsMenu.SetActive(true); //Rematch? Back to menu?
            }
            else if (lap == Timer.GetInstance().lapLimit) //FINAL LAP!!!
            {
                if (!messageShown) //When the message is shown, don't show it again
                {
                    finalLapText.gameObject.SetActive(true);
                    messageTimer += Time.deltaTime; //So that the message doesn't overstay it's welcome
                    if (messageTimer > timeLimit) //Go away when the time limit's up
                    {
                        finalLapText.gameObject.SetActive(false);
                        messageTimer = 0; //in case there's anything else shown
                        messageShown = true;
                    }
                }
            }

            if (Input.GetKey(KeyCode.P) && !paused) //You can't pause what is already paused
            {
                MenuFunctions.GetInstance().PauseGame(); //Gives the menu script it's cue
            }

            if (paused)
            {
                Time.timeScale = 0; //Freeze everything
                                    //Hide the in-game HUD elements while paused
                lapText.gameObject.SetActive(false);
                carrotText.gameObject.SetActive(false);
                rival.GetComponent<COMRacer>().staminaText.gameObject.SetActive(false);
                rival.GetComponent<COMRacer>().moodText.gameObject.SetActive(false);
                Timer.GetInstance().timerText.gameObject.SetActive(false);
            }

            if (Input.GetKey(KeyCode.M) && playerCam.gameObject.activeSelf)
            {
                playerCam.gameObject.SetActive(false);
                overheadCamera.gameObject.SetActive(true);
            }
            else if (Input.GetKey(KeyCode.M) && !playerCam.gameObject.activeSelf)
            {
                playerCam.gameObject.SetActive(true);
                overheadCamera.gameObject.SetActive(false);
            }
        }
    }

    public void UnPause() //Called by MenuFunctions upon clicking 'resume'
    {
        paused = false; //No longer paused, so unfreeze everything
        Time.timeScale = 1;
        //Show the HUD now that it's unpaused and hide the pause menu
        lapText.gameObject.SetActive(true);
        carrotText.gameObject.SetActive(true);
        rival.GetComponent<COMRacer>().staminaText.gameObject.SetActive(true);
        rival.GetComponent<COMRacer>().moodText.gameObject.SetActive(true);
        Timer.GetInstance().timerText.gameObject.SetActive(true);
        MenuFunctions.GetInstance().mainMenu.SetActive(false);
    }

    private void OnTriggerEnter(Collider collider) //when you've crossed the finish line...
    {
        if (collider.gameObject.CompareTag("Finish"))
        {
            lap += 1; //...increase the lap count
        }
    }
}