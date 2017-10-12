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
        _aiPath.EnableTrace(false);
        return false;
    }

    public void deactivate(AI ai)
    {
    }
}
