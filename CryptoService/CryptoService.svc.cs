using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using CryptoLib;

namespace CryptoService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
   [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CryptoService : ICryptoService
    {
        	ICripto algoritam;
		
		public CryptoService()
		{
			algoritam = null;
		}

		public byte[] Crypt(byte[] input)
		{
			if (algoritam == null)
				return null;
			return algoritam.Crypt(input);
		}

		public byte[] Decrypt(byte[] input)
		{
			if (algoritam == null)
				return null;
			return algoritam.Decrypt(input);
		}

		public byte[] GenerateRandomIV()
		{
			if (algoritam == null)
				return null;
			return algoritam.GenerateRandomIV();
		}

		public byte[] GenerateRandomKey()
		{
			if (algoritam == null)
				return null;
			return algoritam.GenerateRandomKey();
		}

		public bool SetAlgorithm(string alg_name)
		{
			if (alg_name == null || alg_name == "")
				return false;
			if (alg_name.Equals("RC4"))
			{
				algoritam = new CryptoLib.RC4();
				return true;
			}
			if (alg_name.Equals("RSA"))
			{
				algoritam = new CryptoLib.RSA();
				return true;
			}
			if (alg_name.Equals("A5_2"))
			{
				algoritam = new CryptoLib.A5_2();
				return true;
			}
			if (alg_name.Equals("TigerHash")) 
			 {
				algoritam = new CryptoLib.TigerHash();
			 	return true;
			}
			return false;
		}

		public bool SetAlgorithmProperties(IDictionary<string, byte[]> specArguments)
		{
			if (algoritam == null)
				return false;
			return algoritam.SetAlgorithmProperties(specArguments);
		}

		public bool SetIV(byte[] input)
		{
			if (algoritam == null)
				return false;
			return algoritam.SetIV(input);
		}

		public bool SetKey(byte[] input)
		{
			if (algoritam == null)
				return false;
			return algoritam.SetKey(input);
		}
    }
}
