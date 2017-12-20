using UnityEditor;
using UnityEngine;

namespace Game04.Util
{
    public class Test3DTex : MonoBehaviour
    {
        public int Size = 16;
        
        private void Start()
        {
//            Texture3D tex = Create();
//            AssetDatabase.CreateAsset(tex, "Assets/testTex3D.asset");

            // TODO Unity加载Dxt5 3D图片格式失败
            Texture3D tex = Resources.Load("textures/splashes/SBumpVolume") as Texture3D;

            if (tex)
            {
                Debug.Log("width: " + tex.width + " height: " + tex.height + " ,depth: " + tex.GetPixels().Length);
            }
            GetComponent<Renderer>().material.SetTexture("_SplashBumpTex", tex);
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