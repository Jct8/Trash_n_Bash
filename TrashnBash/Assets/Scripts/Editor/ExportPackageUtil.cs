using UnityEditor;

public class ExportPackageUtil : Editor
{
    [MenuItem("Tools/CreateSystemsPackage")]
    public static void ExportSystemsPackage()
    {
        AssetDatabase.ExportPackage(AssetDatabase.GetAllAssetPaths(), 
                                    "SystemsPackage.unitypackage", 
                                    ExportPackageOptions.IncludeDependencies |
                                    ExportPackageOptions.IncludeLibraryAssets |
                                    ExportPackageOptions.Interactive |
                                    ExportPackageOptions.Recurse);
    }
}
