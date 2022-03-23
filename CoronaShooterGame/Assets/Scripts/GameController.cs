using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml.Linq;

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

    //Public variables
    public Boss[] AllBosses;
    public float gameSpeed = 1f;
    public GameObject Enemy;
    public GameObject Background;
    public GameObject[] Walls;
    public GameObject Player;
    public GameObject ChasingField;
    public Texture2D PlayerLife;
    public Texture2D PlayerLifeLost;
    [HideInInspector]
    public AudioController audioController;
    public GameObject BossPrefab;

    //Private variables
    private int currentPlayerlives;
    private int maxPlayerLives;
    private Queue<GameObject> backgrounds;
    private Vector2 lastBackground;
    private bool gameIsActive;
    private int score;
    private int enemyCount;
    private List<Level> levels;
    private List<GameObject> playerLifes;
    private int selectedLevelIndex;

    public bool isGameActive
    {
        get { return gameIsActive; }
    }

    void Start()
    {   
        gameIsActive = false;
        levels = LoadLevels();

        if (levels.Count <= 0)
        {
            return;
        }
        maxPlayerLives = 3;
        currentPlayerlives = maxPlayerLives;
        enemyCount = 0;
        playerLifes = UIHandler.UI.AddPlayerLifes(currentPlayerlives, PlayerLife);

        selectedLevelIndex = 0;

        audioController = GetComponent<AudioController>();
        backgrounds = new Queue<GameObject>();
        score = 0;
    }

    /// <summary>
    /// Reads an image to determine where to spawn enemies (optional: player)
    /// </summary>
    /// <param name="spawnImage">Image to read</param>
    private void readSpawnImage(Texture2D spawnImage)
    {
        Color enemyColor = new Color(0, 0, 0, 1);
        Color playerColor = new Color(0, 1, 0, 1);

        for (int y = 0; y < spawnImage.height; y++)
        {
            for (int x = 0; x < spawnImage.width; x++)
            {
                Color pixel = spawnImage.GetPixel(x, y);

                if (pixel == enemyColor)
                {
                    GameObject spawnedEnemy = Instantiate(Enemy, new Vector3(x * 2 - spawnImage.width + 1, y * 2 + Player.transform.position.y, 0), Quaternion.identity);
                    spawnedEnemy.GetComponent<EnemyController>().setSpeed(Random.Range(1f, 2.5f));
                    enemyCount++;
                }
                if (pixel == playerColor)
                {
                    Player.transform.position = new Vector3(0, 0);
                }
            }
        }
    }

    /// <summary>
    /// Adds player score
    /// </summary>
    /// <param name="amount">Amount to add</param>
    public void AddScore(int amount)
    {
        score += amount;
        UIHandler.UI.SetScore(score);
    }

    /// <summary>
    /// Actions to take when player dies
    /// </summary>
    public void OnPlayerDeath()
    {
        audioController.StopAllSounds();
        audioController.PlaySound("Player death");
        foreach (GameObject life in playerLifes)
        {
            UIHandler.UI.SetPlayerLifeLoss(life, PlayerLifeLost);
        }
        Player.gameObject.SetActive(false);
        UIHandler.UI.SetGameOverScreen(score);
        gameIsActive = false;
    }

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

        MoveFields();
    }

    /// <summary>
    /// Moves background and chasing field
    /// </summary>
    private void MoveFields()
    {
        Background.transform.Translate(new Vector3(0, -1 * gameSpeed * Time.deltaTime, 0));
        ChasingField.transform.Translate(new Vector3(0, 1 * gameSpeed * 0.7f * Time.deltaTime, 0));
        foreach (GameObject wall in Walls)
        {
            wall.transform.position = new Vector3(
                wall.transform.position.x,
                Player.transform.position.y,
                wall.transform.position.z
                );
        }

        if (lastBackground.y < Player.transform.position.y)
        {
            GameObject createdBackground = Instantiate(Background,
                                            new Vector3(0, lastBackground.y + 32f, 0),
                                            Quaternion.identity);
            backgrounds.Enqueue(createdBackground);
            lastBackground = createdBackground.transform.position;

            if (backgrounds.Count > 3)
            {
                Destroy(backgrounds.Dequeue());
            }
        }
    }

    /// <summary>
    /// Loads game levels
    /// </summary>
    /// <returns>Enumerable XML elements with information about levels</returns>
    private List<Level> LoadLevels()
    {
        List<Level> levels = new List<Level>();
        TextAsset xmlDocTxt = Resources.Load<TextAsset>("Levels/levels");
        XDocument doc = XDocument.Parse(xmlDocTxt.text);
        XElement levelsDocXML = doc.Element("levels");
        IEnumerable<XElement> levelsXML = levelsDocXML.Elements("level");

        foreach (XElement level in levelsXML)
        {
            levels.Add(LoadLevel(level, levels.Count));
        }
        return levels;
    }

    private Level LoadLevel(XElement levelXML, int index)
    {
        string levelName = levelXML.Element("name").Value;
        IEnumerable<XElement> levelWaves = levelXML.Elements("wave");
        List<Wave> waves = LoadWaves(levelWaves);
        XElement bossWaveXML = levelXML.Element("bossWave");
        BossWave bossWave = LoadBossWave(bossWaveXML);
        Sprite backgroundSprite = LoadLevelBackground(levelXML);

        Level level = new Level(levelName, waves, bossWave, backgroundSprite, LoadLevelSound(levelXML));
        UIHandler.UI.AddLevelToSelector(level, index);

        return level;
    }

    /// <summary>
    /// Loads waves from level
    /// </summary>
    /// <param name="level">Level to load waves from</param>
    private List<Wave> LoadWaves(IEnumerable<XElement> levelWavesXML)
    {
        List<Wave> waves = new List<Wave>();
        foreach (XElement wave in levelWavesXML)
        {
            Texture2D texture = LoadTextureFromResources(wave, "wavePic");

            waves.Add(new Wave(wave.Element("waveName").Value, texture));
        }

        return waves;
    }

    private BossWave LoadBossWave(XElement bossWaveXML)
    {
        return new BossWave(bossWaveXML.Element("waveName").Value,
            LoadTextureFromResources(bossWaveXML, "wavePic"),
            bossWaveXML.Element("bossName").Value, bossWaveXML.Element("bossSound").Value);
    }

    private string LoadLevelSound(XElement levelXML)
    {
        return levelXML.Element("sound").Value;
    }

    private Sprite LoadLevelBackground(XElement levelXML)
    {
        XElement levelBackground = levelXML.Element("levelBackground");
        return Resources.Load<Sprite>(string.Format("Backgrounds/{0}",
                                                        levelBackground.Value.Substring(0, levelBackground.Value.LastIndexOf('.'))));
    }

    private Texture2D LoadTextureFromResources(XElement element, string textureName)
    {
        XElement wavePic = element.Element(textureName);
        string wavePicture = wavePic.Value.Substring(0, wavePic.Value.LastIndexOf('.'));
        Sprite waveSprites = Resources.Load<Sprite>(string.Format("Images/{0}", wavePicture));
        Texture2D texture = new Texture2D((int)waveSprites.rect.width, (int)waveSprites.rect.height);
        var pixels = waveSprites.texture.GetPixels((int)waveSprites.textureRect.x,
                                                        (int)waveSprites.textureRect.y,
                                                        (int)waveSprites.textureRect.width,
                                                        (int)waveSprites.textureRect.height);
        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

    /// <summary>
    /// Loads level background
    /// </summary>
    /// <param name="level">Level to load background for</param>
    private void LoadBackgroundSprite(Sprite backgroundSprite)
    {
        Background.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = backgroundSprite;

        backgrounds.Clear();
        GameObject spawnedBg = Instantiate(Background, Vector3.zero, Quaternion.identity);
        backgrounds.Enqueue(spawnedBg);
        lastBackground = spawnedBg.transform.position;

    }

    /// <summary>
    /// Starts game level
    /// </summary>
    /// <param name="level">Level to start</param>
    public void StartLevel(int index)
    {
        Level level = levels[index];
        LoadBackgroundSprite(level.BackgroundSprite);
        audioController.PlaySound(level.LevelSoundtrack);
        gameIsActive = true;
        UIHandler.UI.StartGame();
        StartWave();
    }

    /// <summary>
    /// Sets wave
    /// </summary>
    private void StartWave()
    {
        Level level = levels[selectedLevelIndex];
        Wave wave = level.NextWave();
        if (wave is BossWave)
        {
            BossWave bossWave = wave as BossWave;
            audioController.StopSound(level.LevelSoundtrack);
            audioController.PlaySound(bossWave.BossSoundtrack);
        }
        UIHandler.UI.SetWaveText(wave.WaveName);
        readSpawnImage(wave.WavePicture);
    }

    public void OnPlayerHit(Collider2D collision)
    {
        ReducePlayerLifes(1);
        DestroyEnemy(collision.gameObject);
    }

    public void OnProjectileHitEnemy(GameObject projectile, Collider2D collision)
    {
        Destroy(projectile.gameObject);
        DestroyEnemy(collision.gameObject);
        AddScore(20);
    }

    public void OnEnemyHitBorder(GameObject enemy)
    {
        DestroyEnemy(enemy);
        ReducePlayerLifes(1);
    }

    private void DestroyEnemy(GameObject enemy)
    {
        audioController.PlaySound("Enemy Death");
        ReduceEnemyCount();
        Destroy(enemy);
    }

    private void ReducePlayerLifes(int amount)
    {
        currentPlayerlives -= amount;
        UIHandler.UI.SetPlayerLifeLoss(playerLifes[currentPlayerlives], PlayerLifeLost);

        if (currentPlayerlives <= 0)
        {
            OnPlayerDeath();
        }
    }

    /// <summary>
    /// Reduces the amount of enemies left (invoke when enemy dies)
    /// </summary>
    public void ReduceEnemyCount()
    {
        enemyCount--;
        if (enemyCount <= 0)
        {
            StartWave();
        }
    }
}
