using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerSpeed = 5;
    Rigidbody2D rb;

    [SerializeField]
    public float speedSmoothness = 0.2f;
    private Vector2 currentVelocity;
    private Vector2 currentVector;

    public GameObject projectile;
    public int projectileSpeed;

    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!GameController.GM.isGameActive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject projectileObject = Instantiate(projectile);
            GameController.GM.audioController.PlaySound("Shooting sound");
            projectileObject.GetComponent<ProjectileController>().setProjectileSpeed(projectileSpeed, transform);
        }
    }

    private void FixedUpdate()
    {
        if (!GameController.GM.isGameActive)
        {
            return;
        }

        float hozirontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector2 direction = new Vector2(hozirontal, vertical);
        direction.Normalize();

        currentVector = Vector2.SmoothDamp(currentVector, direction, ref currentVelocity, speedSmoothness);
        rb.velocity = currentVector * 100 * playerSpeed * Time.deltaTime;
    }

    public void StopPlayer()
    {
        rb.velocity = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            GameController.GM.OnPlayerHit(collision);
        }
        if (collision.gameObject.tag == "Border")
        {
            GameController.GM.OnPlayerDeath();
        }
    }
}
