using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10.0f;
    public float turnspeed = 50.0f;

    public int lap;

    public GameObject carrot;
    GameObject rival;

    public TMP_Text lapText;

    private void Start()
    {
        rival = GameObject.FindGameObjectWithTag("Rival");
    }

    void FixedUpdate()
    {
        float horiz = Input.GetAxis("Horizontal"); // < > arrows or A and D
        float vert = Input.GetAxis("Vertical"); // v ^ arrows or W and S
        float rotation = horiz * turnspeed * Time.deltaTime;
        float translation = vert * speed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
        transform.Translate(0, 0, translation);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(carrot, transform.position, carrot.transform.rotation);
            rival.GetComponent<COMRacer>().GetCarrot(carrot.transform.position);
        }

        lapText.text = "Lap:" + lap;
        //TODO - Maybe if there's time, we can think about adding a jump function
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Finish"))
        {
            lap += 1;
        }
    }
}