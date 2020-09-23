using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public float movementSpeed;
    public float rotationY;
    public float smoothSpeed;
    public float life;

    private bool focus = false;
    public Transform enemy;

    private Time focusTime;


    private float yaw = 0.0f;

    public Sword weapon;

    private void Update()
    {
        Combat();

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

    void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        float run = 1f;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            run = 0.5f;
        }

        if (Input.GetKey("w"))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                run = 2f;
            }
            transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * movementSpeed * run;
        }
        else if (Input.GetKey("s"))
        {
            transform.position += transform.TransformDirection(Vector3.back) * Time.deltaTime * movementSpeed;
        }

        if (Input.GetKey("a"))
        {
            transform.position += transform.TransformDirection(Vector3.left) * Time.deltaTime * movementSpeed;
        }
        else if (Input.GetKey("d"))
        {
            transform.position += transform.TransformDirection(Vector3.right) * Time.deltaTime * movementSpeed;
        }

        yaw += rotationY * Input.GetAxis("Mouse X");

        transform.eulerAngles = new Vector3(0.0f, yaw, 0.0f);
    }

    void Combat()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            weapon.Attack();
        }
        else if (Input.GetKeyDown("z"))
        {
            weapon.HorizontalAttack();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            weapon.Block();
        }
    }
}
