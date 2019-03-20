using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HashAlgorithm = System.Security.Cryptography.HashAlgorithm;

namespace CryptoLib
{
    public partial class TigerHash:ICripto
    {
        private ulong h0;
        private ulong h1;
        private ulong h2;
        

        public TigerHash()
        {
            this.h0 = 0x0123456789ABCDEFUL;
            this.h1 = 0xFEDCBA9876543210UL;
            this.h2 = 0xF096A5B4C3B2E187UL;        
        }

        public byte[] prosiriPoruku(byte[] input)
        {
            if(input.Length*8 % 512 == 0)
            {
                return input;
            }
            else
            {
                int prosirenje = (512 - input.Length * 8 % 512) / 8 ;
                byte[] output = new byte[input.Length + prosirenje];
                Array.Clear(output, 0, output.Length);                           
                Buffer.BlockCopy(input, 0, output, 0, input.Length);
                return output;
            }         
        }

       public byte[] Crypt(byte[] input)
        {
            byte[] prosirenInput = this.prosiriPoruku(input);
            byte[] segment = new byte[64];
            ulong[] w = new ulong[8];

            for(int i=0; i<prosirenInput.Length/64; i++)
            {
                Buffer.BlockCopy(prosirenInput, i * 64, segment, 0, 64);
                for(int j=0; j<8; j++)
                {
                    w[j] = BitConverter.ToUInt64(segment, j * 8);
                }

                ulong a = this.h0;
                ulong b = this.h1;
                ulong c = this.h2;

                for(int ii = 0; ii < 4; ii++)
                {
                    for(int j = 0; j < 8; j++)
                    {
                        c = c ^ w[j];
                        byte[] cbytes = BitConverter.GetBytes(c);                     
                       
                        a = a - (t1[(int)cbytes[0]] ^ t2[(int)cbytes[2]] ^ t3[(int)cbytes[4]] ^ t4[(int)cbytes[6]]);
                        b = b + (t4[(int)cbytes[1]] ^ t3[(int)cbytes[3]] ^ t2[(int)cbytes[5]] ^ t1[(int)cbytes[7]]);
                        b = b * (ulong)(ii + 1);
                    }

                    if(ii == 0)
                    {
                        w[0] = w[0] - (w[7] ^ 0xA5A5A5A5A5A5A5A5UL);
                        w[1] = w[1] ^ w[0];
                        w[2] = w[2] + w[1];
                        w[3] = w[3] - (w[2] ^ ((~w[1]) << 19));
                        w[4] = w[4] ^ w[3];
                        w[5] = w[5] + w[4];
                        w[6] = w[6] - (w[5] ^ ((~w[4]) >> 23));
                        w[7] = w[7] ^ w[6];
                    }
                    else if(ii == 1)
                    {
                        w[0] = w[0] + w[7];
                        w[1] = w[1] - (w[0] ^ ((~w[0]) << 19));
                        w[2] = w[2] ^ w[1];
                        w[3] = w[3] + w[2];
                        w[4] = w[4] - (w[3] ^ ((~w[2]) >> 23));
                        w[5] = w[5] ^ w[4];
                        w[6] = w[6] + w[5];
                        w[7] = w[7] - (w[6] ^ 0x0123456789ABCDEFUL);
                    }
                    
                }

                h0 = h0 + a;
                h1 = h1 + b;
                h2 = h2 + c;
            }

            byte[] output = new byte[24];
            
            Buffer.BlockCopy(BitConverter.GetBytes(h0), 0, output, 0, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(h1), 0, output, 8, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(h2), 0, output, 16, 8);

            return output;
        }

       public bool SetKey(byte[] input)
        {
            throw new ArgumentException("TigerHash dont use that kind of key.");


        }

       public byte[] GenerateRandomKey()
        {
            throw new ArgumentException("TigerHash dont use that kind of key.");

        }

       public bool SetIV(byte[] input)
     {
         throw new ArgumentException("TigerHash dont use IV vector.");

     }

       public byte[] GenerateRandomIV()
     {
         throw new ArgumentException("TigerHash dont use IV vector.");
     }

       public bool SetAlgorithmProperties(IDictionary<string, byte[]> specArguments)
     {
         throw new ArgumentException("TigerHash hasnt arguments to set.");

     }

       public byte[] Decrypt(byte[] output)
     {

         throw new ArgumentException("TigerHash can't be crypted.");

     }
      



   
    }
}
