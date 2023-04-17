using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject instructions;

    private static MenuFunctions instance;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Forest Speedway")
        {
            mainMenu.SetActive(false);
        }
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            mainMenu.SetActive(true);
        }
        instance = this;
    }

    public static MenuFunctions GetInstance()
    {
        return instance;
    }

    public void GoToGame()
    {
        Time.timeScale = 1;
        if (SceneManager.GetActiveScene().name == "Forest Speedway")
        {
            Player.GetInstance().UnPause();
            mainMenu.SetActive(false);
        }
        else
        {
            SceneManager.LoadScene("Forest Speedway");
        }
    }

    public void GoToMenu()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            mainMenu.SetActive(true);
            instructions.SetActive(false);
        }
        else
        {
            SceneManager.LoadScene("Main Menu");
        }
    }

    public void ShowInstructions()
    {
        mainMenu.SetActive(false);
        instructions.SetActive(true);
    }

    public void PauseGame()
    {
        Player.GetInstance().paused = true;
        mainMenu.SetActive(true);
    }

    public void ExitGame()
    {
        //This will check if you're running the game in the unity editor
        #if UNITY_EDITOR //if you are, it will exit play mode when you click 'quit'
        UnityEditor.EditorApplication.isPlaying = false;
        #endif //if you're not, it will quit the application
        Application.Quit();
        //This is because the editorapplication line on its own would cause compiling errors
        //By using this preprocessor directive however, it will ignore that line when building the application
    }
}