using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

#if UNITY_EDITOR

public class AnimationOverrideCreator : MonoBehaviour
{
    [MenuItem("Utilities/Animation Override Creator")]
    static void SliceSprites()
    {
        var obj = Selection.activeObject;
        if (obj == null)
        {
            Debug.LogError("No object selected");
            return;
        }

        if (obj.GetType() != typeof(AnimatorOverrideController))
        {
            Debug.LogError("Select animatior override controller");
            return;
        }

        AnimatorOverrideController overrideController = obj as AnimatorOverrideController;
        if (overrideController == null)
        {
            Debug.LogError("Select animatior override controller");
            return;
        }

        Debug.Log(overrideController.name);
        // get all clips in folder
        string path = AssetDatabase.GetAssetPath(overrideController);
        Debug.Log(path);
        string folderPath = Path.GetDirectoryName(path);
        Debug.Log(folderPath);
        string[] guids = AssetDatabase.FindAssets("t:AnimationClip", new string[] { folderPath });
        List<AnimationClip> targetClips = new List<AnimationClip>();
        foreach (string guid in guids)
        {
            string clipPath = AssetDatabase.GUIDToAssetPath(guid);
            AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
            targetClips.Add(clip);
        }

        for (int i = 0; i < overrideController.animationClips.Length; i++)
        {
            AnimationClip clip = overrideController.animationClips[i];
            if (clip.name.Contains("Attack"))
            {
                AnimationClip newClip = GetClipWithSubstringInName("Attack", targetClips);
                if (newClip != null)
                    overrideController[clip.name] = newClip;
            }

            if (clip.name.Contains("Celebrate"))
            {
                AnimationClip newClip = GetClipWithSubstringInName("Stunned", targetClips);
                if (newClip != null)
                    overrideController[clip.name] = newClip;
            }

            if (clip.name.Contains("Die"))
            {
                AnimationClip newClip = GetClipWithSubstringInName("Die", targetClips);
                if (newClip != null)
                    overrideController[clip.name] = newClip;
            }

            if (clip.name.Contains("Idle"))
            {
                AnimationClip newClip = GetClipWithSubstringInName("Idle", targetClips);
                if (newClip != null)
                    overrideController[clip.name] = newClip;
            }

            if (clip.name.Contains("Move"))
            {
                AnimationClip newClip = GetClipWithSubstringInName("Walk", targetClips);
                if (newClip != null)
                    overrideController[clip.name] = newClip;
            }

            if (clip.name.Contains("Spawn"))
            {
                AnimationClip newClip = GetClipWithSubstringInName("GetUp", targetClips);
                if (newClip != null)
                    overrideController[clip.name] = newClip;
            }

            if (clip.name.Contains("Special"))
            {
                AnimationClip newClip = GetClipWithSubstringInName("Jump", targetClips);
                if (newClip != null)
                    overrideController[clip.name] = newClip;
            }

            if (clip.name.Contains("Take"))
            {
                AnimationClip newClip = GetClipWithSubstringInName("Damage", targetClips);
                if (newClip != null)
                    overrideController[clip.name] = newClip;
            }
        }
    }

    static AnimationClip GetClipWithSubstringInName(string subString, List<AnimationClip> clips)
    {
        foreach (AnimationClip clip in clips)
            if (clip.name.Contains(subString))
                return clip;

        return null;
    }
}
#endif