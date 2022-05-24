using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

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
        scoreText.text = "0";
        OpenMainMenuTitleUI();
    }

    public GameObject LevelSelectorUI;
    public GameObject LevelSelectorLevel;
    public GameObject TitleUI;
    public GameObject PauseUI;
    public GameObject UpgradesUI;
    public GameObject BossUI;
    public GameObject BossHealthSlider;
    public GameObject BossName;

    public GameObject HealthUpgrade;
    public GameObject SpeedUpgrade;
    public GameObject CooldownUpgrade;
    public GameObject DamageUpgrade;

    public Image CooldownImage;

    public GameObject MainMenuUI;
    public GameObject GameUI;
    public GameObject ScoreTextGameObject;
    private TMP_Text scoreText;
    public GameObject GameOverTextGameObject;
    private TMP_Text gameOverText;
    public GameObject WaveTextGameObject;
    private TMP_Text waveText;
    public GameObject PlayerLivesPanel;
    public GameObject PlayerLife;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenUpgradesUI()
    {
        MainMenuUI.SetActive(true);
        TitleUI.SetActive(false);
        PauseUI.SetActive(false);
        GameUI.SetActive(false);
        LevelSelectorUI.SetActive(false);
        UpgradesUI.SetActive(true);
        BossUI.SetActive(false);
        UpdateScore();
    }

    public void OpenPauseUI()
    {
        MainMenuUI.SetActive(false);
        LevelSelectorUI.SetActive(false);
        GameUI.SetActive(true);
        PauseUI.SetActive(true);
        TitleUI.SetActive(false);
        UpgradesUI.SetActive(false);
    }

    public void OpenLevelSelectorUI()
    {
        MainMenuUI.SetActive(true);
        LevelSelectorUI.SetActive(true);
        GameUI.SetActive(false);
        PauseUI.SetActive(false);
        TitleUI.SetActive(false);
        UpgradesUI.SetActive(false);
        BossUI.SetActive(false);
    }

    public void OpenBossUI()
    {
        BossUI.SetActive(true);
    }

    public void OpenMainMenuTitleUI()
    {
        MainMenuUI.SetActive(true);
        TitleUI.SetActive(true);
        PauseUI.SetActive(false);
        GameUI.SetActive(false);
        LevelSelectorUI.SetActive(false);
        UpgradesUI.SetActive(false);
        BossUI.SetActive(false);
    }

    void OnLevelSelectorClick(int index)
    {
        Debug.Log("Starting Game");
        EventSystem.current.SetSelectedGameObject(null);
        GameController.GM.StartLevel(index);
    }

    public void AddLevelToSelector(Level level, int index)
    {
        GameObject sLevel = Instantiate(LevelSelectorLevel, LevelSelectorUI.transform.GetChild(0).GetChild(0));
        sLevel.GetComponent<Button>().onClick.AddListener(delegate { OnLevelSelectorClick(index); });
        sLevel.transform.GetChild(0).GetChild(0).GetComponent<RawImage>().texture = level.BackgroundSprite.texture;
        sLevel.transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = level.Name;
    }

    public void StartGame()
    {
        MainMenuUI.SetActive(false);
        PauseUI.SetActive(false);
        GameUI.SetActive(true);
        GameOverTextGameObject.SetActive(false);
        UpgradesUI.SetActive(false);
        BossUI.SetActive(false);
    }

    public void SetGameOverScreen(int score)
    {
        gameOverText.text = string.Format("Game Over!\nYour score: {0}\nPress space to restart", score);
        GameOverTextGameObject.SetActive(true);
    }

    public void ChangeBossHealthSlider(int amount)
    {
        BossHealthSlider.GetComponent<Slider>().value = amount;
    }
    public void SetupBossHealthSlider(int maxHealth, string name)
    {
        BossHealthSlider.GetComponent<Slider>().maxValue = maxHealth;
        BossHealthSlider.GetComponent<Slider>().value = maxHealth;
        BossName.GetComponent<TMP_Text>().text = name;
    }

    public void SetScore(int score)
    {
        scoreText.text = string.Format("{0}", score.ToString());
    }

    public void SetWaveText(string waveText)
    {
        this.waveText.text = waveText;
    }

    public List<GameObject> AddPlayerLifes(int amount, Texture2D lifeTexture)
    {
        int prevLives = PlayerLivesPanel.transform.childCount;

        for (int i = 0; i < prevLives; i++)
        {
            Destroy(PlayerLivesPanel.transform.GetChild(i).gameObject);
        }

        List<GameObject> lifes = new List<GameObject>();
        for (int i = 0; i < amount; i++)
        {
            GameObject l = Instantiate(PlayerLife, PlayerLivesPanel.transform, false);
            l.GetComponent<RawImage>().texture = lifeTexture;
            lifes.Add(l);
        }

        return lifes;
    }

    public void SetPlayerLifeLoss(GameObject life, Texture2D lostLifeTexture)
    {
        life.GetComponent<RawImage>().texture = lostLifeTexture;
    }
    
    public void UpdateCooldown(float currentCooldown, float maxCooldown)
    {
        CooldownImage.fillAmount = currentCooldown / maxCooldown;
    }

    private void UpdateScore()
    {
        UpgradesUI.transform.GetChild(0).GetChild(3).GetComponent<TMP_Text>().text = string.Format("{0}", GameController.GM.playerStats.Score);
    }

    public void UpgradeCooldown()
    {
        CooldownUpgrade.transform.GetChild(3).GetComponent<TMP_Text>().text = string.Format("Cost: {0}", Mathf.Pow(GameController.GM.playerStats.UpgradeCooldown(), 2f) * 100);
        UpdateScore();
    }

    public void UpgradeSpeed()
    {
        SpeedUpgrade.transform.GetChild(3).GetComponent<TMP_Text>().text = string.Format("Cost: {0}", Mathf.Pow(GameController.GM.playerStats.UpgradeSpeed(), 2f) * 100);
        UpdateScore();
    }

    public void UpgradeHealth()
    {
        HealthUpgrade.transform.GetChild(3).GetComponent<TMP_Text>().text = string.Format("Cost: {0}", Mathf.Pow(GameController.GM.playerStats.UpgradeHealth(), 2f) * 100);
        UpdateScore();
    }

    public void UpgradeDamage()
    {
        DamageUpgrade.transform.GetChild(3).GetComponent<TMP_Text>().text = string.Format("Cost: {0}", Mathf.Pow(GameController.GM.playerStats.UpgradeDamage(), 2f) * 100);
        UpdateScore();
    }
}
