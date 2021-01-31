using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Fader : MonoBehaviour
{
	public CanvasGroup cg;

    void Awake()
    {
		cg = GetComponent<CanvasGroup>();    
    }

	public IEnumerator FadeIn()
	{
		for (float i = cg.alpha; i <= 1; i += (Time.deltaTime * 3.5f))
		{
			cg.alpha = i;
			cg.blocksRaycasts = cg.alpha > 0.5f;
			if (i >= 1)
			{
				cg.alpha = 1;
				break;
			}
			yield return null;
		}
		
	}

	public IEnumerator FadeOut()
	{
		for (float i = cg.alpha; i >= -1; i -= (Time.deltaTime * 3.5f))
		{
			cg.alpha = i;
			cg.blocksRaycasts = cg.alpha > 0.5f;
			if (i <= 0)
			{
				cg.alpha = 0;
				break;
			}
			yield return null;
		}
	}


	public void FadeInFunction()
	{
		StartCoroutine(FadeIn());
	}
	public void FadeOutFunction()
	{
		StartCoroutine(FadeOut());
	}
}
