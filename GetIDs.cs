using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Management;




namespace Forma_System
{
    internal class GetIDs
    {
        public static string GetProcessorId()
        {
            string id = "";
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select ProcessorId from Win32_Processor");
            foreach (ManagementObject mo in searcher.Get())
            {
                id = mo["ProcessorId"].ToString();
                break;
            }
            return id;
        }

        public static string GetWindowsDriveSerialNumber()
        {
            try
            {
                // 1) عرف حرف البارتيشن الذي عليه الويندوز، عادةً "C:"
                string systemDrive = Path.GetPathRoot(Environment.SystemDirectory);  // e.g. "C:\\"
                if (string.IsNullOrEmpty(systemDrive))
                    return "";

                string deviceId = systemDrive.TrimEnd('\\'); // "C:"

                // 2) ابحث عن الـ Partition المرتبط بالبارتيشن
                string assocDiskPartition =
                    $"ASSOCIATORS OF {{Win32_LogicalDisk.DeviceID='{deviceId}'}} " +
                    "WHERE AssocClass=Win32_LogicalDiskToPartition";

                using (var partSearcher = new ManagementObjectSearcher(assocDiskPartition))
                {
                    foreach (ManagementObject part in partSearcher.Get())
                    {
                        // مثال DeviceID = "Disk #0, Partition #1"
                        string partitionId = part["DeviceID"]?.ToString();
                        if (string.IsNullOrWhiteSpace(partitionId))
                            continue;

                        // 3) من الـ Partition اطلع على الـ PhysicalDrive
                        string assocDrive =
                            $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partitionId}'}} " +
                            "WHERE AssocClass=Win32_DiskDriveToDiskPartition";

                        using (var driveSearcher = new ManagementObjectSearcher(assocDrive))
                        {
                            foreach (ManagementObject drive in driveSearcher.Get())
                            {
                                // مثال DeviceID = "\\\\.\\PHYSICALDRIVE0"
                                string physicalId = drive["DeviceID"]?.ToString();
                                if (string.IsNullOrWhiteSpace(physicalId))
                                    continue;

                                // 4) في PhysicalMedia ابحث حسب الـ Tag
                                using (var mediaSearcher = new ManagementObjectSearcher(
                                    "SELECT SerialNumber, Tag FROM Win32_PhysicalMedia"))
                                {
                                    foreach (ManagementObject media in mediaSearcher.Get())
                                    {
                                        string tag = media["Tag"]?.ToString();
                                        if (string.Equals(tag, physicalId, StringComparison.OrdinalIgnoreCase))
                                        {
                                            string serial = media["SerialNumber"]?.ToString()?.Trim();
                                            return string.IsNullOrWhiteSpace(serial)
                                                ? ""
                                                : serial;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // ضع هنا لو حابب لوق أو رسالة خطأ
            }

            // إذا لم نجد شيء نعيد فارغ
            return "";
        }
    }
}
