using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private float Speed = 0f;

    public void setSpeed(float speed)
    {
        Speed = speed;
    }

    private void Update()
    {
        if (!GameController.GM.isGameActive)
        {
            return;
        }
        transform.Translate(new Vector3(0, -1 * Speed * Time.deltaTime, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Border")
        {
            GameController.GM.OnEnemyHitBorder(gameObject);
        }    
    }
}
