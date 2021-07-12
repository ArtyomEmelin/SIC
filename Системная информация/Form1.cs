using MetroFramework.Forms;
using Microsoft.Win32;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Management;
using Microsoft.VisualBasic.Devices;
using System.IO;

namespace Системная_информация
{
    public partial class Form1 : MetroForm
    {


        public Form1()
        {
            InitializeComponent();
            ShowProcess();

        }

        private void Form1_Load(object sender, EventArgs e) // ЗАГРУЗКА ФОРМЫ
        {
            timer1.Start();
            timer2.Start();
            CPUCPU();
            ShowProcess();
            Drivers();
            NETwork();
            GPU();
            User();
            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.AllowUserToResizeRows = false;
             
            cpucounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            memcounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");

            if (MetroFramework.MetroMessageBox.Show(this, "You must agree to the processing of information from your personal computer. By clicking the 'OK' button, you agree to the collection of information from your personal computer, by clicking the 'Cancel' button, the program will automatically close", "Terms of use", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                
            }
            else
            {
                Application.Exit();
            }
        }



        private void timer1_Tick(object sender, EventArgs e) // ТАЙМЕР БЫСТРЫЙ
        {
            metroLabel6.Text = "Count of core CPU: " + CPU();
            metroLabel7.Text = CPUname();

            string cpuvstr = CPUUsage();
            int pos = cpuvstr.IndexOf(',');
            if (pos > 0)
            {
                string cpuvstr2 = cpuvstr.Remove(cpuvstr.IndexOf(','), cpuvstr.Length - (cpuvstr.IndexOf(',') + 1));
                metroLabel4.Text = cpuvstr2;
                int indexP = cpuvstr2.IndexOf("%");
                cpuvstr2 = cpuvstr2.Remove(indexP);
                int cpuv = int.Parse(cpuvstr2);
                metroProgressBar1.Value = cpuv;
            }


            string memvstr = MemUsage();
            int posm = memvstr.IndexOf(',');
            if (posm > 0)
            {
                string memvstr2 = memvstr.Remove(memvstr.IndexOf(','), memvstr.Length - (memvstr.IndexOf(',') + 1));
                metroLabel5.Text = memvstr2;
                int indexPM = memvstr2.IndexOf("%");
                memvstr2 = memvstr2.Remove(indexPM);
                int memv = int.Parse(memvstr2);
                metroProgressBar2.Value = memv;
            }
            metroLabel11.Text = "Mem usage: ";

            metroLabel10.Text = Convert.ToString(DateTime.Now);
            metroLabel15.Text = Convert.ToString(DateTime.Now);
            metroLabel18.Text = Convert.ToString(DateTime.Now);
            metroLabel19.Text = Convert.ToString(DateTime.Now);
            metroLabel20.Text = Convert.ToString(DateTime.Now);
            metroLabel21.Text = Convert.ToString(DateTime.Now);
            metroLabel22.Text = Convert.ToString(DateTime.Now);


            String strResult = String.Empty;
            strResult += Convert.ToString(Environment.TickCount / 86400000) + " days, ";
            strResult += Convert.ToString(Environment.TickCount / 3600000 % 24) + " hours, ";
            strResult += Convert.ToString(Environment.TickCount / 120000 % 60) + " minutes, ";
            strResult += Convert.ToString(Environment.TickCount / 1000 % 60) + " second";
            metroLabel9.Text = "Time since CPU started: " + strResult;

            metroLabel8.Text = "Base frequency: " + CPUMHZ();

            getAvailableRAM();
            MemMem();
            User();
            GPU();
        }

        private void timer2_Tick(object sender, EventArgs e) // ТАЙМЕР МЕДЛЕННЫЙ
        {
            dataGridView1.Rows.Clear();
            ShowProcess();
            Drivers();
            CPUCPU();
            NETwork();
        }


        // ПРОЦЕССЫ НАЧАЛО

        int procid = 0;
        int numstr = 0;

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            int nulls = 0;
            numstr = dataGridView1.CurrentRow.Index;
            int prosnumstr = Convert.ToInt32(dataGridView1[0, numstr].Value);
            procid = prosnumstr;
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            Process processIdKill = Process.GetProcessById(procid);
            processIdKill.Kill();
            dataGridView1.Rows.RemoveAt(numstr);
            dataGridView1.Refresh();
        }

        private void metroTabPage1_Click(object sender, EventArgs e)
        {

        }

        public void ShowProcess()
        {
            timer1.Start();
            if (timer1.Enabled == true)
            {
                var allProcess = from pr in Process.GetProcesses(".")
                                 orderby pr.Id
                                 select pr;
                foreach (var proc in allProcess)
                {

                    string[] bigtable = { "" + proc.Id, "" + proc.ProcessName, "" + proc.WorkingSet64 / 1000000 + " MB", "" + proc.VirtualMemorySize64 / 1000000 + " MB", "" + proc.BasePriority };

                    dataGridView1.Rows.Add(bigtable);

                    dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                }
                metroLabel3.Text = "Process count: " + allProcess.Count();
            }
        }

