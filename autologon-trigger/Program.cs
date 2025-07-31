// Program.cs
using System;
using System.Net;
using Microsoft.Win32;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")] 
class Program
{
    static void main()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:8080/");
        listener.Start();
        Console.WriteLine("Listening for requests on http://localhost:8080/");

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;

            if (request.RawUrl == "/login")
            {
                EnableAutoLogon("YourUsername", "YourPassword");
                RestartComputer();
            }

            context.Response.StatusCode = 200;
            context.Response.Close();
        }
    }

    static void EnableAutoLogon(string username, string password)
    {
        const string keyPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon";

        using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(keyPath, writable: true))
        {
            if (key != null)
            {
                key.SetValue("AutoAdminLogon", "1");
                key.SetValue("DefaultUserName", username);
                key.SetValue("DefaultPassword", password);
                key.SetValue("DefaultDomainName", Environment.MachineName);
                Console.WriteLine("Autologon registry keys set.");
            }
            else
            {
                Console.WriteLine("Failed to open registry key.");
            }
        }
    }

    static void RestartComputer()
    {
        Console.WriteLine("Restarting system...");
        Process.Start(new ProcessStartInfo("shutdown", "/r /t 5")
        {
            CreateNoWindow = true,
            UseShellExecute = false
        });
    }

}