using UnityEditor;

namespace Logical99.Editor
{
    public class LogicalWindow : EditorWindow
    {
        [MenuItem("Window/Logical99")]
        public static void OpenWindow()
        {
            CreateWindow<LogicalWindow>().Show();
        }
    }
}