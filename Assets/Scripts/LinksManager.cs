using UnityEngine;

public class LinksManager : MonoBehaviour
{
	public string moreAppsLink, rateAppUrl;

	public void RateButton()
	{
		Application.OpenURL(rateAppUrl);
	}
	public void MoreAppsButton()
	{
		Application.OpenURL(moreAppsLink);
	}
}
