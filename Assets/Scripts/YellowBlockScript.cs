using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YellowBlockScript : MonoBehaviour
{
    Rigidbody2D rb;
    public GameObject textObject;
    public float speed = 1f;
    Text textComponent;
    public int hitToDestroy;
    public int points;
    private Vector2 initVelocity;
    GameObject playerObj;

    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        playerObj = GameObject.FindGameObjectWithTag("Player");

        if (textObject != null)
        {
            textComponent = textObject.GetComponent<Text>();
            textComponent.text = hitToDestroy.ToString();
        }
 
        initVelocity = new Vector2(speed, 0);
        rb.velocity = initVelocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.tag == "Block" || collision.gameObject.tag == "Wall")
        {
            initVelocity = -initVelocity;
            rb.velocity = initVelocity;
        }

        if (collision.gameObject.tag != "Block" && collision.gameObject.tag != "Wall")
        {

            hitToDestroy--;
            if (hitToDestroy == 0)
            {
                playerObj.GetComponent<PlayerScript>().BlockDestroyed(points);
                Destroy(gameObject);
            }

            else if (textComponent != null)
                textComponent.text = hitToDestroy.ToString();
        }

    }
}
