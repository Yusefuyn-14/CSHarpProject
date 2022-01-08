using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HWID
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Value() fonksiyonundan dönecek olan değer label1'e yazılıyor.
            label1.Text = Value();
        }

        private static string fingerPrint = string.Empty;
        public static string Value()
        {
            // fingerPrint değeri boş ise 
            if (string.IsNullOrEmpty(fingerPrint))
            {
                // fingerPrint'e cpuId biosId baseId videoId fonksiyonlarından
                // dönecek olan verileri MD5 ile HASH'layıp aktarıyor.
                fingerPrint = GetHash("CPU >> " + cpuId() + "\nBIOS >> " +
            biosId() + "\nBASE >> " + baseId() +

            videoId() + "\nMAC >> " + macId());
            }
            return fingerPrint;
        }
        private static string GetHash(string s)
        {
            // MD5 servisi oluşturuluyor.
            MD5 sec = new MD5CryptoServiceProvider();
            ASCIIEncoding enc = new ASCIIEncoding();
            // Gelen s değerini byte'a çevirip byte array'ine atıyor.
            byte[] bt = enc.GetBytes(s);
            // GetHexString fonksiyonuna byte array'ini verip string'e cevirip geri döndürüyor.
            return GetHexString(sec.ComputeHash(bt));
        }

        private static string GetHexString(byte[] bt)
        {
            string s = string.Empty;
            for (int i = 0; i < bt.Length; i++)
            {
                byte b = bt[i];
                int n, n1, n2;
                n = (int)b;
                n1 = n & 15;
                n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + (int)'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char)(n1 - 10 + (int)'A')).ToString();
                else
                    s += n1.ToString();
                if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "-";
            }
            return s;
        }
        #region Original Device ID Getting Code
        // Aşırı yüklenilmiş identifier fonksiyonu.
        // Diğer fonksiyon ile aynı işlemi yapıyor.
        private static string identifier
        (string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            string result = "";
            System.Management.ManagementClass mc =
        new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() == "True")
                {
                    
                    if (result == "")
                    {
                        try
                        {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return result;
        }
        // Bu fonksiyon verilen sınıftan verilen özelliğin değerini almaya çalışıyor. Değeri döndürüyor.
        private static string identifier(string wmiClass, string wmiProperty)
        {

            string result = "";
            // Management-Class&ObjectCollection&Object sınıfları ile WMI'da işlemler yapıyor.
            System.Management.ManagementClass mc =
        new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {

                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }
        private static string cpuId()
        {
            // Bu method win32_Process sınıfından UniqueId değerini alıyor boş ise ProcessorId değerini alıyor
            // Bu değerde boş ise Name boş ise Manufaturer değerini alıp MaxClockSpeed değeriyle topluyor
            // identifier() fonksiyonu verilen sınıftan verilen değeri almaya çalışıyor.
            string retVal = identifier("Win32_Processor", "UniqueId");
            if (retVal == "")
            // Sınıfın UniqueID özelliği boş ise ProcessorId değerini almaya gidiyor.
            {
                retVal = identifier("Win32_Processor", "ProcessorId");
                // Sınıfın ProcessorId özelliği boş ise Name değerini almaya gidiyor.
                if (retVal == "")
                {
                    retVal = identifier("Win32_Processor", "Name");
                    // Sınıfın Name özelliği boş ise Manufacturer değerini almaya gidiyor.
                    if (retVal == "")
                    {
                        retVal = identifier("Win32_Processor", "Manufacturer");
                    }
                    // MaxClockSpeed değerini ekleyerek ekstra güvenlik yaptığını söylemiş :D
                    retVal += identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return retVal;
        }
        // Bios sınıfından bir veri elde etmek için böyle bir fonksiyon hazırlamış.
        private static string biosId()
        {
                // Win32_BIOS sınıfından Manufacturer özelliğinin değeri alınıyor.
            return identifier("Win32_BIOS", "Manufacturer")
                // Win32_BIOS sınıfından SMBIOSBIOSVersion özelliğinin değeri alınıyor.
            + identifier("Win32_BIOS", "SMBIOSBIOSVersion")
                // Win32_BIOS sınıfından IdentificationCode özelliğinin değeri alınıyor.
            + identifier("Win32_BIOS", "IdentificationCode")
                // Win32_BIOS sınıfından SerialNumber özelliğinin değeri alınıyor.
            + identifier("Win32_BIOS", "SerialNumber")
                // Win32_BIOS sınıfından ReleaseDate özelliğinin değeri alınıyor.
            + identifier("Win32_BIOS", "ReleaseDate")
                // Win32_BIOS sınıfından Version özelliğinin değeri alınıyor.
            + identifier("Win32_BIOS", "Version");
        }
        // Disk sınıfından gerekli verileri alıyor.
        private static string diskId()
        {
            return identifier("Win32_DiskDrive", "Model")
            + identifier("Win32_DiskDrive", "Manufacturer")
            + identifier("Win32_DiskDrive", "Signature")
            + identifier("Win32_DiskDrive", "TotalHeads");
        }
        // Anakart sınıfından gerekli verileri alıyor.
        private static string baseId()
        {
            return identifier("Win32_BaseBoard", "Model")
            + identifier("Win32_BaseBoard", "Manufacturer")
            + identifier("Win32_BaseBoard", "Name")
            + identifier("Win32_BaseBoard", "SerialNumber");
        }
        // VideoController sınıfından gerekli verileri alıyor.
        private static string videoId()
        {
            return identifier("Win32_VideoController", "DriverVersion")
            + identifier("Win32_VideoController", "Name");
        }
        // Win32_NetworkAdapterConfiguration sınıfından gerekli verileri alıyor.
        private static string macId()
        {
            return identifier("Win32_NetworkAdapterConfiguration",
                "MACAddress", "IPEnabled");
        }
        #endregion
    }
}

