using System.Security.Cryptography;
using Microservice.Application.Services.Security.Interfaces;

namespace Microservice.Application.Services.Security
{
    /*
        SHA512 is a cryptographic hash function, while Rfc2898DeriveBytes is a key-derivation function.
        Hash functions are too fast and can be brute-forced too easily, 
        so key-derivation function introduce a cost factor to slow down the execution of a brute force attack.
        In the future it is possible to increase the number of iterations (cost factor) to cope with increasing 
        computing powers.

        Hashing password using salt is one of the best practices in protecting user accounts from hackers.
        In case hackers have stolen databases, they also need more time to decryte them. It won't be easy at all. 
        At the same time, you have time to reset all passwords or suggest users to change passwords right away.
    */
    public class HashProviderService : IHashProviderService
    {
        private readonly int _saltCharSize = 16;    // 128 bit 
        private readonly int _keyCharSize  = 32;    // 256 bit
        private readonly int _iterations   = 10000; // Cost factor

        public HashProviderService() {
        }

        // "PASSWORD"  ===>  "salt.hashedKey"
        public string Hash(string password)
        {
            var Algorithm = new Rfc2898DeriveBytes(password, _saltCharSize, _iterations, HashAlgorithmName.SHA512);
            string? Salt = Convert.ToBase64String(Algorithm.Salt);
            string HashedKey = Convert.ToBase64String(Algorithm.GetBytes(_keyCharSize));
            return $"{Salt}.{HashedKey}";
        }


        //'StoredPassword' is a "salt.hashedKey" string stored in a DB, while 'Password' is the password sent in clear text by a user
        public bool Check(string StoredPassword, string Password)
        {
            //Null check
            if (string.IsNullOrEmpty(StoredPassword)) { throw new ArgumentNullException(nameof(StoredPassword)); }
            if (string.IsNullOrEmpty(Password))       { throw new ArgumentNullException(nameof(Password));   }
            //Extract  [SALT . HASHED_KEY]
            string[] splittedStoredPassword = StoredPassword.Split('.', 2);
            if (splittedStoredPassword.Length != 2) { throw new FormatException("Wrong hash format. Should be {iterations}.{salt}.{hash}"); }
            byte[] StoredSalt = Convert.FromBase64String(splittedStoredPassword[0]);
            byte[] StoredHash = Convert.FromBase64String(splittedStoredPassword[1]);
            //Try to hash the current password and then compare the 2 hashes
            var Algorithm = new Rfc2898DeriveBytes(Password, StoredSalt, _iterations, HashAlgorithmName.SHA512);
            var PasswordHash = Algorithm.GetBytes(this._keyCharSize);
            return StoredHash.SequenceEqual(PasswordHash);
        }
    }
}

