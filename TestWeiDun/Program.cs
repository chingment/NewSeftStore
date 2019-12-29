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


      


        static void Main(string[] args)
        {
            try
            {
                IntPtr intptr = System.Runtime.InteropServices.Marshal.AllocHGlobal(10);

                // char[,] d = new char[10,64];
               // string[] d=new string[64];
                var c = FV_EnumDevice(intptr);

                IntPtr hDevDropEvent = c;
               // UInt32 byLen = 0;
                Byte[] bufHandle = new Byte[IntPtr.Size];
                GCHandle gch = GCHandle.Alloc(bufHandle, GCHandleType.Pinned);    //固定托管内存
                Marshal.WriteIntPtr(Marshal.UnsafeAddrOfPinnedArrayElement(bufHandle, 0), hDevDropEvent);
               // bRet = UsbScan_ExchangData(bufHandle, ref byLen, API_USBSCAN_DEVEXIT);
               // gch.Free();

                // Marshal.p
                // char[] s = new char[32];
                char[] s1 =new char[32];

                StringBuilder s2 = new StringBuilder();
                var A = FV_GetSdkVersion(s2);

               // var c = A;
            }
            catch (Exception ex)
            {
                var s2 = ex;
            }
        }
    }
}
