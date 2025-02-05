using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace MReminders.Mobile.Infrastructure.Handlers;

public class IgnoreCertificateValidationHandler : HttpClientHandler
{
    public IgnoreCertificateValidationHandler()
    {
        // Ignorar a verificação de certificado
        ServerCertificateCustomValidationCallback = (message, cert, chain, sslPolicyErrors) => true;
    }
}
