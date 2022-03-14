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
        transform.Translate(new Vector3(0, -1 * Speed * Time.deltaTime, 0));
    }

    public void OnProjectileHit()
    {
        GameController.GM.AddScore(20);
        GameController.GM.ReduceEnemyCount();
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Border")
        {
            GameController.GM.ReduceEnemyCount();
            Destroy(gameObject);
        }    
    }
}