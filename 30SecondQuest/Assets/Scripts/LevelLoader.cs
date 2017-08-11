using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelLoader : MonoBehaviour {


    public delegate void OnScreenChangeHandler(float progress);

    static OnScreenChangeHandler _onLoadProgress;
    public static event OnScreenChangeHandler onLoadProgress
    {
        add { _onLoadProgress += value; }
        remove { _onLoadProgress -= value; }
    }

	public void LoadLevel(int sceneIndex)
	{
		StartCoroutine(loadAsync(sceneIndex));
	}

	IEnumerator loadAsync(int sceneIndex)
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
		while(!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / 0.9f);
			_onLoadProgress(progress);
			yield return null;
		}
	}



}
