using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private float speed = 0f;
    public float Health;
    public bool IsBoss;
    public GameObject Projectile;
    private GameObject player;
    private Vector2 currentVector;
    private Vector2 currentVelocity;
    private Rigidbody2D rb;

    public void SetupEnemy(float speed, float health, bool isBoss, Sprite enemySprite, GameObject player)
    {
        this.speed = speed;
        Health = health;
        IsBoss = isBoss;
        gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = enemySprite;
        this.player = player;
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!GameController.GM.IsGameActive)
        {
            return;
        }

        if (IsBoss)
        {
            float nextX = Mathf.MoveTowards(gameObject.transform.position.x, player.transform.position.x, 0.01f);
            transform.position = new Vector3(nextX, gameObject.transform.position.y, 0);

            GameController.GM.OnProjectileSpawn(new Vector2(transform.position.x, transform.position.y - 1.2f), Projectile, 2, true);
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
