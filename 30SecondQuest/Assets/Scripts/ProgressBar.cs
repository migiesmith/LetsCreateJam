using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Slider))]
public class ProgressBar : MonoBehaviour {

	private Slider _progressBar;

	void Start () {
		LevelLoader.onLoadProgress += onLoadProgress;
		_progressBar = GetComponent<Slider>();
		_progressBar.value = 0.0f;
	}

	protected void onLoadProgress(float progress)
	{
		_progressBar.value = progress;
	}
}
