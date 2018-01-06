using System.IO;
using Imaging.DDSReader;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

namespace Game04.Util
{
    public class Test3DTex : MonoBehaviour
    {
        public int Size = 16;
        
        private void Start()
        {
//            Texture3D tex = Create();
//            AssetDatabase.CreateAsset(tex, "Assets/testTex3D.asset");

            //  Unity加载Dxt5 3D图片格式失败, 改由将ARGB格式的,Volume类型的DDS贴图转成3dTexture Asset
            Stream stream = File.Open(@"Assets/resources/textures/splashes/SBumpVolume-ARGB2.dds", FileMode.Open);
            DDSImage ddsImage = DDS.LoadImageData(stream, true);
//            Texture3D tex = Create(ddsImage);
            Texture2D tex = CreateTexFromTop(ddsImage);

            if (tex)
            {
                Debug.Log("width: " + tex.width + " height: " + tex.height);
//                AssetDatabase.CreateAsset(tex, "Assets/_prefabs/SBumpVolume-ARGB2.asset");
            }
            GetComponent<Renderer>().material.SetTexture("_MainTex", tex);
        }

        private Texture2D CreateTexFromTop(DDSImage ddsImage)
        {
            var texture2D = new Texture2D(ddsImage.Width, ddsImage.Height, TextureFormat.ARGB32, false);
            var colors = new Color[ddsImage.Width * ddsImage.Height];
            
            byte[] rawData = ddsImage.ImageData;
            Color c = Color.white;
            int idx = 0;
            for(int y =0; y<ddsImage.Height; y++)
            {
                for (int x = 0; x < ddsImage.Width; x++, ++idx)
                {
                    c.r = rawData[idx * 4 + 2];
                    c.g = rawData[idx * 4 + 1];
                    c.b = rawData[idx * 4];
                    c.a = rawData[idx * 4 + 3];
                    colors[idx] = c;
                }
            }
            texture2D.SetPixels(colors);
            texture2D.Apply();
            
            // Encode texture into PNG
            byte[] bytes = texture2D.EncodeToPNG();

            // For testing purposes, also write to a file in the project folder
             File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
            return texture2D;
        }

        private Texture3D Create(DDSImage ddsImage)
        {
            var tex = new Texture3D(ddsImage.Width, ddsImage.Height, ddsImage.Depth, TextureFormat.ARGB32, true);
            var colors = new Color[ddsImage.Width * ddsImage.Height * ddsImage.Depth];
            Color c = Color.white;
            int idx = 0;
            byte[] rawData = ddsImage.ImageData;
            for (int z = 0; z < ddsImage.Depth; z++)
            {
                for(int y =0; y<ddsImage.Height; y++)
                {
                    for (int x = 0; x < ddsImage.Width; x++, ++idx)
                    {
                        c.r = rawData[idx * 4 + 2];
                        c.g = rawData[idx * 4 + 1];
                        c.b = rawData[idx * 4];
                        c.a = rawData[idx * 4 + 3];
                        colors[idx] = c;
//                        if (z < 2)
//                        {
//                            Debug.Log("x: " + x + " ,Y: " + y + " ,Z: " + z + ", Clolor: " + c);
//                        }
                    }
                }
            }
            tex.SetPixels(colors);
            tex.Apply();

            return tex;
        }
        
        private Texture3D Create()
        {
            Texture3D tex = new Texture3D (Size, Size, Size, TextureFormat.ARGB32, true);
            var cols = new Color[Size*Size*Size];
            float mul = 1.0f / (Size-1);
            int idx = 0;
            Color c = Color.white;
            for (int z = 0; z < Size; ++z)
            {
                for (int y = 0; y < Size; ++y)
                {
                    for (int x = 0; x < Size; ++x, ++idx)
                    {
                        c.r = x*mul;
                        c.g = y*mul;
                        c.b = z*mul;
                        cols[idx] = c;
                    }
                }
            }
            tex.SetPixels (cols);
            tex.Apply ();

            return tex;
        }
    }
}