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
        private AppConfig _config;

        ConfigManager manager = new ConfigManager();
        private HttpListener listener;
        private bool isServerRunning = false;
        public bool StartServer()
        {
            _config = manager.LoadConfig();
            if (!_config.addedFirewallRule)
            {
                AddFirewallRuleWithUserConsent();
                _config.addedFirewallRule = true;
                ConfigManager.SaveConfig(_config);
            }
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

                // Get the request and response objects
                HttpListenerRequest request = ctx.Request;
                HttpListenerResponse response = ctx.Response;

                // Generate the response content based on your application's data
                string responseString = GenerateDropdownFromParties();

                // Convert the response string to a byte array
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                // Set the content length and MIME type for the response
                response.ContentLength64 = buffer.Length;
                response.ContentType = "text/html";

                // Write the response content
                System.IO.Stream output = response.OutputStream;
                await output.WriteAsync(buffer, 0, buffer.Length);

                // Close the output stream to send the response
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

        public void AddFirewallRuleWithUserConsent()
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
        private string GenerateDropdownFromParties()
        {
            // Start building the HTML for the dropdown
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("<html><body>");
            stringBuilder.Append("<select id='partyDropdown'>");

            // Assuming _config is accessible here. If not, you may need to pass it as a parameter
            foreach (var character in _config.Parties[_config.selectedPartyIndex].Members)
            {
                // Each party name becomes an option in the dropdown
                stringBuilder.AppendFormat("<option value='{0}'>{0}</option>", character.Name);
            }

            stringBuilder.Append("</select>");
            stringBuilder.Append("</body></html>");

            return stringBuilder.ToString();
        }
    }

}
