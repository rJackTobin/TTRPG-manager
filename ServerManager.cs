using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace TTRPG_manager
{
    public class ServerManager
    {
        private HttpListener listener;
        private bool isServerRunning = false;
        public bool StartServer()
        {
            AddFirewallRuleWithUserConsent();
            listener = new HttpListener();
            listener.Prefixes.Add("http://*:8080/");
            try
            {
                listener.Start();
                isServerRunning = true;
                Task.Run(() => HandleIncomingConnections());
                MessageBox.Show("Server started on port 8080.");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to start server: {ex.Message}");
                return false;
            }
        }
        private async Task HandleIncomingConnections()
        {
            while (isServerRunning)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();

                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Process request, for example, serve a simple response
                string responseString = "<html><body>Hello from WPF app!</body></html>";
                var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                resp.ContentLength64 = buffer.Length;
                System.IO.Stream output = resp.OutputStream;
                output.Write(buffer, 0, buffer.Length);

                // Close the output stream.
                output.Close();
            }
        }

        public void StopServer()
        {
            if (listener != null)
            {
                isServerRunning = false;
                listener.Stop();
                listener.Close();
            }
        }
        public static void AddFirewallRule()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "netsh",
                Arguments = "advfirewall firewall add rule name=\"TTRPG-manager\" dir=in action=allow protocol=TCP localport=8080",
                Verb = "runas", // This attempts to run the process with administrator privileges
                CreateNoWindow = true,
                UseShellExecute = true,
            };

            try
            {
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch
            {
                // Handle the case where the user did not allow administrator privileges
            }
        }
        private void AddFirewallRuleWithUserConsent()
        {
            // Prompt the user for consent before attempting to modify firewall settings
            var result = MessageBox.Show("The application needs to add a firewall rule to operate correctly. This requires administrative privileges. Proceed?",
                                         "Firewall Permission", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    AddFirewallRule(); // Call your method to add the firewall rule here
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to add firewall rule: {ex.Message}");
                }
            }
        }
        public string GetLocalIPAddress()
        {
            string wirelessIP = null;
            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Check if the network interface is wireless and operational
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && ni.OperationalStatus == OperationalStatus.Up)
                {
                    var ipProps = ni.GetIPProperties();
                    // Get the first IPv4 address assigned to this interface
                    var ipInfo = ipProps.UnicastAddresses
                                        .FirstOrDefault(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    if (ipInfo != null)
                    {
                        wirelessIP = ipInfo.Address.ToString();
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(wirelessIP))
            {
                throw new Exception("No operational wireless network interfaces with an IPv4 address found.");
            }

            return wirelessIP;
        }
    }

}
