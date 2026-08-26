using UnityEngine;
using UnityEngine.UI; // Phải có dòng này để dùng Slider

public class SettingsMenu : MonoBehaviour
{
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Khi mở bảng Settings, kéo thanh Slider về đúng vị trí âm lượng hiện tại
        if (bgmSlider != null) 
        {
            bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1f);
            bgmSlider.onValueChanged.AddListener(UpdateBGM);
        }

        if (sfxSlider != null) 
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
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