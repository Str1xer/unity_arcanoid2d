using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallScript : MonoBehaviour
{
    public Vector2 ballInitialForce;
    Rigidbody2D rb;
    GameObject playerObj;
    float deltaX;

    AudioSource audioSrc;
    public AudioClip hitSound;
    public AudioClip loseSound;
    public GameDataScript gameData;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerObj = GameObject.FindGameObjectWithTag("Player");
        deltaX = transform.position.x;
        audioSrc = Camera.main.GetComponent<AudioSource>();
    }

    private void setHasStickyBasllToFalse() {
        gameData.hasStickyPlayerHoldBall = false;
    }

    void Update()
    {
        if (rb.isKinematic) 
        { 
            if (Input.GetButtonDown("Fire1"))
            {
                rb.isKinematic = false;
                rb.AddForce(ballInitialForce);
                // Hach for case when we have already have a sticky ball
                // and when we click and expect that ball will be thrown, 
                // but because ball is close to player unity will trigger
                // [OnCollisionEnter2D] and we will not throw a ball
                Invoke("setHasStickyBasllToFalse", 0.5f);
            } 
            else
            {
                var pos = transform.position;
                pos.x = playerObj.transform.position.x + deltaX;
                transform.position = pos;
            }
        }
        if (!rb.isKinematic && Input.GetKeyDown(KeyCode.J))
        {
            var v = rb.velocity;
            if (Random.Range(0, 2) == 0)
                v.Set(v.x - 0.1f, v.y + 0.1f);
            else
                v.Set(v.x + 0.1f, v.y - 0.1f);
            rb.velocity = v;
        }
        if (!rb.isKinematic)
            rb.velocity = rb.velocity.normalized*7;
        Debug.Log(rb.velocity.x*rb.velocity.x + rb.velocity.y*rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (gameData.sound)
            audioSrc.PlayOneShot(loseSound, 5);
        playerObj.GetComponent<PlayerScript>().BallDestroyed();
        Destroy(gameObject);
        Debug.Log("Game Over");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if ball collided with player and
        // we hava a sticky ball and we dont have 
        // any sticky ball yet.
        if (collision.gameObject.tag == "Player" && gameData.isPlayerSticky && !gameData.hasStickyPlayerHoldBall) {
            rb.isKinematic = true;
            gameData.hasStickyPlayerHoldBall = true;
            rb.velocity = new Vector2(0, 0);

        }
        if (gameData.sound)
            audioSrc.PlayOneShot(hitSound, 5);
    }
}
