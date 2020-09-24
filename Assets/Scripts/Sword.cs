using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    public Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
    }
    public void Attack()
    {
        animator.SetTrigger("espadazo");
    }

    public void HorizontalAttack()
    {
        animator.SetTrigger("espadazo_hor");
    }

    public void Block()
    {
        animator.SetTrigger("block");
    }
}
