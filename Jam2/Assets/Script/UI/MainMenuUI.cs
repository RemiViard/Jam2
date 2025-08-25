using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] GameObject game;
    public void PlayButton()
    {
        game.SetActive(true);
        gameObject.SetActive(false);
    }
    public void QuitButton()
    {
        // Quit the application
        Application.Quit();
    }

}
