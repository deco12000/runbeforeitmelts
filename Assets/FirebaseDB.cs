using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;

[Serializable]
public class UserData
{
    public string Name;
    public float score;
    public UserData(string Name, float score)
    {
        this.Name = Name;
        this.score = score;
    }
}

public class FirebaseDB : MonoBehaviour
{
    private DatabaseReference dbRef;
    public string dbUrl = "https://runbeforeitmelts-default-rtdb.firebaseio.com/";
    public UserData[] top3Users = new UserData[3]; // 불러온 상위 유저 저장

    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                var options = new AppOptions
                {
                    DatabaseUrl = new Uri(dbUrl)
                };

                FirebaseApp app = FirebaseApp.Create(options, "Secondary");
                FirebaseDatabase db = FirebaseDatabase.GetInstance(app);
                dbRef = db.RootReference;

                Debug.Log("Firebase 초기화 완료");
            }
            else
            {
                Debug.LogError("Firebase 종속성 확인 실패: " + dependencyStatus);
            }
        });
    }

    public void Submit(UserData userData)
    {
        string key = dbRef.Child("users").Push().Key;
        string json = JsonUtility.ToJson(userData);
        dbRef.Child("users").Child(key).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
                Debug.Log("데이터 저장 성공");
            else
                Debug.LogError("저장 실패: " + task.Exception);
        });
    }

    public IEnumerator LoadHighScoreUsers()
    {
        var task = dbRef.Child("users")
            .OrderByChild("score")
            .LimitToLast(3)
            .GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.Exception != null)
        {
            Debug.LogError("불러오기 실패: " + task.Exception);
            yield break;
        }

        List<UserData> userList = new List<UserData>();
        foreach (var snapshot in task.Result.Children)
        {
            string json = snapshot.GetRawJsonValue();
            UserData user = JsonUtility.FromJson<UserData>(json);
            userList.Add(user);
        }

        userList.Sort((a, b) => b.score.CompareTo(a.score));
        for (int i = 0; i < 3; i++)
        {
            top3Users[i] = i < userList.Count ? userList[i] : null;
        }

        Debug.Log("코루틴 방식: 상위 3명 불러오기 완료");

        highScores[0].text = $"{top3Users[0].score}m - {top3Users[0].Name}";
        highScores[1].text = $"{top3Users[1].score}m - {top3Users[1].Name}";
        highScores[2].text = $"{top3Users[2].score}m - {top3Users[2].Name}";

    }

    [SerializeField] TMP_Text[] highScores;


}
