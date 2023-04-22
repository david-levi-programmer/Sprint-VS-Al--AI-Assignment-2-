using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class COMRacer : MonoBehaviour
{
    public GameObject[] waypoints;
    public NavMeshAgent agent; //the navMesh is really only so the AI is aware of walls
    GameObject carrot; //he will find this when the player drops it
    SphereCollider collision; //how it finds the carrot
    BoxCollider lapDetect; //the collider used for the finish line

    public bool hasDetectedCarrot;
    public float timer; //how long he's been eating the carrot
    public float timeLimit; //how long until he finishes

    public int currentWaypoint = 0;
    public float accuracy; //how close he needs to be to the waypoint to count it and move on
    public int lap;

    public float defaultSpeed; //the speed he runs at when in his normal state
    public float rageSpeed; //the speed he runs at when his angry
    private float detectionArea; //the range at which he can detect the carrot

    public float stamina; //this is always counting down
    //these thresholds are what stamina levels he needs to reach in order to trigger the states
    public float defaultThreshold;
    public float sleepThreshold;
    public float wakeThreshold;

    private bool running; //this is so the script knows when to decrease the AI's stamina
    private bool eating; //this is so it knows when to increase his stamina
    //these booleans are additional conditions for the his states, to ensure he only enters them when he's supposed to
    private bool sleep;
    private bool rage;

    //all the HUD elements
    public TMP_Text staminaText;
    public TMP_Text moodText; //his current state
    public TMP_Text loseText; //when it finishes before the player
    
    void Start()
    {
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint"); //find where the waypoints are
        agent = GetComponent<NavMeshAgent>();
        //agent.SetDestination(waypoints[currentWaypoint].transform.position); //head for the first waypoint
        collision = GetComponent<SphereCollider>(); //this is for finding the carrot dropped by the player
        lapDetect = GetComponent<BoxCollider>(); //this is for detecting when he's crossed the finish line
        detectionArea = collision.radius;
        stamina = defaultThreshold; //the AI starts in its normal state
        hasDetectedCarrot = false; //there won't be a carrot when the race starts, but that could change...
        sleep = false; //wide awake
        //because he starts at the stamina threshold for the default state, he won't be angry once the race starts
        rage = true; //it's just that also one of the conditions for his default state that he's angry
        moodText.text = "Mood: Normal";
        loseText.gameObject.SetActive(false); //the race just started, the player won't lose that fast!
    }

    void Update()
    {
        if (Timer.GetInstance().countdownLoop > 2)
        {
            if (waypoints.Length == 0) return; //if there's no waypoints to follow, the AI can't function

            if (agent.remainingDistance < agent.radius) //when he's reached the waypoint...
            {
                currentWaypoint++; //...move on to the next
                if (currentWaypoint >= waypoints.Length) //when he's crossed all the waypoints...
                {
                    currentWaypoint = 0; //...head back to the first
                }
            }

            LapTrigger(lapDetect); //when the AI's crossed the finish line, increase his lap count

            if (!hasDetectedCarrot) //when the player hasn't dropped a carrot to distract the AI...
            {
                agent.SetDestination(waypoints[currentWaypoint].transform.position); //...follow the waypoints as normal
            }
            else //if there is a carrot and the AI has detected it...
            {
                agent.SetDestination(carrot.transform.position); //...forget the race, he's hungry!
                if (agent.remainingDistance < accuracy && !sleep) //when he's reached it...
                {
                    hasDetectedCarrot = false;
                    eating = true; //...bon appetite! When he's full...
                    agent.SetDestination(waypoints[currentWaypoint].transform.position); //...back to the race!
                }
            }

            if (running)
            {
                stamina -= Time.deltaTime; //when he's running, the Rival's energy depletes over time...
            }
            if (sleep) //...except when he's asleep, which restores his energy
            {
                stamina += Time.deltaTime;
                running = false;
            }
            if (eating) //eating also restores his energy
            {
                stamina += Time.deltaTime;
                timer += Time.deltaTime; //he has a timer for eating the carrot
                running = false;
                if (timer < timeLimit) //he won't be moving until he's finished eating
                {
                    agent.speed = 0;
                    moodText.text = "Mood: Eating";
                }
                if (timer > timeLimit) //once he's full...
                {
                    eating = false;
                    running = true; //...back to the race
                    timer = 0; //reset the timer in case there's another carrot
                    carrot.SetActive(false); //he's finished off the carrot
                    agent.speed = defaultSpeed; //back to his normal state...
                    moodText.text = "Mood: Normal";
                    if (rage) //...unless he was angry before he found the carrot
                    {
                        moodText.text = "Mood: #@*!!!!!"; //in case back to blinding rage
                        agent.speed = rageSpeed;
                    }
                }
            }

            if (!sleep && rage && stamina <= defaultThreshold) //Rival's default state
            {
                agent.speed = defaultSpeed; //this is his usual speed, only a bit faster than the player
                rage = false; //he's calmed down now
                running = true; //when his stamina's low enough, however...
                moodText.text = "Mood: Normal";
            }
            if (stamina < sleepThreshold) //...he falls asleep on the spot once his stamina's low enough
            {
                agent.speed = 0; //He's not a sleepwalker
                moodText.text = "Mood: Zzz...";
                sleep = true; //but his stamina comes back and when it reaches a certain amount...
            }
            if (sleep && stamina > wakeThreshold) //...he wakes up in a rage and blasts off!
            {
                agent.speed = rageSpeed; //his fury makes him even faster!
                sleep = false; //Oh, he is wide awake, now
                rage = true;
                running = true;
                moodText.text = "Mood: #@*!!!!!"; //And he has some choice words for you
            }

            DisplayStamina();
        }
    }

    void DisplayStamina() //so that you can see how Al's doing
    {
        staminaText.text = "Al's Stamina: " + stamina;
    }

    public void GetCarrot(Vector3 position) //called by the player script
    {
        if (Vector3.Distance(position, transform.position) < detectionArea) //When the AI smells it...
        {
            carrot = GameObject.FindGameObjectWithTag("Carrot"); //It finds the object with this tag
            hasDetectedCarrot = true;
        }
    }

    private void LapTrigger(Collider collider) //What happens when it crosses the finish line
    {
        if (collider.gameObject.CompareTag("Finish")) //Onto the next lap
        {
            lap += 1;
            Debug.Log("Al has finished lap 1.");
        }
        
        if (lap > Timer.GetInstance().lapLimit) //When the AI finishes before the player
        {
            //both him and the player get how many laps there are from the timer script
            loseText.gameObject.SetActive(true); //'you lose...' appears on-screen
            Player.GetInstance().paused = true; //when this variable is true, everything stops
            Timer.GetInstance().timerOn = false; //stop the timer
            //there's already a 'resultsMenu' object for the player,
            //so it just makes sense to reuse it instead of copying it for the AI
            Player.GetInstance().resultsMenu.SetActive(true);
        }
    }
}