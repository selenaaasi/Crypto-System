using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections;
using System.Security.Cryptography;


namespace CryptoLib
{
  public class RSA:ICripto
    {
        public BigInteger Message { get; set; }
        public BigInteger P { get; set; }
        public BigInteger Q { get; set; }
        public BigInteger N { get; set; }
        public BigInteger Phi { get; set; }
        public BigInteger E { get; set; }
        public BigInteger D { get; set; }
        public byte[] IV { get; set; }
        private ulong counter; 
        private bool ctr_set;
        private BigInteger[] ibi;

        public RSA()
        {

            counter = 0;
            ctr_set = false;
            Random r = new Random();
          
            // Generise random p i q
            byte[] randP = new byte[4];
            byte[] randQ = new byte[4];
            r.NextBytes(randP);
            r.NextBytes(randQ);
            P = BigInteger.Abs(new BigInteger(randP));
            Q = BigInteger.Abs(new BigInteger(randQ));

            P = BigInteger.Parse("3295723131231232133789379377373737373773");
            Q = BigInteger.Parse("85012313320321831283092183092183092189032183210");

            GenerateRSA();
            this.IV = GenerateRandomIV();
        }

      
        public bool MillerRabinTest(BigInteger number, int k)
        {
            if (number == 2 || number == 3)
                return true;
            if (number < 2 || number % 2 == 0)
                return false;

            BigInteger d = number - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            // k predstavlja preciznost sa kojom se odredjuje
            for (int i = 0; i < k; i++)
            {
                Random rand = new Random();
                byte[] _a = new byte[number.ToByteArray().LongLength];
                BigInteger a;

                do
                {
                    // izabere se random integer izmedju [2, n - 2]
                    // vrti se petlja dok se ne pronadje dati broj
                    rand.NextBytes(_a);
                    a = new BigInteger(_a);
                }
                while (a < 2 || a >= number - 2);

                BigInteger x = BigInteger.ModPow(a, d, number);

                if (x == 1 || x == number - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, number);

                    if (x == 1)
                        return false;
                    if (x == number - 1)
                        break;
                }

                if (x != number - 1)
                    return false;
            }
            return true;
        }

        public BigInteger GenerateNearestPrime(BigInteger number)
        {
            // Dok se ne pronadje prost broj
            while (!MillerRabinTest(number, 10))
            {
                number++;
            }
            return number;
        }

        private void CalculateN()
        {
            if (MillerRabinTest(P, 10) && MillerRabinTest(Q, 10))
            {
                // Ako su prosti brojevi izracuna se moduo N
                N = P * Q;
            }
            else
            {
                // Ako nisu prosta onda se nadju najblizi prosti
                P = GenerateNearestPrime(P);
                Q = GenerateNearestPrime(Q);
                N = P * Q;
            }
        }

        
        private void CalculatePhi()
        {
            Phi = (P - 1) * (Q - 1);
        }

        
        public void GeneratePrivateKey()
        {
            int k = 1;
            while (((Phi * k + 1) % E) != 0)
            {
                k++;
            }
            D = ((Phi * k) + 1) / E;
        }

     
        public void GeneratePublicKey()
        {
            E = 2;
            while (E < Phi)
            {
                // E i Phi treba da budu uzajamno prosti
                // 1 < e < phi
                if (BigInteger.GreatestCommonDivisor(E, Phi) == 1) break;
                else E++;
            }
        }

        public void GenerateRSA()
        {
            CalculateN();
            CalculatePhi();
            GeneratePublicKey();
            GeneratePrivateKey();

        }

