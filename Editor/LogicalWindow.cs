using UnityEditor;

namespace Logical.Editor
{
    public class LogicalWindow : EditorWindow
    {
        [MenuItem("Window/Logical")]
        public static void OpenWindow()
        {
            CreateWindow<LogicalWindow>().Show();
        }
    }
}
