﻿using System.Collections;
using System.Collections.Generic;
using Prototype.NetworkLobby;
using UnityEngine;
using UnityEngine.Networking;

namespace GameDuel
{
    // 负责游戏状态的切换: 内政和战斗
    public class NetworkGameManager : NetworkBehaviour
    {
        static public List<NetworkPlayer> sPlayers = new List<NetworkPlayer>();
        static public NetworkGameManager sInstance = null;

        public GameObject uiScoreZone;
        public Font uiScoreFont;
    
        [Header("Gameplay")]
        //Those are sorte dby level 0 == lowest etc...
        public MobConfig[] MobConfigs;

        [Space]

        protected bool _spawningAsteroid = true;
        protected bool _running = true;

        void Awake()
        {
            sInstance = this;
        }

        void Start()
        {
            if (isServer)
            {
                // TODO 提示内政时间倒计时
//                StartCoroutine(AsteroidCoroutine());
            }

            for(int i = 0; i < sPlayers.Count; ++i)
            {
                sPlayers[i].Init();
            }
        }

        [ServerCallback]
        void Update()
        {
            if (!_running || sPlayers.Count == 0)
                return;

            bool allDestroyed = true;
            for (int i = 0; i < sPlayers.Count; ++i)
            {
                allDestroyed &= (sPlayers[i].lifeCount == 0);
            }

            if(allDestroyed)
            {
                StartCoroutine(ReturnToLoby());
            }
        }

        IEnumerator ReturnToLoby()
        {
            _running = false;
            yield return new WaitForSeconds(3.0f);
            LobbyManager.s_Singleton.ServerReturnToLobby();
        }

//        IEnumerator AsteroidCoroutine()
//        {
//            const float MIN_TIME = 5.0f;
//            const float MAX_TIME = 10.0f;
//
//            while(_spawningAsteroid)
//            {
//                yield return new WaitForSeconds(Random.Range(MIN_TIME, MAX_TIME));
//
//                Vector2 dir = Random.insideUnitCircle;
//                Vector3 position = Vector3.zero;
//
//                if(Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
//                {//make it appear on the side
//                    position = new Vector3( Mathf.Sign(dir.x)* Camera.main.orthographicSize * Camera.main.aspect, 
//                        0, 
//                        dir.y * Camera.main.orthographicSize);
//                }
//                else
//                {//make it appear on the top/bottom
//                    position = new Vector3(dir.x * Camera.main.orthographicSize * Camera.main.aspect, 
//                        0,
//                        Mathf.Sign(dir.y) * Camera.main.orthographicSize);
//                }
//
//                //offset slightly so we are not out of screen at creation time (as it would destroy the asteroid right away)
//                position -= position.normalized * 0.1f;
//            
//
//                GameObject ast = Instantiate(mobPrefabs[mobPrefabs.Length - 1], position, Quaternion.Euler(Random.value * 360.0f, Random.value * 360.0f, Random.value * 360.0f)) as GameObject;
//
//                NetworkAsteroid asteroid = ast.GetComponent<NetworkAsteroid>();
//                asteroid.SetupStartParameters(-position.normalized * 1000.0f, Random.insideUnitSphere * Random.Range(500.0f, 1500.0f));
//
//                NetworkServer.Spawn(ast);
//            }
//        }
        
    }
}