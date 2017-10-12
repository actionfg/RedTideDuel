using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapManager : MonoBehaviour
{
    public const int MinimapLayer = 12;
    public Texture mapTexture;
    public Vector2 mapSize;

    public TrackableTaggedObjectInfo[] TrackableTaggedObjects;

    public Rect mapPositionOnScreen = new Rect(20, 20, 160, 160);
    public bool enableMapRotateOption = false;
    public bool rotateMap = false;
    public bool enableMapClickMove = false;
    public bool enableTrackCamera = true;

    public Transform trackedPlayer;
    public Texture trackPlayerTexture;
    public float VisionRadius = 8f;        // 玩家视野半径
    public float trackPlayerBlipSize = 1;
    [Tooltip("圆点距中心点的距离, 最大为10")]
    public float TrackPlayerBlipBias = 2;
    
    [HideInInspector]
    public float PixelPerWorld;        // 世界距离换算成小地图像素
    [HideInInspector]
    public Vector3 WorldCenter;


    private Transform minimap;
    private Transform mapBg;
    private Transform _mapFog;
    private int _pixelVision;            // 暂定可视范围为10m
    private Transform trackCameraObjectBlip;
    private Shader shader1;

    private Color colour = Color.white;
    private Transform minimapCamTransfrom;
    private Camera minimapCam;
    private Transform camMain;

    private RaycastHit hit;
    private Texture2D _fogTexture;
    private float _nextFogUpdater;

    private void Start()
    {
        shader1 = Shader.Find("Particles/Alpha Blended");

        // 用于绘制小地图的相机
        minimapCamTransfrom = new GameObject("camera_minimap").transform;
        minimapCamTransfrom.gameObject.AddComponent<Camera>();
        minimapCam = minimapCamTransfrom.gameObject.GetComponent("Camera") as Camera;

        minimapCamTransfrom.rotation = Quaternion.Euler(90, 0, 0);
        minimapCam.orthographic = true;
        minimapCam.orthographicSize = Mathf.Max(mapSize.x, mapSize.y) * 0.25f;
        minimapCam.backgroundColor = new Color(0, 0, 0, 0);
        minimapCam.clearFlags = CameraClearFlags.Nothing;
        minimapCam.depth = Camera.main.depth + 1;
        minimapCam.rect = new Rect(mapPositionOnScreen.x / Screen.width,
            (1 - mapPositionOnScreen.y / Screen.height) - mapPositionOnScreen.width / Screen.height,
            mapPositionOnScreen.height / Screen.width, mapPositionOnScreen.width / Screen.height);
        minimapCam.cullingMask = 1 << MinimapLayer;
        minimapCamTransfrom.transform.parent = transform;

        // 地图背景图
        mapBg = GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
        mapBg.transform.position = new Vector3(WorldCenter.x, -100, WorldCenter.z);
        mapBg.transform.rotation = Quaternion.Euler(0, 180, 0);
        // TODO 为什么是0.2? Plane默认大小是10 * 10
        mapBg.transform.localScale = new Vector3(0.2f * mapSize.x, 0, 0.2f * mapSize.y);
        mapBg.GetComponent<Renderer>().material.shader = shader1;
        mapBg.GetComponent<Renderer>().material.mainTexture = mapTexture;
        mapBg.gameObject.layer = MinimapLayer;
        mapBg.gameObject.name = "mapBg";
        mapBg.transform.parent = transform;
        Destroy(mapBg.GetComponent<Collider>());
        
        //加入战争迷雾
        _mapFog = GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
        _mapFog.transform.position = new Vector3(WorldCenter.x, -90, WorldCenter.z);
        _mapFog.transform.rotation = Quaternion.Euler(0, 180, 0);
        _mapFog.transform.localScale = new Vector3(0.2f * mapSize.x, 0, 0.2f * mapSize.y);
        _mapFog.GetComponent<Renderer>().material.shader = shader1;
        
        _fogTexture = new Texture2D(mapTexture.width, mapTexture.height, TextureFormat.RGBA32, false);
        Color[] blackPixels = new Color[mapTexture.width * mapTexture.height];
        for (int i = 0; i < blackPixels.Length; i++)
        {
            blackPixels[i] = Color.black;
        }
        _fogTexture.SetPixels(blackPixels);
        _fogTexture.Apply(false);
        _mapFog.GetComponent<Renderer>().material.mainTexture = _fogTexture;

        _mapFog.gameObject.layer = MinimapLayer;
        _mapFog.gameObject.name = "mapFog";
        _mapFog.transform.parent = transform;
        Destroy(_mapFog.GetComponent<Collider>());        

        camMain = Camera.main.transform.parent;
        
//        var camMainCom = Camera.main.gameObject.GetComponent("Camera") as Camera;
//        camMainCom.cullingMask = ~(1 << MinimapLayer);
//        if (enableTrackCamera)
//        {
//            // 绘制代表Player的图标
//            if (trackedPlayer == null) 
//                trackedPlayer = UnitManager.Instance.currentUnit.transform;
//            trackCameraObjectBlip = new GameObject("PlayerBlipP").transform;
//            trackCameraObjectBlip.transform.localScale = new Vector3(trackPlayerBlipSize, 0, trackPlayerBlipSize);
//            trackCameraObjectBlip.parent = transform;
//            
//            var playerBlipBg = GameObject.CreatePrimitive(PrimitiveType.Plane).transform;
//            playerBlipBg.parent = trackCameraObjectBlip;
//            playerBlipBg.localPosition = new Vector3(0, 0, TrackPlayerBlipBias);
//            playerBlipBg.GetComponent<Renderer>().material.shader = shader1;
//            playerBlipBg.GetComponent<Renderer>().material.mainTexture = trackPlayerTexture;
//            playerBlipBg.gameObject.layer = MinimapLayer;
//            playerBlipBg.gameObject.name = "PlayerBlipBg";
//            Destroy(playerBlipBg.GetComponent<Collider>());
//        }

        if (!enableMapRotateOption) rotateMap = false;
    }

    private void OnGUI()
    {
        if (GUI.Button(
            new Rect((mapPositionOnScreen.x + mapPositionOnScreen.width),
                (mapPositionOnScreen.x + mapPositionOnScreen.width) - 90, 25, 25), new GUIContent("+", "Zoom in")))
        {
            if (minimapCam.orthographicSize > 10) minimapCam.orthographicSize -= 5;
        }
        if (GUI.Button(
            new Rect((mapPositionOnScreen.x + mapPositionOnScreen.width),
                (mapPositionOnScreen.x + mapPositionOnScreen.width) - 60, 25, 25), new GUIContent("-", "Zoom out")))
        {
            if (minimapCam.orthographicSize < Mathf.Max(mapSize.x, mapSize.y) / 2) minimapCam.orthographicSize += 5;
        }

        GUI.Label(
            new Rect(mapPositionOnScreen.x + 5, (mapPositionOnScreen.x + mapPositionOnScreen.width) - 25, 100, 25),
            GUI.tooltip);
    }

    private void Update()
    {
//        if (trackedPlayer == null)
//        {
//            trackedPlayer = UnitManager.Instance.currentUnit.transform;
//        }
        if (enableTrackCamera && trackedPlayer != null)
        {
            trackCameraObjectBlip.position = trackedPlayer.position + new Vector3(0, 10, 0);
            trackCameraObjectBlip.rotation = Quaternion.Euler(0, trackedPlayer.rotation.eulerAngles.y, 0);

            minimapCamTransfrom.position = trackedPlayer.position + new Vector3(0, 500, 0);
            if (rotateMap)
            {
                minimapCamTransfrom.rotation = Quaternion.Euler(minimapCamTransfrom.rotation.eulerAngles.x, trackedPlayer.rotation.eulerAngles.y, minimapCamTransfrom.rotation.eulerAngles.z);
            }
            else
            {
                minimapCamTransfrom.rotation = Quaternion.Euler(minimapCamTransfrom.rotation.eulerAngles.x, 0, minimapCamTransfrom.rotation.eulerAngles.z);
            }
            
            // 每0.2s更新一次BattleFog
            if (Time.time > _nextFogUpdater)
            {
                Vector2 currentPixel = new Vector2(mapTexture.width / 2 + (trackedPlayer.position.x - WorldCenter.x ) * PixelPerWorld, 
                    mapTexture.height / 2 + (trackedPlayer.position.z - WorldCenter.z) * PixelPerWorld);
                for (int i = -_pixelVision ; i < _pixelVision; i++)
                {
                    for (int j = -_pixelVision; j < _pixelVision; j++)
                    {
                        if (Math.Abs(i) * Math.Abs(i) + Math.Abs(j) * Math.Abs(j) <= _pixelVision * _pixelVision)
                        {// 圆形范围
                            int pX = Mathf.Clamp((int) currentPixel.x + i, 0, mapTexture.width);
                            int pY = Mathf.Clamp((int)currentPixel.y + j, 0, mapTexture.height);
                            _fogTexture.SetPixel(pX, pY, new Color(0, 0, 0, 0));
                        }
                    }
                }            
                _fogTexture.Apply(false);
                _nextFogUpdater = Time.time + 0.2f;
            }
        }

        ScanTaggedTrackable();

        DrawTaggedTrackable();
    }

    private void ScanTaggedTrackable()
    {
        if (TrackableTaggedObjects == null)
        {
             return;
        }
        
        foreach (TrackableTaggedObjectInfo t in TrackableTaggedObjects)
        {
            TrackableObject tempTrackable;
            int i;
            var list = t.GetObjectList();

            if (list.Count > 0)
            {
                for (i = 0; i < list.Count; i++)
                {
                    tempTrackable = list[i];
                    if (tempTrackable.obj == null)
                    {
                        Destroy(tempTrackable.blip.gameObject);
                        list.RemoveAt(i);
                    }
                }
            }

            GameObject[] objects = GameObject.FindGameObjectsWithTag(t.tagName);
            foreach (var o in objects)
            {
                bool match = false;
                for (i = 0; i < list.Count; i++)
                {
                    tempTrackable = list[i];
                    if (tempTrackable.obj == o)
                    {
                        match = true;
                        break;
                    }
                }

                if (!match)
                {
                    Transform objectBlip = GameObject.CreatePrimitive(PrimitiveType.Plane).transform;

                    float scaleSize = t.objectBlipSize;
                    if (Math.Abs(scaleSize) < 0.001f) objectBlip.localScale = o.transform.localScale * 0.1f;
                    else
                        objectBlip.localScale = new Vector3(t.objectBlipSize, 0,
                            t.objectBlipSize);
                    objectBlip.transform.GetComponent<Renderer>().material.mainTexture =
                        t.objectBlipTexture;
                    objectBlip.transform.GetComponent<Renderer>().material.shader = shader1;
                    objectBlip.transform.GetComponent<Renderer>().material.color = colour;
                    objectBlip.gameObject.layer = MinimapLayer;

                    objectBlip.gameObject.name = t.tagName;
                    objectBlip.transform.parent = transform;


                    objectBlip.position = o.transform.position;
                    objectBlip.rotation = o.transform.rotation;

                    Destroy(objectBlip.GetComponent<Collider>());

                    list.Add(new TrackableObject(o, objectBlip));
                }
            }
            t.SetObjectList(list);
        }
    }

    private void DrawTaggedTrackable()
    {
        if (TrackableTaggedObjects == null)
        {
            return;
        }
        
        for (var n = 0; n < TrackableTaggedObjects.Length; n++)
        {
            if (TrackableTaggedObjects[n].trackInRuntime)
            {
                var list = TrackableTaggedObjects[n].GetObjectList();

                if (list.Count > 0)
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        var tempTrackable = list[i];
                        if (tempTrackable.obj != null)
                        {
                            tempTrackable.blip.position = tempTrackable.obj.transform.position;
                            tempTrackable.blip.rotation = tempTrackable.obj.transform.rotation;
                        }
                    }
                }
            }
        }
    }

    public void Reset()
    {
        // 修改小地图贴图, 中心位置, 大小
        if (mapBg)
        {
            mapBg.GetComponent<Renderer>().material.mainTexture = mapTexture;
            mapBg.transform.position = new Vector3(WorldCenter.x, -100, WorldCenter.z);
            mapBg.transform.localScale = new Vector3(0.2f * mapSize.x, 0, 0.2f * mapSize.y);

            // 重置战争迷雾
            _mapFog.transform.position = new Vector3(WorldCenter.x, -90, WorldCenter.z);
            _mapFog.transform.localScale = new Vector3(0.2f * mapSize.x, 0, 0.2f * mapSize.y);
            Color[] blackPixels = new Color[mapTexture.width * mapTexture.height];
            for (int i = 0; i < blackPixels.Length; i++)
            {
                blackPixels[i] = Color.black;
            }
            _fogTexture.SetPixels(blackPixels);
            _fogTexture.Apply(false);
        }

        _pixelVision =(int)(PixelPerWorld * VisionRadius);
    }
}