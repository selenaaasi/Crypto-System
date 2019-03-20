using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CryptoLib
{
    public class RC4:ICripto
    {
        private byte[] key;
        private ulong counter; 
        private uint[] box; 
        private uint[] keystream; 
        private bool ctr_set; 
        private byte[] IV; 
        private uint num_rounds; 

        public RC4()
        {
                num_rounds = 256;
                counter = 0;
                box = new uint[256];
                keystream = new uint[256];
                ctr_set = false;
                this.key=GenerateRandomKey();
                this.IV=GenerateRandomIV();
              
            
        }

        public byte[] CryptWithoutCRT(byte[] input)
        {
            uint a, i, j, k, tmp;
            byte[] cipher;
            cipher = new byte[input.Length]; 

            for (i = 0; i < num_rounds; i++)
            {
                keystream[i] = key[i % key.Length]; 
                box[i] = i; 
            }
            for (j = i = 0; i < num_rounds; i++)
            {
                j = (j + box[i] + keystream[i]) % num_rounds;
                tmp = box[i]; 
                box[i] = box[j];
                box[j] = tmp;
            }
            for (a = j = i = 0; i < input.Length; i++) 
            {
                a++;
                a %= num_rounds;
                j += box[a];
                j %= num_rounds;
                tmp = box[a];
                box[a] = box[j];
                box[j] = tmp;
                k = box[((box[a] + box[j]) % num_rounds)];
                cipher[i] = (byte)(input[i] ^ k);
            }
            return cipher;
        }

        public byte[] DecryptWithoutCRT(byte[] output)
        {
            return CryptWithoutCRT(output);
        }

        public byte[] GenerateRandomKey()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] randomNumber = new byte[64];
            rng.GetBytes(randomNumber);
            return randomNumber;
        }

        public bool SetKey(byte[] input)
      {
          if (input.Length <= 64)
          {

                  key = new byte[input.Length];
                  key.CopyTo(input, 0);
                  return true;
            
          }
          else return false;
      }

        public  byte[] GenerateRandomIV() {

          
            Random rnd = new Random();
            IV = new byte[8];
            rnd.NextBytes(IV);
            return IV;
        
        }

        public  bool SetIV(byte[] input)
        {
             if (input.Length == IV.Length)
             {
                 IV.CopyTo(input, 0);
                 return true;
             }
             else
                 return false;

        }

        public byte[] CryptWithCTR(byte[] input)
        {
            byte[] cipher = new byte[input.Length];
            byte[] mod;

            for (int i = 0; i < (input.Length / 8) * 8; i += 8) 
            {
                mod = this.CryptWithoutCRT(this.ExclusiveOR(this.IV, BitConverter.GetBytes(this.counter))); 
                for (int j = i; j < i + 8; j++)
                {
                    cipher[j] = (byte)(input[j] ^ mod[j % 8]);
                }

                if (counter == ulong.MaxValue)
                    counter = 0;
                else
                    counter++;
            }

            counter = 0;

            if (input.Length % 8 == 0)
            {
                counter = 0;
                return cipher;
            }
            else //ako input.length nije deljiva sa 8
            {
                mod = this.CryptWithoutCRT(this.ExclusiveOR(this.IV, BitConverter.GetBytes(this.counter)));
                for (int i = (input.Length / 8) * 8; i < input.Length; i++)
                {
                    cipher[i] = (byte)(input[i] ^ mod[i % 8]);
                }

                counter = 0;
                return cipher;
            }
        }

        public byte[] Crypt(byte[] input)
        {
            if (ctr_set)
               return CryptWithCTR(input);

            else
              return  CryptWithoutCRT(input);


        }

        public byte[] Decrypt(byte[] input)
        {
            if (ctr_set)
                return DecryptWithCTR(input);

            else
                return DecryptWithoutCRT(input);



        }

        public bool SetAlgorithmProperties(IDictionary<string, byte[]> specArguments) {

           if (specArguments.ContainsKey("crt_mod"))

               ctr_set = BitConverter.ToBoolean(specArguments["crt_mod"], 0);

           return true;
 
        }

        public byte[] DecryptWithCTR(byte[] output)
        {
            return CryptWithCTR(output);
        }

        public byte[] ExclusiveOR(byte[] array1, byte[] array2)
        {
            if (array1.Length == array2.Length)
            {
                byte[] result = new byte[array1.Length];

                for (int i = 0; i < array1.Length; i++)
                {
                    result[i] = (byte)(array1[i] ^ array2[i]);
                }
                return result;
            }
            else
            {
                throw new ArgumentException();
            }
        }

    }
    
}
