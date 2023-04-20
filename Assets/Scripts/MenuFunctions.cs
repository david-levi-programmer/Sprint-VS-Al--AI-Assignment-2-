using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFunctions : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject instructions;

    private static MenuFunctions instance; //so that other scripts can communicate with this one

    private void Awake()
    {
        //check if we're in the menu or on the track
        if (SceneManager.GetActiveScene().name == "Forest Speedway") //if on the track...
        {
            //...don't start with the menu open, because in-game, this is set to be the pause menu
            mainMenu.SetActive(false);
        }
        if (SceneManager.GetActiveScene().name == "Main Menu") //if in the menu...
        {
            //...then have the menu open on start-up
            mainMenu.SetActive(true);
        }
        instructions.SetActive(false); //either way, don't show the instructions on start-up
        instance = this;
    }

    public static MenuFunctions GetInstance()
    {
        return instance;
    }

    public void GoToGame()
    {
        //Unfreeze everything when returning from the pause menu, the results screen...
        Time.timeScale = 1; //...or from the main menu after clicking 'back to menu' in the pause menu
        if (SceneManager.GetActiveScene().name == "Forest Speedway") //In-game, this works as a 'resume' function
        {
            mainMenu.SetActive(false); //put away the pause menu
            instructions.SetActive(false);
            Player.GetInstance().UnPause(); //put the player's HUD back on-screen
        }
        else //In the main menu, this takes you to the game
        {
            SceneManager.LoadScene("Forest Speedway");
        }

        if (Timer.GetInstance().timerOn == false) //The only situation where the timer is off is when the race is over
        {
            SceneManager.LoadScene("Forest Speedway"); //So this works as a restart function
        }
    }

    public void GoToMenu()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            //We're already in the menu, so just turn off the instructions. No need to reload the scene
            mainMenu.SetActive(true);
            instructions.SetActive(false);
        }
        else
        {
            //In-game, this function is only tied to the 'back to menu' button
            SceneManager.LoadScene("Main Menu");
        }
    }

    public void ShowInstructions()
    {
        //I was intially going to have the instructions available in the pause menu
        //but switching them on and off didn't work right, so I just removed that button from the pause menu
        mainMenu.SetActive(false);
        instructions.SetActive(true); //so in-game, this goes unused
    }

    public void PauseGame() //Called by the player with the 'P' key
    {
        Player.GetInstance().paused = true; //everything is frozen
        mainMenu.SetActive(true); //When the game's paused, show the menu
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