using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S4G_WelcomeGameScreen : MonoBehaviour
{
    private int s4g_tutorialProgress;
    public GameObject s4g_tutorialWindow;

    void Start()
    {
        LoadLevelProgress();
        if(s4g_tutorialProgress == 0)
        {
            s4g_tutorialWindow.SetActive(true);
        }
    }

    public void TutorialDone()
    {
        s4g_tutorialProgress = 1;
        PlayerPrefs.SetInt("S4G_Tutor", s4g_tutorialProgress);
        PlayerPrefs.Save();
    }

    void LoadLevelProgress()
    {
        s4g_tutorialProgress = PlayerPrefs.GetInt("S4G_Tutor", 0);
    }
}