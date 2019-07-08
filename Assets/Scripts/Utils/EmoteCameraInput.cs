using UnityEngine;
using Affdex;

/**
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
namespace Emote.Utils
{
    public class EmoteCameraInput : CameraInput
    {
        public WebCamTexture WebCamTexture
        {
            get
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_XBOXONE || UNITY_IOS || UNITY_ANDROID
                return cameraTexture;
#else
                return new WebCamTexture();
#endif
            }
        }
    }
}