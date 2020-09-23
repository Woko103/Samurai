using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour{

    public Transform player;

    public float speed;
    private float attackTime = 0.0f;

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
        Vector3 direction = player.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(direction);
        transform.rotation = rotation;

        attackTime += Time.deltaTime;

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

        Combat();
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
        else if(attackTime < 1.5f && noAnimations() && (playerWeapon.animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo") || 
        playerWeapon.animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo_horizontal"))){
            Debug.Log("entra");
            weapon.Block();
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
