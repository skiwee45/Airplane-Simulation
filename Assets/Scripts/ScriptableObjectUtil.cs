using UnityEngine;

public static class ScriptableObjectUtil
{
    public static T CreateScriptableObjectInstance<T>(string path, bool focus) where T : ScriptableObject
    {
        //create and save in file system
        var newScriptableObject = ScriptableObject.CreateInstance<T>();
        UnityEditor.AssetDatabase.CreateAsset(newScriptableObject, path);
        UnityEditor.AssetDatabase.SaveAssets();
        
        //select it in project window and inspector
        if (focus)
        {
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = newScriptableObject;
        }
        return newScriptableObject;
    }
    
    public static void DeleteScriptableObjectInstance(ScriptableObject scriptableObject)
    {
        //get ID and delete
        DeleteScriptableObjectInstance(scriptableObject.GetInstanceID());
    }
    
    public static void DeleteScriptableObjectInstance(int scriptableObjectID)
    {
        //find path
        var path = UnityEditor.AssetDatabase.GetAssetPath(scriptableObjectID);
        UnityEditor.AssetDatabase.DeleteAsset(path);
        UnityEditor.AssetDatabase.SaveAssets();
    }
}