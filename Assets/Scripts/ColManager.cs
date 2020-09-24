using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColManager : MonoBehaviour{

private float hitTimePlayer = 0.0f;
private float hitTimeEnemy = 0.0f;

void Update(){
    hitTimePlayer += Time.deltaTime;
    hitTimeEnemy += Time.deltaTime;
}

void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Enemy" && hitTimePlayer >= 2){
            EnemyAI  enemy = col.gameObject.GetComponent<EnemyAI >();
            //Debug.Log("Enemy life: " + enemy.life);
            enemy.life -= 25;
            //Debug.Log("Enemy life: " + enemy.life);
            hitTimePlayer = 0.0f;
        }

        if(col.gameObject.tag == "Player" && hitTimeEnemy >= 2){
            PlayerActions  player = col.gameObject.GetComponent<PlayerActions >();
            //Debug.Log("Player life: " + player.life);
            player.life -= 25;
            //Debug.Log("Player life: " + player.life);
            hitTimeEnemy = 0.0f;
        }

    }
}
