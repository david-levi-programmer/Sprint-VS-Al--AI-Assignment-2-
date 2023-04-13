using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject instructions;

    private void Awake()
    {
        mainMenu.SetActive(true);
        instructions.SetActive(false);
    }

    public void GoToGame()
    {
        SceneManager.LoadScene("Forest Speedway");
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