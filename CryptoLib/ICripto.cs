using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLib
{
   public interface ICripto
    {
        /// <summary>
        /// Sets the key used to encrypt or decrypt data.
        /// Returns true if the key is formatted correctly and can be set.
        /// Returns false if the key cannot be set.
        /// </summary>
        /// <param name="input">Byte array containing the key.</param>
        /// <returns></returns>
        bool SetKey(byte[] input);

        /// <summary>
        /// Generates a random key in the required format.
        /// The key can then be set using the SetKey method.
        /// </summary>
        /// <returns></returns>
        byte[] GenerateRandomKey();

        /// <summary>
        /// Sets the initialization vector needed for certain algorithms.
        /// Returns true of the initialization vector can be set.
        /// Returns false if the initialization vector cannot be set.
        /// </summary>
        /// <param name="input">Byte array containing the initialization vector.</param>
        /// <returns></returns>
        bool SetIV(byte[] input);

        /// <summary>
        /// Generates a random initialization vector in the required format.
        /// The initialization vector can then be set using the SetIV method.
        /// </summary>
        /// <returns></returns>
        byte[] GenerateRandomIV();

        /// <summary>
        /// Method used to set certain parameters of a cryptographic algorithm.
        /// Contains data dependant on the algorithm.
        /// </summary>
        /// <param name="specArguments">Dictionary containing required parameters.</param>
        /// <returns></returns>
        bool SetAlgorithmProperties(IDictionary<string, byte[]> specArguments);

        /// <summary>
        /// Method used to encrypt data using a cryptographic algorithm.
        /// Returns a byte array containing encrypted data.
        /// Throws an exception if an algorithm parameter is not set.
        /// </summary>
        /// <param name="input">Byte array containing data that needs to be encrypted.</param>
        /// <returns></returns>
        byte[] Crypt(byte[] input);

        /// <summary>
        /// Method used to decrypt data using a cryptographic algorithm.
        /// Returns a byte array containing decrypted data.
        /// Throws an exception if an algorithm parameter is not set.
        /// </summary>
        /// <param name="output">Byte array containing data that needs to be decrypted.</param>
        /// <returns></returns>
        byte[] Decrypt(byte[] output);



    }
}
