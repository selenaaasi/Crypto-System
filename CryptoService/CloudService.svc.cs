using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;
using CryptoLib;

namespace CryptoService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "CloudService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select CloudService.svc or CloudService.svc.cs at the Solution Explorer and start debugging.
    public class CloudService : ICloudService
    {
       private RSA rsa;
		string dir_path;

		public CloudService()
		{
			dir_path = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\Desktop\\Server\\"; 
			rsa = new RSA();
		}

		public Stream GetData(string name) 
		{
			
			if (!Directory.Exists(dir_path))
				Directory.CreateDirectory(dir_path);

			Stream stream = File.Open(dir_path + name, FileMode.Open, FileAccess.Read, FileShare.Read);
			return stream;
		}

		public string[] GetFileNames() 
		{
			return Directory.GetFiles(dir_path);
		}
      
		public void StreamData(RemoteFileInfo rfi) 
		{    
			Stream stream = null;
			BinaryReader br = null;
			BinaryWriter bw = null;

			try
			{
				stream = rfi.stream;
				br = new BinaryReader(stream); 
				bw = new BinaryWriter(File.Open(dir_path + rfi.FileName, FileMode.Create, FileAccess.Write, FileShare.None));

				int buffer_size = 2048;
				byte[] buffer = new byte[buffer_size];
				int bytesRead;
				while ((bytesRead = br.Read(buffer, 0, buffer.Length)) > 0)
					if (bytesRead < buffer_size)
					{
						byte[] new_buffer = new byte[bytesRead];
						Buffer.BlockCopy(buffer, 0, new_buffer, 0, bytesRead);
						bw.Write(rsa.Crypt(new_buffer));
					}
					else
						bw.Write(rsa.Crypt(buffer));
			}
			catch
			{

			}
			finally
			{
				if (stream != null)
				{
					stream.Dispose();
					stream.Close();
					stream = null;
				}
				if(br != null)
				{
					br.Dispose();
					br.Close();
					br = null;
				}
				if(bw != null)
				{
					bw.Dispose();
					bw.Close();
					bw = null;
				}
			}	
		}







    }
}
