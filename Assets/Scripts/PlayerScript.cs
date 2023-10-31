using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    const int maxLevel = 30;
    [Range(1, maxLevel)]
    public int level = 1;
    public float ballVelocityMult = 0.02f;
    public GameObject bluePrefab;
    public GameObject redPrefab;
    public GameObject greenPrefab;
    public GameObject yellowPrefab;
    public GameObject ballPrefab;
    static Collider2D[] colliders = new Collider2D[50];
    static ContactFilter2D contactFilter = new ContactFilter2D();
    public GameDataScript gameData;
    AudioSource audioSrc;
    public AudioClip pointSound;
    static bool gameStarted = false;
    private int ballsCount = 0;
    private int blocksCount = 0;

    private void SetBackground()
    {
        var bg = GameObject.Find("Background").GetComponent<Image>();
        bg.sprite = Resources.Load(level.ToString("d2"), typeof(Sprite)) as Sprite;
    }

    private void CreateBlocks(GameObject prefab, float xMax, float yMax, int count, int maxCount)
    {
        if (count > maxCount)
        {
            count = maxCount;
        }

        blocksCount += count;

        for (int i = 0; i < count; i++)
        {
            for (int k  = 0; k < 20; k++)
            {
                var obj = Instantiate(prefab, new Vector3((Random.value * 2 - 1)* xMax, Random.value*yMax, 0), Quaternion.identity);
                if (obj.GetComponent<Collider2D>().OverlapCollider(contactFilter.NoFilter(), colliders) == 0)
                    break;
                Destroy(obj);
            }
        }
    }

    private void CreateBalls()
    {
        int count = 2;
        if (gameData.balls == 1)
            count = 1;
        ballsCount = count;
        for (int i = 0;i < count;i++)
        {
            var obj = Instantiate(ballPrefab);
            var ball = obj.GetComponent<BallScript>();
            ball.ballInitialForce += new Vector2(10*i, 0);
            ball.ballInitialForce *= 1 + level*ballVelocityMult;
        }
    }

    private void StartLevel()
    {
        gameData.ResetPlayerSize();
        SetBackground();
        var yMax = 4.3f;
        var xMax = 5.5f;
        CreateBlocks(bluePrefab, xMax, yMax, level, 8);
        CreateBlocks(redPrefab, xMax, yMax, 1 + level, 10);
        CreateBlocks(greenPrefab, xMax, yMax, 1 + level, 12);
        CreateBlocks(yellowPrefab, xMax, yMax, 2 + level, 15);
        CreateBalls();
        if (gameData.resetOnStart)
        {
            gameData.Reset();
        }
    }

    public void BallDestroyed()
    {
        ballsCount--;
        gameData.balls--;
        if (gameData.balls > 0)
        {
            if (ballsCount == 0)
                CreateBalls();
        }
        else
        {
            // Проверка и запись нового рекорда. Если значение в топ 5, то возвращает True.
            gameData.NewResult(gameData.points);

            gameData.Reset();
            SceneManager.LoadScene("MainScene");
        }
    }

    int requiredPointsToBall { get { return 400 + (level - 1)*20; } }

    IEnumerator BlockDestroyedCoroutine()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.2f);
            audioSrc.PlayOneShot(pointSound, 5);
        }
    }


    public void BlockDestroyed(int points)
    {
        gameData.points += points;
        if (gameData.sound)
            audioSrc.PlayOneShot(pointSound, 5);
        blocksCount--;
        gameData.pointsToBall += points;
        if (gameData.pointsToBall >= requiredPointsToBall)
        {
            gameData.balls++;
            gameData.pointsToBall -= requiredPointsToBall;
            if (gameData.sound)
                StartCoroutine(BlockDestroyedCoroutine());

        }
        if (blocksCount == 0)
        {
            if (level < maxLevel)
                gameData.level++;
            SceneManager.LoadScene("MainScene");
        }
    }

    public void AddPoint(int points)
    {
        gameData.points += points;
    }

    void SetMusic()
    {
        if (gameData.music)
            audioSrc.Play();
        else
            audioSrc.Stop();
    }

    float clamp(float x, float max, float min)
    {
        if (x>min && x<max) return x;
        if (x<min) return min;
        return max;
    }

    void Start()
    {
        audioSrc = Camera.main.GetComponent<AudioSource>();
        Cursor.visible = false;
        if (!gameStarted)
        {
            gameStarted = true;
            if (gameData.resetOnStart)
                gameData.Load();
        }
        level = gameData.level;
        SetMusic();
        StartLevel();
    }

    void Update()
    {
        if (Time.timeScale > 0)
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var pos = transform.position;
            pos.x = clamp(mousePos.x, 6.6f, -6.6f);
            transform.position = pos;
        }
            
        if (Input.GetKeyDown(KeyCode.M))
        {
            gameData.music = !gameData.music;
            SetMusic();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            gameData.sound = !gameData.sound;
        }
        if (Input.GetButtonDown("Cancel"))
        {
            if(Time.timeScale > 0)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            gameData.Reset();
            SceneManager.LoadScene("MainScene");
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }

    private void OnApplicationQuit()
    {
        gameData.Save();
    }

    string OnOff(bool boolVal)
    {
        return boolVal ? "on" : "off";
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(0 + Mathf.Floor((Screen.width - Screen.height/3*4)/2) + 5, 2, Screen.width - 10 - Mathf.Floor((Screen.width - Screen.height/3*4)/2), 100), string.Format("<color=yellow><size=18>Level <b>{0}</b>    Balls <b>{1}</b>" + "   Score <b>{2}</b></size></color>", gameData.level, gameData.balls, gameData.points));
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperRight;
        GUI.Label(new Rect(5, 6, Screen.width - 10 - Mathf.Floor((Screen.width - Screen.height/3*4)/2), 100),
        string.Format(
            "<color=yellow><size=12><color=white>Space</color>-pause {0}" +
             " <color=white>N</color>-new" +
             " <color=white>J</color>-jump" +
             " <color=white>M</color>-music {1}" +
             " <color=white>S</color>-sound {2}" +
             " <color=white>Esc</color>-exit</size></color>",
             OnOff(Time.timeScale > 0), OnOff(!gameData.music),
             OnOff(!gameData.sound)), style);
    }
}
