using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

public class AISituationConfig {

    private Dictionary<AISituation, HashSet<object>> configData = new Dictionary<AISituation, HashSet<object>>();

    /**
     * 判断目标情形是否为此行为绑定所允许。
     *
     * @param situation 需要检查的目标情形
     * @return 目标情形的当前状态是否与绑定相容
     */
    public bool check(AISituation situation)
    {
        HashSet<object> set;
        if (configData.TryGetValue(situation, out set)) {
            return set.Contains(situation.getCurrentStatus());
        }
        else {
            return true;
        }
    }

    /**
     * 将一个情形注册到配置中
     * 注意：AI中包含而未在此方法中注册的其他情形视为与此绑定行为无关，即无论这些情形为什么状态，都视为此绑定行为可以执行
     *  @param situation 需要绑定的情形
     * @param statuses  可以执行绑定行为的所有状态，即绑定的情形之有处于这里被指定过的状态中的一种时，此行为才视为可以被执行
     */
    public AISituationConfig addSituationCondition(AISituation situation, params int[] statuses) {

        var statusSet = new HashSet<object>();
        foreach (var state in statuses)
        {
            statusSet.Add(state);
        }
        configData[situation] = statusSet;
        return this;
    }
}