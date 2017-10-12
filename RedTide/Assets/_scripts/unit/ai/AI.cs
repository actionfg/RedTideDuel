using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

public abstract class AI : MonoBehaviour
{

    private HashSet<AISituation> activeSituations;

    private LinkedList<AIBehaviorWrapper> availableBehaviors;
    private AIBehaviorWrapper currentBehavior;
    private AIBehaviorWrapper defaultBehavior;      // 主要作用防止发呆情况出现

    private LinkedList<AIBehaviorWrapper> situationsFilterResBuffer;
    private LinkedList<AIBehaviorWrapper> tempBuffer;

    // Use this for initialization
	protected virtual void Start () {
	    InitAI();
	}
	
	// Update is called once per frame
	void Update () {
	    if (activeSituations == null) {
	        InitAI();
	    }
//	    Debug.Log("mob ai update: " + activeSituations.Count);
	    updateSituations(Time.deltaTime);
	    // 新状态触发的新AI行为,需等待当前AI行为结束,或者当前AI行为状态不满足,才进行切换
	    // 修改成每次都重新计算当前AI行为, 而不管以前行为是否做完
	    switchAIBehavior();


	    updateAIBehavior(Time.deltaTime);

	}

    private void updateAIBehavior(float tpf) {
        if (currentBehavior != null) {
            if (!currentBehavior.getBehavior().doBehavior(tpf)) {
                currentBehavior = defaultBehavior;
            }
        }
    }

    protected void InitAI() {
        activeSituations = new HashSet<AISituation>();
        availableBehaviors = new LinkedList<AIBehaviorWrapper>();
        situationsFilterResBuffer = new LinkedList<AIBehaviorWrapper>();
        tempBuffer = new LinkedList<AIBehaviorWrapper>();
    }

    /**
     * 获取此AI所参考的所有情形
     *
     * @return 此AI所参考的所有情形
     */
    protected HashSet<AISituation> GetActiveSituations()
    {
        return activeSituations;
    }

    protected void AddAISituation(AISituation situation)
    {
        activeSituations.Add(situation);
    }

    private void updateSituations(float tpf) {
        foreach (AISituation activeSituation in activeSituations) {
            activeSituation.update(tpf);
        }
    }

    protected bool switchAIBehavior() {

        AIBehaviorWrapper newBehavior = getAIDecision();
        if (newBehavior != currentBehavior) {
            if (currentBehavior != null) {
                currentBehavior.getBehavior().deactivate(this);
            }

            currentBehavior = newBehavior;
            if (currentBehavior == null) {
                currentBehavior = defaultBehavior;
            }
            if (currentBehavior != null) {
                currentBehavior.getBehavior().activate(this);
            }
            return true;
        }
        return false;
    }

    private AIBehaviorWrapper getAIDecision() {
        return getOptimalBehavior(situationsFilter(availableBehaviors));
    }

    /**
     * 从所有已选出的满足当前情形的AI行为中根据权重和优先级等计算出最终的决策行为
     * 其中优先计算权重，然后取所有同最高优先级的行为按权重计算出最终结果
     *
     * @param aiBehaviorWrapper 已选出的满足当前情形的所有AI行为
     * @return 最终的决策行为
     */
    private AIBehaviorWrapper getOptimalBehavior(LinkedList<AIBehaviorWrapper> aiBehaviorWrapper) {
        AIBehaviorWrapper currentData = null;
        tempBuffer.Clear();
        lock (aiBehaviorWrapper) {
            foreach (AIBehaviorWrapper behaviorData in aiBehaviorWrapper) {
                if (currentData == null) {
                    currentData = behaviorData;
                    tempBuffer.AddLast(currentData);
                }
                else {
                    if (behaviorData.getPriority() > currentData.getPriority()) {
                        currentData = behaviorData;
                        tempBuffer.Clear();
                        tempBuffer.AddLast(currentData);
                    }
                    else if (behaviorData.getPriority() == currentData.getPriority()) {
                        tempBuffer.AddLast(behaviorData);
                    }
                }
            }
        }
        if (tempBuffer.Count > 1) {
            float totalWeights = 0;
            lock (tempBuffer) {
                foreach (AIBehaviorWrapper behaviorData in tempBuffer) {
                    totalWeights += behaviorData.getWeights();
                }
                double randomWeights = Random.value * totalWeights;
                totalWeights = 0;
                foreach (AIBehaviorWrapper behaviorData in tempBuffer) {
                    totalWeights += behaviorData.getWeights();
                    if (randomWeights < totalWeights) {
                        currentData = behaviorData;
                        break;
                    }
                }
            }
        }
        return currentData;
    }

    /**
     * 根据当前情形筛选出所有满足执行条件的行为
     *
     * @param availableBehaviors 所有的此AI已绑定的行为
     * @return 所有满足执行条件的行为
     */
    private LinkedList<AIBehaviorWrapper> situationsFilter(LinkedList<AIBehaviorWrapper> availableBehaviors) {
        situationsFilterResBuffer.Clear();
        foreach (AIBehaviorWrapper wrapper in availableBehaviors) {
            if (checkSituations(wrapper.getSituationConfig())) {
                situationsFilterResBuffer.AddLast(wrapper);
            }
        }
        return situationsFilterResBuffer;
    }

    private bool checkSituations(AISituationConfig situationConfig) {
        bool isAvailable = true;
        foreach (AISituation activeSituation in activeSituations) {
            if (!situationConfig.check(activeSituation)) {
                isAvailable = false;
                break;
            }
        }
        return isAvailable;
    }

    /**
     * 将一个AI行为根据绑定和配置信息添加到AI系统中
     *
     * @param behavior        被添加的行为
     * @param priority        优先级，当数个行为同时满足当前情形时只有优先级最高的行为才会经过权重计算作为决策给出，其他低优先级者会被忽略
     * @param weights         权重,所有满足当前情形的最高优先级行为会根据此权重来按概率计算出最终的决策行为，越高(相对于此次计算的其他行为)其可能被选中的概率越大
     * @param activity        主动性倾向，此行为在AI主动性发生改变时会由此导致其权重发生改变(不影响优先级)See:{@link AIActivity}.
     * @param situationConfig 绑定的情形，See{@link AISituationConfig}
     */
    public void AddAIBehavior(AIBehavior behavior, int priority, float weights, AISituationConfig situationConfig) {
        if (availableBehaviors == null) {
            InitAI();
        }
        availableBehaviors.AddLast(new AIBehaviorWrapper(behavior, priority, weights, situationConfig));
    }

    public AIBehavior GetCurrentBehavior()
    {
        if (currentBehavior != null)
        {
            return currentBehavior.getBehavior();
        }
        return null;
    }
}
