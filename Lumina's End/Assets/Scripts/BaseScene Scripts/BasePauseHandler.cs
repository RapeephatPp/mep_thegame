using UnityEngine;
using UnityEngine.SceneManagement;

public class BasePauseHandler : MonoBehaviour
{
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;

            // หยุด / เดินเวลา
            Time.timeScale = isPaused ? 0f : 1f;

            // เปิด/ปิด Pause UI
            if (UIManager.Instance != null)
            {
                UIManager.Instance.TogglePausePanel(isPaused);
            }
        }
    }
    
    // ปุ่ม Resume
    public void OnResumePressed()
    {
        // ถ้ามี GameManager (ฉากสู้หลัก) ก็ใช้ระบบเดิม
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
        else
        {
            // ถ้าอยู่ BaseScene (ไม่มี GameManager)
            Time.timeScale = 1f;
            if (UIManager.Instance != null)
                UIManager.Instance.TogglePausePanel(false);
        }
    }

    // ปุ่ม Return to MainMenu
    public void OnMainMenuPressed()
    {   
        Time.timeScale = 1f;
        if (SceneFader.Instance != null)
            SceneFader.Instance.FadeToScene("MainMenu");
        else
            SceneManager.LoadScene("MainMenu");
        
    }
}