using UnityEngine;
using System.Text;
public class Player : MonoBehaviour
{
    Rigidbody2D rb;


    public Vector3 position
    {
        get => rb.position;
        set => rb.MovePosition(value);
    }


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    public float speed = 3f;
    public float counterTorque = 2f;


    public float pInput{
        get => _input;
    }

    private float _input;

    void Update()
    {
        _input = 0.0f;

        if(Input.touchCount > 0){
            for(int i = 0; i < Input.touchCount; ++i){
                var touch = Input.GetTouch(i);
                var pos = touch.rawPosition;
                Debug.Log(pos);
                _input += pos.x < Screen.width / 2 ? 1f : -1f;
            }
        }


        _input += -Input.GetAxisRaw("Horizontal"); // get input (-1, 0, or 1)

        _input = Mathf.Clamp(_input, -1f, 1f);

        if (!Mathf.Approximately(_input, 0.0f)) {
            rb.AddTorque(_input * speed);
            // Opposite input: apply counter torque
            var signInput = Mathf.Sign(_input);
            var signVelo = Mathf.Sign(rb.angularVelocity);
            //Debug.Log(new StringBuilder("Input Sign: ").Append(signInput).Append(" Velocity Sign: ").Append(signVelo).ToString());
            if(signInput != signVelo) 
                rb.AddTorque(-rb.angularVelocity * counterTorque * Time.deltaTime);
        }
        else {
            // No input: apply counter torque
            rb.AddTorque(-rb.angularVelocity * counterTorque * Time.deltaTime);
        }
    }
}

