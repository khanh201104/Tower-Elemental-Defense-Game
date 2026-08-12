using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Image fillImage;
    public Gradient colorGradient; // Tùy chọn: Đổi màu từ Xanh -> Vàng -> Đỏ khi tụt máu

    public void UpdateHealth(float currentHp, float maxHp)
    {
        if (slider == null) return;

        slider.maxValue = maxHp;
        slider.value = currentHp;

        // Tùy chọn đổi màu thanh máu theo % máu còn lại
        if (fillImage != null && colorGradient != null)
        {
            fillImage.color = colorGradient.Evaluate(slider.normalizedValue);
        }
    }
}