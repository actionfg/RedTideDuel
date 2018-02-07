using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;

[CustomEditor(typeof(SkillConfig), true)]
public class SkillInspector : UnityEditor.Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);
        EditorGUI.indentLevel = 1;

        int toDeleteIndex = -1;
        GUILayout.BeginVertical("box");

        SkillConfig config = target as SkillConfig;
        List<RawEffectConfig> effects = config.rawEffects;

        for (int i = 0; i < effects.Count; i++)
        {
            RawEffectConfig effect = effects[i];
            GUILayout.BeginVertical("box");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(18)))
                toDeleteIndex = i;

            if (effect != null)
            {
                effect.name = EditorGUILayout.TextField("Name: ", effect.name);
                EditorGUILayout.EndHorizontal();

                effect.triggerType = (HitTriggerType) EditorGUILayout.EnumPopup("Hit Trigger: ", effect.triggerType);
                effect.triggerStage = EditorGUILayout.IntField("Trigger Stage: ", effect.triggerStage);
                effect.effectObject = EditorGUILayout.ObjectField("Effect Object", effect.effectObject, typeof(EffectObject), false) as EffectObject;
            }
            else
            {
                EditorGUILayout.LabelField("Effect Type: UNKNOWN");
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        if (toDeleteIndex >= 0)
        {
            effects.RemoveAt(toDeleteIndex);
        }

        GUILayout.BeginVertical("box");

        if (GUILayout.Button("Add New Effect"))
        {
            RawEffectConfig rawEffectConfig = new RawEffectConfig();
            effects.Add(rawEffectConfig);
        }
        GUILayout.EndVertical();

        EditorGUILayout.EndVertical();

        if (GUI.changed)
            EditorUtility.SetDirty(config);
    }
}