        public byte[] GenerateRandomKey()
        {
            //GeneratePublicKey();
            //GeneratePrivateKey();
            BigInteger result;
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] randomNumber = new byte[64];
            rng.GetBytes(randomNumber);
            result = new BigInteger(randomNumber);
            result = BigInteger.Abs(result);
            return randomNumber;

        }

        public bool SetKey(byte[] input)
        {
            BigInteger Epom = new BigInteger(input);
            Epom = BigInteger.Abs(Epom);
            if (Epom > 0 && Epom < Phi && BigInteger.GreatestCommonDivisor(Epom, Phi) == 1)
            {
                this.E = Epom;
                return true;

            }
            else
                return false;

        }

        public  byte[] GenerateRandomIV()
        {
            Random rnd = new Random();
            IV = new byte[8];
            rnd.NextBytes(IV);
            return IV;

        }

        public bool SetIV(byte[] input)
        {
            if (input.Length == IV.Length)
            {
                IV.CopyTo(input, 0);
                return true;
            }
            else
                return false;
        }

        public  bool SetAlgorithmProperties(IDictionary<string, byte[]> specArguments)
        {
            if (specArguments.ContainsKey("p"))
            // P = BitConverter.ToUInt32(specArguments["p"], 0);
                P = new BigInteger(specArguments["p"]);
             
            if (specArguments.ContainsKey("q"))
               // Q = BitConverter.ToUInt32(specArguments["q"], 0);
                Q = new BigInteger(specArguments["q"]);
               
            if (specArguments.ContainsKey("e"))
               // E = BitConverter.ToUInt32(specArguments["e"], 0);
                E = new BigInteger(specArguments["e"]);

            GenerateRSA();

            return true;

        }

        public byte[] Crypt(byte[] input)
        {

            if (!ctr_set)
            {
                BigInteger i = new BigInteger(input);
                if (i >= N)
                {
                    ibi = new BigInteger[input.Length];
                    int next_poz=0;
                    byte[] b = new byte[1];
                    byte[] o = new byte[2];
                    byte[] output = new byte[input.Length];
                    BigInteger bi;
                    for (int j = 0; j < input.Length; j++)
                    {
                        byte[] proveri = new byte[1];
                        Buffer.BlockCopy(input, j, proveri, 0, 1);

                            bi = CryptRSA(new BigInteger(input[j]));
                            ibi[j] = bi;
                            o = bi.ToByteArray();
                            //  Buffer.BlockCopy(o, 0, output, next_poz, o.Length);
                            Buffer.BlockCopy(o, 0, output, j, 1);
                            //next_poz += o.Length;
                        
                    }
                    return output;
                }
                else
                {
                    BigInteger rez = CryptRSA(i);
                    return rez.ToByteArray();
                }
            }
            else
                return CryptRSAWithCTR(input);

        }

        public byte[] Decrypt(byte[] output)
        {
            if (!ctr_set)
            {
                BigInteger i = new BigInteger(output);
                if (i >= N)
                {

                    int next_poz = 0;
                    byte[] o = new byte[100];
                    byte[] rezultat = new byte[100];
                    for (int j = 0; j < ibi.Length; j++)
                    {
                        BigInteger bi = DecryptRSA(ibi[j]);

                        o = bi.ToByteArray();
                        Buffer.BlockCopy(o, 0, rezultat, next_poz, o.Length);
                        next_poz += o.Length;
                    }
                    return rezultat;
                    //int next_poz = 0;
                    //byte[] outp = new byte[2];
                    //byte[] rez = new byte[output.Length];
                    //for (int j=0;j<output.Length;j++)
                    //{
                     
                    //    BigInteger o = DecryptRSA(new BigInteger(output[j]));
                    //        outp = o.ToByteArray();
                    //       Buffer.BlockCopy(outp, 0, rez, next_poz, outp.Length);
                    //        next_poz += outp.Length;

                    //}
                    //return rez;

                }
                else
                {
                    BigInteger rez = DecryptRSA(i);
                    return rez.ToByteArray();
                }
            }
            else
            {
                return DecryptRSAWithCTR(output);
            }
        }

        public BigInteger CryptRSA(BigInteger input)
        {

            return BigInteger.ModPow(input, E, N);
        }

        public BigInteger DecryptRSA(BigInteger output)
        {
            return BigInteger.ModPow(output, D, N);
        }

        public byte[] CryptRSAWithCTR(byte[] pom)
        {
           // byte[] pom = input.ToByteArray();
            byte[] cipher = new byte[pom.Length];
            byte[] mod;

            for (int i = 0; i < (pom.Length / 8) * 8; i += 8) //jer je vi duzine 8 uzima prvih 8 pa sledecih 8 i sve tako
            {
                mod = this.Crypt(this.ExclusiveOR(this.IV, BitConverter.GetBytes(this.counter))); //ovde mozda treba obican +??
                for (int j = i; j < i + 8; j++)
                {
                    cipher[j] = (byte)(pom[j] ^ mod[j % 8]);
                }

                if (counter == ulong.MaxValue)
                    counter = 0;
                else
                    counter++;
            }

            counter = 0;

            if (pom.Length % 8 == 0)
            {
                counter = 0;
                return cipher;
            }
            else //ako input.length nije deljiva sa 8
            {
                mod = this.Crypt(this.ExclusiveOR(this.IV, BitConverter.GetBytes(this.counter)));
                for (int i = (pom.Length / 8) * 8; i < pom.Length; i++)
                {
                    cipher[i] = (byte)(pom[i] ^ mod[i % 8]);
                }

                counter = 0;
                return cipher;
            }


        }
        public byte[] DecryptRSAWithCTR(byte[] output)
        {
            return CryptRSAWithCTR(output);
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
