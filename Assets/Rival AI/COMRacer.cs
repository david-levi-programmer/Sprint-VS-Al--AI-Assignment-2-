using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class COMRacer : MonoBehaviour
{
    public GameObject[] waypoints;
    int currentWaypoint = 0;
    private float accuracy = 1.0f;

    //public NavMeshAgent agent;
    
    public float speed;
    public float turnSpeed;

    public float stamina;
    public bool sleep;
    public bool rage;

    public TMP_Text staminaText;
    public TMP_Text moodText;
    
    // Start is called before the first frame update
    void Start()
    {
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        //agent = GetComponent<NavMeshAgent>();
        stamina = 40.0f;
        sleep = false;
        rage = false;
    }

    // Update is called once per frame
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
        this.transform.Rotate(0, 0, speed * Time.deltaTime);

        stamina -= Time.deltaTime; //this variable will constantly be lowering
        //TODO - Have stamina visible on the GUI
        if (!sleep && rage && stamina < 35)
        {
            //speed--;
            rage = false;
            moodText.text = "Mood: Normal";
        }
        if (stamina < 10)
        {
            /*speed = 0;
            //TODO - figure out how to set speed of increase
            stamina += Time.deltaTime;
            stamina++;
            stamina++;*/
            moodText.text = "Mood: Zzz...";
            sleep = true;
        }
        if (sleep && stamina < 50)
        {
            //speed = 50;
            moodText.text = "Mood: #@*!!!!!";
        }

        DisplayStamina();
    }

    void DisplayStamina()
    {
        staminaText.text = "Stamina: " + stamina;
    }
}