using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour{

    public Transform player;

    [Header("Speed & Actions")]
    public float speed;
    public float runSpeed;
    private float attackTime = 0.0f;
    private float defenseTime = 0.0f;
    private float waitForAttack = 0.0f;
    private float blockTime = 0.0f;

    [Header("Dash")]
    public float dashSpeed;
    private float dashTime = 1;
    private float dashCooldown = 3;
    private bool dashing = false;

    [Header("Combo")]
    private int comboNum;
    private float resetCombo = 0.0f;

    [Header("Life")]
    public HealthBar healthBar;
    public float maxHealth;
    public float currentHealth;

    [Header("Swords & RigidBody")]
    public Sword weapon;
    public Sword playerWeapon;
    private Rigidbody rb;

    private bool close = false;
    // Start is called before the first frame update
    void Start(){
        rb = this.GetComponent<Rigidbody>();

        currentHealth = maxHealth;
        healthBar.setMaxHealth(maxHealth);
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
                transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * speed * runSpeed;
                waitForAttack = 0.0f;
            }
            else{
                if(!close){
                    close = true;
                }
            }

            if(close){
                if(waitForAttack <= 3){
                    transform.position += transform.TransformDirection(Vector3.left) * Time.deltaTime * 1.5f;
                }
                else{
                    if(inRange(1)){
                        transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * speed;
                    }
                }
            }

            if(close && (inRange(3))){
                close = false;
            }
        }
        
        Combat();

        if(defenseTime >= 1){
            defenseTime = 0.0f;
        }
    }

    void Combat(){
        if(!inRange(1) && (attackTime >= 2f || comboNum > 0) && noAnimations() && !dashing){
            if(comboNum == 0){
                weapon.Attack();
                comboNum++;
                resetCombo = 0.0f;
            }
            else{
                weapon.HorizontalAttack();
                comboNum--;
            }
            attackTime = 0.0f;
        }
        else if(noAnimations() && !dashing && comboNum == 0 &&
        (playerWeapon.animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo") || 
        playerWeapon.animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo_horizontal"))){
            if((dashCooldown < 3 || defenseTime <= 0.4f)){
                Debug.Log("Block");
                weapon.Block();
                blockTime = 0.0f;
            }
            else if(!weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("blocking") && 
            !weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("disblock") && 
            !weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("block")){
                Debug.Log("Dash");
                dashing = true;
                dashTime = 0.0f;
            }
        }

        if(weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("blocking") && blockTime > 2){
            Debug.Log("disblock");
            weapon.Disblock();
        }

        if(comboNum > 0 && resetCombo >= 0.75f){
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
            return true;
        }
        return false;
    }

    public void actulizeTimers(){
        attackTime += Time.deltaTime;
        defenseTime += Time.deltaTime;
        waitForAttack += Time.deltaTime;
        blockTime += Time.deltaTime;

        dashTime += Time.deltaTime;
        dashCooldown += Time.deltaTime;

        resetCombo += Time.deltaTime;
    }
}