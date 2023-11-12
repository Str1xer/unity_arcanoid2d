using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PlayerScript : MonoBehaviour
{
    const int maxLevel = 30;
    [Range(1, maxLevel)]
    public int level = 1;
    public float ballVelocityMult = 0.02f;
    public Canvas mainMenu;
    public GameObject bluePrefab;
    public GameObject redPrefab;
    public GameObject greenPrefab;
    public GameObject yellowPrefab;
    public GameObject SpecialYellowPrefab;
    public GameObject ballPrefab;
    static Collider2D[] colliders = new Collider2D[50];
    static ContactFilter2D contactFilter = new ContactFilter2D();
    public GameDataScript gameData;
    AudioSource audioSrc;
    public AudioClip pointSound;
    static bool gameStarted = false;
    static bool gamePaused = true;
    private int ballsCount = 0;
    private int blocksCount = 0;
    private bool showGUI = false;
    public InputField inputField;
    public Text StartTextButton;
    public Text Records;
    public Text NewRecord;
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
            for (int k = 0; k < 20; k++)
            {
                var obj = Instantiate(prefab, new Vector3((Random.value * 2 - 1) * xMax, Random.value * yMax, 0), Quaternion.identity);
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
        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(ballPrefab);
            var ball = obj.GetComponent<BallScript>();
            ball.ballInitialForce += new Vector2(10 * i, 0);
            ball.ballInitialForce *= 1 + level * ballVelocityMult;
        }
    }

    private void StartLevel()
    {
        gameData.ResetStickyPlayer();
        gameData.ResetPlayerSize();

        gameStarted = true;

        Debug.Log("Level started");

        SetBackground();
        var yMax = 4.3f;
        var xMax = 5.5f;

        CreateBlocks(bluePrefab, xMax, yMax, level, 8);
        CreateBlocks(redPrefab, xMax, yMax, 1 + level, 10);
        CreateBlocks(greenPrefab, xMax, yMax, 1 + level, 12);
        CreateBlocks(yellowPrefab, xMax, yMax, 2 + level, 15);
        CreateBlocks(SpecialYellowPrefab, xMax, yMax, 2 + level, 15);
        CreateBalls();

        if (gameData.level > 1)
        {
            ChangeMenuState();
        }

        if (gameData.resetOnStart)
        {
            this.StartTextButton.text = "Start";
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
            // Check recordin new record. If record in top 5, then return True.
            Cursor.visible = true;
            if (gameData.NewResult(gameData.points, gameData.username))
                gameData.newRecord = true;
            gameStarted = false;
            gameData.username = "";
            StartTextButton.text = "Start";
            gameData.Reset();
            SceneManager.LoadScene("MainScene");
        }
    }

    int requiredPointsToBall { get { return 400 + (level - 1) * 20; } }

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
        if (x > min && x < max) return x;
        if (x < min) return min;
        return max;
    }
    public void OnStartButtonClick()
    {
        Debug.Log("Start from button");

        NewRecord.gameObject.SetActive(false);
        if (!string.IsNullOrEmpty(inputField.text))
        {
            gameData.username = inputField.text;

            if (!gameStarted)
            {
                gameStarted = true;
                gamePaused = false;
                StartLevel();
            }
            this.StartTextButton.text = "Continue";
            ChangeMenuState();

        }
        else
        {
            inputField.placeholder.GetComponent<Text>().text = "Really enter name, please";
            inputField.Select();
        }

    }
    public void OnExitButtonClick()
    {
        StartTextButton.text = "Start";
        gameData.Reset();
        Application.Quit();
       
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }
    public void OnNewGameClick()
    {

        gameStarted = false;
        gameData.username = "";
        this.StartTextButton.text = "Continue";
        gameData.Reset();
        SceneManager.LoadScene("MainScene");

    }


    void Start()
    {

        Debug.Log("General Start");
        if (gameData.newRecord)
        {
            NewRecord.gameObject.SetActive(true);
        }
        else
        {
            NewRecord.gameObject.SetActive(false);
        }
        gameData.newRecord = false;
        audioSrc = Camera.main.GetComponent<AudioSource>();

        Time.timeScale = 0;

        if (!string.IsNullOrEmpty(gameData.username))
        {
            inputField.text = gameData.username;
        }

        mainMenu.sortingOrder = 1;

        for (int i = 0; i < gameData.bestResults.list.Count; i++)
        {
            Records.text += gameData.bestResults.list[i].playerName + " - " + gameData.bestResults.list[i].recordValue + '\n';
        }

        level = gameData.level;
        SetMusic();

        if (gameData.level > 1)
        {
            StartLevel();
        }

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

        if (Input.GetKeyDown(KeyCode.M) && gameStarted && !gamePaused)
        {
            gameData.music = !gameData.music;
            SetMusic();
        }
        if (Input.GetKeyDown(KeyCode.S) && gameStarted && !gamePaused)
        {
            gameData.sound = !gameData.sound;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && gameStarted)
        {

            ChangeMenuState();
        }
    }
    public void ChangeMenuState()
    {

        Debug.Log("Show Menu");

        mainMenu.sortingOrder = mainMenu.sortingOrder * -1;
        if (Time.timeScale > 0)
        {
            gamePaused = true;
            Cursor.visible = true;
            this.showGUI = false;
            Time.timeScale = 0;
        }
        else
        {
            gamePaused = false;
            Cursor.visible = false;
            this.showGUI = true;
            Time.timeScale = 1;
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
        if (showGUI)
        {
            GUI.Label(new Rect(0 + Mathf.Floor((Screen.width - Screen.height / 3 * 4) / 2) + 5, 2, Screen.width - 10 - Mathf.Floor((Screen.width - Screen.height / 3 * 4) / 2), 100), string.Format("<color=yellow><size=18>Player <b>{0}</b>    Level <b>{1}</b>    Balls <b>{2}</b>" + "   Score <b>{3}</b></size></color>", gameData.username, gameData.level, gameData.balls, gameData.points));

            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.UpperRight;
            GUI.Label(new Rect(5, 6, Screen.width - 10 - Mathf.Floor((Screen.width - Screen.height / 3 * 4) / 2), 100),
                string.Format(
                    "<color=yellow><size=12>" +
                    " <color=white>J</color>-jump" +
                    " <color=white>M</color>-music {1}" +
                    " <color=white>S</color>-sound {2}" +
                    " <color=white>Esc</color>-menu</size></color>",
                    OnOff(Time.timeScale > 0), OnOff(!gameData.music),
                    OnOff(!gameData.sound)), style);
        }
    }
}
