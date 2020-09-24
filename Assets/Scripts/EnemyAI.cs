using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour{

    public Transform player;

    public float speed;
    public float dashSpeed;
    public float life;
    private float attackTime = 0.0f;
    private float defenseTime = 0.0f;
    private float dashTime = 1;
    private float dashCooldown = 3;

    private bool dashing = false;

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

        attackTime += Time.deltaTime;
        defenseTime += Time.deltaTime;

        dashTime += Time.deltaTime;
        dashCooldown += Time.deltaTime;

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
        if(!inRange(1) && attackTime >= 1.5f && noAnimations()){
            attackTime = 0.0f;
            int attackType = Random.Range(0, 100) % 2;
            switch(attackType){
                case 0:
                    weapon.Attack();
                break;

                case 1:
                    weapon.HorizontalAttack();
                break;
            }
        }
        else if(attackTime < 1.5f && noAnimations() && !weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("block") && 
        (playerWeapon.animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo") || 
        playerWeapon.animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo_horizontal"))){
            if(!dashing && (dashCooldown < 3 || defenseTime <= 0.4f)){
                Debug.Log("Block");
                weapon.Block();
            }
            else if(!dashing){
                Debug.Log("Dash");
                dashing = true;
                dashTime = 0.0f;
            }
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
        !weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("block")){
            return true;
        }
        return false;
    }
}
