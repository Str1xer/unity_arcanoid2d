using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockScript : MonoBehaviour
{

    public GameObject textObject;
    Text textComponent;
    public int hitToDestroy;
    public int points;
    GameObject playerObj;

    void Start()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        if (textObject != null)
        {
            textComponent = textObject.GetComponent<Text>();
            textComponent.text = hitToDestroy.ToString();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
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
