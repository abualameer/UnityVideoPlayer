using UnityEngine;
using UnityEngine.UI;

public class SelectVideo : MonoBehaviour
{
	public int index;
    // Start is called before the first frame update
    void Start()
    {
		GetComponent<Button>().onClick.AddListener(OnClickButton);
    }

	void OnClickButton()
	{
		MainVideoPlayer.Instance.PlayVideo(index);
	}
}
