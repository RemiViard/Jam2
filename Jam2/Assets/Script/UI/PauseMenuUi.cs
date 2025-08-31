using UnityEngine;
using UnityEngine.Audio;

public class PauseMenuUi : MonoBehaviour
{
   [SerializeField] AudioListener audioListener;
   public void QuitButton()
   {
        Application.Quit();
   }
}
