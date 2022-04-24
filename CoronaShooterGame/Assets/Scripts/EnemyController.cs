using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private float speed = 0f;
    public float Health;

    public void SetupEnemy(float speed, float health)
    {
        this.speed = speed;
        Health = health;
    }

    private void Update()
    {
        if (!GameController.GM.IsGameActive)
        {
            return;
        }
        transform.Translate(new Vector3(0, -1 * speed * Time.deltaTime, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Border"))
        {
            GameController.GM.OnEnemyHitBorder(gameObject);
        }    
    }
}
