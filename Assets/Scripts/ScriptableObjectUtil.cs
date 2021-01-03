using UnityEngine;

public static class ScriptableObjectUtil
{
    public static T CreateScriptableObjectInstance<T>(string path) where T : ScriptableObject
    {
        //create and save in file system
        var newScriptableObject = ScriptableObject.CreateInstance<T>();
        UnityEditor.AssetDatabase.CreateAsset(newScriptableObject, path);
        UnityEditor.AssetDatabase.SaveAssets();
        
        //select it in project window and inspector
        UnityEditor.EditorUtility.FocusProjectWindow();
        UnityEditor.Selection.activeObject = newScriptableObject;
        
        return newScriptableObject;
    }
}