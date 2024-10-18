using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager GameMan;

    public GameObject playerPrefab;
    public GameObject foodPrefab;
    public GameObject goalGameObject;
    public Transform arenaCenter;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI youWinText;

    public GameObject healthImage1;
    public GameObject healthImage2;
    public GameObject healthImage3;

    private float gameTime = 45f;
    private bool isGameOver = false;
    private int score = 0;
    private int health = 3;

    void Awake()
    {
        if (GameMan == null)
        {
            GameMan = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Start method called");
        health = 3;
        InstantiatePlayer();
        InvokeRepeating("SpawnFood", 1f, 3f);
        UpdateScoreText();
        UpdateTimerText();
        gameOverText.gameObject.SetActive(false);
        youWinText.gameObject.SetActive(false);
        healthImage1.SetActive(true);
        healthImage2.SetActive(true);
        healthImage3.SetActive(true);
    }

    void InstantiatePlayer()
    {
        Vector2 startPosition = Vector2.zero;
        Instantiate(playerPrefab, startPosition, Quaternion.identity);
    }

    void SpawnFood()
    {
        Vector2 foodPosition;
        Collider2D goalCollider = goalGameObject.GetComponent<Collider2D>();

        do
        {
            foodPosition = new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        } while (goalCollider.OverlapPoint(foodPosition));

        Debug.Log("Spawning Food at: " + foodPosition);
        Debug.Log("Play Sound Spawn");
        GameObject food = Instantiate(foodPrefab, foodPosition, Quaternion.identity);
        SoundManager.SoundMan.PlaySound(SoundManager.SoundMan.SpawnSFX);
        StartCoroutine(DespawnFoodAfterTime(food, 3f));
    }

    IEnumerator DespawnFoodAfterTime(GameObject food, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (food != null)
        {
            Destroy(food);
            SoundManager.SoundMan.PlaySound(SoundManager.SoundMan.DespawnSFX);
            Debug.Log("Play Sound Despawn");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver) return;

        gameTime -= Time.deltaTime;
        UpdateTimerText();

        if (gameTime <= 0)
        {
            GameOver();
        }
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
        Debug.Log("Score: " + score);
        if (score >= 3000)
        {
            YouWin();
        }
    }

    public void DecreaseHealth()
    {
        health--;
        SoundManager.SoundMan.PlaySound(SoundManager.SoundMan.HitSFX); 
        Debug.Log("Health: " + health);

        if (health == 2)
        {
            healthImage3.SetActive(false);
        }
        else if (health == 1)
        {
            healthImage2.SetActive(false);
        }
        else if (health <= 0) 
        {
            healthImage1.SetActive(false);
            GameOver();
        }
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    void UpdateTimerText()
    {
        timerText.text = "Time: " + Mathf.Max(0, gameTime).ToString("F2");
    }

    void GameOver()
    {
        isGameOver = true;
        gameOverText.gameObject.SetActive(true);   
        Debug.Log("Game Over)");

        StopAllCoroutines();
        CancelInvoke("SpawnFood");
        Time.timeScale = 0f;

        StartCoroutine(ResetGameAfterDelay(5f));
    }
    
    void YouWin()
    {
        isGameOver = true;
        youWinText.gameObject.SetActive(true);
        Debug.Log("You Win");

        StopAllCoroutines();
        CancelInvoke("SpawnFood");
        Time.timeScale = 0f;

        StartCoroutine(ResetGameAfterDelay(5f));
    }

    IEnumerator ResetGameAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        foreach (GameObject food in GameObject.FindGameObjectsWithTag("Food"))
        {
            Destroy(food);
        }

        foreach (GameObject tail in GameObject.FindGameObjectsWithTag("Tail"))
        {
            Destroy(tail);
        }

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Destroy(player);
        }

        score = 0;
        gameTime = 45f;
        isGameOver = false;

        scoreText.text = "Score: 0";
        timerText.text = "Time: 45.00";
        gameOverText.gameObject.SetActive(false);
        youWinText.gameObject.SetActive(false);
        
        Time.timeScale = 1f;
        InstantiatePlayer();
        InvokeRepeating("SpawnFood", 1f, 3f);
        health = 3;
        healthImage1.SetActive(true);
        healthImage2.SetActive(true);
        healthImage3.SetActive(true);
    }
}

