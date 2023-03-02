using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COMRacer : MonoBehaviour
{
    public float speed;
    public float turnSpeed;

    public float stamina;

    Animator mood;
    
    // Start is called before the first frame update
    void Start()
    {
        mood = GetComponent<Animator>();
        stamina = 30.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO - Use NavMeshAgent and waypoints to make ai race
        //TODO - Should code for racing around track be in state scripts?
        stamina -= Time.deltaTime;
        mood.SetFloat("Energy", stamina);
    }
}