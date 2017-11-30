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

        // CHANGEME: Filepath must be under "Resources" and named appropriately. Extension is ignored.
        // {0:000} means zero padding of 3 digits, i.e. 001, 002, 003 ... 010, 011, 012, ...
        // 如patten= "Smoke/smoke_{0:000}"
        public static Texture2DArray CreateArray(string resPattern, int count, int width, int height, TextureFormat format)
        {
            var textureArray = new Texture2DArray(width, height, count, format, false);
//            var textureArray = new Texture2DArray(width, height, count, TextureFormat.Alpha8, true);

            for (int i = 0; i < count; i++)
            {
                var fileName = string.Format(resPattern, i);
                Debug.Log("loading: " + fileName);
                var tex = Resources.Load(fileName) as Texture2D;
                textureArray.SetPixels(tex.GetPixels(0), i, 0);
            }
            textureArray.Apply();
            
            // CHANGEME: Path where you want to save the texture array. It must end in .asset extension for Unity to recognise it.
//            string path = "Assets/_prefabs/TextureArray.asset";
//            AssetDatabase.CreateAsset(textureArray, path);
//            Debug.Log("Saved asset to " + path);
            
            return textureArray;
        }
    }
}