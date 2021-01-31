using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainVideoPlayer : MonoBehaviour
{
	public static MainVideoPlayer Instance;
	public static int playingVideoIndex = 0;
	public Fader playButton, pauseButton, fullScreen, exitFullScreen, Controllers;
	[HideInInspector] public VideoPlayer videoPlayer;
	public Slider progressBar;
	public Image progressBarImg;
	public RectTransform fullScreenVideo;
	public RectTransform minimizedVideo;
	public Button forwardBtn, backwardBtn;
	public Text forwardTxt, backwardTxt;

	private GameObject loadingUrl;

	private bool isEnded = false;

	private void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Application.runInBackground = true;
		videoPlayer = GetComponent<VideoPlayer>();
		videoPlayer.clip = LoadAllVideos.Instance.videos[0];
		StartCoroutine(PlayVideo());
	}

	public void PlayVideo(int index)
	{
		videoPlayer.Stop();
		videoPlayer.clip = LoadAllVideos.Instance.videos[index];
		playingVideoIndex = index;
		isEnded = false;
		progressBarImg.fillAmount = 0;
		progressBar.value = 0;
		StopCoroutine(PlayVideo());
		StartCoroutine(PlayVideo());
		videoPlayer.Play();
		StartCoroutine(Controllers.FadeOut());
		StartCoroutine(playButton.FadeOut());
		StartCoroutine(pauseButton.FadeIn());
	}

	public void PlayNextVideo()
	{
		if (playingVideoIndex == LoadAllVideos.Instance.videos.Length - 1)
			return;

		PlayVideo(++MainVideoPlayer.playingVideoIndex);
	}

	public void PlayPreviousVideo()
	{
		if (playingVideoIndex == 0)
			return;

		PlayVideo(--MainVideoPlayer.playingVideoIndex);
	}
	IEnumerator PlayVideo()
	{
		if (Admop.Instance != null) Admop.Instance.showInterstitial();

		isEnded = false;
		videoPlayer.Prepare();
		while (!videoPlayer.isPrepared)
		{
			yield return null;
		}
		videoPlayer.Play();
		StartCoroutine(Controllers.FadeOut());

		SelectVideo[] selectVideos = FindObjectsOfType<SelectVideo>();

		for (int i = 0; i < selectVideos.Length; i++)
		{
			if (selectVideos[i].index == playingVideoIndex)
			{
				selectVideos[i].transform.GetChild(0).gameObject.SetActive(false);
			}
			else
			{
				selectVideos[i].transform.GetChild(0).gameObject.SetActive(true);
			}
		}

		while (!isEnded)
		{
			progressBarImg.fillAmount = (float)videoPlayer.frame / (float)videoPlayer.frameCount;

			if (((ulong)videoPlayer.frame >= videoPlayer.frameCount - 10) && videoPlayer.frameCount > 0 && videoPlayer.frame > 0)
			{
				Debug.Log("ENDED");
				StartCoroutine(playButton.FadeOut());
				StartCoroutine(Controllers.FadeIn());
				StartCoroutine(playButton.FadeIn());
				StartCoroutine(pauseButton.FadeOut());
				if (FullScreenEnable.Instance.isFullScreen) FullScreenToggle();
				isEnded = true;
				StopCoroutine(PlayVideo());
			}
			yield return new WaitForEndOfFrame();
		}
	}

	public void ShowControllers()
	{
		Controllers.FadeInFunction();
		Controllers.Invoke("FadeOutFunction", 3f);
	}

	public void PlayPauseToggle()
	{
		if (isEnded)
		{
			isEnded = false;
			progressBarImg.fillAmount = 0;
			progressBar.value = 0;
			videoPlayer.Stop();
			StopCoroutine(PlayVideo());
			StartCoroutine(PlayVideo());
			videoPlayer.Play();
			StartCoroutine(Controllers.FadeOut());
			StartCoroutine(playButton.FadeOut());
			StartCoroutine(pauseButton.FadeIn());
			return;
		}

		if (videoPlayer.isPlaying)
		{
			videoPlayer.Pause();
			StartCoroutine(playButton.FadeIn());
			StartCoroutine(pauseButton.FadeOut());
			Controllers.CancelInvoke("FadeOutFunction");
		}
		else
		{
			videoPlayer.Play();
			StartCoroutine(pauseButton.FadeIn());
			StartCoroutine(playButton.FadeOut());
			Controllers.FadeOutFunction();
		}
	}

	public void FullScreenToggle()
	{
		if (FullScreenEnable.Instance.isFullScreen)
		{
			FullScreenEnable.Instance.Minimize();
			Admop.Instance.ShowBanner();
			fullScreen.FadeInFunction();
			exitFullScreen.FadeOutFunction();
			Controllers.GetComponent<RectTransform>().rotation = minimizedVideo.rotation;
			Controllers.GetComponent<RectTransform>().localScale = minimizedVideo.localScale;
			Controllers.GetComponent<RectTransform>().offsetMin = new Vector2(minimizedVideo.offsetMin.x, minimizedVideo.offsetMin.y);//left-botto
			Controllers.GetComponent<RectTransform>().offsetMax = new Vector2(minimizedVideo.offsetMax.x, minimizedVideo.offsetMax.y);//left-botto
		}
		else
		{
			FullScreenEnable.Instance.Maximize();
			Admop.Instance.HideBanner();
			fullScreen.FadeOutFunction();
			exitFullScreen.FadeInFunction();

			//Controllers.GetComponent<RectTransform>().position = fullScreenVideo.position;
			Controllers.GetComponent<RectTransform>().rotation = fullScreenVideo.rotation;
			Controllers.GetComponent<RectTransform>().localScale = fullScreenVideo.localScale;
			//Controllers.GetComponent<RectTransform>().anchoredPosition = fullScreenVideo.anchoredPosition;
			Controllers.GetComponent<RectTransform>().offsetMin = new Vector2(fullScreenVideo.offsetMin.x, fullScreenVideo.offsetMin.y);//left-botto
			Controllers.GetComponent<RectTransform>().offsetMax = new Vector2(fullScreenVideo.offsetMax.x, fullScreenVideo.offsetMax.y);//left-botto
		}
	}
	public void Seek(float seekPoint)
	{
		Controllers.CancelInvoke();
		Controllers.Invoke("FadeOutFunction", 3f);
		Debug.Log("Seeking: " + (long)(seekPoint * (float)videoPlayer.frameCount) + " out of " + videoPlayer.frameCount);
		videoPlayer.frame = (long)(seekPoint * (float)videoPlayer.frameCount);
		videoPlayer.Play();
		StartCoroutine(playButton.FadeOut());
	}

	bool canForward, canBackward, isFirstTouch = true;
	float step = 0;

	public void ForwardButton()
	{
		canForward = videoPlayer.time < LoadAllVideos.Instance.videos[playingVideoIndex].length;
		if (!isFirstTouch && canForward)
		{
			Controllers.CancelInvoke("FadeOutFunction");
			videoPlayer.Pause();
			//StopCoroutine(PlayVideo());
			step += 10;
			forwardTxt.text = "+" + Mathf.FloorToInt(step);
			forwardTxt.gameObject.SetActive(true);
			backwardBtn.interactable = false;
			CancelInvoke("ResetFirstTouch");
			Invoke("ResetFirstTouch", 0.6f);
		}
		if (isFirstTouch)
		{
			Controllers.Invoke("FadeOutFunction", 1f);
			isFirstTouch = false;
		}
	}

	public void BackwardButton()
	{
		canBackward = videoPlayer.time > 0;
		if (!isFirstTouch && canBackward)
		{
			Controllers.CancelInvoke("FadeOutFunction");
			videoPlayer.Pause();
			//StopCoroutine(PlayVideo());
			step -= 10;
			backwardTxt.text = "" + Mathf.FloorToInt(step);
			backwardTxt.gameObject.SetActive(true);
			forwardBtn.interactable = false;
			CancelInvoke("ResetFirstTouch");
			Invoke("ResetFirstTouch", 0.6f);
		}
		if (isFirstTouch)
		{
			Controllers.Invoke("FadeOutFunction", 1f);
			isFirstTouch = false;
		}
	}

	public void ResetFirstTouch()
	{
		Controllers.FadeOutFunction();
		videoPlayer.time += step;
		step = 0;
		videoPlayer.Play();
		StartCoroutine(PlayVideo());
		forwardTxt.gameObject.SetActive(false);
		backwardTxt.gameObject.SetActive(false);
		forwardBtn.interactable = true;
		backwardBtn.interactable = true;
		isFirstTouch = true;
	}
	public void HideVideo()
	{
		GetComponentInParent<Fader>().FadeOutFunction();
		videoPlayer.Stop();
	}
}
