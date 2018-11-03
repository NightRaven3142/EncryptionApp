﻿namespace FactaLogicaSoftware.CryptoTools.Digests.KeyDerivation
{
    using FactaLogicaSoftware.CryptoTools.PerformanceInterop;
    using System;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    /// <inheritdoc />
    /// <summary>
    /// </summary>
    public sealed class SCryptKeyDerive : KeyDerive
    {
        private uint _read;

        private (ulong N, uint r, uint p) _tuneFlags;

        /// <summary>
        /// Default constructor that isn't valid for derivation
        /// </summary>
        public SCryptKeyDerive()
        {
            this.Usable = false;
        }

        /// <summary>
        /// Creates an instance of an object used to hash
        /// </summary>
        /// <param name="password">The bytes of the password to hash</param>
        /// <param name="salt">The salt used to hash
        /// underlying Rfc2898DeriveBytes objects</param>
        /// <param name="tuneFlags">The tuple containing the SCrypt tuning
        /// parameters (N, r, and p)</param>
        public SCryptKeyDerive(byte[] password, byte[] salt, (ulong N, uint r, uint p) tuneFlags)
        {
            this._tuneFlags = tuneFlags;
            this.Salt = salt;
            this.Password = password;
            this._read = 0;
            this.Usable = true;
        }

        /// <summary>
        /// Creates an instance of an object used to hash
        /// </summary>
        /// <param name="password">The string of the password to hash</param>
        /// <param name="salt">The salt used to hash
        /// underlying Rfc2898DeriveBytes objects</param>
        /// <param name="tuneFlags">The tuple containing the SCrypt tuning
        /// parameters (N, r, and p)</param>
        public SCryptKeyDerive(string password, byte[] salt, (ulong N, uint r, uint p) tuneFlags)
        {
            this._tuneFlags = tuneFlags;
            this.Salt = salt;
            this.Password = Encoding.UTF8.GetBytes(password);
            this._read = 0;
            this.Usable = true;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override byte[] Password
        {
            get => ProtectedData.Unprotect(this.BackEncryptedArray, null, DataProtectionScope.CurrentUser);

            private protected set
            {
                this.BackEncryptedArray = ProtectedData.Protect(value, null, DataProtectionScope.CurrentUser);

                const bool isPowerOf2 = true; // TODO V. IMPORTANT check if _tuneFlags.N is a power of 2

                if (isPowerOf2 && this._tuneFlags.r > 0 && this._tuneFlags.p > 0)
                {
                    this.Usable = true;
                }
            }
        }

        public override object PerformanceValues
        {
            get => this._tuneFlags;
            private protected set
            {
                var newCastTuple = (ValueTuple<int, int, int>)value;
                this._tuneFlags = ((ulong)newCastTuple.Item1, (uint)newCastTuple.Item2, (uint)newCastTuple.Item3);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="size"></param>
        public override byte[] GetBytes(int size)
        {
            // TODO manage checked overflows
            return Replicon.Cryptography.SCrypt.SCrypt.DeriveKey(
                this.Password,
                this.Salt,
                this._tuneFlags.N,
                this._tuneFlags.r,
                this._tuneFlags.p,
                (uint)size + this._read).Skip((int)this._read).ToArray();
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override void Reset()
        {
            this._read = 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="performanceDerivative"></param>
        /// <param name="milliseconds"></param>
        public static (ulong N, uint r, uint p) TransformPerformance(PerformanceDerivative performanceDerivative, ulong milliseconds)
        {
            return performanceDerivative.TransformToScryptTuning(milliseconds);
        }
    }
}