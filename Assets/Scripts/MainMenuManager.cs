using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;

[System.Serializable]
public class PlayerData {
    public string playerName;
    public string timeString;
    public int time;
}

[System.Serializable]
public class PlayerDataList {
    public List<PlayerData> players = new List<PlayerData>();
}

public class MainMenuManager : MonoBehaviour {
    
    public GameObject saveRun;
    public GameObject options;
    public TMP_InputField nameInputField;
    public TextMeshProUGUI error;
    public GameObject textPrefab;
    public Transform playerNameContainer; 
    public Transform playerTimeContainer;
    public RectTransform content;
    
    private void Start() {
        if(Data.completed) {
            saveRun.SetActive(true);
            options.SetActive(false);
            Data.completed = false;
        }
    }
    public void Play() {
        SceneManager.LoadScene("Level1");
    }

    public void SaveToLeaderboard() {
        if(nameInputField.text.Length < 3) {
            error.text = "Name must be 3 or more than characters";
            return;
        }

        if (File.Exists(Path.Combine(Application.persistentDataPath, "leaderboard.json"))) {
            string jsonRead = File.ReadAllText(Path.Combine(Application.persistentDataPath, "leaderboard.json"));
            PlayerDataList loadedData = JsonUtility.FromJson<PlayerDataList>(jsonRead);

            foreach (PlayerData playerData in loadedData.players) {
                if (playerData.playerName.Equals(nameInputField.text)) {
                    error.text = "Name already taken. Please choose another name.";
                    return;
                }
            }
        }

        string filePath = Path.Combine(Application.persistentDataPath, "leaderboard.json");
        PlayerDataList playerDataList;

        if (File.Exists(filePath)) {
            string jsonRead = File.ReadAllText(filePath);
            playerDataList = JsonUtility.FromJson<PlayerDataList>(jsonRead) ?? new PlayerDataList();
        } else {
            playerDataList = new PlayerDataList();
        }

        PlayerData newPlayerData = new PlayerData 
        { 
            playerName = nameInputField.text, 
            timeString = Data.timeString, 
            time = Data.time
        };
        
        playerDataList.players.Add(newPlayerData);
        playerDataList.players.Sort((a, b) => a.time.CompareTo(b.time));

        string jsonWrite = JsonUtility.ToJson(playerDataList, true);
        Debug.Log("Serialized JSON: " + jsonWrite);

        File.WriteAllText(filePath, jsonWrite);

        saveRun.SetActive(false);
        options.SetActive(true);
        OpenLeaderBoard();
    }   

    public void OpenLeaderBoard() {

        if (File.Exists(Path.Combine(Application.persistentDataPath, "leaderboard.json"))) {
            string jsonRead = File.ReadAllText(Path.Combine(Application.persistentDataPath, "leaderboard.json"));
            PlayerDataList loadedData = JsonUtility.FromJson<PlayerDataList>(jsonRead);

            int i = 0;

            foreach (var player in loadedData.players) {
                GameObject playerNameObj = Instantiate(textPrefab, playerNameContainer);
                playerNameObj.GetComponent<TextMeshProUGUI>().text = $"{i + 1}. {player.playerName}";
                RectTransform rectTransform = playerNameObj.GetComponent<RectTransform>();
                rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, 125 + (-50 * i), 0);

                GameObject playerTimeObj = Instantiate(textPrefab, playerTimeContainer);
                playerTimeObj.GetComponent<TextMeshProUGUI>().text = player.timeString.ToString();
                RectTransform rectTimeTransform = playerTimeObj.GetComponent<RectTransform>();
                rectTimeTransform.localPosition = new Vector3(rectTimeTransform.localPosition.x, 125 + (-50 * i), 0);

                i++;
            }
            content.sizeDelta = new Vector2(content.sizeDelta.x, (i * 100) + 500);
        }
        GameObject.FindWithTag("Leaderboard").GetComponent<Canvas>().sortingOrder = 1;
    }

    public void CloseLeaderBoard() {
        GameObject.FindWithTag("Leaderboard").GetComponent<Canvas>().sortingOrder = -1;
    }

    public void Quit() {
        Application.Quit();
    }
}
