using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Cinemachine;
using System.Xml;
using System.Xml.Serialization;
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
    public float gameSpeed = 1f;
    public GameObject Enemy;
    public GameObject Background;
    public GameObject[] Walls;
    public GameObject Player;
    public GameObject ChasingField;

    //Private variables
    private int currentPlayerlives;
    private int maxPlayerLives;
    private Queue<GameObject> backgrounds;
    private GameObject lastBackground;
    private bool gameIsActive;
    private int score;
    private int enemyCount;
    private int currentWave;
    private List<Texture2D> wavePictures;
    private List<string> waveNames;
    private IEnumerator<XElement> levels;

    void Start()
    {
        maxPlayerLives = 3;
        currentPlayerlives = maxPlayerLives;
        enemyCount = 0;
        currentWave = 0;
        UIHandler.UI.SetLives(currentPlayerlives);
        levels = LoadLevels().GetEnumerator();

        if (!levels.MoveNext())
        {
            return;
        }

        backgrounds = new Queue<GameObject>();
        StartLevel(levels.Current);
        gameIsActive = true;
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
        UIHandler.UI.SetLives(0);
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
    }

    private void FixedUpdate()
    {
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

        if (lastBackground.transform.position.y < Player.transform.position.y)
        {
            GameObject createdBackground = Instantiate(Background,
                                            new Vector3(0, lastBackground.transform.position.y + 32f, 0),
                                            Quaternion.identity);
            backgrounds.Enqueue(createdBackground);
            lastBackground = createdBackground;

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
    private IEnumerable<XElement> LoadLevels()
    {
        TextAsset xmlDocTxt = Resources.Load<TextAsset>("Levels/levels");
        XDocument doc = XDocument.Parse(xmlDocTxt.text);
        return doc.Elements("level");
    }

    /// <summary>
    /// Loads waves from level
    /// </summary>
    /// <param name="level">Level to load waves from</param>
    private void LoadWaves(XElement level)
    {
        wavePictures = new List<Texture2D>();
        waveNames = new List<string>();

        XElement levelName = level.Element("name");
        IEnumerable<XElement> waves = level.Elements("wave");

        foreach (XElement wave in waves)
        {

            XElement waveName = wave.Element("waveName");
            waveNames.Add(waveName.Value);

            XElement wavePic = wave.Element("wavePic");
            string wavePicture = wavePic.Value.Substring(0, wavePic.Value.LastIndexOf('.'));
            Sprite waveSprites = Resources.Load<Sprite>(string.Format("Images/{0}", wavePicture));
            Texture2D texture = new Texture2D((int)waveSprites.rect.width, (int)waveSprites.rect.height);
            var pixels = waveSprites.texture.GetPixels((int)waveSprites.textureRect.x,
                                                            (int)waveSprites.textureRect.y,
                                                            (int)waveSprites.textureRect.width,
                                                            (int)waveSprites.textureRect.height);
            texture.SetPixels(pixels);
            texture.Apply();
            wavePictures.Add(texture);
        }
    }

    /// <summary>
    /// Loads level background
    /// </summary>
    /// <param name="level">Level to load background for</param>
    private void LoadBackground(XElement level)
    {
        XElement levelBackground = level.Element("levelBackground");
        Sprite backgroundSprite = Resources.Load<Sprite>(string.Format("Backgrounds/{0}", 
                                                        levelBackground.Value.Substring(0, levelBackground.Value.LastIndexOf('.'))));
        Background.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = backgroundSprite;

        backgrounds.Clear();
        GameObject spawnedBg = Instantiate(Background, Vector3.zero, Quaternion.identity);
        backgrounds.Enqueue(spawnedBg);
        lastBackground = spawnedBg;

    }

    /// <summary>
    /// Starts game level
    /// </summary>
    /// <param name="level">Level to start</param>
    private void StartLevel(XElement level)
    {
        LoadBackground(level);
        LoadWaves(level);
        UIHandler.UI.SetWaveText(waveNames[currentWave]);
        readSpawnImage(wavePictures[currentWave]);
    }

    /// <summary>
    /// Sets wave
    /// </summary>
    private void setWave()
    {
        if (wavePictures.Count < currentWave + 1)
        {
            Debug.Log("Level End");
            //TODO: Add level changing system
            return;
        }
        UIHandler.UI.SetWaveText(waveNames[currentWave]);
        readSpawnImage(wavePictures[currentWave]);
    }

    public void OnPlayerHit(Collider2D collision)
    {
        ReducePlayerLifes(1);
        Destroy(collision.gameObject);
    }

    public void OnProjectileHitEnemy(GameObject projectile, Collider2D collision)
    {
        Destroy(projectile.gameObject);
        Destroy(collision.gameObject);
        AddScore(20);
        ReduceEnemyCount();
    }

    public void OnEnemyHitBorder(GameObject enemy)
    {
        Destroy(enemy);
        ReducePlayerLifes(1);
    }

    private void ReducePlayerLifes(int amount)
    {
        currentPlayerlives -= amount;
        UIHandler.UI.SetLives(currentPlayerlives);

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
            currentWave++;
            setWave();
        }
    }
}
