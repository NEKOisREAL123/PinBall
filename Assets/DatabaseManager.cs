using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class DatabaseManager : MonoBehaviour
{
    private const string API_URL = "http://localhost:3000";
    private string playerId = "player1";  // 可以根據需求修改玩家ID

    // 更新分數
    public IEnumerator UpdateScore(int currentScore, int bestScore)
    {
        string json = JsonUtility.ToJson(new PlayerScore
        {
            playerId = playerId,
            currentScore = currentScore,
            bestScore = bestScore
        });

        using (UnityWebRequest request = new UnityWebRequest($"{API_URL}/updateScore", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"分數更新成功: {request.downloadHandler.text}");
            }
            else
            {
                Debug.LogError($"分數更新失敗: {request.error}");
            }
        }
    }

    // 獲取玩家最高分
    public IEnumerator GetBestScore()
    {
        using (UnityWebRequest request = UnityWebRequest.Get($"{API_URL}/player/{playerId}"))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                PlayerScore playerData = JsonUtility.FromJson<PlayerScore>(request.downloadHandler.text);
                // 更新遊戲中的最高分
                //ScoreManager.Instance.UpdateHighScore(playerData.bestScore);
                Debug.Log($"獲取最高分成功: {playerData.bestScore}");
            }
            else
            {
                Debug.LogError($"獲取最高分失敗: {request.error}");
            }
        }
    }
}

[System.Serializable]
public class PlayerScore
{
    public string playerId;
    public int currentScore;
    public int bestScore;
}