using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private int speed;
    private float yCoords;
    public bool fromBoss = false;
    private int dir = 1;

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

        gameObject.transform.Translate(0, dir * Time.deltaTime * speed, 0);

        if (fromBoss)
        {
            if (transform.position.y < yCoords - 10)
            {
                Destroy(gameObject);
            }
            return;
        }

        if (transform.position.y > yCoords + 10)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.tag);

        if (collision.gameObject.CompareTag("Enemy") && !fromBoss)
        {
            GameController.GM.OnProjectileHitEnemy(gameObject, collision);
            return;
        }

        if (collision.gameObject.CompareTag("Player") && fromBoss)
        {
            GameController.GM.OnProjectileHitPlayer(gameObject);
        }
    }

    public void SetProjectile(int speed, Vector2 position, bool fromBoss, RuntimeAnimatorController sprite)
    {
        this.fromBoss = fromBoss;
        dir = 1;
        if (fromBoss)
        {
            transform.GetChild(2).GetComponent<TrailRenderer>().startColor = Color.white;
            transform.GetChild(2).GetComponent<TrailRenderer>().endColor = Color.white;

            transform.rotation = new Quaternion(0f, 0f, 180f, 0f);
        }
        this.speed = speed;
        transform.position = new Vector3(position.x, position.y + 0.6f * dir, 0);
        transform.GetChild(0).GetComponent<Animator>().runtimeAnimatorController = sprite;
    }
}
