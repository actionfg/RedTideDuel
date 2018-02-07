using System;
using System.IO;
using UnityEngine;

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


        // 用于拆分贴图，如4096 × 256拆成16张256×256
        public static void SplitTexture(string path, int width, int height)
        {
            var originTex = Resources.Load<Texture2D>(path);
            int xSplit = (int) (originTex.width / width);
            int ySplit = (int) (originTex.height / height);
            Debug.Log("origin Tex format: " + originTex.format);

            Color c = Color.white;
            var originPixels = originTex.GetPixels();
            for (int i = 0; i < ySplit; i++)
            {
                for (int j = 0; j < xSplit; j++)
                {
                    var newTex = new Texture2D(width, height, TextureFormat.RGBA32, true);
                    var colors = new Color[width * height];
                    int idx = 0;
                    for (int yLoop = 0; yLoop < height; yLoop++)
                    {
                        for (int xLoop = 0; xLoop < width; xLoop++, ++idx)
                        {
                            c.r = originPixels[(i * height + yLoop) * originTex.width + j * width + xLoop].r;
                            c.g = originPixels[(i * height + yLoop) * originTex.width + j * width + xLoop].g;
                            c.b = originPixels[(i * height + yLoop) * originTex.width + j * width + xLoop].b;
                            colors[idx] = c;
                        }
                    }

                    newTex.SetPixels(colors);
                    newTex.Apply();
                    
                    byte[] bytes = newTex.EncodeToPNG();
                    File.WriteAllBytes(Application.dataPath + "/../assets/resources/SBump-" + (i * ySplit + j) + ".png", bytes);

                }
            }
        }

    }
}