using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour{

    public Transform player;

    public float speed;

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

        float playerPosX = player.position.x;
        float playerPosZ = player.position.z;

        float enemyPosX = transform.position.x;
        float enemyPosZ = transform.position.z;

        if(!close && (enemyPosX + 2 < playerPosX - 2 || enemyPosX - 2 > playerPosX + 2 || 
        enemyPosZ + 2 < playerPosZ - 2 || enemyPosZ - 2 > playerPosZ + 2)){
            transform.position += transform.TransformDirection(Vector3.forward) * Time.deltaTime * speed;
        }
        else{
            if(!close){
                close = true;
            }
        }

        if(close){
            transform.position += transform.TransformDirection(Vector3.left) * Time.deltaTime * 1.5f;
            if(close && (enemyPosX + 4 > playerPosX || enemyPosX - 4 < playerPosX || 
        enemyPosZ + 4 > playerPosZ || enemyPosZ - 4 < playerPosZ)){
                close = false;
            }
        }
    }
}
