using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public float movementSpeed;

    public Sword weapon;

    private void Update()
    {
        Combat();
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
