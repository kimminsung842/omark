using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class sc_findPW1 : MonoBehaviour
{
    public TMP_InputField Input_Email;
    public TextMeshProUGUI Txt_Error;
    public AccountRecoveryApi Api;

    public void OnClickNext()
    {
        Txt_Error.text = "";

        string email = Input_Email.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            Txt_Error.text = "이메일을 입력해주세요.";
            return;
        }

        Api.FindPasswordStep1(
            email,
            onCompleted: (res) =>
            {
                if (res.status != "success")
                {
                    Txt_Error.text = res.message;
                    return;
                }

                AccountRecoverySession.Email = email;
                AccountRecoverySession.AskId = res.data.askId;

                SceneManager.LoadScene("sc_findPW2");
            },
            onError: (err) =>
            {
                Txt_Error.text = "서버 오류: " + err;
            }
        );
    }
}
