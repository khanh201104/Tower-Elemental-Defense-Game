using UnityEngine;
using System.IO;

// Class này chứa toàn bộ dữ liệu bạn muốn lưu
[System.Serializable]
public class GameData
{
    public int highestUnlockedLevel = 1;
    public float bgmVolume = 1f;
    public float sfxVolume = 1f;
    // Sau này có thêm Vàng, Kim Cương, Nâng cấp... thì cứ khai báo thêm vào đây!
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public GameData data;
    private string saveFilePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Bay xuyên các Scene giống AudioManager
            
            // persistentDataPath là đường dẫn an toàn không bị xóa khi update game
            saveFilePath = Application.persistentDataPath + "/SaveData.json";
            
            LoadGame(); // Tự động đọc dữ liệu ngay khi vừa mở game
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        // Chuyển Data thành chữ JSON (true = format đẹp có xuống dòng)
        string json = JsonUtility.ToJson(data, true); 
        File.WriteAllText(saveFilePath, json);
        Debug.Log("💾 Đã lưu game thành công tại: " + saveFilePath);
    }

    public void LoadGame()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            data = JsonUtility.FromJson<GameData>(json);
            Debug.Log("📂 Đã tải dữ liệu game cũ.");
        }
        else
        {
            Debug.Log("🆕 File save không tồn tại. Tạo dữ liệu mới.");
            data = new GameData(); 
            SaveGame();
        }
    }
}