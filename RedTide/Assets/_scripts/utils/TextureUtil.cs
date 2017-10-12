using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game04.Util
{
    public class TextureUtil
    {
        public static Texture2D GetSnapShot(Camera virtualCam, int size, string fileName, bool mipmap)
        {

            virtualCam.aspect = 1.0f;
            RenderTexture tempRT = new RenderTexture(size,size, 24, RenderTextureFormat.ARGB32);
     
            virtualCam.targetTexture = tempRT;
            virtualCam.Render();
     
            RenderTexture.active = tempRT;
            Texture2D virtualMap = new Texture2D(size,size, TextureFormat.RGBA32, mipmap);
            // false, meaning no need for mipmaps
            virtualMap.ReadPixels( new Rect(0, 0, size,size), 0, 0);
            virtualMap.Apply();
     
            RenderTexture.active = null; //can help avoid errors 
            virtualCam.targetTexture = null;
     
            // saveToFile
//            byte[] bytes;
//            bytes = virtualMap.EncodeToPNG();
//            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + fileName, bytes );           

            return virtualMap;
        }

    }
}