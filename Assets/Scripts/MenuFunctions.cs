using UnityEngine;
using UnityEngine.SceneManagement; //enables the code to switch scenes and check for specific ones

//This script is built to be used for ALL menus in the game, not just the start menu
public class MenuFunctions : MonoBehaviour
{
    //v the HUD elements of the menu
    public GameObject mainMenu; //This is called the 'main' menu, but in-game this will be the pause menu
    public GameObject instructions;

    private static MenuFunctions instance; //so that other scripts can communicate with this one

    private void Awake() //do this when loading into the scene
    {
        //check if we're in the menu or on the track
        if (SceneManager.GetActiveScene().name == "Forest Speedway") //if on the track...
        {
            mainMenu.SetActive(false); //...don't start with the pause menu open
        }
        if (SceneManager.GetActiveScene().name == "Main Menu") //if in the menu...
        {
            mainMenu.SetActive(true); //...then have the menu open on start-up
        }
        instructions.SetActive(false); //either way, don't show the instructions on start-up
        instance = this;
    }

    public static MenuFunctions GetInstance() //called by the other scripts to communicate with each other
    {
        return instance;
    }

    public void GoToGame()
    {
        //v Unfreeze everything when returning from the pause menu, the results screen...
        Time.timeScale = 1; //...or from the main menu to ensure nothing is frozen upon entering gameplay

        if (SceneManager.GetActiveScene().name == "Forest Speedway") //If in-game, this is assigned as a 'resume' function
        {
            mainMenu.SetActive(false); //put away the pause menu
            instructions.SetActive(false); //and the instructions, just in case
            Player.GetInstance().UnPause(); //and put the player's HUD back on-screen
        }
        else //If in the main menu...
        {
            SceneManager.LoadScene("Forest Speedway"); //...head into the game
        }

        if (Timer.GetInstance().timerOn == false) //The only situation where the timer is off is when the race is over
        {
            SceneManager.LoadScene("Forest Speedway"); //So this can be assigned as a restart function
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