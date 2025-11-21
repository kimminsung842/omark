using UnityEngine.Networking;

public class CertHandlerAcceptAll : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        // 모든 인증서를 허용함 (주의!)
        return true;
    }
}
