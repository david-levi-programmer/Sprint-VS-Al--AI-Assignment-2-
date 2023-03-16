using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class COMRacer : MonoBehaviour
{
    public GameObject[] waypoints;
    int currentWaypoint = 0;
    private float accuracy = 1.0f;
    
    public float speed;
    public float turnSpeed;

    public float stamina;
    private bool running;
    public bool sleep;
    public bool rage;

    public TMP_Text staminaText;
    public TMP_Text moodText;
    
    void Start()
    {
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        sleep = false;
        rage = true;
        moodText.text = "Mood: Normal";
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        Vector3 target = new Vector3(waypoints[currentWaypoint].transform.position.x, this.transform.position.y,
                                    waypoints[currentWaypoint].transform.position.z);
        Vector3 direction = target - this.transform.position;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction),
                                    Time.deltaTime * turnSpeed);
        if (direction.magnitude < accuracy)
        {
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
            {
                //TODO - Code for some sort of 'Player loses' event here
                currentWaypoint = 0;
            }
        }
        this.transform.Translate(0, 0, speed * Time.deltaTime);

        if (running)
        {
            stamina -= Time.deltaTime; //Rival's energy depletes over time...
        }
        if (sleep) //...except he's asleep, which restores his energy
        {
            stamina += Time.deltaTime;
        }

        if (!sleep && rage && stamina <= 25) //Rival's default state
        {
            speed = 17;
            rage = false;
            running = true;
            moodText.text = "Mood: Normal";
        }
        if (stamina < 10) //Rival falls asleep on the spot
        {
            speed = 0;
            moodText.text = "Mood: Zzz...";
            running = false;
            sleep = true;
        }
        if (sleep && stamina > 35) //Rival wakes up in a rage and blasts off
        {
            speed = 26;
            sleep = false;
            rage = true;
            running = true;
            moodText.text = "Mood: #@*!!!!!";
        }

        DisplayStamina();
    }

    void DisplayStamina()
    {
        staminaText.text = "Al's Stamina: " + stamina;
    }
}