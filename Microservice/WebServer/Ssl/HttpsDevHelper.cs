using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microservice.WebServer.Ssl
{
    public static class HttpsDevHelper
    {
        private static string PfxFileDirectory = Path.Combine(Directory.GetCurrentDirectory(), "WebServer", "Ssl", "X509");
        private static string PfxFileName = "SelfSignedLocalhostX509Certificate.pfx";
        private static string PfxPassword = "P@ssw0rd";

        public static X509Certificate2? InstallSelfSignedLocalhostX509Certificate(StoreName StoreName) {
            string PfxFilePath = Path.Combine(PfxFileDirectory, PfxFileName);
            X509Store store = new X509Store(StoreName);
            store.Open(OpenFlags.ReadWrite);
            ISet<X509Certificate2> CertificatesSet = store.Certificates.ToHashSet();
            //Clean up the store by eliminating all SelfSigned certificates so as not to accumulate them over time
            if (!File.Exists(PfxFilePath)) { //First time or if the file is deleted
                store.Certificates.Where(c=>c.Issuer=="CN=localhost" && c.Subject=="CN=localhost").ToList().ForEach(c=>store.Remove(c));
                GenerateLocalhostSelfSignedX509CertificateAsPfxFile();
            }
            //Load the X509Certificate by reading it from the generated file
            X509Certificate2 SelfSignedLocalhostX509Certificate = new X509Certificate2(PfxFilePath,PfxPassword);
            //If it is contained in the store (it will be seen by the browser locally) and it is valid, return the certificate
            if (CertificatesSet.Contains(SelfSignedLocalhostX509Certificate) && SelfSignedLocalhostX509Certificate.Verify()) {
                store.Close();
                return SelfSignedLocalhostX509Certificate;
            }
            //If it is contained in the store (it will be seen by the browser locally) but it is not valid anymore
            //(example: it has expired) remove it from the store and recreate it.
            if (CertificatesSet.Contains(SelfSignedLocalhostX509Certificate) && !SelfSignedLocalhostX509Certificate.Verify()) {
                store.Remove(SelfSignedLocalhostX509Certificate);
            }
            GenerateLocalhostSelfSignedX509CertificateAsPfxFile();
            X509Certificate2 NewCertificate = new X509Certificate2(PfxFilePath, PfxPassword);
            store.Add(NewCertificate);
            store.Close();
            return NewCertificate.Verify() ? NewCertificate : null;
        }


        public static void GenerateLocalhostSelfSignedX509CertificateAsPfxFile()
        {
            // Generate private-public key pair
            RSA RsaKey = RSA.Create(2048);
            /* The Common Name (CN) represents the name of the server protected by the SSL certificate. The certificate is 
            valid only if the request hostname matches the certificate common name. 
            Most web browsers display a warning message when connecting to an address that does not match the common 
            name in the certificate.
            In this case the certificate must authenticate the local server 127.0.0.1 known as 'localhost'.  */
            // Describe certificate
            const string CommonName = "CN=localhost";

            /* Create a CertificateRequest object. Tt has the purpose of containing the information necessary
            to build the certificate including keys, the format of the key and the algorithm to be used to compute
            the hash and the signature. */
            CertificateRequest Request = new CertificateRequest(CommonName, RsaKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            /* The BasicConstraint is an X.509 certificate v3 extension. 
            This extension describes whether the certificate is a CA certificate or an END_ENTITY certificate:
                * END_ENTITY :
                      An EndEntity is a user system that is the subject of a certificate, such as a web server.
                * CA :
                      It is a CertificateAuthority. Is a trusted organization that issues digital certificates 
                      for websites and other entities.
                      There are two types of certificate authorities (CAs): ROOT CAs and INTERMEDIATE CAs. 
                      For an SSL certificate to be trusted, that certificate must have been issued by a CA that's 
                      included in the trusted store of the device that's connecting. If the certificate wasn't issued 
                      by a trusted CA, the connecting client device (eg. a web browser) checks to see if the certificate 
                      of the issuing CA was issued by a trusted CA. It continues checking until either a trusted CA is found 
                      (at which point a trusted, secure connection will be established), or no trusted CA can be found 
                      (at which point the device will usually display an error).
                      The list of SSL certificates, from the root ca certificate to the end-user certificate, 
                      represents the SSL certificate chain :
             
            +------------------+
            | SubjectType: CA  | <------ ROOT CA
            | PathLenConstr: 1 |
            +------------------+
                                \
                                 +------------------+
                                 | SubjectType: CA  | <------ INTERMEDIATE CA 
                                 | PathLenConstr: 0 |
                                 +------------------+
                                                     \
                                                     +-------------------------+
                                                     | SubjectType: End Entity | <------ END ENTITY (web server, or app, or some stuff)
                                                     | PathLenConstr: None     |
                                                     +-------------------------+
            A web browser tries to find a trusted CA by checking the stores and going up until he meets one, up to the ROOT CA.
            Therefore this extension is used to determine the type of certificate. 
            In this case it's a simple endpoint certificate  for localhost : */
            Request.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(
                    certificateAuthority    : false, //Localhost is an endpoint, not a CA!
                    hasPathLengthConstraint : false, //An endpoint certificate cannot have children certificate
                    pathLengthConstraint    : 0,
                    critical                : true ) //The CriticalityIndicator is a flag that indicates to the software that the certificate should be discarded if it does not meet the extension.
            );

            /* This extension is used to determine how keys can be used. 
            To enable TLS Web server authentication, the following usage values ​​must be set: 
            Digital signature, key encipherment or key agreement
            */
            Request.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    keyUsages: X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, 
                    critical : true)
            );

            /* This extension is used to define the purpose of the certificate. It can be the authentication of a client 
            for example, or the authentication of a mail server */
            const string ServerAuthenticationObjectId = "1.3.6.1.5.5.7.3.1";
            Request.CertificateExtensions.Add(
                new X509EnhancedKeyUsageExtension(
                    enhancedKeyUsages: new OidCollection { new Oid(ServerAuthenticationObjectId) }, 
                    critical: true)
            );

            /* Both the TLS and SSL protocols use what is known as an 'asymmetric' Public Key Infrastructure (PKI) system. 
            An asymmetric system uses two 'keys' to encrypt communications, a 'public' key and a 'private' key. 
            This extension is used to provide browsers with the public key. With this, they can verify the digital signature 
            and encrypt the data to send. */
            Request.CertificateExtensions.Add(
                new X509SubjectKeyIdentifierExtension(
                    key     : Request.PublicKey,
                    critical: true)
            );

            //Browsers often require that the SubjectAlternativeName field specify the subject value to trust a certificate.
            //In this case, the subject is localhost
            var SanBuilder = new SubjectAlternativeNameBuilder();
            SanBuilder.AddDnsName(CommonName.Split('=').Last());
            Request.CertificateExtensions.Add(SanBuilder.Build(critical: true));

            //Set the certificate expiration
            DateTime NotBeforeDate = DateTime.Now;

            //Build the certificate
            X509Certificate2 LocalHostCertificate = Request.CreateSelfSigned(NotBeforeDate, NotBeforeDate.AddYears(2));

            // Export certificate with private key
            LocalHostCertificate = new X509Certificate2(
                LocalHostCertificate.Export(X509ContentType.Cert),
                string.Empty,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet
            ).CopyWithPrivateKey(RsaKey);

            //Add a Description (only for windows SO)
            bool IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (IsWindows) { 
                LocalHostCertificate.FriendlyName = "Dotnet Dev Localhost SelfSigned TLS certificate ";
            }

            // Create password for certificate protection
            SecureString Pwd = new SecureString();
            foreach (char @char in PfxPassword){
                Pwd.AppendChar(@char);
            }

            //Write the pfx file 
            if (!Directory.Exists(PfxFileDirectory)) { Directory.CreateDirectory(PfxFileDirectory); }
            File.WriteAllBytes(
                Path.Combine(PfxFileDirectory,PfxFileName),
                LocalHostCertificate.Export(X509ContentType.Pfx, Pwd)
            );
        }
    }
}