        // ПРОЦЕССЫ КОНЕЦ

        // ПРОЦЕССОР НАЧАЛО

        PerformanceCounter cpucounter;

        public string CPUname()
        {
            RegistryKey registrykeyHKLM = Registry.LocalMachine;
            string keyPath = @"HARDWARE\DESCRIPTION\System\CentralProcessor\0";
            RegistryKey registrykeyCPU = registrykeyHKLM.OpenSubKey(keyPath, false);
            string MHz = registrykeyCPU.GetValue("~MHz").ToString();
            string ProcessorNameString = (string)registrykeyCPU.GetValue("ProcessorNameString");
            registrykeyCPU.Close();
            registrykeyHKLM.Close();
            string countCP = Convert.ToString(ProcessorNameString);
            return countCP;
        }

        public string CPUMHZ()
        {
            RegistryKey registrykeyHKLM = Registry.LocalMachine;
            string keyPath = @"HARDWARE\DESCRIPTION\System\CentralProcessor\0";
            RegistryKey registrykeyCPU = registrykeyHKLM.OpenSubKey(keyPath, false);
            string MHz = registrykeyCPU.GetValue("~MHz").ToString();
            registrykeyCPU.Close();
            registrykeyHKLM.Close();
            string countCP = MHz;
            return countCP;
        } 

        public string CPU()
        {
            string cpu1 = Convert.ToString(Environment.ProcessorCount);
            return cpu1.ToString();
        }

        public string CPUUsage()
        {
            return cpucounter.NextValue().ToString() + " %";
        }

        private void metroTabPage7_Click(object sender, EventArgs e)
        {
            
        }

