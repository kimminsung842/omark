using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class sc_resetPW : MonoBehaviour
{
    public TMP_InputField Input_NewPW;
    public TMP_InputField Input_CheckPW;
    public TextMeshProUGUI Txt_Error;
    public AccountRecoveryApi Api;

    public void OnClickReset()
    {
        Txt_Error.text = "";

        string pw1 = Input_NewPW.text.Trim();
        string pw2 = Input_CheckPW.text.Trim();

        if (pw1.Length < 10 || pw1.Length > 20)
        {
            Txt_Error.text = "비밀번호는 10~20자여야 합니다.";
            return;
        }

        if (pw1 != pw2)
        {
            Txt_Error.text = "비밀번호가 서로 일치하지 않습니다.";
            return;
        }

        Api.ResetPasswordByQuestion(
            AccountRecoverySession.Email,
            AccountRecoverySession.AskId,
            AccountRecoverySession.AskAnswer,
            pw1,
            onCompleted: (res) =>
            {
                if (res.status != "success")
                {
                    Txt_Error.text = res.message;
                    return;
                }

                SceneManager.LoadScene("sc_resetPW2");
            },
            onError: (err) =>
            {
                Txt_Error.text = "서버 오류: " + err;
            }
        );
    }
}
