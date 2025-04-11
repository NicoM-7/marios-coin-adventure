using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuckyBlock : MonoBehaviour
{
    public float bounceHeight = 0.2f;
    public float bounceSpeed = 5f;

    public Sprite empty;

    public GameObject coin; 

    private Vector2 originalPosition;
    private bool hit = false;

    private void Start() {
        originalPosition = transform.position;
    }

    public void Hit() {
        if(!hit) {
            hit = true;
            GetComponent<AudioSource>().Play();
            GetComponent<SpriteRenderer>().sprite = empty;
            GetComponent<Animator>().enabled = false;
            StartCoroutine(BounceCoroutine());
        } 
    }

    private IEnumerator BounceCoroutine() {
        Vector2 targetPosition = originalPosition + Vector2.up * bounceHeight;
        while (transform.position.y < targetPosition.y) {
            transform.position = new Vector2(transform.position.x, transform.position.y + bounceSpeed * Time.deltaTime);
            yield return null;
        }

        while (transform.position.y > originalPosition.y) {
            transform.position = new Vector2(transform.position.x, transform.position.y - bounceSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = originalPosition;
        Instantiate(coin, (Vector2)transform.position + Vector2.up, Quaternion.identity);
    }

    public bool GetHit() {
        return hit;
    }
}
