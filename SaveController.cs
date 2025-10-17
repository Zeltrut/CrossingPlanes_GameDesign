using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SaveController : MonoBehaviour
{
    [Tooltip("A reference to the SegmentGenerator in the scene.")]
    [SerializeField] private SegmentGenerator segmentGenerator; 

    private const string SaveKey = "saveData";

    void Start()
    {
        Time.timeScale = 1f;
        LoadGame();
    }

    public void SaveGame()
    {
        if (segmentGenerator == null) {
            Debug.LogError("Save failed: Segment Generator is not assigned in the SaveController Inspector!");
            return;
        }
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) {
            Debug.LogError("Save failed: Player object not found in scene!");
            return;
        }

        SaveData saveData = new SaveData {
            playerPosition = player.transform.position,
            carrotCounter = GameInfo.carrotCount,
            generatedSegments = segmentGenerator.generatedSegmentData 
        };

        // Convert the save data to a JSON string.
        string jsonString = JsonUtility.ToJson(saveData);

        // Save the JSON string to PlayerPrefs.
        PlayerPrefs.SetString(SaveKey, jsonString);
        PlayerPrefs.Save(); // Write to browser storage.
        Debug.Log("Game Saved to PlayerPrefs.");
    }

    public void LoadGame()
    {
        if (segmentGenerator == null) {
            Debug.LogError("Load failed: Segment Generator is not assigned in the SaveController Inspector!");
            return;
        }

        // Check if the save key exists in PlayerPrefs.
        if (PlayerPrefs.HasKey(SaveKey)) 
        {
            // Retrieve the JSON string from PlayerPrefs.
            string jsonString = PlayerPrefs.GetString(SaveKey);
            SaveData saveData = JsonUtility.FromJson<SaveData>(jsonString);
            
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) {
                player.transform.position = saveData.playerPosition;
            }
            GameInfo.carrotCount = saveData.carrotCounter;

            segmentGenerator.LoadSegments(saveData.generatedSegments);
            Debug.Log("Game Loaded from PlayerPrefs.");
        } 
        else 
        {
            Debug.Log("No save data found in PlayerPrefs. Creating a new game.");
            segmentGenerator.LoadSegments(new List<SegmentData>()); 
            SaveGame();
        }
    }

    public void RestartGame()
    {
        // Delete the save data from PlayerPrefs.
        if (PlayerPrefs.HasKey(SaveKey))
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
            Debug.Log("Save data deleted for restart.");
        }

        GameInfo.carrotCount = 0;
        Time.timeScale = 1f;

        if (typeof(PauseController).GetMethod("SetPause") != null)
        {
            PauseController.SetPause(false);
        }
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void ResetAndGoToMenu()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();
            Debug.Log("Save data deleted.");
        }

        GameInfo.carrotCount = 0;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("StartMenu");
    }
}

