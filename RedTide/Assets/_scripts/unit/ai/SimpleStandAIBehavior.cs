using UnityEngine;
using System.Collections;

public class SimpleStandAIBehavior : AIBehavior {
    private readonly MobUnit _mobUit;
    private MobAIPath _aiPath;

    public SimpleStandAIBehavior(MobUnit mobUit)
    {
        _mobUit = mobUit;
        _aiPath = _mobUit.GetComponent<MobAIPath>();
    }

    public void activate(AI ai)
    {

    }

    public bool doBehavior(float tpf)
    {
        // 暂不清除移动目的地,以免覆盖掉准备阶段的移动指令    
//        _aiPath.EnableTrace(false);
        return false;
    }

    public void deactivate(AI ai)
    {
    }
}
