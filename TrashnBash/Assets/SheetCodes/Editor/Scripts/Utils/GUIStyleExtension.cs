using UnityEngine;

namespace SheetCodesEditor
{
    public static class GUIStyleExtension
    {
        public static Vector2 CalculateTotalSize(this GUIStyle guiStyle, string content)
        {
            return guiStyle.CalcScreenSize(guiStyle.CalcSize(new GUIContent(content)));
        }
    }
}