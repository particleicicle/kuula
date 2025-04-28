using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    public float speed = 2f;
    public float counterTorque = 1f; // how strongly it resists drifting

    void Update()
    {
        var input = -Input.GetAxisRaw("Horizontal"); // get input (-1, 0, or 1)

        if (input != 0.0f) {
            rb.AddTorque(input * speed);
            // Opposite input: apply counter torque
            if(Mathf.Sign(input) != Mathf.Sign(rb.angularVelocity)) 
                rb.AddTorque(-rb.angularVelocity * counterTorque * Time.deltaTime);
        }
        else {
            // No input: apply counter torque
            rb.AddTorque(-rb.angularVelocity * counterTorque * Time.deltaTime);
        }
    }
}

