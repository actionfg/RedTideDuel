using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

public abstract class BaseAISituation : AISituation  {

    private GameUnit owner;
    private int currentStatusIndex;
    private float acc; // Accumulated time

    public BaseAISituation(GameUnit owner) {
        this.owner = owner;
    }

    public int getCurrentStatus()
    {
        return currentStatusIndex;
    }

    public void update(float tpf) {
        acc += tpf;
        if (shouldUpdate(acc)) {
            int status = updateSituation(acc);
            acc = 0;
            setStatus(status);
        }
    }

    public virtual void setStatus(int status)
    {
        currentStatusIndex = status;
    }

    protected abstract bool shouldUpdate(float acc);

    /**
     * 更新此情形的状态
     *
     * @param tpf 更新的时间间隔
     * @return 情形所处的当前状态
     */
    protected abstract int updateSituation(float tpf);

    public GameUnit GetOwner() {
        return owner;
    }
}
