using UnityEngine;
using UnityEngine.UI; // Phải có dòng này để dùng Slider

public class SettingsMenu : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        // [ĐÃ SỬA] Kéo dữ liệu từ "Két sắt JSON" thay vì PlayerPrefs
        if (bgmSlider != null) 
        {
            if (SaveManager.Instance != null)
            {
                bgmSlider.value = SaveManager.Instance.data.bgmVolume;
            }
            bgmSlider.onValueChanged.AddListener(UpdateBGM);
        }

        if (sfxSlider != null) 
        {
            if (SaveManager.Instance != null)
            {
                sfxSlider.value = SaveManager.Instance.data.sfxVolume;
            }
            sfxSlider.onValueChanged.AddListener(UpdateSFX);
        }
    }

    // Hàm gọi khi người chơi kéo thanh BGM
    public void UpdateBGM(float value)
    {
        if (AudioManager.Instance != null) 
        {
            AudioManager.Instance.SetBGMVolume(value);
        }
    }

    // Hàm gọi khi người chơi kéo thanh SFX
    public void UpdateSFX(float value)
    {
        if (AudioManager.Instance != null) 
        {
            AudioManager.Instance.SetSFXVolume(value);
            // Phát thử một tiếng click nhẹ để người chơi nghe test âm lượng
            if (!Input.GetMouseButton(0)) // Chỉ kêu khi nhả chuột ra (tùy chọn)
            {
                AudioManager.Instance.PlayButtonClick(); 
            }
        }
    }
}