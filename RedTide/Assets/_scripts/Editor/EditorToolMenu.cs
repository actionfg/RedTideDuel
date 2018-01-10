using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorToolMenu
{
    [MenuItem("Tools/CombineTo3D")]
    private static void CreateRedBlueGameObject()
    {
        // 需先16张法线图
//            TextureUtil.SplitTexture("textures/splashes/SBumpNormal", 256, 256);

        // 将16张法线图合并成3D 贴图
//        var tex3D = new Texture3D(256, 256, 16, TextureFormat.ARGB32, true);
//        var colors = new Color[256 * 256 * 16];
//        Color c = Color.white;
//        int idx = 0;
//        for (int i = 0; i < 16; i++) // depth
//        {
//            var path = "SBump-" + i;
//            var tex2D = Resources.Load<Texture2D>(path);
//            if (tex2D)
//            {
//                var pixels = tex2D.GetPixels();
//                for (int x = 0; x < 256 * 256; x++, ++idx)
//                {
//                    c = pixels[x];
//                    colors[idx] = c;
//                    if (i < 1)
//                    {
//                        Debug.Log("pixel: " + c);
//                    }
//                }
//            }
//            else
//            {
//                Debug.LogWarning("cannot load tex from: " + path);
//                break;
//            }
//        }
//
//        tex3D.SetPixels(colors);
//        tex3D.Apply();
//        
//        AssetDatabase.CreateAsset(tex3D, "Assets/_prefabs/SBumpNormal3D.asset");
//        Debug.Log("Create SBumpNormal3D Success!");
    }
}