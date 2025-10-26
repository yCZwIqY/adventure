using System.IO;
using UnityEngine;

public static class SaveManager
{
    private static string filePath => Path.Combine(Application.persistentDataPath, "save.json");

    // 저장
    public static void Save(GameData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"게임 데이터 저장 완료: {filePath} {data.isFirstPlay}");
    }

    // 로드
    public static GameData Load()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("저장된 파일이 없습니다. 기본 데이터 반환");
            return new GameData();
        }

        string json = File.ReadAllText(filePath);
        GameData data = JsonUtility.FromJson<GameData>(json);
        Debug.Log($"게임 데이터 로드 완료 {data.isFirstPlay}" );
        return data;
    }

    // 삭제
    public static void DeleteSave()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Debug.Log("저장 파일 삭제 완료");
        }
    }
}