﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;

namespace PromDate.Installer
{
    public class Installer
    {
        public static void InstallPromDate(string path, string downloadUrl)
        {
            UninstallPromDate(path);

            DirectoryInfo monsterPromDir = new DirectoryInfo(path);
            monsterPromDir.CreateSubdirectory("Installer");
            string zip = monsterPromDir.FullName + "/Installer/promdate-dl.zip";
            string extract = monsterPromDir.FullName + "/Installer/extracted";

            WebClient webClient = new WebClient();
            webClient.Headers["user-agent"] = "Mozilla / 5.0(Windows NT 6.1; WOW64; rv: 40.0) Gecko / 20100101 Firefox / 40.1";
            webClient.DownloadFile(new Uri(downloadUrl), zip);
            if (Directory.Exists(extract))
                Directory.Delete(extract);
            ZipFile.ExtractToDirectory(zip, extract);

            string managedPath = monsterPromDir + "/MonsterProm_Data/Managed";
            File.Copy(managedPath + "/Assembly-CSharp.dll", managedPath + "/Assembly-CSharp.dll.backup");
            Injector.Injector.Inject(managedPath + "/Assembly-CSharp.dll");
            if (!Directory.Exists(monsterPromDir + "/MonsterProm_Data/Mods"))
            {
                Directory.CreateDirectory(monsterPromDir + "/MonsterProm_Data/Mods");
            }
            File.Copy(extract + "/PromDate.dll", monsterPromDir + "/MonsterProm_Data/Mods/PromDate.dll");

            if (!Directory.Exists(monsterPromDir + "/MonsterProm_Data/Mods/Audio"))
                Directory.CreateDirectory(monsterPromDir + "/MonsterProm_Data/Mods/Audio");
            if (!Directory.Exists(monsterPromDir + "/MonsterProm_Data/Mods/Characters"))
                Directory.CreateDirectory(monsterPromDir + "/MonsterProm_Data/Mods/Characters");
            if (!Directory.Exists(monsterPromDir + "/MonsterProm_Data/Mods/EndingConditions"))
                Directory.CreateDirectory(monsterPromDir + "/MonsterProm_Data/Mods/EndingConditions");
            if (!Directory.Exists(monsterPromDir + "/MonsterProm_Data/Mods/Events"))
                Directory.CreateDirectory(monsterPromDir + "/MonsterProm_Data/Mods/Events");
            if (!Directory.Exists(monsterPromDir + "/MonsterProm_Data/Mods/Images"))
                Directory.CreateDirectory(monsterPromDir + "/MonsterProm_Data/Mods/Images");
            if (!Directory.Exists(monsterPromDir + "/MonsterProm_Data/Mods/Items"))
                Directory.CreateDirectory(monsterPromDir + "/MonsterProm_Data/Mods/Items");
            Directory.Delete(monsterPromDir.FullName + "/Installer", true);
        }

        public static void UninstallPromDate(string path)
        {
            DirectoryInfo monsterPromDir = new DirectoryInfo(path);
            string assemPath = monsterPromDir + "/MonsterProm_Data/Managed/Assembly-CSharp.dll";
            string firstPassPath = monsterPromDir + "/MonsterProm_Data/Managed/Assembly-CSharp-firstpass.dll";
            if (File.Exists(assemPath + ".backup"))
            {
                File.Delete(assemPath);
                File.Move(assemPath + ".backup", assemPath);
            }
            if (File.Exists(firstPassPath + ".backup"))
            {
                File.Delete(firstPassPath);
                File.Move(firstPassPath + ".backup", firstPassPath);
            }
            if (File.Exists(monsterPromDir + "/MonsterProm_Data/Mods/PromDate.dll"))
                File.Delete(monsterPromDir + "/MonsterProm_Data/Mods/PromDate.dll");

        }
    }
}