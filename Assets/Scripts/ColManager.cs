﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColManager : MonoBehaviour{

private float hitTimePlayer = 0.0f;
private float hitTimeEnemy = 0.0f;

public Canvas gameOverCanvas;
public Canvas victoryCanvas;

public PlayerActions player;
public Animator enemyAnimator;
public ParticleSystem particle;

public AudioSource swordClashAudio;
public AudioSource deathSound;

void Update(){
    hitTimePlayer += Time.deltaTime;
    hitTimeEnemy += Time.deltaTime;
}

void OnTriggerEnter(Collider col){
        if(col.gameObject.tag == "Enemy" && hitTimeEnemy >= 2){
            EnemyAI  enemy = col.gameObject.GetComponent<EnemyAI >();

            if(player.animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo") || 
        player.animator.GetCurrentAnimatorStateInfo(0).IsName("espadazo_horizontal")){
                if(!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("blocking") && 
                !enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("block")){
                    //Debug.Log("Enemy life: " + enemy.life);
                    enemy.currentHealth -= 1;
                    enemy.healthBar.setHealth(enemy.currentHealth);
                    //Debug.Log("Enemy life: " + enemy.life);
                    hitTimeEnemy = 0.0f;
                    
                    if(enemy.currentHealth == 0){
                        enemy.isDead = true;
                        enemyAnimator.SetTrigger("death");
                        deathSound.Play();
                        Invoke("victory", 2.5f);
                    }
                }
                else{
                    if(enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("blocking") || 
                    enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("block")){
                        StartCoroutine(Part());
                    }
                }
            }
        }

        if(col.gameObject.tag == "Player" && hitTimePlayer >= 2){
            PlayerActions  player = col.gameObject.GetComponent<PlayerActions >();

            if(enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("espadazo") || 
        enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("espadazo_horizontal")){
                //Debug.Log("Player life: " + player.life);
                if(!player.animator.GetCurrentAnimatorStateInfo(0).IsName("blocking") && 
                !player.animator.GetCurrentAnimatorStateInfo(0).IsName("block")){
                    player.currentHealth -= 1;
                    player.healthBar.setHealth(player.currentHealth);
                    //Debug.Log("Player life: " + player.life);
                    hitTimePlayer = 0.0f;

                    if(player.currentHealth == 0){
                        player.isDead = true;
                        player.animator.SetTrigger("death");
                        deathSound.Play();
                        Invoke("gameOver", 2.5f);
                    }
                }
                else{
                    if(player.animator.GetCurrentAnimatorStateInfo(0).IsName("blocking") || 
                    player.animator.GetCurrentAnimatorStateInfo(0).IsName("block")){
                        StartCoroutine(Part());
                    }
                }
            }
        }
    }

    public void gameOver(){
        gameOverCanvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }

    public void victory(){
        victoryCanvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }

    private IEnumerator Part(){
        particle.gameObject.SetActive(true);
        swordClashAudio.Play();
        yield return new WaitForSeconds(0.25f);
        particle.gameObject.SetActive(false);
    }
}
