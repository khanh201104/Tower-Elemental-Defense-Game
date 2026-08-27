using UnityEngine;

[CreateAssetMenu(fileName = "NewBaseWaveData", menuName = "Tower Defense/Base Wave Data")]
public class BaseWaveData : ScriptableObject
{
    [Header("Dữ liệu Wave chuẩn (Dùng cho Màn 1)")]
    public Wave[] defaultWaves;
}