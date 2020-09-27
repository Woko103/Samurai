using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("Rotation and movement")]
    public float movementSpeed;
    public float runSpeed;
    public float rotationY;
    public float smoothSpeed;
    private float yaw = 0.0f;
    
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
    private float lastPosition;
    private bool espadazo_hor;
    private bool last_combo;

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

    void Start(){
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        dashTime += Time.deltaTime;
        dashCooldown += Time.deltaTime;

        Combat();
        //AutoFocus();
    }

    void FixedUpdate()
    {
        Movement();
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
        
        if(!isDead){
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
                if(Input.GetKey(KeyCode.LeftShift)){
                    animator.SetTrigger("run");
                }
                else if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d")){
                    animator.SetTrigger("walk");
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
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            animator.SetTrigger("block");
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            animator.SetTrigger("disblock");
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && comboNum < 3)
        {
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
                if(Input.GetKey(KeyCode.LeftShift)){
                    animator.SetTrigger("run");
                }
                else if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d")){
                    animator.SetTrigger("walk");
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

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("last_combo")){
            if(Input.GetKey(KeyCode.LeftShift)){
                animator.SetTrigger("run");
            }
            else if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d")){
                animator.SetTrigger("walk");
            }
            else {
                animator.SetTrigger("reset");
            }
        }
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
        if((Input.GetKeyDown("w") || Input.GetKeyDown("a") || Input.GetKeyDown("s") || Input.GetKeyDown("d")) && 
        !Input.GetKey(KeyCode.LeftShift)){
            animator.SetTrigger("walk");
        }

        if(Input.GetKeyDown(KeyCode.LeftShift) && 
        (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))){
            animator.SetTrigger("run");
        }

        if(Input.GetKey(KeyCode.LeftShift) && 
        (Input.GetKeyDown("w") || Input.GetKeyDown("a") || Input.GetKeyDown("s") || Input.GetKeyDown("d"))){
            animator.SetTrigger("run");
        }

        if(Input.GetKeyUp(KeyCode.LeftShift) || (Input.GetKey(KeyCode.LeftShift) && 
        (Input.GetKeyUp("w") || Input.GetKeyUp("a") || Input.GetKeyUp("s") || Input.GetKeyUp("d")))){
            animator.SetTrigger("stop_running");
        }

        if(Input.GetKeyUp("w") || Input.GetKeyUp("a") || Input.GetKeyUp("s") || Input.GetKeyUp("d")){
            animator.SetTrigger("stop_moving");
        }
    }
}
