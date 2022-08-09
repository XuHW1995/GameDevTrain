using Sirenix.OdinInspector.Editor;
using UnityEditor;

public class LeetCodeWindows : OdinMenuEditorWindow
{
    [MenuItem("LeetCodeWindows/AllCode")]
    private static void OpenWindow()
    {
        GetWindow<LeetCodeWindows>().Show();
    }
    
    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree(true);

        foreach (var oneExercise in ExerciseRegiester.allExercisesRegiest)
        {
            tree.Add(oneExercise.GetType().Name, oneExercise);
        }
        
        return tree;
    }
}