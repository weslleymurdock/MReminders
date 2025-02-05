using System.Security.Cryptography.X509Certificates;

namespace MReminders.Mobile.Infrastructure.Handlers;

public class CertificateHandler(X509Certificate2 certificate) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        //var base64Certificate = Convert.ToBase64String(_certificate.Export(X509ContentType.Cert));
        //var base64Certificate = certificate.ToString().Replace("\n", " ");
        request.Headers.Add("X-Client-Certificate", Convert.ToBase64String(certificate.RawData));

        return base.SendAsync(request, cancellationToken);
    }
}