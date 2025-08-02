// Program.cs
using System;
using System.Net;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.Versioning;
using Microsoft.Extensions.Configuration;

[SupportedOSPlatform("windows")] 
class Program
{
    static void Main()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        IConfiguration config = builder.Build();

        string username = config["Credentials:Username"] ?? throw new Exception("Username is missing from configuration.");
        string password = config["Credentials:Password"] ?? throw new Exception("Password is missing from configuration.");
        Console.WriteLine($"USERNAME: {username}");
        Console.WriteLine($"PASSWORD: {password}");

        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://+:8080/login/");
        listener.Start();
        Console.WriteLine("Listening for requests on port 8080");

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;

            if (request.RawUrl == "/login")
            {
                EnableAutoLogon(username, password);
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