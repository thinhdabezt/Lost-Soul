using UnityEngine;
using Proyecto26;
using Assets.Scripts;

public class Firebase : MonoBehaviour
{
    public string firebaseUrl;

    private void Awake()
    {
        LoadConfig();
    }

    private void LoadConfig()
    {
        TextAsset configText = Resources.Load<TextAsset>("firebase_config");
        if (configText != null)
        {
            FirebaseConfig config = JsonUtility.FromJson<FirebaseConfig>(configText.text);
            firebaseUrl = config.firebaseUrl;
        }
        else
        {
            Debug.LogError("firebase_config.json not found in Resources!");
        }
    }
    public void SavePlayerData(string username, int health, int level)
    {
        Player data = new Player
        {
            username = username,
            health = health,
            level = level
        };

        string url = $"{firebaseUrl}/users/{username}.json";
        RestClient.Put(url, data, (err, res) =>
        {
            if (err != null)
                Debug.LogError($"Save failed: {err.Message}");
            else
                Debug.Log("Player data saved successfully.");
        });
    }

    public void LoadPlayerData(string username, System.Action<Player> onLoaded)
    {
        string url = $"{firebaseUrl}/users/{username}.json";
        RestClient.Get<Player>(url, (err, res, data) =>
        {
            if (err != null)
            {
                Debug.LogError($"Load failed: {err.Message}");
                onLoaded?.Invoke(null);
            }
            else
            {
                Debug.Log($"Loaded player: {data?.username}, health: {data?.health}, level: {data?.level}");
                onLoaded?.Invoke(data);
            }
        });
    }
}
