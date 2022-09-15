using Microservice.Application.Services.Crud.Interfaces;
using Microservice.Application.Services.Security.Interfaces;
using Microservice.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microservice.Application.Services.Security.Context;

namespace Microservice.Application.Services.Security
{
    /*
        ----------------------------------------------------------------------------------------------------------------------------------
            
            The JSON Web Token (JWT) is an open standard (RFC 7519) that defines a schema in JSON format for exchanging information 
            between various services.
            The generated token can be signed (with a secret key that only those who generate the token know) using the HMAC algorithm, 
            or using a pair of keys (public / private) using the RSA or ECDSA standards.

            A JWT consists in an encoded string in the format <HEADER>.<PAYLOAD>.<SIGNATURE>:
        +--------------------------------------------------------------------------------------------------------------------------------+        
        |    * Header:    The header contains two main information: the type of token (in this case valued to JWT because it is a JSON   |
        |                 Web Token) and the type of encryption algorithm used. An Header example is:                                    |
        |                                        +---------------------+                                                                 |
        |                                        |  {                  |                                                                 |
        |                                        |    "alg": "HS256",  |                                                                 |
        |                                        |    "typ": "JWT"     |                                                                 |
        |                                        |  }                  |                                                                 |
        |                                        +---------------------+                                                                 |
        |                 It is Base64Url encoded to form the first part of the JWT.                                                     |
        +--------------------------------------------------------------------------------------------------------------------------------+   
        |    * Payload:   The payload contains a list of parameters, we can categorize them into three blocks:                           |
        |                                                                                                                                |
        |             1) Registered parameters: they are predefined properties that indicate information about the token,such as:        |
        |                                                                                                                                |
        |                 - iss (issuer)     : is a string that contains the identification name of the entity that generated the token  |
        |                 - aud (audience)   : is an array of values ​​indicating the token's abilities                                    |
        |                 - exp (expiration) : is an integer (timestamp in seconds) indicating until when the token will be valid        |
        |                 - nbf (not before) : it is an integer (timestamp in seconds), the token will be valid only after that date     |
        |                 - iat (issued at)  : it is an integer (timestamp in seconds) that indicates the token creation date            | 
        |                                                                                                                                |
        |             2) Public parameters: refer to parameters defined in the IANA (https://www.iana.org/assignments/jwt/jwt.xhtml),    |
        |                they can be compiled at will by paying attention to the content that is entered to avoid conflicts.             |
        |                                                                                                                                |
        |             3) Private parameters: here you can indulge yourself by inserting what you want having full flexibility thanks     |
        |                to the JSON structure.                                                                                          |
        |                                        +----------------------------+                                                          |
        |                                        |  {                         |                                                          |
        |                                        |    "iss": "NOME_APP",      |                                                          |
        |                                        |    "name": "Mario Rossi",  |                                                          |
        |                                        |    "iat": 1540890704,      |                                                          |
        |                                        |    "exp": 1540918800       |                                                          |
        |                                        |  }                         |                                                          |
        |                                        +----------------------------+                                                          |
        |                 It is Base64Url encoded to form the second part of the JWT.                                                    |
        +--------------------------------------------------------------------------------------------------------------------------------+ 
        |  * Signature: The signature is used to verify that the issuer of the JWT is who it says it is and to ensure                    |
        |               that the message wasn't changed along the way.                                                                   |
        +--------------------------------------------------------------------------------------------------------------------------------+

            The generation of the TOKEN occurs by coding the HEADER and the PAYLOAD in base 64 and joining the two results separating 
            them by a ".". Then the algorithm is applied in the header to the string obtained using a secret key.

                        For example, using the HMAC SHA256 algorithm, the token could be obtained as follows:

                  TokenString = HMACSHA256( [base64UrlEncode(header)+"."+base64UrlEncode(payload)] , [secretKey])

        ----------------------------------------------------------------------------------------------------------------------------------

        In an asymmetric scenario, the possession and the use of the key materials are the following:

        For signing:
                
            * The private key is owned by the token issuer and used to compute the signature.
            * The public key can be shared with all parties that need to verify the signature.
        
        For encrypt:

            * The private key is owned by who receives the data, and used to decrypt the data.
            * The public key can be shared to any party that want to send sensitive data to the recipient.
            
        The encryption is rarely used with JWT. Most of the time the HTTPS layer is sufficient and the token 
        itself only contain a few information that are not sensitive (datatime, IDs...).

        The issuer of the token (the authentication server) has a private key to generate signed tokens (JWS). 
        These tokens are sent to the clients (an API server, a web/native application...). 
        The clients can verify the token with the public key. The key is usually fetched using a public URI.

        If you have sensitive data that shall not be disclosed to a third party (phone numbers, personal address...), 
        then the encrypted tokens (JWE) is highly recommended. In this case, each client (i.e. recipient of a token) 
        shall have a private key and the issuer of the token must encrypt the token using the public key of each recipient. 
        This means that the issuer of the token can select the appropriate key for a given client.   
    */


    public sealed class TokenProviderService : ITokenProviderService<SignedJwt>
    {
        private readonly IConfiguration _configuration;
        private readonly IHashProviderService _hashProviderService;
        private readonly ICrudService<User> _userCrudService;

