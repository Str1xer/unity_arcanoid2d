using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "GameData", menuName = "Game Data", order = 51)]
public class GameDataScript : ScriptableObject
{
    [Serializable]
    public class Record
    {
        public string playerName;
        public int recordValue;
    }

    [Serializable]
    public class SerializableList<T>
    {
        public List<T> list;
    }

    public string username = "";
    public bool resetOnStart;
    public bool music = true;
    public bool sound = true;
    public int level = 1;
    public int balls = 6;
    public int points = 0;
    public int pointsToBall = 0;
    public SerializableList<Record> bestResults = new SerializableList<Record>();

    public GameObject baseBonus;
    public GameObject expandBonus;
    public GameObject shrinkBonus;
    public GameObject stickyBonus;
    public GameObject simpleBonus;

    // Counter of size changing
    // Increasing when we multiply player size on 1.5
    // Decreasing when we divide player size on 1.5
    public int playerSizeChanges = 0;


    public bool isPlayerSticky = false;
    public bool hasStickyPlayerHoldBall = false;

    public void ResetStickyPlayer() {
        isPlayerSticky = false;
        hasStickyPlayerHoldBall = false;

        var playerObj = GameObject.FindGameObjectWithTag("Player");

        var spriteRenderer = playerObj.GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
    }

    public void ResetPlayerSize() {
        playerSizeChanges = 0;

        var playerObj = GameObject.FindGameObjectWithTag("Player");

        playerObj.transform.localScale = new Vector3(2, 2, 1);
    }

    // Bonus for instantiating in GreenBlockScript
    public GameObject Bonus()
    {

        // Define the probabilities of different cases
        float baseBonusProb = 0.4f;
        float expandBonusProb = 0.2f;
        float shrinkBonusProb = 0.2f;
        float stickyBonusProb = 0.1f;
        // Probability of simpleBonusProb is calculated:
        // 1 - sum of all others probabilities


        // Generate a random value between 0 and 1
        float randomValue = UnityEngine.Random.Range(0f, 1f);

        GameObject result;

        // Determine the selected case based on the random value
        if (randomValue < baseBonusProb)
        {
            result = baseBonus;
        }
        else if (randomValue < baseBonusProb + expandBonusProb)
        {
            result = expandBonus;
        }
        else if (randomValue < baseBonusProb + expandBonusProb + shrinkBonusProb)
        {
            result = shrinkBonus;
        }
        else if (randomValue < baseBonusProb + expandBonusProb + shrinkBonusProb + stickyBonusProb)
        {
            result = stickyBonus;
        }
        else
        {
            result = simpleBonus;
        }

        return result;
    }


    public void Reset()
    {
        level = 1;
        balls = 6;  
        points = 0;
        pointsToBall = 0;
        if (resetOnStart)
            bestResults = new SerializableList<Record>();
    }

    public bool NewResult(int points,string userName)
    {
        bool cond = false;

        if (bestResults.list.Count < 5 || points > bestResults.list[4].recordValue)
        {
            cond = true;
            Record record = new Record();
            record.playerName = userName;
            record.recordValue = points;

            bestResults.list.Add(record);

            bestResults.list = bestResults.list.OrderBy(x => x.recordValue).ToList();
            bestResults.list.Reverse();
            if (bestResults.list.Count > 5)
                bestResults.list.RemoveAt(5);
        }

        return cond;
    }

    public void Save()
    {
        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetInt("balls", balls);
        PlayerPrefs.SetInt("points", points);
        PlayerPrefs.SetInt("pointsToBall", pointsToBall);
        PlayerPrefs.SetInt("music", music ? 1 : 0);
        PlayerPrefs.SetInt("sound", sound ? 1 : 0);
        PlayerPrefs.SetString("bestResults", JsonUtility.ToJson(bestResults));
    }

    public void Load()
    {
        level = PlayerPrefs.GetInt("level", 1);
        balls = PlayerPrefs.GetInt("balls", 6);
        points = PlayerPrefs.GetInt("points", 0);
        pointsToBall = PlayerPrefs.GetInt("pointsToBall", 0);
        music = PlayerPrefs.GetInt("music", 1) == 1;
        sound = PlayerPrefs.GetInt("sound", 1) == 1;

        bestResults = JsonUtility.FromJson<SerializableList<Record>>(PlayerPrefs.GetString("bestResults"));
    }
}
