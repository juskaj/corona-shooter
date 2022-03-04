using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public static UIHandler UI { get; private set; }

    private void Awake()
    {
        if (UI != null)
        {
            Destroy(this);
            return;
        }

        UI = this;
    }

    public GameObject ScoreTextGameObject;
    private TMP_Text scoreText;
    public GameObject GameOverTextGameObject;
    private TMP_Text gameOverText;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = ScoreTextGameObject.GetComponent<TMP_Text>();
        gameOverText = GameOverTextGameObject.GetComponent<TMP_Text>();
        scoreText.text = "Score: 0";
        GameOverTextGameObject.SetActive(false);
    }

    public void SetGameOverScreen(int score)
    {
        gameOverText.text = string.Format("Game Over!\nYour score: {0}\nPress space to restart", score);
        GameOverTextGameObject.SetActive(true);
        Debug.Log("GG");
    }

    public void SetScore(int score)
    {
        scoreText.text = string.Format("Score: {0}", score.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
