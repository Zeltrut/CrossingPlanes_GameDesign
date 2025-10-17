using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    private bool isMenuOpen = false;

    void Start()
    {
        menuCanvas.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Check for the Tab key press to toggle the menu.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menuCanvas.activeSelf && PauseController.IsGamePaused)
            {
                return;
            }
            isMenuOpen = !isMenuOpen;
            menuCanvas.SetActive(isMenuOpen);
            PauseController.SetPause(menuCanvas.activeSelf);
            UpdateCursorState();
        }
    }

    void UpdateCursorState()
    {
        if (isMenuOpen)
        {
            // If the menu is open, make the cursor visible and unlock it.
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            // If the menu is closed, hide the cursor and then lock it to center of the screen.
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
