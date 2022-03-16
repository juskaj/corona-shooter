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

        scoreText = ScoreTextGameObject.GetComponent<TMP_Text>();
        gameOverText = GameOverTextGameObject.GetComponent<TMP_Text>();
        waveText = WaveTextGameObject.GetComponent<TMP_Text>();
        livesText = LivesTextGameObject.GetComponent<TMP_Text>();
        scoreText.text = "Score: 0";
        GameOverTextGameObject.SetActive(false);
    }

    public GameObject ScoreTextGameObject;
    private TMP_Text scoreText;
    public GameObject GameOverTextGameObject;
    private TMP_Text gameOverText;
    public GameObject WaveTextGameObject;
    private TMP_Text waveText;
    public GameObject LivesTextGameObject;
    private TMP_Text livesText;

    void Start()
    {

    }


    public void SetGameOverScreen(int score)
    {
        gameOverText.text = string.Format("Game Over!\nYour score: {0}\nPress space to restart", score);
        GameOverTextGameObject.SetActive(true);
    }

    public void SetScore(int score)
    {
        scoreText.text = string.Format("Score: {0}", score.ToString());
    }

    public void SetWaveText(string waveText)
    {
        this.waveText.text = waveText;
    }

    public void SetLives(int lives)
    {
        livesText.text = string.Format("Lives: {0}", lives.ToString());
    }
}
