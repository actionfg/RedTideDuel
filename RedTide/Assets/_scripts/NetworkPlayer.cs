using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace GameDuel
{
    // 玩家自身, 负责处理玩家一切操作, 如组织并排列队形, 释放卡牌技能等 
    public class NetworkPlayer : NetworkBehaviour
    {

        //Network syncvar
        [SyncVar(hook = "OnScoreChanged")]
        public int score;
        [SyncVar]
        public Color color;
        [SyncVar]
        public string playerName;
        [SyncVar(hook = "OnLifeChanged")]
        public int lifeCount;

        protected Text _scoreText;


        //hard to control WHEN Init is called (networking make order between object spawning non deterministic)
        //so we call init from multiple location (depending on what between spaceship & manager is created first).
        protected bool _wasInit = false;
        private bool _canControl = true;

        void Awake()
        {
            //register the spaceship in the gamemanager, that will allow to loop on it.
            NetworkGameManager.sPlayers.Add(this);
        }

        void Start()
        {
            if (NetworkGameManager.sInstance != null)
            {//we MAY be awake late (see comment on _wasInit above), so if the instance is already there we init
                Init();
            }
        }

        public void Init()
        {
            if (_wasInit)
                return;

            GameObject scoreGO = new GameObject(playerName + "score");
            scoreGO.transform.SetParent(NetworkGameManager.sInstance.uiScoreZone.transform, false);
            _scoreText = scoreGO.AddComponent<Text>();
            _scoreText.alignment = TextAnchor.MiddleCenter;
            _scoreText.font = NetworkGameManager.sInstance.uiScoreFont;
            _scoreText.resizeTextForBestFit = true;
            _scoreText.color = color;
            _wasInit = true;

            UpdateScoreLifeText();
        }

        void OnDestroy()
        {
            NetworkGameManager.sPlayers.Remove(this);
        }

        [ClientCallback]
        void Update()
        {
            if (!isLocalPlayer)
                return;

            if(Input.GetButton("Jump") )
            {
                // TODO 此处可释放当前卡牌技能等 
                //we call a Command, that will be executed only on server, to spawn a new bullet
//                CmdFire(transform.position, transform.forward, _rigidbody.velocity);
            }

        }


        [ClientCallback]
        void FixedUpdate()
        {
            if (!hasAuthority)
                return;
            
        }

        // --- Score & Life management & display
        void OnScoreChanged(int newValue)
        {
            score = newValue;
            UpdateScoreLifeText();
        }

        void OnLifeChanged(int newValue)
        {
            lifeCount = newValue;
            UpdateScoreLifeText();
        }

        void UpdateScoreLifeText()
        {
            if (_scoreText != null)
            {
                _scoreText.text = playerName + "\nSCORE : " + score + "\nLIFE : ";
                for (int i = 1; i <= lifeCount; ++i)
                    _scoreText.text += "X";
            }
        }

        //===================================

        public void EnablePlayer(bool enable)
        {
            _canControl = enable;
        }

        [Client]
        public void LocalDestroy()
        {
            if (!_canControl)
                return;//already destroyed, happen if destroyed Locally, Rpc will call that later

            EnablePlayer(false);
        }

        //this tell the game this should ONLY be called on server, will ignore call on client & produce a warning
        [Server]
        public void Kill()
        {
            lifeCount -= 1;

            RpcDestroyed();
            EnablePlayer(false);

            if (lifeCount > 0)
            {
                // TODO 总基地生命值大于0, 进入下一回合
            }
        }

        [Server]
        public void Respawn()
        {
            EnablePlayer(true);
            RpcRespawn();
        }

        // =========== NETWORK FUNCTIONS

        [Command]
        public void CmdFire(Vector3 position, Vector3 forward, Vector3 startingVelocity)
        {
            if (!isClient)
            {    //avoid to create bullet twice (here & in Rpc call) on hosting client
                // TODO 释放技能, 小兵升级等
            } 

            RpcFire();
        }

        //
        [Command]
        public void CmdCollideAsteroid()
        {
            Kill();
        }

        [ClientRpc]
        public void RpcFire()
        {
            
        }


        //called on client when the player die, spawn the particle (this is only cosmetic, no need to do it on server)
        [ClientRpc]
        void RpcDestroyed()
        {
            LocalDestroy();
        }

        [ClientRpc]
        void RpcRespawn()
        {
            EnablePlayer(true);

        }
    }
}