        public void CPUCPU()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                metroLabel26.Text = "Socket: " + queryObj["SocketDesignation"];
            }
        }

        // ПРОЦЕССОР КОНЕЦ

        //RAM НАЧАЛА

        PerformanceCounter memcounter;
        public string MemUsage()
        {
            return memcounter.NextValue().ToString() + " %";
        }

        public void getAvailableRAM()
        {
            PerformanceCounter ramavailable = new PerformanceCounter("Memory", "Available MBytes");
            double ramvail = ramavailable.NextValue() / 1024;
            string ramv = String.Format("Memory available: {0:0.#} GB", ramvail);
            metroLabel14.Text = ramv;

            ComputerInfo CI = new ComputerInfo();
            ulong mem = ulong.Parse(CI.TotalPhysicalMemory.ToString());
            mem = mem / (1024 * 1024) / 1000;
            string memsize = String.Format("Memmory size: {0:0.#} GB", mem);
            metroLabel16.Text = memsize;

            double usemem = mem - ramvail;
            string usememgbstr = String.Format("Memory usage: {0:0.#} GB", usemem);
            metroLabel13.Text = usememgbstr;

        }

        void MemMem()
        {
            ManagementObjectSearcher searcher12 = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_PhysicalMemory");

            foreach (ManagementObject queryObj in searcher12.Get())
            {
                metroLabel12.Text = "Bank: " + queryObj["BankLabel"];
                metroLabel23.Text = "Capacity: " + Math.Round(System.Convert.ToDouble(queryObj["Capacity"]) / 1024 / 1024 / 1024, 2) + "GB";
                metroLabel24.Text = "Speed: " + queryObj["Speed"];
                metroLabel25.Text = "Manufacturer: " + queryObj["Manufacturer"];
            } 
        } 

        private void metroLabel7_Click(object sender, EventArgs e)
        {

        }

        //RAM КОНЕЦ

        // ДИСКИ НАЧАЛО

        void Drivers()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            listBox1.Items.Clear();
            foreach (DriveInfo d in allDrives)
            {
                listBox1.Items.Add("Drive: " + d.Name);
                listBox1.Items.Add("Drive type: " + d.DriveType);
                if (d.IsReady == true)
                {
                    listBox1.Items.Add("Volume label: " + d.VolumeLabel);
                    listBox1.Items.Add("File system: " + d.DriveFormat);
                    listBox1.Items.Add("Available space to current user: " + d.AvailableFreeSpace / (1024 * 1024) / 1024 + "GB");
                    listBox1.Items.Add("Total available space: " + d.TotalFreeSpace / (1024 * 1024) / 1024 + "GB");
                    listBox1.Items.Add("Total size of drive: " + d.TotalSize / (1024 * 1024) / 1024 + "GB") ;
                    listBox1.Items.Add("---------------------------------------------------------");
                }
                else
                {
                    listBox1.Items.Add("---------------------------------------------------------");
                }
            }
        }

        // ДИСКИ КОНЕЦ

        // СЕТЬ НАЧАЛО

        public void NETwork()
        {
            ManagementObjectSearcher searcher =
   new ManagementObjectSearcher("root\\CIMV2",
   "SELECT * FROM Win32_NetworkAdapterConfiguration");

            foreach (ManagementObject queryObj in searcher.Get())
            {
                listBox2.Items.Add("---------------------------------------------------------");
                listBox2.Items.Add("Caption: " + queryObj["Caption"]);

                if (queryObj["DefaultIPGateway"] == null)
                    listBox2.Items.Add("Default IP gateway: " + queryObj["DefaultIPGateway"]);
                else
                {
                    String[] arrDefaultIPGateway = (String[])(queryObj["DefaultIPGateway"]);
                    foreach (String arrValue in arrDefaultIPGateway)
                    {
                        listBox2.Items.Add("Default IP gateway: " + arrValue);
                    }
                }

                if (queryObj["DNSServerSearchOrder"] == null)
                    listBox2.Items.Add("DNS server search order: " + queryObj["DNSServerSearchOrder"]);
                else
                {
                    String[] arrDNSServerSearchOrder = (String[])(queryObj["DNSServerSearchOrder"]);
                    foreach (String arrValue in arrDNSServerSearchOrder)
                    {
                        listBox2.Items.Add("DNS server search order: " + arrValue);
                    }
                }

                if (queryObj["IPAddress"] == null)
                    listBox2.Items.Add("IP address: " + queryObj["IPAddress"]);
                else
                {
                    String[] arrIPAddress = (String[])(queryObj["IPAddress"]);
                    foreach (String arrValue in arrIPAddress)
                    {
                        listBox2.Items.Add("IP address: " + arrValue);
                    }
                }

                if (queryObj["IPSubnet"] == null)
                    listBox2.Items.Add("IP subnet: " + queryObj["IPSubnet"]);
                else
                {
                    String[] arrIPSubnet = (String[])(queryObj["IPSubnet"]);
                    foreach (String arrValue in arrIPSubnet)
                    {
                        listBox2.Items.Add("IP subnet: " + arrValue);
                    }
                }
                listBox2.Items.Add("MAC address: " + queryObj["MACAddress"]);
                listBox2.Items.Add("Service ame: " + queryObj["ServiceName"]);
            }
        }

        // СЕТЬ КОНЕЦ

        // ВИДЕО КАРТА НАЧАЛО

        void GPU()
        {
            ManagementObjectSearcher searcher11 = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_VideoController");

            foreach (ManagementObject queryObj in searcher11.Get())
            {
                metroLabel29.Text = ("Adapter RAM: " + Convert.ToDouble(queryObj["AdapterRAM"]) /(1024 * 1024) / 1024 + "GB");
                metroLabel27.Text = (Convert.ToString(queryObj["Caption"]));
                metroLabel30.Text = ("Driver version: " + queryObj["DriverVersion"]);
                metroLabel28.Text = ("Video processor: " + queryObj["VideoProcessor"]);
                metroLabel42.Text = ("Video processor: " + queryObj["DriverDate"]);
            }
        }

        // ВИДЕО КАРТА КОНЕЦ

        // ПОЛЬЗОВАТЕЛЬ И СИСТЕМА НАЧАЛО

        void User()
        {
            string bootmode = Convert.ToString(SystemInformation.BootMode);
            string ComputerName = Convert.ToString(SystemInformation.ComputerName);
            string MonitorCount = Convert.ToString(SystemInformation.MonitorCount);
            string Network = Convert.ToString(SystemInformation.Network);
            string Secure = Convert.ToString(SystemInformation.Secure);
            string UserDomainName = Convert.ToString(SystemInformation.UserDomainName);
            string UserName = Convert.ToString(SystemInformation.UserName);
            string is64bit = Convert.ToString(Environment.Is64BitOperatingSystem);
            string osversion = Convert.ToString(Environment.OSVersion);
            string version = Convert.ToString(Environment.Version);

            ComputerInfo os = new ComputerInfo();
            string osname = Convert.ToString(os.OSFullName);

            metroLabel40.Text = "Boot mode: " + bootmode;
            metroLabel32.Text = "Computer name: " + ComputerName;
            metroLabel33.Text = "Monitor count: " + MonitorCount;
            metroLabel34.Text = "Network: " + Network;
            metroLabel35.Text = "Secure: " + Secure;
            metroLabel36.Text = "User domain name: " + UserDomainName;
            metroLabel41.Text = "Is 64 bit: " + is64bit;
            metroLabel38.Text = "Os version: " + osversion;
            metroLabel39.Text = "Version: " + version;
            metroLabel31.Text = "User name: " + UserName;
            metroLabel37.Text = "Os name: " + osname;

        }

        // ПОЛЬЗОВАТЕЛЬ И СИСТЕМА КОНЕЦ
    }
}
