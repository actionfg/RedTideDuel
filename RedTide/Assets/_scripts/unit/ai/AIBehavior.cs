using UnityEngine;
using System.Collections;

public interface AIBehavior {

    /**
     * 此行为在被启用时调用
     *
     * @param ai 调用此行为的AI
     */
    void activate(AI ai);

    /**
     * 执行此行为实际代码
     *
     * @param tpf 每次更新的时间间隔
     * @return 返回false表示主动行为结束
     */
    bool doBehavior(float tpf);

    /**
     * 此行为在被禁用时调用
     * @param ai
     */
    void deactivate(AI ai);
}
