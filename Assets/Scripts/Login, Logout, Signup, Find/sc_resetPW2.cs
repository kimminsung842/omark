using UnityEngine;
using UnityEngine.SceneManagement;

public class sc_resetPW2 : MonoBehaviour
{
    public void OnClickGoLogin()
    {
        SceneManager.LoadScene("sc_login");
    }
}
