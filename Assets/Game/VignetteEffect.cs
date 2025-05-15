using UnityEngine;

[ExecuteInEditMode]
public class VignetteEffect : MonoBehaviour
{
    public Material vignetteMaterial;

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (vignetteMaterial != null)
        {
            Graphics.Blit(src, dest, vignetteMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}

