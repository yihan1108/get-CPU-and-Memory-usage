using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Logout
{
    class Program
    {
        static void Main(string[] args)
        {
            // 獲取電腦名稱 Get the computer name
            string computername = Environment.MachineName; //使用 Environment.MachineName 取得電腦名稱
            Console.WriteLine($"Computer Name: {computername}");

            //獲取IP位址 Get IP address
            string localIP = string.Empty;
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
            Console.WriteLine(@"IP Address = " + localIP);
            // 獲取硬碟使用情況 Get hard disk usage
            DriveInfo[] allDrives = DriveInfo.GetDrives(); //使用 DriveInfo.GetDrives() 取得所有硬碟的資訊
            

            // 連接到 SQL Server Connecting to SQL Server
            SqlConnection conn = new SqlConnection("Data Source = ; Initial Catalog = ; Persist Security Info = True; User ID = ; Password = ");

            conn.Open();
            try
            {
                foreach (DriveInfo drive in allDrives) //透過 foreach 迴圈，針對每個硬碟執行以下的程式區塊
                {
                    SqlCommand cmd = new SqlCommand("INSERT INTO logout(computername, ipaddress, drive, volume, freespace, totalspace, freespacepercent, cpu, ram)values(@computername, @localIP, @driveName, @volumeLabel, @availableFreeSpace, @totalSize, @FreeSpacePercent, @cpuValue, @ramValue)", conn);                   
                    cmd.Parameters.AddWithValue("@computername", computername);
                    cmd.Parameters.AddWithValue("@localIP", localIP);
                    cmd.Parameters.AddWithValue("@driveName", drive.Name);
                    cmd.Parameters.AddWithValue("@volumeLabel", drive.VolumeLabel);
                    string FreeSpace = (drive.AvailableFreeSpace / (1024 * 1024* 1024)).ToString("N0");
                    cmd.Parameters.AddWithValue("@availableFreeSpace", FreeSpace);
                    string Size = (drive.TotalSize / (1024 * 1024 * 1024)).ToString("N0");
                    cmd.Parameters.AddWithValue("@totalSize", Size);
                    int FreeSpacePercent = Convert.ToInt32(Convert.ToDouble(drive.TotalFreeSpace) / Convert.ToDouble(drive.TotalSize) * 100);
                    cmd.Parameters.AddWithValue("@FreeSpacePercent", FreeSpacePercent);
                    PerformanceCounter cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    double cpuAvg = 0;
                    for (int i = 0; i < 5; i ++)
                    {
                        cpuAvg += cpu.NextValue();
                        Thread.Sleep(1000);
                    }
                    cpuAvg = Math.Round(cpuAvg / 5, 0);
                    cmd.Parameters.AddWithValue("@cpuValue", cpuAvg);
                    PerformanceCounter memory = new PerformanceCounter("Memory", "% Committed Bytes in Use");                    
                    cmd.Parameters.AddWithValue("@ramValue", Math.Round(memory.NextValue(),0));

                    // 執行 SQL 指令 Executing SQL commands
                    int result = cmd.ExecuteNonQuery();
                    Console.WriteLine($"成功新增{result}筆資料!");
                    Console.WriteLine($"Drive: {drive.Name}, Volume: {drive.VolumeLabel}, Free Space: {drive.AvailableFreeSpace / (1024 * 1024 * 1024)}, Total Space: {drive.TotalSize / (1024 * 1024 * 1024)}");
                    Console.WriteLine("FreeSpacePercent:{0:n0}%", FreeSpacePercent);
                    Console.WriteLine("CPU: {0:n1}%", cpuAvg);
                    Console.WriteLine("Memory: {0:n0}%", memory.NextValue());
                }
            }               
                
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }
        }        
    }
}



