using System;
using System.Security.Cryptography;
using System.Text;

namespace DirectoryNavigator
{
    public class DirectoryHash
    {
        private MD5 _md5 = MD5.Create();


        public void TransformBlock(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            _md5.TransformBlock(bytes, 0, bytes.Length, bytes, 0);
        }

        public void TransformFinal()
        {                        
            _md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        }


    }
}
