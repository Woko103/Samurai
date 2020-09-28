using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("Rotation and movement")]
    public float movementSpeed;
    public float runSpeed;
    public float rotationY;
    public float smoothSpeed;
    private float yaw = 0.0f;
    private bool walk = false;
    private bool running = false;
    
    [Header("Dash")]
    public float dashSpeed;
    private float dashTime = 1;
    private float dashCooldown = 3;
    private char direction = '-';
    private char lastKey = '-';
    private bool dashing = false;
    
    [Header("Combo")]
    private int comboNum;
    private float reset;
    private float resetTime;
    private bool espadazo = false;
    private bool espadazo_hor = false;
    private bool last_combo = false;
    public AudioSource swordAudio;
    private bool blocking = false;

    [Header("Auto-Focus")]
    //private bool focus = false;
    //public Transform enemy;
    //private Time focusTime;

    [Header("Life")]
    public HealthBar healthBar;
    public float maxHealth;
    public float currentHealth;
    public bool isDead = false;

    [Header("Other")]
    public Animator animator;
    public GameObject intro;
    private bool hasStart;

    void Start(){
        Invoke("startDuel", 7.08f);
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(hasStart)
        {
            dashTime += Time.deltaTime;
            dashCooldown += Time.deltaTime;

            Combat();
            //AutoFocus();
        }
    }

    void FixedUpdate()
    {
        if(hasStart)
        {
            Movement();
        }
    }

    void Movement()
    {
        float run = 1f;
        float dSpeed = 1;

        if(!dashing && lastKey != '-'){
            lastKey = '-';
        }

        if (Input.GetKey(KeyCode.LeftControl)){
            run = 0.5f;
        }

        if (Input.GetKey(KeyCode.LeftShift)){
            run = runSpeed;
        }

        if(dashing){
            dSpeed = dashSpeed;
        }
        
        if(!isDead && !blocking){
            if ((Input.GetKey("w") && !dashing) || (dashing && direction == 'w')){
                transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * movementSpeed * run * dSpeed;
                if(!dashing){
                    lastKey = 'w';
                }
            }
            else if ((Input.GetKey("s") && !dashing) || (dashing && (direction == 's' || direction == '-'))){
                transform.position += transform.TransformDirection(Vector3.back) * Time.deltaTime * movementSpeed * run * dSpeed;
                if(!dashing){
                    lastKey = 's';
                }
            }

            if ((Input.GetKey("a") && !dashing) || (dashing && direction == 'a')){
                transform.position += transform.TransformDirection(Vector3.left) * Time.deltaTime * movementSpeed * run * dSpeed;
                if(!dashing){
                    lastKey = 'a';
                }
            }
            else if ((Input.GetKey("d") && !dashing) || (dashing && direction == 'd')){
                transform.position += transform.TransformDirection(Vector3.right) * Time.deltaTime * movementSpeed * run * dSpeed;
                if(!dashing){
                    lastKey = 'd';
                }
            }

            Animations();

            if(!dashing && dashCooldown >= 2.5f && Input.GetKeyDown(KeyCode.Space)){
                dashTime = 0;
                dashing = true;
                animator.SetTrigger("dash");
                direction = lastKey;
            }

            if(dashing && dashTime >= 0.15f){
                dashing = false;
                dashCooldown = 0;
                if(Input.GetKey(KeyCode.LeftShift) && 
                (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))){
                    animator.SetTrigger("run");
                    running = true;
                }
                else if((Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))){
                    animator.SetTrigger("walk");
                    walk = true;
                }
                else {
                    animator.SetTrigger("stop_moving");
                }
            }
        }

        yaw += rotationY * Input.GetAxis("Mouse X");

        transform.eulerAngles = new Vector3(0.0f, yaw, 0.0f);
    }

    void Combat()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && !animator.GetCurrentAnimatorStateInfo(0).IsName("run")){
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("walk")){
                animator.SetTrigger("stop_moving");
            }
            animator.SetTrigger("block");
            blocking = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1) && !animator.GetCurrentAnimatorStateInfo(0).IsName("run")){
            animator.SetTrigger("disblock");
            blocking = false;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && comboNum < 3)
        {
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("walk") || 
            animator.GetCurrentAnimatorStateInfo(0).IsName("run")){
                animator.SetTrigger("stop_moving");
            }
            if (comboNum == 0)
                animator.SetTrigger("espadazo");
            else if (comboNum == 1)
                animator.SetTrigger("espadazo_hor");
            else if (comboNum == 2)
                animator.SetTrigger("last_combo");

            comboNum++;
            reset = 0f;
        }

        if (comboNum > 0)
        {
            reset += Time.deltaTime;
            if (reset > resetTime)
            {
                if(Input.GetKey(KeyCode.LeftShift) && !animator.GetCurrentAnimatorStateInfo(0).IsName("run") && 
                (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))){
                    animator.SetTrigger("run");
                    running = true;
                }
                else if(!animator.GetCurrentAnimatorStateInfo(0).IsName("walk") && 
                (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))){
                    animator.SetTrigger("walk");
                    walk = true;
                }
                else {
                    animator.SetTrigger("reset");
                }
            
                comboNum = 0;
            }
        }

        if (comboNum == 3)
        {
            resetTime = 3f;
            comboNum = 0;
        }
        else
        {
            resetTime = 0.4f;
        }

        //Sonidos espada
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo")){
            if (!espadazo)
            {   
                swordAudio.Play();
                espadazo = true;
            }
        }
        else espadazo = false;

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo_horizontal")){
            if (!espadazo_hor)
            {   
                swordAudio.Play();
                espadazo_hor = true;
            }
        }
        else espadazo_hor = false;

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("last_combo")){
            if (!last_combo)
            {   
                swordAudio.Play();
                last_combo = true;
            }
            if(Input.GetKey(KeyCode.LeftShift) && !animator.GetCurrentAnimatorStateInfo(0).IsName("run") && 
                (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))){
                animator.SetTrigger("run");
                running = true;
            }
            else if(!animator.GetCurrentAnimatorStateInfo(0).IsName("walk") && 
            (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))){
                animator.SetTrigger("walk");
                walk = true;
            }
            else {
                animator.SetTrigger("reset");
            }
        }
        else last_combo = false;

    }

    //void AutoFocus()
    //{
    //    if(Input.GetKeyDown(KeyCode.C)){
    //        if(focus){
    //            focus = false;
    //        }
    //        else{
    //            focus = true;
    //        }
    //    }
