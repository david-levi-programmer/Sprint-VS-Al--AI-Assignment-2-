using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10.0f;
    public float turnspeed = 50.0f;
    private int carrotSupply = 3; //There's no getting these back, so use them wisely

    private int lap; //The AI has their own lap variable though theirs is public
    bool messageShown; //You don't need 'FINAL LAP!!!' flashing in your face all the time

    public GameObject carrot;
    GameObject rival; //So that this script can tell the AI script what's up

    public TMP_Text lapText;
    public TMP_Text finalLapText;
    public TMP_Text victoryText;
    public TMP_Text carrotText;
    public GameObject resultsMenu;

    float messageTimer; //How long stuff like 'FINAL LAP!' is on-screen
    public float timeLimit;

    private static Player instance;

    public bool paused = false;

    public static Player GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        rival = GameObject.FindGameObjectWithTag("Rival");
        finalLapText.gameObject.SetActive(false);
        victoryText.gameObject.SetActive(false);
        resultsMenu.SetActive(false);
        messageShown = false;
        instance = this;
    }

    void FixedUpdate()
    {
        float horiz = Input.GetAxis("Horizontal"); // < > arrows or A and D
        float vert = Input.GetAxis("Vertical"); // v ^ arrows or W and S
        float rotation = horiz * turnspeed * Time.deltaTime;
        float translation = vert * speed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
        transform.Translate(0, 0, translation);

        if (Input.GetKeyDown(KeyCode.Space)) //How hard does it want the player to hit the button???
        {
            //when the carrot spawns, it will tell the ai's script to be on the lookout for it
            Instantiate(carrot, transform.position, carrot.transform.rotation);
            rival.GetComponent<COMRacer>().GetCarrot(carrot.transform.position);
            carrotSupply -= 1;
        }

        lapText.text = "Lap " + lap + "/" + Timer.GetInstance().lapLimit;
        carrotText.text = carrotSupply + " carrots left";

        if (lap > Timer.GetInstance().lapLimit) //You've crossed the finish line. Huzzah!
        {
            victoryText.gameObject.SetActive(true);
            paused = true; //Pause the game so nothing WEIRD happens after the race ends
            Timer.GetInstance().timerOn = false; //The race is over, you can stop the timer
            resultsMenu.SetActive(true); //Rematch? Back to menu?
        }
        else if (lap == Timer.GetInstance().lapLimit) //FINAL LAP!!!
        {
            if (!messageShown) //When the message is shown, don't show again
            {
                finalLapText.gameObject.SetActive(true);
                messageTimer += Time.deltaTime; //So that the message doesn't overstay it's welcome
                if (messageTimer > timeLimit) //Go away when the time limit's up
                {
                    finalLapText.gameObject.SetActive(false);
                    messageTimer = 0;
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

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Finish"))
        {
            lap += 1;
        }
    }
}