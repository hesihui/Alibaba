#if UNITY_EDITOR
using UnityEditor;

public static class ForceDomainReload
{
    [MenuItem("Tools/Force Domain Reload")]
    public static void ReloadDomain()
    {
        EditorUtility.RequestScriptReload();
    }
}
#endif