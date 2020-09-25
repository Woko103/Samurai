﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColManager : MonoBehaviour{

private float hitTimePlayer = 0.0f;
private float hitTimeEnemy = 0.0f;

void Update(){
    hitTimePlayer += Time.deltaTime;
    hitTimeEnemy += Time.deltaTime;
}

void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Enemy" && hitTimeEnemy >= 2){
            EnemyAI  enemy = col.gameObject.GetComponent<EnemyAI >();
            PlayerActions player = transform.parent.gameObject.GetComponent<PlayerActions>();

            if(player.weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo") || 
        player.weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo_horizontal")){
                if(!enemy.weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("blocking")){
                    //Debug.Log("Enemy life: " + enemy.life);
                    enemy.currentHealth -= 25;
                    enemy.healthBar.setHealth(enemy.currentHealth);
                    //Debug.Log("Enemy life: " + enemy.life);
                    hitTimeEnemy = 0.0f;
                    
                    if(enemy.currentHealth == 0){
                        SceneManager.LoadScene("Menu");
                    }
                }
            }
        }

        if(col.gameObject.tag == "Player" && hitTimePlayer >= 2){
            PlayerActions  player = col.gameObject.GetComponent<PlayerActions >();
            EnemyAI enemy = transform.parent.gameObject.GetComponent<EnemyAI>();

            if(enemy.weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo") || 
        enemy.weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo_horizontal")){
                //Debug.Log("Player life: " + player.life);
                if(!player.weapon.animator.GetCurrentAnimatorStateInfo(0).IsName("blocking")){
                    player.currentHealth -= 25;
                    player.healthBar.setHealth(player.currentHealth);
                    //Debug.Log("Player life: " + player.life);
                    hitTimePlayer = 0.0f;
                }

                if(player.currentHealth == 0){
                    SceneManager.LoadScene("Menu");
                }
            }
        }
    }
}
