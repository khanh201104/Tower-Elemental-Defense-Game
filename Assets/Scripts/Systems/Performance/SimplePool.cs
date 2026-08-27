using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePool : MonoBehaviour
{
    public static SimplePool Instance;

    // Bảng lưu trữ: 1 Prefab sẽ có 1 "thùng rác" (Queue) chứa các GameObject đã sử dụng
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
    
    // Lưu lại thông tin: GameObject này là bản sao của Prefab nào (để cất cho đúng thùng)
    private Dictionary<GameObject, GameObject> activeObjectsPrefab = new Dictionary<GameObject, GameObject>();

    void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    // THAY THẾ CHO LỆNH: Instantiate(...)
    // THAY THẾ CHO LỆNH: Instantiate(...)
    public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (Instance == null) return Instantiate(prefab, position, rotation);

        if (!Instance.poolDictionary.ContainsKey(prefab))
        {
            Instance.poolDictionary.Add(prefab, new Queue<GameObject>());
        }

        GameObject objToSpawn = null;

        // Vòng lặp: Rút đồ trong thùng rác ra kiểm tra liên tục
        while (Instance.poolDictionary[prefab].Count > 0)
        {
            objToSpawn = Instance.poolDictionary[prefab].Dequeue();
            
            // Nếu vật thể vẫn tồn tại thật (chưa bị xóa do chuyển Scene hoặc sót lệnh Destroy)
            if (objToSpawn != null) 
            {
                break; // Hàng thật, thoát vòng lặp để lấy ra dùng!
            }
        }

        // Nếu tìm được hàng dùng lại
        if (objToSpawn != null)
        {
            objToSpawn.transform.position = position;
            objToSpawn.transform.rotation = rotation;
            objToSpawn.SetActive(true);
        }
        else // Nếu thùng rỗng (hoặc toàn rác "ảo" đã bị xóa hết) -> Đẻ mới
        {
            objToSpawn = Instantiate(prefab, position, rotation);
        }

        Instance.activeObjectsPrefab[objToSpawn] = prefab;
        return objToSpawn;
    }

    // THAY THẾ CHO LỆNH: Destroy(...)
    public static void Despawn(GameObject obj, float delay = 0f)
    {
        if (Instance == null) { Destroy(obj, delay); return; }
        
        if (delay > 0) 
            Instance.StartCoroutine(Instance.DespawnWithDelay(obj, delay));
        else 
            Instance.DoDespawn(obj);
    }

    private IEnumerator DespawnWithDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        DoDespawn(obj);
    }

    private void DoDespawn(GameObject obj)
    {
        if (obj == null) return;
        
        obj.SetActive(false); // Đóng băng vật thể (Tắt đi)
        
        if (activeObjectsPrefab.TryGetValue(obj, out GameObject prefab))
        {
            poolDictionary[prefab].Enqueue(obj); // Quăng vào thùng rác
        }
        else
        {
            Destroy(obj); // Rác vãng lai không rõ nguồn gốc -> Tiêu hủy
        }
    }
}