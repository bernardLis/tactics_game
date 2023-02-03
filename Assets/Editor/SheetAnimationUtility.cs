using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR

public class SheetAnimationUtility : MonoBehaviour
{
    [MenuItem("Utilities/SheetAnimation/SliceSprites")]
    static void SliceSprites()
    {
        // Change the below for the with and height dimensions of each sprite within the spritesheets
        int sliceWidth = 64;
        int sliceHeight = 64;

        // Change the below for the path to the folder containing the sprite sheets (warning: not tested on folders containing anything other than just spritesheets!)
        // Ensure the folder is within 'Assets/Resources/' (the below example folder's full path within the project is 'Assets/Resources/ToSlice')
        string folderPath = "ToAnimate";

        Object[] spriteSheets = Resources.LoadAll(folderPath, typeof(Texture2D));

        for (int z = 0; z < spriteSheets.Length; z++)
        {
            string path = AssetDatabase.GetAssetPath(spriteSheets[z]);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = true;
            ti.spriteImportMode = SpriteImportMode.Multiple;

            List<SpriteMetaData> newData = new List<SpriteMetaData>();

            Texture2D spriteSheet = spriteSheets[z] as Texture2D;

            for (int i = 0; i < spriteSheet.width; i += sliceWidth)
            {
                for (int j = spriteSheet.height; j > 0; j -= sliceHeight)
                {
                    SpriteMetaData smd = new SpriteMetaData();
                    smd.pivot = new Vector2(0.5f, 0.5f);
                    smd.alignment = 9;
                    smd.name = (spriteSheet.height - j) / sliceHeight + ", " + i / sliceWidth;
                    smd.rect = new Rect(i, j - sliceHeight, sliceWidth, sliceHeight);

                    newData.Add(smd);
                }
            }

            //ti.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }
        Debug.Log("Done Slicing!");
    }

    [MenuItem("Utilities/SheetAnimation/AnimateFullSheet")]
    static void AnimateFullSheet()
    {
        // get all spirte sheets
        string folderPath = "ToAnimate";

        Object[] spriteSheets = Resources.LoadAll(folderPath, typeof(Texture2D));

        // iterate on sprite sheets
        foreach (Object obj in spriteSheets)
        {
            string savePath = "Assets/Resources/Animated";

            // create a folder named like the sprite sheet & update savePath;
            AssetDatabase.CreateFolder(savePath, obj.name);
            savePath += "/" + obj.name + "/";

            Sprite[] sprites = Resources.LoadAll<Sprite>(folderPath + "/" + obj.name);

            // create override controller based on my main controller
            RuntimeAnimatorController mainController = Resources.Load<RuntimeAnimatorController>("Base");

            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(mainController);
            animatorOverrideController.name = obj.name;

            animatorOverrideController["Idle N"] = CreateClip(sprites, "Idle N", 1, 0, 1, savePath, true);
            animatorOverrideController["Idle W"] = CreateClip(sprites, "Idle W", 1, 7, 1, savePath, true);
            animatorOverrideController["Idle S"] = CreateClip(sprites, "Idle S", 1, 14, 1, savePath, true);
            animatorOverrideController["Idle E"] = CreateClip(sprites, "Idle E", 1, 21, 1, savePath, true);

            animatorOverrideController["Spellcast N"] = CreateClip(sprites, "Spellcast N", 12, 0, 7, savePath, false);
            animatorOverrideController["Spellcast W"] = CreateClip(sprites, "Spellcast W", 12, 7, 7, savePath, false);
            animatorOverrideController["Spellcast S"] = CreateClip(sprites, "Spellcast S", 12, 14, 7, savePath, false);
            animatorOverrideController["Spellcast E"] = CreateClip(sprites, "Spellcast E", 12, 21, 7, savePath, false);

            animatorOverrideController["Thrust N"] = CreateClip(sprites, "Thrust N", 12, 28, 8, savePath, false);
            animatorOverrideController["Thrust W"] = CreateClip(sprites, "Thrust W", 12, 36, 8, savePath, false);
            animatorOverrideController["Thrust S"] = CreateClip(sprites, "Thrust S", 12, 44, 8, savePath, false);
            animatorOverrideController["Thrust E"] = CreateClip(sprites, "Thrust E", 12, 52, 8, savePath, false);

            animatorOverrideController["Walk N"] = CreateClip(sprites, "Walk N", 12, 60, 9, savePath, true);
            animatorOverrideController["Walk W"] = CreateClip(sprites, "Walk W", 12, 69, 9, savePath, true);
            animatorOverrideController["Walk S"] = CreateClip(sprites, "Walk S", 12, 78, 9, savePath, true);
            animatorOverrideController["Walk E"] = CreateClip(sprites, "Walk E", 12, 87, 9, savePath, true);

            animatorOverrideController["Slash N"] = CreateClip(sprites, "Slash N", 12, 96, 6, savePath, false);
            animatorOverrideController["Slash W"] = CreateClip(sprites, "Slash W", 12, 102, 6, savePath, false);
            animatorOverrideController["Slash S"] = CreateClip(sprites, "Slash S", 12, 108, 6, savePath, false);
            animatorOverrideController["Slash E"] = CreateClip(sprites, "Slash E", 12, 114, 6, savePath, false);

            animatorOverrideController["Bow N"] = CreateClip(sprites, "Bow N", 12, 120, 13, savePath, false);
            animatorOverrideController["Bow W"] = CreateClip(sprites, "Bow W", 12, 133, 13, savePath, false);
            animatorOverrideController["Bow S"] = CreateClip(sprites, "Bow S", 12, 146, 13, savePath, false);
            animatorOverrideController["Bow E"] = CreateClip(sprites, "Bow E", 12, 159, 13, savePath, false);

            animatorOverrideController["Hurt"] = CreateClip(sprites, "Hurt", 12, 172, 6, savePath, false);

            string pathToSave = savePath + obj.name + ".controller"; // TODO: use the previously created folder to save
            AssetDatabase.CreateAsset(animatorOverrideController, pathToSave);
        }

        Debug.Log("Animating done!");
    }

    [MenuItem("Utilities/SheetAnimation/AnimateSlashSheet")]
    static void AnimateSlashSheet()
    {
        // get all spirte sheets
        string folderPath = "ToAnimate";

        Object[] spriteSheets = Resources.LoadAll(folderPath, typeof(Texture2D));
        // iterate on sprite sheets
        foreach (Object obj in spriteSheets)
        {
            string savePath = "Assets/Resources/Animated";

            // create a folder named like the sprite sheet & update savePath;
            AssetDatabase.CreateFolder(savePath, obj.name);
            savePath += "/" + obj.name + "/";

            Sprite[] sprites = Resources.LoadAll<Sprite>(folderPath + "/" + obj.name);

            // create override controller based on my main controller
            RuntimeAnimatorController mainController = Resources.Load<RuntimeAnimatorController>("Base"); // TODO: different controller?

            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(mainController);
            animatorOverrideController.name = obj.name;

            animatorOverrideController["Slash N"] = CreateClip(sprites, "Slash N", 12, 0, 6, savePath, false);
            animatorOverrideController["Slash W"] = CreateClip(sprites, "Slash W", 12, 6, 6, savePath, false);
            animatorOverrideController["Slash S"] = CreateClip(sprites, "Slash S", 12, 12, 6, savePath, false);
            animatorOverrideController["Slash E"] = CreateClip(sprites, "Slash E", 12, 18, 6, savePath, false);

            string pathToSave = savePath + obj.name + ".controller"; // TODO: use the previously created folder to save
            AssetDatabase.CreateAsset(animatorOverrideController, pathToSave);
        }
        Debug.Log("Animating done!");
    }

    [MenuItem("Utilities/SheetAnimation/AnimateThrustSheet")]
    static void AnimateThrustSheet()
    {
        // get all spirte sheets
        string folderPath = "ToAnimate";

        Object[] spriteSheets = Resources.LoadAll(folderPath, typeof(Texture2D));
        // iterate on sprite sheets
        foreach (Object obj in spriteSheets)
        {
            string savePath = "Assets/Resources/Animated";

            // create a folder named like the sprite sheet & update savePath;
            AssetDatabase.CreateFolder(savePath, obj.name);
            savePath += "/" + obj.name + "/";

            Sprite[] sprites = Resources.LoadAll<Sprite>(folderPath + "/" + obj.name);

            // create override controller based on my main controller
            RuntimeAnimatorController mainController = Resources.Load<RuntimeAnimatorController>("Base"); // TODO: different controller?

            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(mainController);
            animatorOverrideController.name = obj.name;

            animatorOverrideController["Thrust N"] = CreateClip(sprites, "Thrust N", 12, 0, 8, savePath, false);
            animatorOverrideController["Thrust W"] = CreateClip(sprites, "Thrust W", 12, 8, 8, savePath, false);
            animatorOverrideController["Thrust S"] = CreateClip(sprites, "Thrust S", 12, 16, 8, savePath, false);
            animatorOverrideController["Thrust E"] = CreateClip(sprites, "Thrust E", 12, 24, 8, savePath, false);

            string pathToSave = savePath + obj.name + ".controller"; // TODO: use the previously created folder to save
            AssetDatabase.CreateAsset(animatorOverrideController, pathToSave);
        }
        Debug.Log("Animating done!");
    }

    [MenuItem("Utilities/SheetAnimation/AnimateBowSheet")]
    static void AnimateBowSheet()
    {
        // get all spirte sheets
        string folderPath = "ToAnimate";

        Object[] spriteSheets = Resources.LoadAll(folderPath, typeof(Texture2D));
        // iterate on sprite sheets
        foreach (Object obj in spriteSheets)
        {
            string savePath = "Assets/Resources/Animated";

            // create a folder named like the sprite sheet & update savePath;
            AssetDatabase.CreateFolder(savePath, obj.name);
            savePath += "/" + obj.name + "/";

            Sprite[] sprites = Resources.LoadAll<Sprite>(folderPath + "/" + obj.name);

            // create override controller based on my main controller
            RuntimeAnimatorController mainController = Resources.Load<RuntimeAnimatorController>("Base"); // TODO: different controller?

            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(mainController);
            animatorOverrideController.name = obj.name;

            animatorOverrideController["Bow N"] = CreateClip(sprites, "Bow N", 12, 0, 13, savePath, false);
            animatorOverrideController["Bow W"] = CreateClip(sprites, "Bow W", 12, 13, 13, savePath, false);
            animatorOverrideController["Bow S"] = CreateClip(sprites, "Bow S", 12, 26, 13, savePath, false);
            animatorOverrideController["Bow E"] = CreateClip(sprites, "Bow E", 12, 39, 13, savePath, false);

            string pathToSave = savePath + obj.name + ".controller"; // TODO: use the previously created folder to save
            AssetDatabase.CreateAsset(animatorOverrideController, pathToSave);
        }
        Debug.Log("Animating done!");
    }

    static AnimationClip CreateClip(Sprite[] sprites, string name, int frameRate, int startFrame, int frameNumber, string savePath, bool _isLooping)
    {
        string pathToSave = savePath + name + ".anim"; // TODO: use passed on folder
                                                       //http://forum.unity3d.com/threads/lack-of-scripting-functionality-for-creating-2d-animation-clips-by-code.212615/
        AnimationClip animClip = new AnimationClip();
        animClip.frameRate = frameRate;
        animClip.wrapMode = _isLooping == true ? WrapMode.Loop : WrapMode.Once; // TODO: does this work?

        // First you need to create e Editor Curve Binding
        EditorCurveBinding curveBinding = new EditorCurveBinding();

        // I want to change the sprites of the sprite renderer, so I put the typeof(SpriteRenderer) as the binding type.
        curveBinding.type = typeof(SpriteRenderer);
        // Regular path to the gameobject that will be changed (empty string means root)
        curveBinding.path = "";
        // This is the property name to change the sprite of a sprite renderer
        curveBinding.propertyName = "m_Sprite";

        // An array to hold the object keyframes
        ObjectReferenceKeyframe[] keyFrames = new ObjectReferenceKeyframe[frameNumber];
        for (int j = 0; j < frameNumber; j++)
        {
            keyFrames[j] = new ObjectReferenceKeyframe();
            // set the time
            keyFrames[j].time = j * 0.1f; // TODO: this is wrong, I want keyframes to be spaced by 1 frame;
                                          // set reference for the sprite you want
            keyFrames[j].value = sprites[startFrame + j];
        }

        AnimationUtility.SetObjectReferenceCurve(animClip, curveBinding, keyFrames);
        AssetDatabase.CreateAsset(animClip, pathToSave); // TODO: use the previously created folder to save

        return animClip;
    }
}
#endif
