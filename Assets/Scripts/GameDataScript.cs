using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.TextCore.Text;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "GameData", menuName = "Game Data", order = 51)]
public class GameDataScript : ScriptableObject
{
    public bool resetOnStart;
    public bool music = true;
    public bool sound = true;
    public int level = 1;
    public int balls = 6;
    public int points = 0;
    public int pointsToBall = 0;
    public List<int> bestResults = new List<int>();

    public GameObject baseBonus;
    public GameObject expandBonus;
    public GameObject shrinkBonus;
    public GameObject stickyBonus;
    public GameObject simpleBonus;

    // Некоторое хранение изменений размеров пользователя 
    // При увеличении размера счетчик увеличивается на 1
    // При уменьшении размера счетчик уменьшается на 1
    public int playerSizeChanges = 0;

    public void ResetPlayerSize() {
        playerSizeChanges = 0;

        var playerObj = GameObject.FindGameObjectWithTag("Player");

        playerObj.transform.localScale = new Vector3(2, 2, 1);
    }

    // Возвращает бонус
    public GameObject Bonus()
    {

        // Define the probabilities of different cases
        float baseBonusProb = 0;
        float expandBonusProb = 0.5f;
        float shrinkBonusProb = 0.5f;
        float stickyBonusProb = 0;
        // вероятность simpleBonusProb будет высчитана по формуле:
        // 1 - сумма всех остальных


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
            bestResults = new List<int>();
    }

    public bool NewResult(int points)
    {
        bool cond = false;

        if (bestResults.Count < 5 || points > bestResults[4])
        {
            cond = true;
            bestResults.Add(points);
            bestResults.Sort();
            bestResults.Reverse();
            if (bestResults.Count > 5)
                bestResults.RemoveAt(5);
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
        PlayerPrefs.SetString("bestResults", String.Join(",", bestResults));
    }

    public void Load()
    {
        level = PlayerPrefs.GetInt("level", 1);
        balls = PlayerPrefs.GetInt("balls", 6);
        points = PlayerPrefs.GetInt("points", 0);
        pointsToBall = PlayerPrefs.GetInt("pointsToBall", 0);
        music = PlayerPrefs.GetInt("music", 1) == 1;
        sound = PlayerPrefs.GetInt("sound", 1) == 1;
        try
        {
            bestResults = new List<int>();
            string[] temp = PlayerPrefs.GetString("bestResults").Split(',');
            for (int i = 0; i < temp.Length; i++)
            {
                bestResults.Add(Int32.Parse(temp[i]));
            }
            bestResults.Sort();
        }
        catch
        {
            Debug.Log("Records Empty");
        }
    }
}
