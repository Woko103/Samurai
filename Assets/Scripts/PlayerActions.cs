using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    public float movementSpeed;

    void FixedUpdate()
    {
        if (Input.GetKey("w"))
        {
            float run = 1f;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                run = 2f;
            }
            else if (Input.GetKey(KeyCode.LeftControl))
            {
                run = 0.5f;
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
}
