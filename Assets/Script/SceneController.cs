using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public string sceneToLoad;
    public string additiveSceneToLoad;
    public string sceneToUnload;

    public void LoadSceneByName()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneToLoad);
        Debug.Log($"Loaded scene: {sceneToLoad}");
    }

    public void LoadSceneByIndex(int index)
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(index);
        Debug.Log($"Loaded scene at index: {index}");
    }

    public void LoadSceneAsyncByName()
    {
        Time.timeScale = 1f;

        StartCoroutine(LoadSceneAsync(sceneToLoad));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        Time.timeScale = 1f;

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log($"Asynchronously loaded scene: {sceneName}");
    }

    public void UnloadSceneByName()
    {
        Time.timeScale = 1f;

        SceneManager.UnloadSceneAsync(sceneToUnload);
        Debug.Log($"Unloaded scene: {sceneToUnload}");
    }

    public void PrintActiveScene()
    {
        Time.timeScale = 1f;

        Scene activeScene = SceneManager.GetActiveScene();
        Debug.Log("Active Scene: " + activeScene.name);
    }

    public void SetActiveScene()
    {
        Time.timeScale = 1f;

        Scene scene = SceneManager.GetSceneByName(sceneToLoad);
        if (scene.IsValid())
        {
            SceneManager.SetActiveScene(scene);
            Debug.Log($"Set active scene to: {sceneToLoad}");
        }
        else
        {
            Debug.LogError($"Scene {sceneToLoad} is not loaded or doesn't exist.");
        }
    }

    public void PrintAllLoadedScenes()
    {
        int sceneCount = SceneManager.sceneCount;
        Time.timeScale = 1f;

        for (int i = 0; i < sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            Debug.Log("Loaded Scene " + i + ": " + scene.name);
        }
    }

    public void LoadAdditiveScene()
    {
        SceneManager.LoadScene(additiveSceneToLoad, LoadSceneMode.Additive);
        Debug.Log($"Additively loaded scene: {additiveSceneToLoad}");
    }

    public void Exit()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
