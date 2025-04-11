using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour {
    
    public void Hit() {
        GetComponent<Renderer>().enabled = false;
        GetComponent<AudioSource>().Play();
        StartCoroutine(DestroyBrick());
    }

    private IEnumerator DestroyBrick() {
        yield return new WaitForSeconds(0.25f);
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
