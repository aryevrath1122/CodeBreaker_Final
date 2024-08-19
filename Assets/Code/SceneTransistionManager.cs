using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransistionManager : MonoBehaviour
{
    public GameObject errorPanel; // Assign this in the Inspector
    public string newSceneName; // Assign this in the Inspector
    public GameObject[] gameObjects; // Assign GameObjects 0, 1, 2, 3 here in order

    private bool[] touched = new bool[4]; // Array to track touched state of each GameObject

    void Start()
    {
        // Ensure the error panel is hidden at the start
        if (errorPanel != null)
        {
            errorPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Check for Enter key press
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Check if only GameObject 3 is touched
            if (touched[3] && !touched[0] && !touched[1] && !touched[2])
            {
                // Load the new scene
                LoadNewScene();
            }
            else
            {
                // Show the error panel
                ShowErrorPanel();
            }
        }

        // Check for R key press to restart the scene
        if (Input.GetKeyDown(KeyCode.R) && errorPanel.activeSelf)
        {
            RestartScene();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check which GameObject was touched and update the array
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (collision.gameObject == gameObjects[i])
            {
                touched[i] = true;
                Debug.Log($"Collided with GameObject {i}: touched[{i}] = true");
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Check which GameObject was exited and update the array
        for (int i = 0; i < gameObjects.Length; i++)
        {
            if (collision.gameObject == gameObjects[i])
            {
                touched[i] = false;
                Debug.Log($"Exited collision with GameObject {i}: touched[{i}] = false");
            }
        }
    }

    private void LoadNewScene()
    {
        Debug.Log("Attempting to load new scene: " + newSceneName);
        // Load the scene assigned in the Inspector
        SceneManager.LoadScene(newSceneName);
    }

    private void ShowErrorPanel()
    {
        if (errorPanel != null)
        {
            errorPanel.SetActive(true);
        }
    }

    private void RestartScene()
    {
        if (errorPanel != null)
        {
            errorPanel.SetActive(false);
        }
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}


