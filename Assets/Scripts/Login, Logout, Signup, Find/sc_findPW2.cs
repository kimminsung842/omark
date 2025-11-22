using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class sc_findPW2 : MonoBehaviour
{
    public TextMeshProUGUI txtQuestion;
    public TMP_InputField inputAnswer;
    public TextMeshProUGUI txtError;
    public AccountRecoveryApi api;

    Dictionary<int, string> questionMap = new Dictionary<int, string>()
    {
        { 1, "졸업한 초등학교 이름은?" },
        { 2, "어머니의 성함은?" },
        { 3, "가장 친한 친구의 이름은?" },
        { 4, "가장 기억에 남는 여행지는?" },
        { 5, "처음으로 키운 반려동물의 이름은?" },
        { 6, "가장 존경하는 인물의 이름은?" },
        { 7, "어릴 적 살던 동네(거리) 이름은?" },
        { 8, "가장 좋아하는 책/영화의 제목은?" }
    };

    void Start()
    {
        int id = AccountRecoverySession.AskId;

        if (questionMap.ContainsKey(id))
            txtQuestion.text = questionMap[id];
        else
            txtQuestion.text = "질문을 불러오지 못했습니다.";
    }

    public void OnClickNext()
    {
        txtError.text = "";

        string answer = inputAnswer.text.Trim();
        if (string.IsNullOrEmpty(answer))
        {
            txtError.text = "답변을 입력해주세요.";
            return;
        }

        api.FindPasswordStep2(
            AccountRecoverySession.Email,
            AccountRecoverySession.AskId,
            answer,
            onCompleted: (res) =>
            {
                if (res.status != "success")
                {
                    txtError.text = res.message;
                    return;
                }

                AccountRecoverySession.AskAnswer = answer;
                SceneManager.LoadScene("sc_resetPW");
            },
            onError: (err) =>
            {
                txtError.text = "서버 오류: " + err;
            }
        );
    }
}
