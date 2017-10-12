using System.Collections.Generic;
using UnityEngine;

public class SkillStage
{
    public int stageId = 0;
    public HashSet<GameObject> hitSet = new HashSet<GameObject>();

    public SkillStage(int stageId)
    {
        this.stageId = stageId;
    }
}