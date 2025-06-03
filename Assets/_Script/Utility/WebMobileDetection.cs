using System.Runtime.InteropServices;
using UnityEngine;

public class WebMobileDetection : MonoBehaviour
{
    #if UNITY_WEBGL
    public static WebMobileDetection Instance;
    [DllImport ("__Internal")] private static extern bool IsMobile ();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public bool IsRunningOnMobile()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer && !Application.isEditor)
        {
            return IsMobile();
        }
        else
        {
            return false;
        }
    }
    #endif
}
