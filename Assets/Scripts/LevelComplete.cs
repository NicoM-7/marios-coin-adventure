using System.Collections; 
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class LevelComplete : MonoBehaviour {
    
    public float restartDelay = 5f;
    
    private bool oneTime = true;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && oneTime) {
            GameObject.FindWithTag("Hud").GetComponent<Hud>().StopStopwatch();
            
            Data.timeString = GameObject.FindWithTag("Hud").GetComponent<Hud>().UpdateTimerText().Split(" ")[1]; 
            Data.time = GameObject.FindWithTag("Hud").GetComponent<Hud>().GetTime();
            Data.completed = true;

            GetComponent<Renderer>().enabled = false;
            other.GetComponent<Animator>().SetBool("complete", true);
            StartCoroutine(WaitForGrounded(other.GetComponent<Animator>(), other.GetComponent<PlayerController>(), other.GetComponent<Rigidbody2D>()));
            StartCoroutine(RestartSceneAfterDelay());
        }
    }
        
    private IEnumerator RestartSceneAfterDelay() {
        yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadScene("MainMenu");
    }

    private IEnumerator WaitForGrounded(Animator animator, PlayerController controller, Rigidbody2D rb) {
        GameObject.FindWithTag("AudioManager").GetComponent<AudioSource>().Stop();
        while (!animator.GetBool("onGround")) {
            yield return null;
        }

        rb.velocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        controller.enabled = false;
        GetComponent<AudioSource>().Play(); 
    }
}