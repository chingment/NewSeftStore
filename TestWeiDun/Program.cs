using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TestWeiDun
{

    class Program
    {
        //internal unsafe struct PKI_DATA
        //{
        //    internal int size;
        //    internal char* value;
        //}

        [DllImport("BioVein.Win32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FV_GetSdkVersion([MarshalAs(UnmanagedType.LPStr)] StringBuilder version);


        [DllImport("BioVein.Win32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FV_EnumDevice(IntPtr devNameList);

        [DllImport("BioVein.Win32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr FV_MatchFeatureEx([MarshalAs(UnmanagedType.LPArray)] byte[] matchFeature, [MarshalAs(UnmanagedType.LPArray)] byte[] regFeature, int regCnt, [MarshalAs(UnmanagedType.LPArray)] byte[] aiFeature, int aiCnt, byte flag, byte securityLevel, ref int diff, int aiBufLen);



        static void Main(string[] args)
        {
            try
            {
                string s = "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";

                string s1 = "EREAMcuOgE7/////yo/YBBoAAABweQ15VXl5VXl5eXl5eXrKeX95eXB5KHl4ecp5DXlteXJ5eV5VeXlDeQ2vd8ryeXl5dXl5cHlteXkobHl5KHp5eXl5eHl5eWx5eQ15eXl5DXl5eVJweXlVeXl5eXl5eXmGeXl5c3nuAOpQBnmmax/y8ms28PLy8vLyNfDw8vLyvXl5ee7yGnjuUHkoa3l5eXltVXl53Hd5eVA8BEq8eW15ebpZ5Xny4PIAeHl1eQ102GtDeCdZeXkNeXnceqaGankoeHl5VXl9SXkovHp5KHh5eW3ceIZ5r2t5eSh5eXmroeV6eUlreXl5eXnr8n1JeSh4hXl5eXnK8qJ3bXl5eX15eXl58HjlfXl5cCh3eXl53H155Xp5yhvqenl5KFB5KHl5KPHy8vLsqvJreVV5efHyUyjyAvJ9cHlzKNnyrnl5KHlDeXmvKfLycJR5eXl5be543PLdbnZ5cHl5KHrlAvDyDXl5eWRteet55fJ3inl5eXnrdPFuees5eXh5eXl53PJTeXkaaV55eXl5eeuHeXmmeHZ5eXl5eXkG8AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";

                byte[] matchFeature = Convert.FromBase64String(s1);
                byte[] regFeature = Convert.FromBase64String(s);
                byte[] aiFeature =new byte[512];
                int diff = 0;
                int aiBufLen = 0;
                var ret = FV_MatchFeatureEx(matchFeature, regFeature, 3, aiFeature, 1, 0x03, 0x04,ref diff, aiBufLen);
               
                //IntPtr intptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(10);

                // char[,] d = new char[10,64];
                // string[] d=new string[64];
                //var c = FV_EnumDevice(intptr);

                // IntPtr hDevDropEvent = c;
                // UInt32 byLen = 0;
                // Byte[] bufHandle = new Byte[IntPtr.Size];
                // GCHandle gch = GCHandle.Alloc(bufHandle, GCHandleType.Pinned);    //固定托管内存
                // Marshal.WriteIntPtr(Marshal.UnsafeAddrOfPinnedArrayElement(bufHandle, 0), hDevDropEvent);
                // bRet = UsbScan_ExchangData(bufHandle, ref byLen, API_USBSCAN_DEVEXIT);
                // gch.Free();

                // Marshal.p
                // char[] s = new char[32];
                //char[] s1 =new char[32];

                //StringBuilder s2 = new StringBuilder();
                //var A = FV_GetSdkVersion(s2);

                // var c = A;
            }
            catch (Exception ex)
            {
                var s2 = ex;
            }
        }
    }
}
