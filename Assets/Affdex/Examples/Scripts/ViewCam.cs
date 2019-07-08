using UnityEngine;
using UnityEngine.UI;
using Affdex;

/**
 * Add this script to Preview component.
 * Claudemir Casa
 * claudemir.casa@ufpr.br
 * IMAGO Research Group
 */
public class ViewCam : MonoBehaviour {
    public Affdex.CameraInput cameraInput;
    public Affdex.VideoFileInput movieInput;

    private bool m_IsRawImage = false;

	void Start () {
		if (!AffdexUnityUtils.ValidPlatform ())
			return;

        Texture texture = movieInput != null ? movieInput.Texture : cameraInput.Texture;

        if (texture == null)
            return;

        if (GetComponent<RawImage>())
        {
            GetComponent<RawImage>().texture = texture;
            GetComponent<RawImage>().material.mainTexture = texture;
            m_IsRawImage = true;
        }
        else
        {
            GetComponent<MeshRenderer>().material.mainTexture = texture;

            // rotate the image to be upright on the display
            if (cameraInput != null)
            {
                float videoRotationAngle = -cameraInput.videoRotationAngle;
                transform.rotation = transform.rotation * Quaternion.AngleAxis(videoRotationAngle, Vector3.forward);
            }

            float wscale = (float)texture.width / (float)texture.height;

            transform.localScale = new Vector3(transform.localScale.y * wscale, transform.localScale.y, 1);
        }
    }
}