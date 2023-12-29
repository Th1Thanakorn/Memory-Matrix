using System;
using UnityEngine;

public class Profiler : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Save(int score)
    {
        int i = 0;
        try
        {
            i = PlayerPrefs.GetInt("Matrix.Highscore");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }

        PlayerPrefs.SetInt("Matrix.Highscore", Mathf.Max(i, score));
    }
}
