using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10.0f;
    public float turnspeed = 50.0f;
    public GameObject carrot;
    GameObject rival;

    private void Start()
    {
        rival = GameObject.FindGameObjectWithTag("Rival");
    }

    void FixedUpdate()
    {
        float horiz = Input.GetAxis("Horizontal"); // < > arrows or A and D
        float vert = Input.GetAxis("Vertical");     // \/ /\ arrows or W and S
        float rotation = horiz * turnspeed * Time.deltaTime;
        float translation = vert * speed * Time.deltaTime;
        transform.Rotate(0, rotation, 0);
        transform.Translate(0, 0, translation);
        //TODO - Should there be a jump button?
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(carrot, this.transform.position, carrot.transform.rotation);

        }
    }
}