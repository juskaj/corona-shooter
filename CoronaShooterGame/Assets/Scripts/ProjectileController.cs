using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private int speed;
    private float yCoords;

    private void Start()
    {
        yCoords = gameObject.transform.position.y;
    }

    void Update()
    {
        if (!GameController.GM.IsGameActive)
        {
            return;
        }
        gameObject.transform.Translate(0, Vector2.up.y * Time.deltaTime * speed, 0);
        if (transform.position.y > yCoords + 10)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameController.GM.OnProjectileHitEnemy(gameObject, collision);
        }
    }

    public void SetProjectileSpeed(int speed, Vector2 position)
    {
        this.speed = speed;
        this.transform.position = new Vector3(position.x, position.y + 0.6f, 0);
    }
}
