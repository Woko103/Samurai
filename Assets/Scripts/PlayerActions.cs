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

    [Header("Auto-Focus")]
    private bool focus = false;
    public Transform enemy;
    private Time focusTime;

    [Header("Life")]
    public HealthBar healthBar;
    public float maxHealth;
    public float currentHealth;

    [Header("Other")]
    public Sword weapon;

    void Start(){
        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
    }

    private void Update()
    {
        dashTime += Time.deltaTime;
        dashCooldown += Time.deltaTime;

        Combat();
        AutoFocus();
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

        if(!dashing && dashCooldown >= 3 && Input.GetKey(KeyCode.LeftAlt)){
            dashTime = 0;
            dashing = true;
            direction = lastKey;
        }

        if(dashing && dashTime >= 0.25f){
            dashing = false;
            dashCooldown = 0;
        }

        yaw += rotationY * Input.GetAxis("Mouse X");

        transform.eulerAngles = new Vector3(0.0f, yaw, 0.0f);
    }

    void Combat()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            weapon.Block();
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            weapon.Disblock();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && comboNum < 2)
        {
            if (comboNum == 0)
                weapon.Attack();
            else if (comboNum == 1)
                weapon.HorizontalAttack();

            comboNum++;
            reset = 0f;
        }

        if (comboNum > 0)
        {
            reset += Time.deltaTime;
            if (reset > resetTime)
            {
                weapon.Reset();
                comboNum = 0;
            }
        }

        if (comboNum == 2)
        {
            resetTime = 3f;
            comboNum = 0;
        }
        else
        {
            resetTime = 0.4f;
        }
    }

    void AutoFocus()
    {
        if(Input.GetKeyDown(KeyCode.C)){
            if(focus){
                focus = false;
            }
            else{
                focus = true;
            }
        }

        if(focus){
            Vector3 desiredPosition = enemy.position;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            transform.LookAt(enemy);
        }
    }
}
