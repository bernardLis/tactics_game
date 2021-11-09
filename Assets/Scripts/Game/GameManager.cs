using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    PlayerInput playerInput;

    public static GameManager instance;

    void Awake()
    {
        // singleton
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerInput = MovePointController.instance.GetComponent<PlayerInput>();
    }

    public void EnableFMPlayerControls()
    {
        playerInput.SwitchCurrentActionMap("FMPlayer");
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

}
