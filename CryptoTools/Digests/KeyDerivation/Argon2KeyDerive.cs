﻿using FactaLogicaSoftware.CryptoTools.Exceptions;
using FactaLogicaSoftware.CryptoTools.PerformanceInterop;
using Liphsoft.Crypto.Argon2;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace FactaLogicaSoftware.CryptoTools.Digests.KeyDerivation
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// TODO Make based off Konscious security, better lib, but not on nuGet
    public sealed class Argon2KeyDerive : KeyDerive
    {
        private readonly PasswordHasher _baseObject;

        private (ulong N, uint r, uint p) _tuneFlags;
        private uint _read;

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override object PerformanceValues
        {
            get => _tuneFlags;

            private protected set
            {
                try
                {
                    var newCastTuple = (ValueTuple<int, int, int>)value;
                    _tuneFlags = ((ulong)newCastTuple.Item1, (uint)newCastTuple.Item2, (uint)newCastTuple.Item3);
                }
                catch (InvalidCastException)
                {
                    _tuneFlags = (ValueTuple<ulong, uint, uint>)value;
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// The password, stored encrypted
        /// </summary>
        public override byte[] Password
        {
            get => ProtectedData.Unprotect(BackEncryptedArray, null, DataProtectionScope.CurrentUser);

            private protected set
            {
                BackEncryptedArray = ProtectedData.Protect(value, null, DataProtectionScope.CurrentUser);
                Usable = PerformanceValues != null;
            }
        }

        /// <summary>
        /// Default constructor that isn't valid for derivation
        /// </summary>
        public Argon2KeyDerive()
        {
            Usable = false;
        }

        /// <summary>
        /// Creates an instance of an object used to hash
        /// </summary>
        /// <param name="password">The bytes of the password to hash</param>
        /// <param name="salt">The salt used to hash</param>
        /// <param name="tuneFlags">The tuning parameters</param>
        public Argon2KeyDerive(byte[] password, byte[] salt, (ulong N, uint r, uint p) tuneFlags)
        {
            PerformanceValues = tuneFlags;
            Salt = salt;
            Password = password;
            _baseObject = new PasswordHasher((uint)_tuneFlags.N, tuneFlags.r, tuneFlags.p, Argon2Type.Argon2d, 1024 * 1024);
            Usable = true;
        }

        /// <summary>
        /// Creates an instance of an object used to hash
        /// </summary>
        /// <param name="password">The string the password to hash</param>
        /// <param name="salt">The salt used to hash</param>
        /// <param name="tuneFlags">The tuning parameters</param>
        public Argon2KeyDerive(string password, byte[] salt, (ulong N, uint r, uint p) tuneFlags)
        {
            PerformanceValues = tuneFlags;
            Salt = salt;
            Password = Encoding.UTF8.GetBytes(password);
            _baseObject = new PasswordHasher((uint)_tuneFlags.N, tuneFlags.r, tuneFlags.p, Argon2Type.Argon2d, 1024 * 1024);
            Usable = true;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="toFill"></param>
        public override byte[] GetBytes(int size)
        {
            if (!Usable)
            {
                throw new InvalidCryptographicOperationException("Password not set");
            }
            _baseObject.HashLength = _read + (uint)size;
            return _baseObject.HashRaw(Password, Salt).Skip((int)_read).ToArray();
        }

        public override void Reset()
        {
            _read = 0;
        }

        public override void TransformPerformance(PerformanceDerivative performanceDerivative, ulong milliseconds)
        {
            PerformanceValues = performanceDerivative.TransformToArgon2Tuning(milliseconds);
        }
    }
}