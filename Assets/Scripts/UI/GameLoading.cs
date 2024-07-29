using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameLoading : MonoBehaviour
{
    public Slider loadingGameSlider;

    void Start()
    {
        StartCoroutine(LoadSceneAsync("Game")); 
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        
        while (!asyncLoad.isDone)
        {
            
            loadingGameSlider.value = asyncLoad.progress;

            
            if (asyncLoad.progress >= 0.9f)
            {
                loadingGameSlider.value = 1f;
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
