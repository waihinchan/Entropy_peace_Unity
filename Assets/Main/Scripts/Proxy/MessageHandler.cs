using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Main.Scripts.Proxy
{
    class MessageHandler
    {
        private byte[] data = new byte[1024];
        private int startIndex = 0;//我们存取了多少个字节的数据在数组里面

        public byte[] Data
        {
            get { return data; }
        }
        public int StartIndex
        {
            get { return startIndex; }
        }
        public int RemainSize
        {
            get { return data.Length - startIndex; }
        }
        
        public void ReadMessage(int newDataAmount, Action<string,string> funInvoke)
        {
            startIndex += newDataAmount;
            while (true)
            {
                //粘包分包
                if (startIndex <= 4) return;
                int count = BitConverter.ToInt32(data, 0);
                if ((startIndex - 4) >= count)
                {
                    FuncCode funcCode = (FuncCode)BitConverter.ToInt32(data, 4);
                    Debug.Log("调用函数："+funcCode.ToString());
                    string modelStr = Encoding.UTF8.GetString(data, 12, count - 8);
                    funInvoke(funcCode.ToString(), modelStr);
                    Array.Copy(data, count + 4, data, 0, startIndex - 4 - count);
                    startIndex -= (count + 4);
                    return;
                }
                break;
            }
        }
        public static byte[] PackData(FuncCode funcCode, string data)
        {
            byte[] requestCodeBytes = BitConverter.GetBytes((int)funcCode);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            int dataAmount = requestCodeBytes.Length + dataBytes.Length;
            byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);
            return dataAmountBytes.Concat(requestCodeBytes).ToArray<byte>()
                .Concat(dataBytes).ToArray<byte>();
        }
    }
}