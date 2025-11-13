using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GalleryHandler : MonoBehaviour
{
    [Header("UI References")]
    public RawImage displayImage;
    public Text feedbackText;

    private Texture2D loadedTexture;

    public void PickImage()
    {
        // 갤러리 열기
        NativeGallery.GetImageFromGallery((path) =>
        {
            if (path == null)
            {
                ShowFeedback("이미지 선택이 취소되었습니다.", Color.gray);
                return;
            }

            // 이미지 불러오기
            Texture2D texture = NativeGallery.LoadImageAtPath(path, maxSize: 1024);
            if (texture == null)
            {
                ShowFeedback("이미지를 불러올 수 없습니다.", Color.red);
                return;
            }

            loadedTexture = texture;
            displayImage.texture = loadedTexture;
            displayImage.color = Color.white;

            ShowFeedback("이미지가 성공적으로 로드되었습니다!", new Color(0.2f, 0.8f, 0.2f));
            
        },
        "이미지를 선택하세요",
        "image/*");
    }

    private void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            StartCoroutine(ClearFeedbackAfterDelay(3f));
        }
    }

    private IEnumerator ClearFeedbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (feedbackText != null)
            feedbackText.text = "";
    }
}
