using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class GameManager : MonoBehaviour {
    
    public GameObject pauseMenuUI;
    public Sprite coinBlock;
    
    private bool isPaused = false;
    private bool oneTime = true;

    private void Update() {
        int coins = GameObject.FindWithTag("Player").GetComponent<PlayerController>().GetCoins();
        if(coins == 100 && oneTime) {
            GetComponent<AudioSource>().Play();
            oneTime = false;
            GameObject[] blocks = GameObject.FindGameObjectsWithTag("CoinBlock");
            foreach (GameObject block in blocks) {
                block.GetComponent<BoxCollider2D>().enabled = true;
                block.GetComponent<SpriteRenderer>().sprite = coinBlock;
            }
        }
    }
    public void Resume() {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        ToggleAudio(true);
        ToggleAnimators(true);  
    }

    public void Pause() {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        ToggleAudio(false);  
        ToggleAnimators(false);
    }

    private void ToggleAudio(bool play) {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in audioSources) {
            if (play) {
                audio.UnPause();
            } else {
                audio.Pause();
            }
        }
    }

    private void ToggleAnimators(bool enable) {
        Animator[] animators = FindObjectsOfType<Animator>();
        foreach (Animator animator in animators) {
            if(animator.gameObject.tag == "LuckyBlock") {
                if(!animator.gameObject.GetComponent<LuckyBlock>().GetHit()) {
                    animator.enabled = enable;
                }
            } else {
                animator.enabled = enable;
            }
        }
    }

    public void Quit() {
        Resume();
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart() {
        SceneManager.LoadScene("Level1");
        Resume();  
    }

    public bool GetPaused() {
        return isPaused;
    }
}

