using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    // Hàm gọi khi bấm nút "Thử lại"
    public void RestartGame()
    {
        Time.timeScale = 1f; // BẮT BUỘC: Mở lại thời gian game trước khi Load lại màn
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Hàm gọi khi bấm nút "Về Menu" (Tùy chọn nếu bạn có Main Menu)
    public void MainMenu()
    {
        Time.timeScale = 1f;
        // SceneManager.LoadScene("MainMenu"); // Mở comment dòng này nếu có scene MainMenu
        Debug.Log("Về Main Menu!");
    }
}