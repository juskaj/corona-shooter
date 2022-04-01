using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml.Linq;
using Cinemachine;

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
    public ParticleSystem ExplosionEffect;
    public GameObject Camera;
    public GameObject HitPointsCanvas;

    //Private variables
    private int playerDamage;
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
    private float cameraShakeTime;
    private float cameraShakeIntensity;

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
                    spawnedEnemy.GetComponent<EnemyController>().SetupEnemy(Random.Range(1f, 2.5f), Random.Range(5f, 35f));
                    enemyCount++;
                }
                if (pixel == playerColor)
                {
                    Player.SetActive(true);
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
            if (!Player.activeSelf && UIHandler.UI.GameUI.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    RestartLevel();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UnpauseGame();
            }

            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
            return;
        }

        cameraShakeTime -= Time.deltaTime;

        if (cameraShakeTime <= 0)
        {
            ShakeCamera(0, 0);
        }

        MoveFields();
    }

    private void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin camShake = 
            Camera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cameraShakeIntensity = intensity;
        cameraShakeTime = time;
        camShake.m_AmplitudeGain = intensity;
    }

    private void UnpauseGame()
    {
        Debug.Log("Unpausing game.. ");
        audioController.UnpauseAllSounds();
        gameIsActive = true;
    }

    private void PauseGame()
    {
        Debug.Log("Pausing game.. ");
        audioController.PauseAllSounds();
        Player.GetComponent<PlayerController>().StopPlayer();
        gameIsActive = false;
    }

    /// <summary>
    /// Moves background and chasing field
    /// </summary>
    private void MoveFields()
    {
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

            foreach (GameObject background in backgrounds)
            {
                Debug.Log("Moving background: " + background.name.ToString());
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

        Level level = new Level(levelName, waves, bossWave, backgroundSprite, LoadLevelSound(levelXML), index == 0 ? true : false);
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

        while (backgrounds.Count > 0)
        {
            Destroy(backgrounds.Dequeue().gameObject);
        }

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

        if (!level.Playable)
        {
            return;
        }

        enemyCount = 0;
        Background.transform.position = (Vector3.zero);
        ChasingField.transform.position = (Vector3.zero);

        currentPlayerlives = maxPlayerLives;
        playerLifes = UIHandler.UI.AddPlayerLifes(currentPlayerlives, PlayerLife);
        selectedLevelIndex = index;

        DestoyAllEnemies();
        DestroyAllParticles();
        DestroyAllProjectiles();
        level.ResetWaves();
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

        if (level.IsBossWave())
        {
            LevelCompleted();
            return;
        }

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

    public void LevelCompleted()
    {
        Level level = levels[selectedLevelIndex];
        int nextLevel = selectedLevelIndex + 1;
        levels[nextLevel].Playable = true;
        audioController.StopAllSounds();
        gameIsActive = false;
        UIHandler.UI.OpenLevelSelectorUI();
    }

    public void RestartLevel()
    {
        StartLevel(selectedLevelIndex);
    }

    private void DestroyAllProjectiles()
    {
        GameObject[] allProjectiles = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject projectile in allProjectiles)
        {
            Destroy(projectile);
        }
    }

    private void DestroyAllParticles()
    {
        GameObject[] allParticles = GameObject.FindGameObjectsWithTag("Particle");
        foreach (GameObject particle in allParticles)
        {
            Destroy(particle);
        }
    }

    private void DestoyAllEnemies()
    {
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in allEnemies)
        {
            Destroy(enemy);
        }
    }

    public void OnPlayerHit(Collider2D collision)
    {
        ReducePlayerLifes(1);
        DestroyEnemy(collision.gameObject);
    }

    public void OnProjectileHitEnemy(GameObject projectile, Collider2D collision)
    {
        GameObject spawnedHitPoints = Instantiate(HitPointsCanvas, new Vector3(
            collision.transform.position.x,
            collision.transform.position.y + 0.5f,
            collision.transform.position.z),
            Quaternion.identity);
        spawnedHitPoints.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        spawnedHitPoints.GetComponent<HitPointHandler>().HitPointsSetup(10, 1f, 0.7f, Color.yellow);

        EnemyController EC = collision.GetComponent<EnemyController>();

        ShakeCamera(3.5f, .1f);
        EC.Health -= 10;

        if (EC.Health <= 0)
        {
            DestroyEnemy(collision.gameObject);
            AddScore(20);
        }

        Destroy(projectile.gameObject);
    }

    public void OnEnemyHitBorder(GameObject enemy)
    {
        DestroyEnemy(enemy);
        ReducePlayerLifes(1);
    }

    private void DestroyEnemy(GameObject enemy)
    {
        ParticleSystem expl = Instantiate(ExplosionEffect, enemy.transform.position, Quaternion.identity);
        expl.Play();
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
