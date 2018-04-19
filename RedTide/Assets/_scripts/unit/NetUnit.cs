using UnityEngine;
using UnityEngine.Networking;
using _scripts.unit.ai;

namespace _scripts.unit
{
    // 用于同步一些RemoteClient需要知道的关于MobUnit的信息
    public class NetUnit : NetworkBehaviour
    {
        [SyncVar]
        public int ConfigIndex;

        [SyncVar] public int PlayerId;
        [SyncVar]
        public float CurrentHp;

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            writer.Write(name);
            writer.Write(ConfigIndex);
            writer.Write(PlayerId);
            return base.OnSerialize(writer, initialState);
            
        }

        // 用于生成一些未在Prefab中Register的组件
        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            name = reader.ReadString();
            Debug.Log(name + " on deserialize");
            base.OnDeserialize(reader, initialState);

            ConfigIndex = reader.ReadInt32();
            PlayerId = reader.ReadInt32();
//            if (!isServer)
            if (GetComponent<MobUnit>() == null)
            {
                MobUnit mobUnit = gameObject.AddComponent<MobUnit>();
                mobUnit.Init(GameDuel.NetworkGameManager.sInstance.MobConfigs[ConfigIndex], GameContext.mobLevel);
                mobUnit.PlayerId = PlayerId;

                var childObj = new GameObject("NearbyEnemy");
                var sphereCollider = childObj.AddComponent<SphereCollider>();
                // TODO 实装警戒距离
                sphereCollider.radius = UnitManager.ACTIVE_RANGE;
                sphereCollider.isTrigger = true;
                childObj.transform.SetParent(gameObject.transform);
                childObj.transform.localPosition = Vector3.zero;
                childObj.transform.localRotation = Quaternion.identity;
                childObj.layer = LayerMask.NameToLayer("Ignore Raycast");
                // 加入对周边地方单位的收集
                childObj.AddComponent<NearbyEnemys>();
            }

        }
    }
}