using System.Collections.Generic;

public class SkillStageControl
{
    public Dictionary<HitTriggerType, SkillStage> stages = new Dictionary<HitTriggerType, SkillStage>();

    public void Reset()
    {
        stages.Clear();
    }

    public void NewStage(HitTriggerType colliderType)
    {
        SkillStage stage;
        int stageId = 0;
        if (stages.TryGetValue(colliderType, out stage))
        {
            stageId = stage.stageId + 1;
        }

        stages[colliderType] = new SkillStage(stageId);
    }
}