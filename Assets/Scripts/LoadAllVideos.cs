using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LoadAllVideos : MonoBehaviour
{
	public static LoadAllVideos Instance;

	public GameObject videoSelectionPrefab;

	public VideoClip[] videos;

	private VideoPlayer videoPlayer;

	private void Awake()
	{
		Instance = this;
		videoPlayer = GetComponent<VideoPlayer>();
		videos = Resources.LoadAll<VideoClip>("VideoClips");
	}
	
	void Start()
    {
		StartCoroutine(SetThumbnails());
    }

	private IEnumerator SetThumbnails()
	{
		Debug.Log(videos.Length);
		for (int i = 0; i < videos.Length; i++)
		{
			Debug.Log(videos[i].name);
			videoPlayer.Prepare();
			videoPlayer.clip = videos[i];
			int width = 1280/3;
			int height = 720/3;
			videoPlayer.time = videos[i].length > 5 ? 5 : 0;
			RenderTexture renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
			renderTexture.name = videos[i].name + "Render";
			videoPlayer.targetTexture = renderTexture;
			videoPlayer.Play();
			yield return new WaitWhile(CheckThumbnail);
			Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
			RenderTexture.active = videoPlayer.targetTexture;
			texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
			texture.Apply();
			videoPlayer.Pause(); 
			RenderTexture.active = null;
			GameObject go = Instantiate(videoSelectionPrefab, transform);
			go.name = videos[i].name;
			go.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
			go.GetComponent<SelectVideo>().index = i;
			if(i == 0)
			{
				go.transform.GetChild(0).gameObject.SetActive(false);
			}
			videoPlayer.targetTexture = null;
			Destroy(renderTexture);
			yield return null;
		}
	}

	bool CheckThumbnail()
	{
		return (videoPlayer.time < 5.1f);
	}
}
