
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;

public class RenderTextureToPng : MonoBehaviour
{
    [SerializeField] private Camera currentCamera;
    [SerializeField] private Vector2Int imageResolution;
    [SerializeField] private string saveName;
    [SerializeField, FolderPath] private string savePath;
    
    [Button]
    void SaveToPng()
    {
        RenderTexture renderTexture = new RenderTexture(imageResolution.x, imageResolution.y, 24 ,RenderTextureFormat.ARGB32);
        currentCamera.targetTexture = renderTexture;
        currentCamera.Render();
        currentCamera.targetTexture = null;
        Texture2D texture = ToTexture2D(renderTexture);
        texture.Apply();
        byte[] bytes = texture.EncodeToPNG();
        if(!Directory.Exists(savePath)) {
            Directory.CreateDirectory(savePath);
        }
        File.WriteAllBytes(savePath + "/" + saveName + ".png", bytes);
    }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    
    Texture2D ToTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.ARGB32, false);
        var oldRT = RenderTexture.active;
        RenderTexture.active = rTex;

        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        
        tex.Apply();

        RenderTexture.active = oldRT;
        return tex;
    }
}