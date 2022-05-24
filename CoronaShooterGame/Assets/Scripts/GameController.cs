using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml.Linq;
using Cinemachine;
using System.Collections;

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
    public GameObject ExplosionEffect;
    public GameObject Camera;
    public GameObject HitPointsCanvas;
    public PlayerStats playerStats;
    public bool bossWave;

    private Level currentLevel;
    private int currentPlayerlives;
    private Queue<GameObject> backgrounds;
    private Vector2 lastBackground;
    private bool gameIsActive;
    private int enemyCount;
    private List<Level> levels;
    private List<GameObject> playerLifes;
    private int selectedLevelIndex;
    private float cameraShakeTime;
    private float cameraShakeIntensity;
    private float cooldownTime;
    private float bossCooldownTime;

    public bool IsGameActive
    {
        get { return gameIsActive; }
    }

    void Start()
    {
        playerStats = new PlayerStats(7000, 1, 1, 1, 1);

        gameIsActive = false;
        levels = LoadLevels();

        if (levels.Count <= 0)
        {
            return;
        }

        bossWave = false;
        audioController = GetComponent<AudioController>();
        backgrounds = new Queue<GameObject>();
    }

    /// <summary>
    /// Reads an image to determine where to spawn enemies (optional: player)
    /// </summary>
    /// <param name="spawnImage">Image to read</param>
    private void ReadSpawnImage(Texture2D spawnImage, Sprite enemySprite, Boss boss)
    {
        Color enemyColor = new Color(0, 0, 0, 1);
        Color playerColor = new Color(0, 1, 0, 1);
        Color bossColor = new Color(0, 0, 1, 1);

        for (int y = 0; y < spawnImage.height; y++)
        {
            for (int x = 0; x < spawnImage.width; x++)
            {
                Color pixel = spawnImage.GetPixel(x, y);

                if (pixel == enemyColor)
                {
                    GameObject spawnedEnemy = Instantiate(Enemy, new Vector3(x * 2 - spawnImage.width + 1, y * 2 + Player.transform.position.y, 0), Quaternion.identity);
                    spawnedEnemy.GetComponent<EnemyController>().SetupEnemy(Random.Range(1f, 2.5f), Random.Range(5f, 35f), false, enemySprite, Player);
                    enemyCount++;
                }
                if (pixel == bossColor)
                {
                    GameObject spawnedEnemy = Instantiate(Enemy, new Vector3(x * 2 - spawnImage.width + 1, y * 2 + Player.transform.position.y, 0), Quaternion.identity);
                    spawnedEnemy.GetComponent<EnemyController>().SetupEnemy(0f, boss.Health, true, boss.Sprite, Player);
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
        playerStats.Score += amount;
        UIHandler.UI.SetScore(playerStats.Score);
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
        Player.SetActive(false);
        UIHandler.UI.SetGameOverScreen(playerStats.Score);
        gameIsActive = false;
    }

    void Update()
    {
        if (!gameIsActive)
        {
            if (!Player.activeSelf && UIHandler.UI.GameOverTextGameObject.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    RestartLevel();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) && !UIHandler.UI.GameOverTextGameObject.activeSelf && UIHandler.UI.GameUI.activeSelf)
            {
                UnpauseGame();
            }

            return;
        }

        if (bossWave)
        {
            if (bossCooldownTime > 0)
            {
                bossCooldownTime -= Time.deltaTime * currentLevel.LevelBoss.Cooldown;
            }
        }


        if (cooldownTime > 0)
        {
            cooldownTime -= Time.deltaTime * playerStats.CooldownLevel;
            UIHandler.UI.UpdateCooldown(cooldownTime, 0.5f);
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

        if (!bossWave)
        {
            MoveFields();
        }

    }

    private void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin camShake = 
            Camera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cameraShakeIntensity = intensity;
        cameraShakeTime = time;
        camShake.m_AmplitudeGain = intensity;
    }

    public void UnpauseGame()
    {
        audioController.UnpauseAllSounds();
        gameIsActive = true;
        UIHandler.UI.StartGame();
    }

    private void PauseGame()
    {
        Debug.Log("Pausing game.. ");
        audioController.PauseAllSounds();
        Player.GetComponent<PlayerController>().StopPlayer();
        gameIsActive = false;
        UIHandler.UI.OpenPauseUI();
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
        Sprite enemySprite = LoadEnemySprite(levelXML);
        Sprite bossSprite = LoadBossSprite(bossWaveXML);
        int bossHealth = int.Parse(bossWaveXML.Element("bossHealth").Value);
        float bossCooldown = float.Parse(bossWaveXML.Element("bossCooldown").Value);
        string bossName = bossWaveXML.Element("bossName").Value;
        float bossProjectileSpeed = float.Parse(bossWaveXML.Element("bossProjectileSpeed").Value);

        string playerProjectileSpriteString = levelXML.Element("projectileSprite").Value;
        RuntimeAnimatorController playerProjectileSprite = Resources.Load<RuntimeAnimatorController>(string.Format("Images/{0}",
            playerProjectileSpriteString.Substring(0, playerProjectileSpriteString.LastIndexOf('.'))));

        string bossProjectileSpriteString = bossWaveXML.Element("bossProjectileSprite").Value;
        RuntimeAnimatorController bossProjectileSprite = Resources.Load<RuntimeAnimatorController>(string.Format("Images/{0}",
            bossProjectileSpriteString.Substring(0, bossProjectileSpriteString.LastIndexOf('.'))));

        Level level = new Level(levelName, waves, bossWave, backgroundSprite, LoadLevelSound(levelXML), index == 0, enemySprite,
            new Boss(bossName, bossSprite, bossCooldown, bossHealth, bossProjectileSpeed), playerProjectileSprite, bossProjectileSprite);
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

    private Sprite LoadEnemySprite(XElement levelXML)
    {
        XElement enemySprite = levelXML.Element("enemySprite");
        return Resources.Load<Sprite>(string.Format("Images/{0}", enemySprite.Value.Substring(0, enemySprite.Value.LastIndexOf('.'))));
    }

    private Sprite LoadBossSprite(XElement levelXML)
    {
        XElement bossSprite = levelXML.Element("bossSprite");
        return Resources.Load<Sprite>(string.Format("Images/{0}", bossSprite.Value.Substring(0, bossSprite.Value.LastIndexOf('.'))));
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
            Destroy(backgrounds.Dequeue());
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

        currentLevel = level;

        enemyCount = 0;
        Background.transform.position = (Vector3.zero);
        ChasingField.transform.position = (Vector3.zero);
        Player.transform.position = (Vector3.zero);

        currentPlayerlives = playerStats.HealthLevel;
        playerLifes = UIHandler.UI.AddPlayerLifes(currentPlayerlives, PlayerLife);
        selectedLevelIndex = index;
        Debug.Log("Selected Level: " + selectedLevelIndex);

        bossCooldownTime = 0;
        bossWave = false;

        DestoyAllEnemies();
        DestroyAllParticles();
        DestroyAllProjectiles();
        level.ResetWaves();
        LoadBackgroundSprite(level.BackgroundSprite);
        audioController.PlaySound(level.LevelSoundtrack);
        gameIsActive = true;
        UIHandler.UI.StartGame();
        UIHandler.UI.SetScore(playerStats.Score);
        Player.SetActive(true);
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
        UIHandler.UI.SetWaveText(wave.WaveName);

        if (wave is BossWave)
        {
            BossWave bossWave = wave as BossWave;
            UIHandler.UI.OpenBossUI();
            UIHandler.UI.SetupBossHealthSlider(level.LevelBoss.Health, level.LevelBoss.Name);
            audioController.StopSound(level.LevelSoundtrack);
            audioController.PlaySound(bossWave.BossSoundtrack);
            this.bossWave = true;
        }

        ReadSpawnImage(wave.WavePicture, level.EnemySprite, level.LevelBoss);
    }

    public void LevelCompleted()
    {
        int nextLevel = selectedLevelIndex + 1;
        if (nextLevel < levels.Count)
        {
            levels[nextLevel].Playable = true;
        }

        audioController.StopAllSounds();
        UIHandler.UI.GameUI.SetActive(false);
        gameIsActive = false;
        UIHandler.UI.OpenLevelSelectorUI();
    }

    public void RestartLevel()
    {
        if (UIHandler.UI.GameUI.activeSelf)
        {
            Debug.Log("Restarting Level " + selectedLevelIndex);
            StartLevel(selectedLevelIndex);
        }
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
    
    public void OnProjectileHitPlayer(GameObject projectile)
    {
        ReducePlayerLifes(1);
        Destroy(projectile);
    }

    public void OnPlayerHit(Collider2D collision)
    {
        ReducePlayerLifes(1);

        if (collision.GetComponent<EnemyController>().IsBoss)
        {
            OnPlayerDeath();
            return;
        }

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
        spawnedHitPoints.GetComponent<HitPointHandler>().HitPointsSetup(10 * playerStats.DamageLevel, 1f, 0.7f, Color.yellow);

        EnemyController EC = collision.GetComponent<EnemyController>();

        ShakeCamera(3.5f, .1f);
        EC.Health -= 10 * playerStats.DamageLevel;

        if (EC.IsBoss)
        {
            UIHandler.UI.ChangeBossHealthSlider((int)EC.Health);
        }

        if (EC.Health <= 0)
        {
            if (EC.IsBoss)
            {
                AddScore(300);
            }
            else
            {
                AddScore(20);
            }
            DestroyEnemy(collision.gameObject);
        }

        Destroy(projectile);
    }

    public void OnProjectileSpawn(Vector2 pos, GameObject projectileToSpawn, int projectileSpeed, bool isBoss)
    {
        if (!isBoss)
        {
            if (cooldownTime > 0)
            {
                return;
            }
            cooldownTime = 0.5f;
        }
        else
        {
            if (bossCooldownTime > 0)
            {
                return;
            }
            projectileSpeed = (int)currentLevel.LevelBoss.ProjectileSpeed;
            bossCooldownTime = 2.5f;
        }

        GameObject projectileObject = Instantiate(projectileToSpawn);
        audioController.PlaySound("Shooting sound");
        projectileObject.GetComponent<ProjectileController>().SetProjectile(projectileSpeed, pos, isBoss,
            isBoss ? currentLevel.BossProjectileSprite : currentLevel.PlayerProjectileSprite);
    }

    public void OnEnemyHitBorder(GameObject enemy)
    {
        DestroyEnemy(enemy);
        ReducePlayerLifes(1);
    }

    private void DestroyEnemy(GameObject enemy)
    {
        GameObject expl = Instantiate(ExplosionEffect, enemy.transform.position, Quaternion.identity);
        ParticleSystem explParticles = expl.transform.GetChild(0).GetComponent<ParticleSystem>();
        explParticles.Play();
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
