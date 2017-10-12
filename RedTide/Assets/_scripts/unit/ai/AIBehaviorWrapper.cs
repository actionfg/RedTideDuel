using UnityEngine;
using System.Collections;

public class AIBehaviorWrapper {

    private readonly AIBehavior behavior;

    private int priority;
    private float weights;
//    private AIActivity activeRelation;
    private AISituationConfig situationConfig;

    public AIBehaviorWrapper(AIBehavior behavior, int priority, float weights, AISituationConfig situationConfig) {
        this.behavior = behavior;
        this.priority = priority;
        this.weights = weights;
        this.situationConfig = situationConfig;
    }

    public AIBehavior getBehavior() {
        return behavior;
    }

    public int getPriority() {
        return priority;
    }

    public void setPriority(int priority) {
        this.priority = priority;
    }

    public float getWeights() {
        return weights;
    }

    protected float getWeights(float activeFactor) {
        return weights;
    }

    public void setWeights(float weights) {
        this.weights = weights;
    }

//    public void setActiveRelation(AIActivity activeRelation) {
//        this.activeRelation = activeRelation;
//    }

    public AISituationConfig getSituationConfig() {
        return situationConfig;
    }
}
