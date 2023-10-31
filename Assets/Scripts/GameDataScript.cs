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

    public void Reset()
    {
        level = 1;
        balls = 6;  
        points = 0;
        pointsToBall = 0;
        if (resetOnStart)
            bestResults = new SerializableList<Record>();
    }

    public bool NewResult(int points)
    {
        bool cond = false;

        if (bestResults.list.Count < 5 || points > bestResults.list[4].recordValue)
        {
            cond = true;
            Record record = new Record();
            record.playerName = username;
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
