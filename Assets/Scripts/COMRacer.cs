using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class COMRacer : MonoBehaviour
{
    public GameObject[] waypoints;
    public NavMeshAgent agent;
    GameObject carrot;
    SphereCollider collision;
    BoxCollider lapDetect;

    public bool hasDetectedCarrot;
    public float timer;
    public float timeLimit;

    int currentWaypoint = 0;
    private float accuracy = 1.0f;
    public int lap;

    private float speed;
    public float defaultSpeed;
    public float rageSpeed;
    public float turnSpeed;
    private float detectionArea;

    private float stamina;
    public float defaultThreshold;
    public float sleepThreshold;
    public float wakeThreshold;

    private bool running;
    private bool sleep;
    private bool eating;
    private bool rage;

    public TMP_Text staminaText;
    public TMP_Text moodText;
    public TMP_Text loseText;
    
    void Start()
    {
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        agent = GetComponent<NavMeshAgent>();
        collision = GetComponent<SphereCollider>();
        lapDetect = GetComponent<BoxCollider>();
        detectionArea = collision.radius;
        stamina = defaultThreshold;
        agent.speed = speed;
        hasDetectedCarrot = false;
        sleep = false;
        rage = true;
        moodText.text = "Mood: Normal";
        loseText.gameObject.SetActive(false);
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
                    currentWaypoint = 0;
                }
            }

            LapTrigger(lapDetect);
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
                moodText.text = "Mood: Normal";
                if (rage)
                {
                    moodText.text = "Mood: #@*!!!!!";
                }
            }
        }

        if (!sleep && rage && stamina <= defaultThreshold) //Rival's default state
        {
            speed = defaultSpeed;
            rage = false;
            running = true;
            moodText.text = "Mood: Normal";
        }
        if (stamina < sleepThreshold) //Rival falls asleep on the spot
        {
            speed = 0;
            moodText.text = "Mood: Zzz...";
            sleep = true;
        }
        if (sleep && stamina > wakeThreshold) //Rival wakes up in a rage and blasts off
        {
            speed = rageSpeed;
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

    private void LapTrigger(Collider collider)
    {
        if (collider.gameObject.tag == "Finish")
        {
            lap += 1;
        }
        
        if (lap > Timer.GetInstance().lapLimit)
        {
            loseText.gameObject.SetActive(true);
        }
    }
}