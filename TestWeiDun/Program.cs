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
        public static extern IntPtr FV_MatchFeatureEx([MarshalAs(UnmanagedType.LPArray)] byte[] matchFeature, [MarshalAs(UnmanagedType.LPArray)] byte[] regFeature, int regCnt, [MarshalAs(UnmanagedType.LPArray)] byte[] aiFeature, int aiCnt, byte flag, byte securityLevel, UInt32 diff, UInt32 aiBufLen);



        static void Main(string[] args)
        {
            try
            {
                string s = @"EREAMWovJD3\/\/\/\/\/yo\/YBBoAAAB5KCd5bXmLtW55eXlzeUMoeH958Xp5eXB5eHlVeXl7eRooeW12eXl5VXl5dyh5DXnZenl5eXlVE2x5eXhebXl5eXl5aQ3wKHh5f3l5eXl5eXt57nl5r3h5eXl5eXl5eWTl8vJ4VXkEAHh5eSjL8vLy8vJ98PJ9eXnr2G553H3c8nnZUHl5hnsceeV4eW15KPLy8nooaXnceXl5eVXr8vJ5ymt57oZ5eSh0VXlMeeVrKPJ0eQFQeVV5THnyadx48QjydXl5eUnhaxgUedyheXl5eXlrKA26bXnKdXl5bXnKhnl5XHl5Xnl5eXt5UIV5bSh5eXl5ecp43Hd7ynhVeXl5eXk6J4Z5djcofnl5eXmm8vJ6eXmFeX15eXmv8gJQeXkoEfB0eXnc8nV5oHko6vLy8gh4T2V5eU955fJ9edny8ndteXDreXlweXl5KI15dHlt7nh5eXl5eXl\/eXl5ePF3eXkEeXl5fK933Mmta3l5BGN5G3jc8qHceW95edzyvHh5N+t9auV9eSjrC1F5eXkNeXt5eXmvhnkA\/wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAREQAxtusKF\/\/\/\/\/\/Kj9gEGgAAAHl7eXmveYt5eXl5eXkoenlti3lqdnkNeXl5DXl5eG158lBwi3l5eXB5eXl4KKikdHBVeXl5H3WvUHl5dXWjdHl5eboO6vJ6eXl5U3l5eSir8vKkenl5KHV5eXkoeWR5KHl5eXt5eXl5eXlzeXl5DVJ3eXl5cG95dnkEKPLy8vB07PJ9DHl53vIa8ODs8hqioXl55e9rea93eRJ5efB3XvA9dXmvenl5eXnr8vJZJFx55V15eXkorrzyesxTeOVreXl5THl13K6kd3nrGicAeXV5deUENmzKfevg8ll5eXY6BGxR63mvhnl2eXl5nWd563d5eXd5eV55KDNXeU95eSh5eXlVefARbChreXl5eXl5bdx3AyQ6ZXl5eXl5yglreShRbUl5eXl5eeryfXkEbHhteXl5efLy2Hp5eWzYWXl5KPBuea94eaNQ8vIIxhqUeXlVeOvyayjb8hp4eXl5cwjyG3h5eXkKeHl5Onl3eSh5eXl5DXlweYR5bSh5eXh5eVV5dih3eXZ5eQx2eXl5eXWCeXZ5eSh4eADDAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABERADEXh82K\/\/\/\/\/8qP2AQaAAAAeVV5eXB5eSh5eXl5d3l5Xnl8eYJ5eXl5ynh5eXB5dXlOKHl5eHkoeXltDWwoNQh45Xh5eXkNvBTyeSUBKGt5eXl5eTZ53Hh5DXl5eXl5eXl2ee54eQ15eXl5KHh5dXkreXlVeXl5eV51eXleoHmvGnlwedkadXl569fy8vLy8gny8mt5KGvu8ifwbeuhecry8vJ6c3l5oHkoeHkoSPLseXl5eZp5eXl5dVV53HDrJ3nxeHl5pnhVedwb8lnKvDh58mt5bHnuJPJ03Cjy8lB5KHh5anmvd6B53IZ5eQR5KGt53CRueTp5eXl8efCheSjyeHlVeXl5euV3fnnZa3l5eXl5ynifeSR5vXR5eXl5eeryfXkNAHl9eXl5eevy8nh5r3coenl5edx0edx5ea962Xp5edxQeXnueXnc6vIabHnybHl53Hjl8gg38vJQdXl5Utx08VN4eXkAhsp3eXzcUHl5eXl5eXINd8p3HGl5eV55eXl7yrzyyXdteXnYVXl5eQ3ghu4pe3l5n+7yenleDXoDeXl5eR9SAP0AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";


                byte[] matchFeature = new byte[512];
                byte[] regFeature = new byte[1356];
                byte[] aiFeature = new byte[512];
                UInt32 diff = 0;
                UInt32 aiBufLen = 0;
                var ret = FV_MatchFeatureEx(matchFeature, regFeature, 3, aiFeature, 1, 0x03, 0x06, diff, aiBufLen);
               
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
