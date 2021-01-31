using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullScreenEnable : MonoBehaviour
{
	public static FullScreenEnable Instance;
	public GameObject fullVideo, minimizedVideo;
	public bool isFullScreen = false;

	private void Awake()
	{
		Instance = this;
	}

	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape) && isFullScreen)
		{
			MainVideoPlayer.Instance.FullScreenToggle();
		}
	}

	public void Maximize()
	{
		isFullScreen = true;
		fullVideo.SetActive(true);
		minimizedVideo.SetActive(false);
	}

	public void Minimize()
	{
		isFullScreen = false;
		fullVideo.SetActive(false);
		minimizedVideo.SetActive(true);
	}
}
