using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class RewardCSVImporter : EditorWindow
{
    private TextAsset csvFile;
    private string savePath = "Assets/Rewards";

    [MenuItem("Tools/Import Reward CSV")]
    public static void ShowWindow()
    {
        GetWindow<RewardCSVImporter>("Import Reward CSV");
    }

    private void OnGUI()
    {
        GUILayout.Label("Reward CSV Importer", EditorStyles.boldLabel);

        csvFile = (TextAsset)EditorGUILayout.ObjectField("CSV File", csvFile, typeof(TextAsset), false);
        savePath = EditorGUILayout.TextField("Save Path", savePath);

        if (GUILayout.Button("Import"))
        {
            if (csvFile == null)
            {
                EditorUtility.DisplayDialog("Error", "CSV 파일을 지정하세요.", "확인");
                return;
            }

            ImportCSV(csvFile.text);
        }
    }

    private void ImportCSV(string csv)
    {
        var lines = csv.Split('\n');
        if (lines.Length <= 1)
        {
            Debug.LogWarning("CSV에 유효한 데이터가 없습니다.");
            return;
        }

        Directory.CreateDirectory(savePath);

        // 헤더 스킵
        for (int i = 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            var parts = SplitCSVLine(line);
            if (parts.Length < 5)
            {
                Debug.LogWarning($"잘못된 형식의 줄: {line}");
                continue;
            }

            string name = parts[0].Trim();
            string abilityStr = parts[1].Trim();
            string amountStr = parts[2].Trim();
            string gradeStr = parts[3].Trim();
            string desc = parts[4].Trim();

            if (!System.Enum.TryParse(abilityStr, out Ability ability))
            {
                Debug.LogWarning($"Ability 파싱 실패: {abilityStr}");
                continue;
            }

            if (!System.Enum.TryParse(gradeStr, out RewardGrade grade))
            {
                Debug.LogWarning($"Grade 파싱 실패: {gradeStr}");
                continue;
            }

            if (!float.TryParse(amountStr, out float amount))
            {
                Debug.LogWarning($"Amount 파싱 실패: {amountStr}");
                continue;
            }

            Reward reward = ScriptableObject.CreateInstance<Reward>();
            reward.ability = ability;
            reward.amount = amount;
            reward.rewardGrade = grade;
            reward.description = desc;
            reward.name = name; // Asset 이름용

            string assetPath = Path.Combine(savePath, $"{name}.asset");
            AssetDatabase.CreateAsset(reward, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("CSV 임포트 완료!");
    }

    private string[] SplitCSVLine(string line)
    {
        // 쉼표로 나누되, 따옴표 안 쉼표는 무시
        return System.Text.RegularExpressions.Regex.Split(line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
    }
}
