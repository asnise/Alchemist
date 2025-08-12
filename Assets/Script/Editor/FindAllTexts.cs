using UnityEngine;
using UnityEditor;
using System.IO;
using TMPro;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using System.Text;

public class FindAllTexts : EditorWindow
{
    [MenuItem("Tools/Debug/Find All Texts (with Hierarchy)")]
    public static void ShowWindow()
    {
        GetWindow<FindAllTexts>("Find All Texts");
    }

    private void OnGUI()
    {
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 14;
        labelStyle.wordWrap = true;
        labelStyle.padding = new RectOffset(10, 10, 10, 10);

        GUILayout.Label("เครื่องมือนี้จะค้นหาและบันทึกข้อความทั้งหมด (ทั้ง Text และ TMP_Text) ใน Scene ที่เปิดอยู่" +
                        "\nพร้อมแสดงลำดับชั้น (Hierarchy) ของ GameObject นั้นๆ " +
                        "\nผลลัพธ์จะถูกบันทึกเป็นไฟล์ .txt ในโฟลเดอร์ Assets", labelStyle);

        GUILayout.Space(10);

        if (GUILayout.Button("Find and Log Texts", GUILayout.Height(30)))
        {
            FindAndLogAllTextsWithHierarchy();
        }
    }

    private void FindAndLogAllTextsWithHierarchy()
    {
        string currentSceneName = EditorSceneManager.GetActiveScene().name;
        string outputLogPath = $"Assets/Text_Contents_Log_{currentSceneName}.txt";

        StringBuilder logContent = new StringBuilder();

        TMP_Text[] allTMPTexts = FindObjectsOfType<TMP_Text>(true);
        foreach (var tmpText in allTMPTexts)
        {
            string fullPath = GetGameObjectPath(tmpText.transform);
            logContent.AppendLine($"\"{fullPath}\" : \"{tmpText.text}\"");
        }

        Text[] allLegacyTexts = FindObjectsOfType<Text>(true);
        foreach (var legacyText in allLegacyTexts)
        {
            string fullPath = GetGameObjectPath(legacyText.transform);
            logContent.AppendLine($"\"{fullPath}\" : \"{legacyText.text}\"");
        }

        File.WriteAllText(outputLogPath, logContent.ToString());
        Debug.Log("Text contents with hierarchy path logged to: " + outputLogPath);

        EditorUtility.OpenWithDefaultApp(outputLogPath);
    }

    private static string GetGameObjectPath(Transform transform)
    {
        StringBuilder pathBuilder = new StringBuilder();
        Transform currentTransform = transform;

        // วนลูปย้อนกลับจาก Child ไปหา Parent จนถึง Root
        while (currentTransform != null)
        {
            pathBuilder.Insert(0, currentTransform.name);
            if (currentTransform.parent != null)
            {
                pathBuilder.Insert(0, "/");
            }
            currentTransform = currentTransform.parent;
        }

        return pathBuilder.ToString();
    }
}
