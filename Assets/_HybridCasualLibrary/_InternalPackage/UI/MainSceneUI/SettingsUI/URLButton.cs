using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class URLButton : MonoBehaviour
{
    [SerializeField] string iosURL;
    [SerializeField] string androidURL;

    public void OpenLinkByURL(string _url)
    {
        if (_url == null || _url == "") return;
        Application.OpenURL(_url);
    }

    public void OpenLinkByURLByDevice()
    {
        if (iosURL == null || iosURL == "") return;
        if (androidURL == null || androidURL == "") return;
        if (Application.platform == RuntimePlatform.Android)
            Application.OpenURL(androidURL);
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
            Application.OpenURL(iosURL);
    }
}
