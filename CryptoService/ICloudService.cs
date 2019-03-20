using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CryptoService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ICloudService" in both code and config file together.
    [ServiceContract]
    public interface ICloudService
    {
        [OperationContract]
        Stream GetData(string name);

        [OperationContract]
        void StreamData(RemoteFileInfo rfi); 

        [OperationContract]
        string[] GetFileNames();
    }

    [MessageContract]
    public class RemoteFileInfo : IDisposable  
    {
        [MessageHeader(MustUnderstand = true)]
        public string FileName;

        [MessageBodyMember(Order = 1)]
        public Stream stream;

        public void Dispose()
        {
            if (stream != null)
            {
                stream.Close();
                stream = null;
            }
        }
    }
}
