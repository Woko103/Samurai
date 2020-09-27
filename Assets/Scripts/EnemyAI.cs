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
    private bool attacking = false;
    private bool blocking = false;

    [Header("Dash")]
    public float dashSpeed;
    private float dashTime = 1;
    private float dashCooldown = 2.5f;
    private bool dashing = false;
    private bool aggressive = false;
    private float aggressiveCooldown = 10;

    [Header("Combo")]
    private int comboNum;
    private float resetCombo = 0.0f;

    [Header("Life")]
    public HealthBar healthBar;
    public float maxHealth;
    public float currentHealth;
    public bool isDead = false;

    [Header("Swords & RigidBody")]
    public Animator enemyAnimator;
    public Animator playerAnimator;
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

        if(!isDead){
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
                        if(!inRange(2.5f) && !dashing && aggressiveCooldown >= 10){
                            dashing = true;
                            aggressive = true;
                            dashTime = 0.0f;
                        }
                        else{
                            transform.position += transform.TransformDirection(Vector3.left) * Time.deltaTime * 1.5f;
                        }
                    }
                    else{
                        if(inRange(1)){
                            transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * speed;
                        }
                        else{
                            if(!inRange(0.5f)){
                                transform.position += transform.TransformDirection(Vector3.back) * Time.deltaTime * speed;
                            }
                        }
                    }
                }

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
        if(!inRange(1) && (attackTime >= 2f || comboNum > 0) && noAnimations() && !dashing && !blocking && !attacking){
            if(comboNum == 0){
                enemyAnimator.SetTrigger("espadazo");
                comboNum++;
                resetCombo = 0.0f;
            }
            else if(comboNum == 1){
                enemyAnimator.SetTrigger("espadazo_hor");
                comboNum++;
            }
            else{
                enemyAnimator.SetTrigger("last_combo");
                comboNum = 0;
            }
            attackTime = 0.0f;
            attacking = true;
        }
        else if(!inRange(1f) && noAnimations() && !dashing && !blocking && !attacking && 
        (playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("espadazo") || 
        playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("espadazo_horizontal"))){
            if((dashCooldown < 2.5f || defenseTime <= 0.3f)){
                blockTime = 0.0f;
                blocking = true;
                enemyAnimator.SetTrigger("block");
            }
            else if(noBlockingAnim() && noDisblockAnim() && noBlockAnim()){
                dashing = true;
                dashTime = 0.0f;
            }
        }

        if(noEspadazoAnim() && noEspadazoHorizAnim() && attacking){
            attacking = false;
            attackTime = 0.0f;
        }

        if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("espadazo") || 
        playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("espadazo_horizontal")){
            blockTime = 0.0f;
        }
        else if(!noBlockingAnim() && blockTime > 1 && blocking){
            enemyAnimator.SetTrigger("disblock");
            blocking = false;
        }

        if(comboNum > 0 && resetCombo >= 0.75f){
            comboNum = 0;
            attackTime = 0.0f;
            enemyAnimator.SetTrigger("reset");
        }
    }

    public void dash(){
        if(dashing){
            if(dashTime < 0.15f){
                if(aggressive){
                    transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * speed * dashSpeed;
                }
                else{
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
            }
            else{
                dashCooldown = 0.0f;
                dashing = false;
                if(aggressive){
                    aggressive = false;
                    aggressiveCooldown = 0.0f;
                }
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
        if(noEspadazoAnim() && noEspadazoHorizAnim() && noBlockAnim() && noBlockingAnim() && noDisblockAnim()){
            return true;
        }
        return false;
    }

    public bool noEspadazoAnim(){
        if(!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("espadazo")){
            return true;
        }

        return false;
    }

    public bool noEspadazoHorizAnim(){
        if(!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("espadazo_horizontal")){
            return true;
        }

        return false;
    }

    public bool noBlockAnim(){
        if(!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("block")){
            return true;
        }

        return false;
    }

    public bool noBlockingAnim(){
        if(!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("blocking")){
            return true;
        }

        return false;
    }

    public bool noDisblockAnim(){
        if(!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("dislock")){
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
        aggressiveCooldown += Time.deltaTime;

        resetCombo += Time.deltaTime;
    }
}