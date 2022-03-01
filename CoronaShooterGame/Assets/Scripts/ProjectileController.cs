using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{

    private int speed;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(0, Vector2.up.y * Time.deltaTime * speed, 0);
        if (transform.position.y > 500)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Hit something...");

        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("Hit enemy");
            EnemyController enemy = collision.GetComponent<EnemyController>();
            enemy.OnProjectileHit();
            Destroy(gameObject);
        }
    }

    public void setProjectileSpeed(int speed, Transform transform)
    {
        this.speed = speed;
        this.transform.position = new Vector3(transform.position.x, transform.position.y + 0.6f, 0);
    }
}
