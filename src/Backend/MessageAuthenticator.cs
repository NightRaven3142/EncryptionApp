using System;
using System.Linq;
using System.Security.Cryptography;

namespace Encryption_App.Backend
{
    /// <summary>
    /// Used for signing and verifying HMACs
    /// </summary>
    internal class MessageAuthenticator
    {
        /// <summary>
        /// Creates a byte[] hashcode that represents the file and key hashed with SHA384. Do not try and verify this yourself, use the VerifyHMAC() func
        /// </summary>
        /// <param name="data">A byte[] of the encrypted message data</param>
        /// <param name="key">A byte[] of the key</param>
        /// <returns>A byte[] hash that is the file and key hashed</returns>
        public byte[] CreateHMAC(byte[] data, byte[] key)
        {
            byte[] hashKey;

            using (var hmac = new HMACSHA384(key))
            {
                hashKey = hmac.ComputeHash(data);
            }

            return hashKey;
        }
        
        /// <summary>
        /// Signs a encrypted file and key with a hash algorithm of your choosing. Do not try and verify this yourself, use the VerifyHMAC() func
        /// </summary>
        /// <param name="data">A byte[] of the encrypted message data</param>
        /// <param name="key">A byte[] of the key</param>
        /// <param name="typeOfHash">typeof() any derivative of the System.Security.Cryptography.HMAC class</param>
        /// <returns>A byte[] hash that is the file and key hashed</returns>
        public byte[] CreateHMAC(byte[] data, byte[] key, Type typeOfHash)
        {
            HMAC hmac;
            if (typeOfHash.IsSubclassOf(typeof(HMAC)))
            {
                hmac = (HMAC)Activator.CreateInstance(typeOfHash);
            }
            else
            {
                throw new ArgumentException("TypeOfHash is not a derivative of \"System.Security.Cryptography.HMAC\"");
            }

            byte[] hashKey;

            using (hmac)
            {
                hashKey = hmac.ComputeHash(data);
            }

            return hashKey;
        }

        /// <summary>
        /// A function that verifies a HMAC file with SHA384
        /// </summary>
        /// <param name="data">A byte[] of encrypted message data</param>
        /// <param name="key">A byte[] of the key</param>
        /// <param name="hash">The hash in the header file/the hash provided, that's been hashed with SHA384</param>
        /// <returns>True if they match, otherwise false</returns>
        public bool VerifyHMAC(byte[] data, byte[] key, byte[] hash)
        {
            byte[] hashKey;

            using (var hmac = new HMACSHA384(key))
            {
                hashKey = hmac.ComputeHash(data);
            }

            return hashKey.SequenceEqual(hash);
        }


        /// <summary>
        /// A function that verifies a HMAC file with a hash algorithm of your choice
        /// </summary>
        /// <param name="data">A byte[] of encrypted message data</param>
        /// <param name="key">A byte[] of the key</param>
        /// <param name="hash">The hash in the header file/the hash provided, that's been hashed with typeOfHash</param>
        /// <param name="typeOfHash">typeof() the hash algorithm you used to create this, derived from System.Security.Cryptography.HMAC</param>
        /// <returns>True if they match, otherwise false</returns>
        public bool VerifyHMAC(byte[] data, byte[] key, byte[] hash, Type typeOfHash)
        {
            HMAC hmac;
            if (typeOfHash.IsSubclassOf(typeof(HMAC)))
            {
                hmac = (HMAC)Activator.CreateInstance(typeOfHash, key);
            }
            else
            {
                throw new ArgumentException("TypeOfHash is not a derivative of \"System.Security.Cryptography.HMAC\"");
            }

            byte[] hashKey;

            using (hmac)
            {
                hashKey = hmac.ComputeHash(data);
            }

            return data.SequenceEqual(hashKey);  // returns true if they match
        }
    }
}