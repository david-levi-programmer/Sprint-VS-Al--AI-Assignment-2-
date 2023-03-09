using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COMRacer : MonoBehaviour
{
    public GameObject[] waypoints;
    
    public float speed;
    public float turnSpeed;

    public float stamina;
    
    // Start is called before the first frame update
    void Start()
    {
        stamina = 30.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO - Use NavMeshAgent and waypoints to make ai race
        //TODO - Should code for racing around track be in state scripts?
        stamina--; //this variable will constantly be lowering
        //TODO - Have stamina visible on the GUI
        if (stamina < 10)
        {
            speed--;
            //TODO - figure out how to set speed of increase
            stamina += Time.deltaTime;
            stamina++;
            stamina++;
        }
        if (stamina > 50)
        {
            speed = 50;
        }
        if (stamina < 35)
        {
            speed = 10;
        }
    }
}