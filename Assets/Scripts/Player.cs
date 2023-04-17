using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10.0f;
    public float turnspeed = 50.0f;
    private int carrotSupply = 3;

    public int lap;
    bool messageShown;

    public GameObject carrot;
    GameObject rival;

    public TMP_Text lapText;
    public TMP_Text finalLapText;
    public TMP_Text victoryText;
    public TMP_Text carrotText;

    float messageTimer;
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

        if (lap > Timer.GetInstance().lapLimit)
        {
            victoryText.gameObject.SetActive(true);
        }
        else if (lap == Timer.GetInstance().lapLimit)
        {
            if (!messageShown)
            {
                finalLapText.gameObject.SetActive(true);
                messageTimer += Time.deltaTime;
                if (messageTimer > timeLimit)
                {
                    finalLapText.gameObject.SetActive(false);
                    messageTimer = 0;
                    messageShown = true;
                }
            }
        }

        if (Input.GetKey(KeyCode.P) && !paused)
        {
            MenuFunctions.GetInstance().PauseGame();
        }

        if (paused)
        {
            Time.timeScale = 0;
            lapText.gameObject.SetActive(false);
            carrotText.gameObject.SetActive(false);
            rival.GetComponent<COMRacer>().staminaText.gameObject.SetActive(false);
            rival.GetComponent<COMRacer>().moodText.gameObject.SetActive(false);
            Timer.GetInstance().timerText.gameObject.SetActive(false);
        }
    }

    public void UnPause()
    {
        paused = false;
        Time.timeScale = 1;
        lapText.gameObject.SetActive(true);
        carrotText.gameObject.SetActive(true);
        rival.GetComponent<COMRacer>().staminaText.gameObject.SetActive(true);
        rival.GetComponent<COMRacer>().moodText.gameObject.SetActive(true);
        Timer.GetInstance().timerText.gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Finish"))
        {
            lap += 1;
        }
    }
}