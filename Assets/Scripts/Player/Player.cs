using UnityEngine;
using System.Text;
using System.Collections;
public class Player : MonoBehaviour
{
    Rigidbody2D rb;


    public Vector3 position
    {
        get => rb != null ? rb.position : Vector3.zero;
        set {
            if(rb != null){
                rb.MovePosition(value);
            }
        }
    }


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundMask = Physics.DefaultRaycastLayers & ~(1 << LayerMask.NameToLayer("Player"));
    }
    [SerializeField]
    GameObject deathEffect;

    public bool isDead {get; private set;}

    public void Die(){
        if(isDead)
            return;
            
        isDead = true;
        deathEffect.transform.SetParent(null, true);
        deathEffect.SetActive(true);
        GameManager.Instance.GameOver();
        StartCoroutine(Shrink());
    }

    private IEnumerator Shrink(){
        enabled = false;

        rb.bodyType = RigidbodyType2D.Static;

        float shrinkTime = (float)Fractions.ThreeFifths;

        float t = 0.0f;
        while(t <= shrinkTime){
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t / shrinkTime);
            yield return null;
        }

        Destroy(gameObject);
    }

    public float speed = 3f;
    public float counterTorque = 2f;

    public float jumpForce = 5f;
    private bool grounded;



    public float PInput{
        get => _input;
    }

    private float _input;

    [SerializeField]
    AudioSource rollingAudio;

    public bool jump;

    void Update()
    {
        // Ground check (raycast down)
        grounded = Physics2D.Raycast(transform.position, Vector2.down, groundRayLength, groundMask);

        _input = GetInput();

        if (!Mathf.Approximately(_input, 0.0f)) {
            rb.AddTorque(_input * speed * Time.deltaTime);

            var signInput = Mathf.Sign(_input);
            var signVelo = Mathf.Sign(rb.angularVelocity);
            if (signInput != signVelo)
                rb.AddTorque(-rb.angularVelocity * counterTorque * Time.deltaTime);
        }
        else {
            rb.AddTorque(-rb.angularVelocity * counterTorque * Time.deltaTime);
        }




        // Jump input
        if (grounded && jump) { 
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        
        UpdateRollingSound();
    }


    LayerMask groundMask;

    public float maxVolume = 1.0f;
    public float maxPitch = 1.5f;
    public float minPitch = 0.8f;

    public float velocityDivider = 100.0f;

    public float groundRayLength = 0.52f;
    void UpdateRollingSound(){

        if(Mathf.Approximately(rb.angularVelocity, 0.0f) || !grounded)
        {
            rollingAudio.mute = true;
            return;
        }

        float angularSpeed = Mathf.Abs(rb.angularVelocity);

        // Normalize angular speed to [0,1] range for sound control
        float normalizedSpeed = Mathf.Clamp01(angularSpeed / velocityDivider); // Tune denominator as needed

        rollingAudio.volume = normalizedSpeed * maxVolume;
        rollingAudio.pitch = Mathf.Lerp(minPitch, maxPitch, normalizedSpeed);

        rollingAudio.mute = false;
    }

    float GetInput(){
        var _i = 0.0f;

        jump = Input.GetButtonDown("Jump");

        if(Input.touchCount > 0){
            for(int i = 0; i < Input.touchCount; ++i){
                var touch = Input.GetTouch(i);
                
                var pos = touch.rawPosition;

                if(!jump && touch.phase == TouchPhase.Began && pos.x > Screen.width * Fractions.ThreeFifths && pos.y > Screen.height * Fractions.ThreeFifths)
                    jump = true;
                
                //Debug.Log(pos);
                _i += pos.x < Screen.width / 2 ? 1f : -1f;
            }
        }

        _i += -Input.GetAxisRaw("Horizontal"); // get input (-1, 0, or 1)

        return Mathf.Clamp(_i, -1f, 1f);    
    }
}

