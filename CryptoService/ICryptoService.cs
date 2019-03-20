using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace CryptoService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface ICryptoService
    {
        [OperationContract]
        bool SetAlgorithm(string alg_name);

        [OperationContract]
        bool SetKey(byte[] input);

        [OperationContract]
        byte[] GenerateRandomKey();

        [OperationContract]
        bool SetIV(byte[] input);

        [OperationContract]
        byte[] GenerateRandomIV();

        [OperationContract]
        byte[] Crypt(byte[] input);

        [OperationContract]
        byte[] Decrypt(byte[] input);

        [OperationContract]
        bool SetAlgorithmProperties(IDictionary<string, byte[]> specArguments);
        
    }
}
