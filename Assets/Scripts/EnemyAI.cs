using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour{

    public Transform player;

    [Header("Speed & Life")]
    public float speed;
    public float life;
    private float attackTime = 0.0f;
    private float defenseTime = 0.0f;

    [Header("Dash")]
    public float dashSpeed;
    private float dashTime = 1;
    private float dashCooldown = 3;
    private bool dashing = false;

    [Header("Combo")]
    private int comboNum;
    private float resetCombo = 0.0f;

    [Header("Swords & RigidBody")]
    public Sword weapon;
    public Sword playerWeapon;
    private Rigidbody rb;

    private bool close = false;
    // Start is called before the first frame update
    void Start(){
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update(){
        if(!dashing){
            Vector3 direction = player.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = rotation;
        }

        actulizeTimers();

        if(dashing){
            dash();
        }
        else{
            if(!close && (inRange(2))){
                transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * speed;
            }
            else{
                if(!close){
                    close = true;
                }
            }

            if(close){
                transform.position += transform.TransformDirection(Vector3.left) * Time.deltaTime * 1.5f;
                if(close && (inRange(3))){
                    close = false;
                }
            }
        }
        
        Combat();

        if(defenseTime >= 1){
            defenseTime = 0.0f;
        }
    }

    void Combat(){
        if(!inRange(1) && (attackTime >= 1.5f || comboNum > 0) && noAnimations() && !dashing){
            if(comboNum == 0){
                Debug.Log("here");
                weapon.Attack();
                comboNum++;
                resetCombo = 0.0f;
            }
            else{
                Debug.Log("entra");
                weapon.HorizontalAttack();
                comboNum = 0;
                attackTime = 0.0f;
            }
        }
        else if(attackTime < 1.5f && noAnimations() && 
        (playerWeapon.animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo") || 
        playerWeapon.animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo_horizontal"))){
            if(!dashing && (dashCooldown < 3 || defenseTime <= 0.4f)){
                Debug.Log("Block");
                weapon.Block();
                weapon.Disblock();
            }
            else if(!dashing){
                Debug.Log("Dash");
                dashing = true;
                dashTime = 0.0f;
            }
        }

        if(comboNum > 0 && resetCombo >= 0.75f){
            Debug.Log("reset");
            comboNum = 0;
            attackTime = 0.0f;
        }
    }

    public void dash(){
        if(dashing){
            if(dashTime < 0.25f){
                if(transform.position.x > player.position.x){
                    transform.position += transform.TransformDirection(Vector3.right) * Time.deltaTime * speed * dashSpeed;
                }
                else if(transform.position.x < player.position.x){
                    transform.position += transform.TransformDirection(Vector3.left) * Time.deltaTime * speed * dashSpeed;
                }
                else{
                    transform.position += transform.TransformDirection(Vector3.back) * Time.deltaTime * speed * dashSpeed;
                }
            }
            else{
                dashCooldown = 0.0f;
                dashing = false;
            }
        }
    }

    public bool inRange(float range){
        float playerPosX = player.position.x;
        float playerPosZ = player.position.z;

        float enemyPosX = transform.position.x;
        float enemyPosZ = transform.position.z;

        if(enemyPosX + range < playerPosX - range || enemyPosX - range > playerPosX + range || 
        enemyPosZ + range < playerPosZ - range || enemyPosZ - range > playerPosZ + range){
            return true;
        }

        return false;
    }

    public bool noAnimations(){
        if(!weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo") && 
        !weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo_horizontal") && 
        !weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("block") && 
        !weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("blocking") &&
        !weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("disblock")){
            Debug.Log("No hay");
            return true;
        }
        Debug.Log("Hay animaciones");
        return false;
    }

    public void actulizeTimers(){
        attackTime += Time.deltaTime;
        defenseTime += Time.deltaTime;

        dashTime += Time.deltaTime;
        dashCooldown += Time.deltaTime;

        resetCombo += Time.deltaTime;
    }
}