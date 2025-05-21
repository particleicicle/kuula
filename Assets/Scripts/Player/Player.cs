using UnityEngine;
using System.Text;
using System.Collections;
public class Player : MonoBehaviour
{
    Rigidbody2D rb;


    public Vector2 position
    {
        get => rb != null ? rb.position : Vector2.zero;
        set {
            if(rb != null){
                rb.MovePosition(value);
            }
        }
    }

    
    public Vector2 MoveDir {
        get {
            if(rb != null) 
                return rb.linearVelocity.normalized;

            return Vector2.zero;
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

    public float midairMovementForce = 200.0f;
    public float midairCounterMovementForce = 800.0f;

    public void Die()
    {
        if (isDead)
            return;

        GameManager.Instance.StopTimer(false);

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
    public float counterTorque = 5f;

    public float jumpForce = 6f;


    public float PInput{
        get => _input;
    }

    private float _input;

    [SerializeField]
    AudioSource rollingAudio;

    bool jumpPressed;

    bool jumped;

    [SerializeField] private int groundRayCount = 5;
    [SerializeField] private float radius = 0.5f;

    bool IsGrounded()
    {
        Vector2 origin = transform.position;

        // Cast rays in an arc across the bottom hemisphere
        for (int i = 0; i < groundRayCount; i++)
        {
            // -45° to +45° in radians
            float angle = Mathf.Lerp(-45f, 45f, i / (float)(groundRayCount - 1)) * Mathf.Deg2Rad;

            // Direction from center toward edge of lower hemisphere
            Vector2 dir = new (Mathf.Sin(angle), -Mathf.Cos(angle)); // Points down and slightly out
            Vector2 start = origin + dir * (radius - 0.01f); // Just inside collider edge

            if (Physics2D.Raycast(start, dir, groundRayLength, groundMask))
            {
                return true;
            }

            // Debug visualization
            Debug.DrawRay(start, dir * groundRayLength, Color.green);
        }

        return false;
    }


    void Update()
    {
        _input = GetInput();
    }

    [SerializeField] private float coyoteTime = 0.2f; // How long after leaving ground a jump is still allowed
    [SerializeField] private float jumpCooldown = 0.2f; // How much time must pass between jumps

    private float coyoteTimer = 0f;
    private float lastJumpTime = float.NegativeInfinity;
        
    bool grounded;

    void FixedUpdate()
    {
        var signInput = Mathf.Sign(_input);
        

        if (!Mathf.Approximately(_input, 0.0f)) {
            rb.AddTorque(_input * speed * Time.deltaTime);
            if (signInput != Mathf.Sign(rb.angularVelocity))
                rb.AddTorque(-rb.angularVelocity * counterTorque * Time.deltaTime);
        }
        else {
            rb.AddTorque(-rb.angularVelocity * counterTorque * Time.deltaTime);
        }

        // Ground check
        grounded = IsGrounded();

        if (!grounded) {
            rb.AddForce(_input * (signInput != Mathf.Sign(-rb.linearVelocityX) ? midairCounterMovementForce : midairMovementForce) * Time.deltaTime * Vector2.left);
        }

        // Jump input
        // Update coyote timer
        coyoteTimer = grounded ? coyoteTime : Mathf.Clamp(coyoteTimer - Time.deltaTime, 0.0f, float.MaxValue);


        // Jump logic with coyote time and cooldown
        if (jumpPressed && coyoteTimer >= Mathf.Epsilon && (Time.time - lastJumpTime > jumpCooldown))
        {
            jumped = true;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            lastJumpTime = Time.time;
            coyoteTimer = 0f; // prevent double-jump during buffer
        }

        UpdateRollingSound(!grounded);
    }


    LayerMask groundMask;

    public float maxVolume = 1.0f;
    public float maxPitch = 1.5f;
    public float minPitch = 0.8f;

    public float velocityDivider = 100.0f;

    public float groundRayLength = 0.05f;
    void UpdateRollingSound(bool mute){

        if(Mathf.Approximately(rb.angularVelocity, 0.0f) || mute)
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

    float resetCounter;

    float GetInput(){
        var _i = 0.0f;

        if(Input.GetKey(KeyCode.Escape))
        {
            resetCounter += Time.deltaTime;
            if(resetCounter > 2.0f) {
                resetCounter = 0.0f;
                GameManager.Instance.ReloadCurrentLevel();
                return 0.0f;
            }
        }
        else
            resetCounter = 0.0f;

            
        if(!jumpPressed){
            jumpPressed = Input.GetButton("Jump") && grounded && !jumped;
        }
        else if(jumped){
            jumpPressed = false;
            jumped = false;
        }

        for(int i = 0; i < Input.touchCount; ++i){
            var touch = Input.GetTouch(i);
            
            var pos = touch.rawPosition;

            if(!jumpPressed && touch.phase == TouchPhase.Began && pos.x > Screen.width * Fractions.ThreeFifths && pos.y > Screen.height * Fractions.ThreeFifths)
                jumpPressed = true;
            
            //Debug.Log(pos);
            _i += pos.x < Screen.width / 2 ? 1f : -1f;
        }

        _i += -Input.GetAxisRaw("Horizontal"); // get input (-1, 0, or 1)

        return Mathf.Clamp(_i, -1f, 1f);    
    }
}

