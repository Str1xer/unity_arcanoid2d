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