//
    //    if(focus){
    //        Vector3 desiredPosition = enemy.position;
    //        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    //        transform.position = smoothedPosition;
//
    //        transform.LookAt(enemy);
    //    }
    //}

    void Animations(){
        if(!animator.GetCurrentAnimatorStateInfo(0).IsName("walk") && 
        (Input.GetKeyDown("w") || Input.GetKeyDown("a") || Input.GetKeyDown("s") || Input.GetKeyDown("d")) && 
        !Input.GetKey(KeyCode.LeftShift)){
            animator.SetTrigger("walk");
            walk = true;
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && !animator.GetCurrentAnimatorStateInfo(0).IsName("run") &&
        (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))){
            animator.SetTrigger("run");
            running = true;
        }

        if(Input.GetKey(KeyCode.LeftShift) && !animator.GetCurrentAnimatorStateInfo(0).IsName("run") &&
        (Input.GetKeyDown("w") || Input.GetKeyDown("a") || Input.GetKeyDown("s") || Input.GetKeyDown("d"))){
            animator.SetTrigger("run");
            running = true;
        }

        if((Input.GetKeyUp(KeyCode.LeftShift) || (Input.GetKey(KeyCode.LeftShift)) && running && 
        (!Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d")))){
            animator.SetTrigger("stop_running");
            running = false;
        }

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("walk") && walk &&
        (!Input.GetKey("w") && !Input.GetKey("a") && !Input.GetKey("s") && !Input.GetKey("d"))){
            animator.SetTrigger("stop_moving");
            walk = false;
        }
    }

    void startDuel()
    {
        intro.SetActive(false);
        hasStart = true;
    }
}
