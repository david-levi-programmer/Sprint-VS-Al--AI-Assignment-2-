using TMPro; //This enables the text elements to be used by the code
using UnityEngine;
using UnityEngine.AI; //Enables the use of Unity's AI features

public class COMRacer : MonoBehaviour
{
    public GameObject[] waypoints; //the waypoints the AI follows to navigate the track
    public NavMeshAgent agent; //the navMesh that allows him to stay on track and avoid walls
    GameObject carrot; //he will find this when the player drops it
    SphereCollider collision; //a second collider that keeps an eye out for the carrot

    public bool hasDetectedCarrot; //whether he's found the carrot
    public float timer; //how long he's been eating the carrot
    public float timeLimit; //how long until he finishes eating

    public int currentWaypoint = 0; //the waypoint count starts at 0, so 0 is the first waypoint
    public float accuracy; //how close he needs to the waypoint he needs to be to count it and move on
    public int lap; //what lap he's on

    public float defaultSpeed; //the speed he runs at when in his normal state
    public float rageSpeed; //the speed he runs at when he's angry
    private float detectionArea; //the range at which he can detect the carrot

    public float stamina; //this is always counting down, determing what mood he's in
    //v the stamina levels he needs to reach in order to trigger the states
    public float defaultThreshold; //when he returns to normal
    public float sleepThreshold; //when he falls asleep
    public float wakeThreshold; //when he wakes up

    private bool running; //this is so the script knows when to decrease the AI's stamina
    private bool eating; //this is so it knows when to increase his stamina
    //these booleans are additional conditions for his states, to ensure he only enters them when he's supposed to
    private bool sleep; //< has the same function as 'eating' but...
    //...for the purposes of the AI's mood system, it's best to make this a seperate variable
    private bool rage; //so that the script knows when to speed up the AI

    //v all the HUD elements
    public TMP_Text staminaText; //how much stamina he has
    public TMP_Text moodText; //his current state
    public TMP_Text loseText; //when it finishes before the player
    
    void Start() //Do this on the first frame of gameplay
    {
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint"); //find where the waypoints are
        agent = GetComponent<NavMeshAgent>(); //make use of the nav mesh
        collision = GetComponent<SphereCollider>(); //this is for finding the carrot dropped by the player
        detectionArea = collision.radius;
        stamina = defaultThreshold; //the AI starts in its normal state
        hasDetectedCarrot = false; //there won't be a carrot when the race starts
        sleep = false; //wide awake at the start of the race
        //v This does not mean he is angry at the start of the race.
        rage = true; //This will shut off once the race begins and he enters his normal state
        moodText.text = "Mood: Normal"; //< Indicate that he is in his normal state
        loseText.gameObject.SetActive(false); //don't show the lose text, the race just started!
    }

    void Update() //what happens every frame
    {
        if (Timer.GetInstance().countdownLoop > 2) //once the start countdown is over
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

            if (!hasDetectedCarrot) //when the player hasn't dropped a carrot to distract the AI...
            {
                agent.SetDestination(waypoints[currentWaypoint].transform.position); //...follow the waypoints as normal
            }
            else //if there is a carrot and the AI has detected it...
            {
                agent.SetDestination(carrot.transform.position); //...forget the race, he's hungry!
                if (agent.remainingDistance < accuracy && !sleep) //when he's reached it...
                {
                    hasDetectedCarrot = false; //...switch off food-seeking and...
                    eating = true; //...bon appetite! When he's full...
                    agent.SetDestination(waypoints[currentWaypoint].transform.position); //...back to the race!
                }
            }

            if (running)
            {
                stamina -= Time.deltaTime; //when he's running, the Rival's energy depletes over time...
            }
            if (sleep) //...except when he's asleep...
            {
                stamina += Time.deltaTime; //...which restores his energy
                running = false; //he's not running so it can't counteract the stamina increase
            }
            if (eating) //eating also restores his energy
            {
                stamina += Time.deltaTime;
                timer += Time.deltaTime; //he has a timer for eating the carrot
                running = false; //he's not running so it can't counteract the stamina increase
                if (timer < timeLimit) //until he's finished eating...
                {
                    agent.speed = 0; //...he won't be moving
                    moodText.text = "Mood: Eating";
                }
                if (timer > timeLimit) //once he's done eating...
                {
                    eating = false; //...his stamina is no longer increasing
                    running = true; //...back to the race, his stamina decreases again
                    timer = 0; //reset the timer in case there's another carrot
                    carrot.SetActive(false); //the carrot vanishes
                    agent.speed = defaultSpeed; //back to his normal state...
                    moodText.text = "Mood: Normal";
                    if (rage) //...unless he was angry before he found the carrot
                    {
                        moodText.text = "Mood: #@*!!!!!"; //in which case back to BLINDING RAGE
                        agent.speed = rageSpeed;
                    }
                }
            }

            if (!sleep && rage && stamina <= defaultThreshold) //if he was angry before...
            {
                rage = false; //...he's calmed down now that he's in his normal state
                agent.speed = defaultSpeed; //this is his usual speed, only a bit faster than the player
                running = true; //his stamina is decreasing when he's running
                moodText.text = "Mood: Normal";
            }
            if (stamina < sleepThreshold) //he falls asleep on the spot once his stamina's low enough
            {
                agent.speed = 0; //He stops in his tracks
                moodText.text = "Mood: Zzz..."; //and sleeps
                sleep = true; //but his stamina comes back as he does so and when it reaches a certain amount...
            }
            if (sleep && stamina > wakeThreshold) //...he wakes up in a rage and blasts off!
            {
                agent.speed = rageSpeed; //his fury makes him even faster!
                sleep = false; //Oh, he is wide awake now so his stamina's not increasing
                running = true; //In fact, it's decreasing again
                rage = true;
                moodText.text = "Mood: #@*!!!!!"; //And he has some choice words for you
            }

            DisplayStamina(); //show his stamina on-screen
        }
    }

    void DisplayStamina() //so that you can see how Al's doing
    {
        staminaText.text = "Al's Stamina: " + stamina;
    }

    public void GetCarrot(Vector3 position) //called by the player script, telling the AI to start looking
    {
        if (Vector3.Distance(position, transform.position) < detectionArea) //When the AI nears the carrot...
        {
            carrot = GameObject.FindGameObjectWithTag("Carrot"); //...he checks it for this tag
            hasDetectedCarrot = true; //he's found it and heads straight for it
        }
    }

    private void OnTriggerEnter(Collider collider) //What happens when it crosses the finish line
    {
        if (collider.gameObject.CompareTag("Finish")) //Onto the next lap
        {
            //Make the sphere collider ignore the finish line, that's for carrot-hunting
            Physics.IgnoreCollision(collider, collision, true); //This way, only his box collider will trigger it
            lap += 1; //Increase the AI's lap count
            Debug.Log("Al is on Lap " + lap + "."); //log it for debugging purposes
        }
        
        if (lap > Timer.GetInstance().lapLimit) //When the AI finishes before the player
        {
            //Both him and the player get how many laps there are from the timer script
            loseText.gameObject.SetActive(true); //'you lose...' appears on-screen
            Player.GetInstance().paused = true; //when this variable is true, everything stops
            Timer.GetInstance().timerOn = false; //stop the timer
            //there's already a 'resultsMenu' object for the player,
            //so it just makes sense to reuse that instead of copying it for the AI
            Player.GetInstance().resultsMenu.SetActive(true);
        }
    }
}