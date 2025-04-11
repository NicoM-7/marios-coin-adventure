using System.Collections; 
using UnityEngine;
using UnityEngine.SceneManagement;

public class Coin : MonoBehaviour {

    private bool oneTime = true;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && oneTime) {
            other.gameObject.GetComponent<PlayerController>().SetCoins(other.gameObject.GetComponent<PlayerController>().GetCoins() + 1);
            oneTime = false;
            GetComponent<AudioSource>().Play();
            GetComponent<Renderer>().enabled = false;
            StartCoroutine(DestroyCoin());
        }
    }

    private IEnumerator DestroyCoin() {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}