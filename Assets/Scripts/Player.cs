using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10.0f;
    public float turnspeed = 50.0f;

    public int lap;
    bool messageShown;

    public GameObject carrot;
    GameObject rival;

    public TMP_Text lapText;
    public TMP_Text finalLapText;
    public TMP_Text victoryText;

    float messageTimer;
    public float timeLimit;

    private void Start()
    {
        rival = GameObject.FindGameObjectWithTag("Rival");
        finalLapText.gameObject.SetActive(false);
        victoryText.gameObject.SetActive(false);
        messageShown = false;
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
            Instantiate(carrot, transform.position, carrot.transform.rotation);
            rival.GetComponent<COMRacer>().GetCarrot(carrot.transform.position);
        }

        lapText.text = "Lap:" + lap;

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
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Finish")
        {
            lap += 1;
        }
    }
}