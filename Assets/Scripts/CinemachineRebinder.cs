using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CinemachineRebinder : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Move player to spawn point
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");

        if (player != null && spawnPoint != null)
        {
            player.transform.position = spawnPoint.transform.position;
        }

        var vcam = Object.FindFirstObjectByType<CinemachineCamera>();
        if (vcam != null && player != null)
        {
            vcam.Follow = player.transform;
            vcam.LookAt = player.transform;
        }
    }
}
