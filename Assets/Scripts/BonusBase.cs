using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusBase : MonoBehaviour
{
    Rigidbody2D rb;
    GameObject playerObj;
    SpriteRenderer spriteRenderer;
    public float speed = 1f;
    public Color color;
    public Color textColor;
    public string textName;
    public GameObject textObject;
    Text textComponent;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerObj = GameObject.FindGameObjectWithTag("Player");
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;
        textComponent = textObject.GetComponent<Text>();
        textComponent.color = textColor;
        textComponent.text = textName;
    }

    void FixedUpdate()
    {
        if (rb.isKinematic)
        {
            var pos = transform.position;
            pos.y = pos.y - speed * Time.fixedDeltaTime;
            transform.position = pos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
        Debug.Log("Bonus lost");
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Player collided");
        BonusActivate();
        Destroy(gameObject);
    }

    public virtual void BonusActivate()
    {
        playerObj.GetComponent<PlayerScript>().AddPoint(100);
    }
}
