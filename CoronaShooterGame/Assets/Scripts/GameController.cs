using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController GM;

    private void Awake()
    {
        if (GM != null)
        {
            Destroy(this);
            return;
        }

        GM = this;
    }

    public GameObject EnemyField;
    public float gameSpeed = 1f;
    public GameObject[] Walls;
    public GameObject Player;
    public GameObject ChasingField;
    private bool gameIsActive;
    private int score;

    void Start()
    {
        gameIsActive = true;
        score = 0;
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log(score);
        UIHandler.UI.SetScore(score);
    }

    public void OnPlayerDeath()
    {
        UIHandler.UI.SetGameOverScreen(score);
        gameIsActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameIsActive)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            return;
        }

        EnemyField.transform.Translate(new Vector3(0, -1 * gameSpeed * Time.deltaTime, 0));
        ChasingField.transform.Translate(new Vector3(0, 1 * gameSpeed * 0.7f * Time.deltaTime, 0));
        foreach(GameObject wall in Walls)
        {
            wall.transform.position = new Vector3(
                wall.transform.position.x,
                Player.transform.position.y,
                wall.transform.position.z
                );
        }
    }
}
