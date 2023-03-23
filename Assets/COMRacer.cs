using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class COMRacer : MonoBehaviour
{
    public GameObject[] waypoints;
    GameObject carrot;
    SphereCollider collision;

    public bool hasDetectedCarrot;
    public float timer;
    public float timeLimit;

    int currentWaypoint = 0;
    private float accuracy = 1.0f;
    
    public float speed;
    public float defaultSpeed;
    public float turnSpeed;
    private float detectionArea;

    public float stamina;
    private bool running;
    public bool sleep;
    public bool eating;
    public bool rage;

    public TMP_Text staminaText;
    public TMP_Text moodText;
    
    void Start()
    {
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        collision = GetComponent<SphereCollider>();
        detectionArea = collision.radius;
        hasDetectedCarrot = false;
        sleep = false;
        rage = true;
        moodText.text = "Mood: Normal";
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        Vector3 target;

        if (!hasDetectedCarrot)
        {
            target = new Vector3(waypoints[currentWaypoint].transform.position.x, this.transform.position.y,
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
        }
        else
        {
            target = new Vector3(carrot.transform.position.x, carrot.transform.position.y, carrot.transform.position.z);
            Vector3 direction = target - this.transform.position;
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(direction),
                                        Time.deltaTime * turnSpeed);
            if (direction.magnitude < accuracy && !sleep)
            {
                hasDetectedCarrot = false;
                eating = true;
            }
        }

        this.transform.Translate(0, 0, speed * Time.deltaTime);

        if (running)
        {
            stamina -= Time.deltaTime; //Rival's energy depletes over time...
        }
        if (sleep) //...except when he's asleep, which restores his energy
        {
            stamina += Time.deltaTime;
            running = false;
        }
        if (eating)
        {
            stamina += Time.deltaTime;
            timer += Time.deltaTime;
            running = false;
            if (timer < timeLimit)
            {
                speed = 0;
                moodText.text = "Mood: Eating";
            }
            if (timer > timeLimit)
            {
                eating = false;
                running = true;
                timer = 0;
                speed = defaultSpeed;
                carrot.gameObject.SetActive(false);
            }
        }

        //TODO - Keep tuning how fast Al moves and changes states
        if (!sleep && rage && stamina <= 25) //Rival's default state
        {
            speed = defaultSpeed;
            rage = false;
            running = true;
            moodText.text = "Mood: Normal";
        }
        if (stamina < 10) //Rival falls asleep on the spot
        {
            speed = 0;
            moodText.text = "Mood: Zzz...";
            sleep = true;
        }
        if (sleep && stamina > 33.5) //Rival wakes up in a rage and blasts off
        {
            speed = 45;
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

    public void GetCarrot(Vector3 position)
    {
        if (Vector3.Distance(position, this.transform.position) < detectionArea)
        {
            carrot = GameObject.FindGameObjectWithTag("Carrot");
            hasDetectedCarrot = true;
        }
    }
}