        public TokenProviderService(IConfiguration Configuration,IHashProviderService HashService,ICrudService<User> CrudService) {
            this._configuration = Configuration;
            this._hashProviderService = HashService;
            this._userCrudService = CrudService;
        }

        //PRIVATE KEY: Used to sign a token. It must not be shared! Only those who know this key can produce valid signed tokens
        private RsaSecurityKey GetSignatureKey() {
            string PrivateKeyString = this._configuration.GetSection("Authentication:AsymmetricJwt:PrivateKeyPkcs8").Get<string[]>()[1];
            byte[] PrivateKeyBinary = Convert.FromBase64String(PrivateKeyString);
            RSA RsaInstance = RSA.Create();
            RsaInstance.ImportPkcs8PrivateKey(PrivateKeyBinary, out _);
            return new RsaSecurityKey(RsaInstance);
        }

        //PUBLIC KEY: Used to verify a token. It can only be used to verify the issuing source of the token
        public RsaSecurityKey GetSignatureVerificationKey() {
            string PublicKeyString = this._configuration.GetSection("Authentication:AsymmetricJwt:PublicKeyX_509").Get<string[]>()[1];
            byte[] PublicKeyBinary = Convert.FromBase64String(PublicKeyString);
            RSA RsaInstance = RSA.Create();
            RsaInstance.ImportSubjectPublicKeyInfo(PublicKeyBinary, out _);
            return new RsaSecurityKey(RsaInstance);
        }
        
        public async Task<string?> GetTokenString(string Email, string Password) {
            //Check the login credentials provided by the user:
            User? AuthenticatedUser = await this.UserCredentialsAuthentication(Email, Password);
            if(AuthenticatedUser == null) { // Wrong credentials!
                return null; 
            }

            //Build the Token Claims List. It is preferable not to insert too much information about the user in the token, and
            //not to introduce sensitive data. Better to enter IDs, or other information of a logical nature.
            //If there are sensitive data that shall not be disclosed to a third party (phone numbers, personal address...) the
            //encrypted tokens (JWE) are highly recommended. 
            IEnumerable<Claim> TokenPayloadClaims = new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier,AuthenticatedUser.UserName!.TrimEnd().TrimStart()),
                new Claim(ClaimTypes.Role,string.Join(",",AuthenticatedUser.UsersRoles!.Select(obj => obj.RoleName!.TrimEnd().TrimStart())))
            };

            //Build SigningCredentials to sign the token. SigningCredentials consists of a [<SignatureKey>,<EncryptionAlgorithm>] pair.
            //In an asymmetrical scenario the SignatureKey corresponds to the private key while the EncryptionAlgorithm is an asymmetric
            //encryption algorithm.
            SigningCredentials Credentials = new SigningCredentials(this.GetSignatureKey(), SecurityAlgorithms.RsaSha256);

            //Build the token string
            string Issuer   = this._configuration.GetValue<string>("Authentication:AsymmetricJwt:Issuer");
            string Audience = this._configuration.GetValue<string>("Authentication:AsymmetricJwt:Audience");
            double Minutes  = this._configuration.GetValue<double>("Authentication:AsymmetricJwt:TokenValidityMinutes");
            DateTime NotBeforeDate = DateTime.Now;
            SecurityToken TokenDescriptor = new JwtSecurityToken(Issuer, Audience, TokenPayloadClaims, NotBeforeDate, NotBeforeDate.AddMinutes(Minutes), Credentials);
            string TokenString = new JwtSecurityTokenHandler().WriteToken(TokenDescriptor);
            return TokenString;
        }

        private async Task<User?> UserCredentialsAuthentication(string Email, string Password) {
            //Email check
            User? UserByEmail = await this._userCrudService.Retrieve(user => user.Email == Email, user=>user.UsersRoles);
            if (UserByEmail == null) { return null; }
            //Password hash check
            if(!this._hashProviderService.Check(UserByEmail.Password!,Password)) { return null; }
            return UserByEmail;
        }

        //Just for completeness. The [Authorize] attribute already validates the token automatically.
        public bool VerifyToken(string TokenString)
        {
            TokenValidationParameters ValidationParameters = this.GetTokenValidationParameters();
            try {
                new JwtSecurityTokenHandler().ValidateToken(TokenString, ValidationParameters, out var ValidatedToken);
                return true;
            } catch {
                return false;
            }
        }

        public IDictionary<string, string> DecodeToken(string TokenString) {
            JwtSecurityToken TokenDescriptor = new JwtSecurityTokenHandler().ReadJwtToken(TokenString);
            IDictionary<string,string> ClaimsDictionary = new Dictionary<string,string>();
            TokenDescriptor.Claims.ToList().ForEach(claim => ClaimsDictionary.Add(claim.Type.Split('/').Last(), claim.Value));
            return ClaimsDictionary;
        }

        //Utilities for Dependency Injection 
        public TokenValidationParameters GetTokenValidationParameters() {
            return new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = this.GetSignatureVerificationKey(), //Public Key
                ValidateIssuer = true,
                ValidIssuer = this._configuration["Authentication:AsymmetricJwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = this._configuration["Authentication:AsymmetricJwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireSignedTokens = true,
                RequireExpirationTime = true
            };
        }
    }
}
