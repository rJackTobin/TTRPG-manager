using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.IO;
using System.Web;
using System.Reflection;
using System.Text.Json;

namespace TTRPG_manager
{

    public class ServerManager
    {
        private AppConfig _config;

        private HttpListener listener;
        private bool isServerRunning = false;
        public bool StartServer()
        {
            _config = ConfigManager.LoadConfig();
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
                HttpListenerRequest request = ctx.Request;
                HttpListenerResponse response = ctx.Response;
                string responseString = "";
                try
                {
                    if (request.Url.AbsolutePath.StartsWith("/images/"))
                    {
                        // Extract character name from URL
                        string characterName = request.Url.AbsolutePath.Substring("/images/".Length);
                        var character = _config.Parties[_config.selectedPartyIndex].Members.FirstOrDefault(c => c.safeName.Equals(characterName, StringComparison.OrdinalIgnoreCase));
                        if (character != null && !string.IsNullOrEmpty(character.ImagePath) && File.Exists(character.ImagePath))
                        {
                            byte[] imageBytes = File.ReadAllBytes(character.ImagePath);
                            response.ContentType = "image/jpeg"; // Adjust based on the actual image format
                            response.ContentLength64 = imageBytes.Length;
                            await response.OutputStream.WriteAsync(imageBytes, 0, imageBytes.Length);
                        }
                        else
                        {
                            response.StatusCode = 404; // Not Found
                        }
                    }
                    else if (request.HttpMethod == "POST")
                    {
                        if (request.RawUrl.Contains("/useSkill"))
                        {
                            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                            {
                                string postData = await reader.ReadToEndAsync();
                                var formData = HttpUtility.ParseQueryString(postData);

                                string characterName = formData["CharacterName"];
                                string skillName = formData["SkillName"];

                                var character = _config.Parties[_config.selectedPartyIndex].Members.FirstOrDefault(c => c.safeName == characterName);
                                if (character != null)
                                {
                                    var skill = character.Skills.FirstOrDefault(s => s.safeName == skillName);
                                    if (skill != null)
                                    {
                                        // Apply skill use logic
                                        character.useSkill(skill);
                                        ConfigManager.SaveConfig(_config); // Save changes
                                        Application.Current.Dispatcher.Invoke(() =>
                                        {
                                            var mainWindow = Application.Current.MainWindow as MainWindow;
                                            mainWindow?.StartAnimation(character);
                                            mainWindow?.PopulateCharacterPanels();
                                             
                                        });
                                        responseString = "Skill used successfully.";
                                    }
                                    else
                                    {
                                        responseString = "Skill not found.";
                                    }
                                }
                                else
                                {
                                    responseString = "Character not found.";
                                }
                            }

                        }
                        else if (request.RawUrl.Contains("/upHP") && request.HttpMethod == "POST")
                        {
                            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                            {
                                string postData = await reader.ReadToEndAsync();
                                var formData = HttpUtility.ParseQueryString(postData);

                                string characterName = formData["CharacterName"];
                                int amount = int.Parse(formData["amount"]); // Consider adding error handling for parsing

                                var character = _config.Parties[_config.selectedPartyIndex].Members.FirstOrDefault(c => c.safeName == characterName);
                                if (character != null)
                                {
                                    character.UpHP(amount);
                                    ConfigManager.SaveConfig(_config); // Save changes
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        var mainWindow = Application.Current.MainWindow as MainWindow;
                                        mainWindow?.PopulateCharacterPanels();

                                    });
                                    responseString = "HP updated successfully.";
                                }
                                else
                                {
                                    responseString = "Character not found.";
                                }
                            }
                        }
                        else if (request.RawUrl.Contains("/downHP") && request.HttpMethod == "POST")
                        {
                            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                            {
                                string postData = await reader.ReadToEndAsync();
                                var formData = HttpUtility.ParseQueryString(postData);

                                string characterName = formData["CharacterName"];
                                int amount = int.Parse(formData["amount"]); // Consider adding error handling for parsing

                                var character = _config.Parties[_config.selectedPartyIndex].Members.FirstOrDefault(c => c.safeName == characterName);
                                if (character != null)
                                {
                                    character.DownHP(amount);
                                    ConfigManager.SaveConfig(_config); // Save changes
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        var mainWindow = Application.Current.MainWindow as MainWindow;
                                        mainWindow?.PopulateCharacterPanels();

                                    });
                                    responseString = "HP updated successfully.";
                                }
                                else
                                {
                                    responseString = "Character not found.";
                                }
                            }
                        }
                        else if (request.RawUrl.Contains("/upMP") && request.HttpMethod == "POST")
                        {
                            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                            {
                                string postData = await reader.ReadToEndAsync();
                                var formData = HttpUtility.ParseQueryString(postData);

                                string characterName = formData["CharacterName"];
                                int amount = int.Parse(formData["amount"]); // Consider adding error handling for parsing

                                var character = _config.Parties[_config.selectedPartyIndex].Members.FirstOrDefault(c => c.safeName == characterName);
                                if (character != null)
                                {
                                    character.UpMP(amount);
                                    ConfigManager.SaveConfig(_config); // Save changes
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        var mainWindow = Application.Current.MainWindow as MainWindow;
                                        mainWindow?.PopulateCharacterPanels();

                                    });
                                    responseString = "HP updated successfully.";
                                }
                                else
                                {
                                    responseString = "Character not found.";
                                }
                            }
                        }
                        else if (request.RawUrl.Contains("/downMP") && request.HttpMethod == "POST")
                        {
                            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                            {
                                string postData = await reader.ReadToEndAsync();
                                var formData = HttpUtility.ParseQueryString(postData);

                                string characterName = formData["CharacterName"];
                                int amount = int.Parse(formData["amount"]); // Consider adding error handling for parsing

                                var character = _config.Parties[_config.selectedPartyIndex].Members.FirstOrDefault(c => c.safeName == characterName);
                                if (character != null)
                                {
                                    character.DownMP(amount);
                                    ConfigManager.SaveConfig(_config); // Save changes
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        var mainWindow = Application.Current.MainWindow as MainWindow;
                                        mainWindow?.PopulateCharacterPanels();

                                    });
                                    responseString = "HP updated successfully.";
                                }
                                else
                                {
                                    responseString = "Character not found.";
                                }
                            }
                        }
                        else
                        {
                            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                            {
                                string postData = await reader.ReadToEndAsync();
                                var formData = HttpUtility.ParseQueryString(postData);

                                if (request.RawUrl.Contains("/updateCharacter"))
                                {
                                    // Process the update form submission
                                    string name = formData["Name"];
                                    string description = formData["Description"];
                                    int age = int.Parse(formData["Age"]); // Add error handling for parsing
                                    string gender = formData["Gender"];
                                    string race = formData["Race"];
                                    string title = formData["Title"];
                                    // Find and update the character
                                    var characterToUpdate = _config.Parties[_config.selectedPartyIndex].Members.FirstOrDefault(c => c.safeName == name);
                                    if (characterToUpdate != null)
                                    {
                                        characterToUpdate.Description = description;
                                        characterToUpdate.Age = age;
                                        characterToUpdate.Gender = gender;
                                        characterToUpdate.Race = race;
                                        characterToUpdate.Title = title;
                                        // Update other fields as needed
                                    }
                                    ConfigManager.SaveConfig(_config);

                                    // Optionally redirect to a confirmation page or back to the form
                                    response.Redirect("/editCharacter?name=" + HttpUtility.UrlEncode(name));

                                }
                                else
                                {
                                    // Handling other POST requests if any
                                    responseString = "Invalid request.";
                                }
                            }
                        }
                    }
                    else if (request.HttpMethod == "GET")
                    {
                        // Serve the form with the dropdown for character selection
                        // or editing if a specific character is selected
                        if (request.RawUrl.Contains("/editCharacter"))
                        {
                            // Extract character name from URL if needed to pre-fill the form
                            string characterName = HttpUtility.ParseQueryString(request.Url.Query).Get("name");
                            responseString = GenerateCharacterInfoHtml(characterName); // Method that generates HTML with character info in editable form
                        }
                        
                        
                        else
                        {
                            // Generate the initial page with dropdown
                            responseString = GenerateDropdownFromParties();
                        }
                    }

                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    response.ContentType = "text/html";
                    await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {
                    // Handle exceptions (log them or inform the user)
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                finally
                {
                    response.OutputStream.Close();
                }
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
        public string StartOnlineServer()
        {
            AppConfig config = ConfigManager.LoadConfig();
            ProcessStartInfo authInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = $"ngrok.exe config add-authtoken {config.NgrokAuthKey}",
                UseShellExecute = false,
                RedirectStandardOutput = false,
                CreateNoWindow = true
            };
            Process process = Process.Start(authInfo);
            
                process.Kill();  // This will still wait for the cmd to close, but it will remain open because of /K.
            
            // Optional: Start the ngrok HTTP tunnel similarly
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = $"/K ngrok.exe http 8080 --log=stdout",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = false
            };

            Process proc = new Process() { StartInfo = startInfo };
            proc.Start();

            string ngrokUrl = null;
            while (!proc.StandardOutput.EndOfStream)
            {
                var line = proc.StandardOutput.ReadLine();
                Console.WriteLine(line);  // Optionally print the line for debugging

                // Check if the line contains the URL
                if (line.Contains("url=https://"))
                {
                    int startIndex = line.IndexOf("url=") + 4;
                    int endIndex = line.IndexOf(".ngrok-free.app") + 15; // 15 is the length of ".ngrok-free.app"
                    ngrokUrl = line.Substring(startIndex, endIndex - startIndex);
                    break;
                }
            }

              // Wait for the process to exit after finding the URL

            return ngrokUrl;
        }
        public string GetNgrokAddress()
        {
            return "";
        }
        private string GenerateDropdownFromParties()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("<html><head><title>Select Character</title></head><body>");
            stringBuilder.Append("<meta name='viewport' content='width=device-width, initial-scale=1'>");
            stringBuilder.Append("<style>");
            stringBuilder.Append("body { font-size: 20px; }"); // Default font size for large screens
            stringBuilder.Append("p { font-size: 20px; }");
            stringBuilder.Append("select { font-size: 24px; }");
            stringBuilder.Append("input { font-size: 24px; }");
            stringBuilder.Append("@media (max-width: 1300px) { body { font-size: 32px; } }");
            stringBuilder.Append("@media (max-width: 1300px) { p { font-size: 24px; } }");// Larger font size for screens narrower than 600px
            stringBuilder.Append(".character-image { max-width: 100%; height: auto; position: absolute; top: 0; right: 0; }");
            stringBuilder.Append("@media (min-width: 100px) { .character-image { width: 40%; } }");
            stringBuilder.Append("@media (min-width: 1500px) { .character-image { width: 50%; } }");
            stringBuilder.Append("</style>");
            stringBuilder.Append("<h2>Choose your Character</h2>");
            stringBuilder.Append("<form method='GET' action='/editCharacter'>");
            stringBuilder.Append("<select name='name'>");

            foreach (var character in _config.Parties[_config.selectedPartyIndex].Members)
            {
                // Use UrlEncode to safely encode character names for use in URLs
                stringBuilder.AppendFormat("<option value='{0}'>{1}</option>", HttpUtility.UrlEncode(character.safeName), character.Name);
            }
            stringBuilder.Append("</select>");
            stringBuilder.Append("<input type='submit' value='View Character'/>");
            stringBuilder.Append("</form>");
            stringBuilder.Append("</body></html>");

            return stringBuilder.ToString();
        }
        private string GenerateCharacterInfoHtml(string characterName)
        {
            _config = ConfigManager.LoadConfig();
            var character = _config.Parties[_config.selectedPartyIndex].Members.FirstOrDefault(c => c.safeName.Equals(characterName, StringComparison.OrdinalIgnoreCase));
            if (character == null) return "<html><body>Character not found.</body></html>";

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("<html><head><title>View Character</title></head><body>");
            stringBuilder.Append("<meta name='viewport' content='width=device-width, initial-scale=1'>");
            stringBuilder.Append("<style>");
            stringBuilder.Append("input[type=\"submit\"], button {\r\n    font-size: 24px;\r\n    padding: 10px 20px;\r\n    background-color: #007BFF;\r\n    color: white;\r\n    border: none;\r\n    border-radius: 5px;\r\n    cursor: pointer;\r\n    transition: background-color 0.3s;\r\n}\r\n\r\ninput[type=\"submit\"]:hover, button:hover {\r\n    background-color: #0056b3; /* Darker blue when hovered for better user interaction */\r\n}\r\n");
            stringBuilder.Append(".expander-label {\r\n cursor: pointer;\r\n  max-width: 800px;\r\n  padding: 5px;\r\n    background-color: #f9f9f9;\r\n    border: solid 1px #ccc;\r\n    display: block;\r\n    color: #000;\r\n}\r\n\r\n.expander-content {\r\n    display: none;\r\n  max-width: 800px;\r\n  padding: 5px;\r\n    border: solid 1px #ccc;\r\n    border-top: none;\r\n    background-color: #fff;\r\n}");
            stringBuilder.Append(".bar-container {\r\n max-width: 400px;\r\n   width: 110%; \r\n    background-color: #ddd; \r\n    border-radius: 8px; \r\n    margin: 5px 0;\r\n    display: flex;  \r\n align-items: center; \r\n}" +
                ".bar {\r\n    flex-grow: 1; /* Bar takes up most of the space */\r\n    height: 20px;\r\n    color: white;\r\n    text-align: center;\r\n    line-height: 20px;\r\n    border-radius: 8px;\r\n}" +
                "\r\n\r\n.hp-bar { background-color: #f44336; }" +
                "\r\n.mp-bar { background-color: #2196F3; }" +
                "\r\n\r\n.adjust-btn {\r\n    width: 30px; /* Fixed width for buttons */\r\n    height: 32px;\r\n    font-size: 24px;\r\n  padding: 10;\r\n  margin: 0 5px;\r\n    background-color: #f0f0f0;" +
                "\r\n    color: #333; \r\n    border: 1px solid #bbb; /* Subtle border */\r\n    border-radius: 4px; /* Rounded corners */\r\n    cursor: pointer;\r\n justify-content: center;\r\n  line-height: 0px;\r\n}");
            stringBuilder.Append("body { font-size: 20px; }"); // Default font size for large screens
            stringBuilder.Append("p { font-size: 20px; }");
            stringBuilder.Append("@media (max-width: 1300px) { body { font-size: 28px; } }");
            stringBuilder.Append("@media (max-width: 1300px) { p { font-size: 24px; } }");// Larger font size for screens narrower than 600px
            stringBuilder.Append(".character-image { max-width: 100%; height: auto; position: absolute; top: 0; right: 0; }");
            stringBuilder.Append("@media (min-width: 100px) { .character-image { width: 40%; } }");
            stringBuilder.Append("@media (min-width: 1500px) { .character-image { width: 50%; } }");
            stringBuilder.Append("</style>");
            // Include the JavaScript function for AJAX calls
            stringBuilder.Append("<script>");
            stringBuilder.Append(@"
                function useSkill(characterName, skillName) {
                const data = `CharacterName=${encodeURIComponent(characterName)}&SkillName=${encodeURIComponent(skillName)}`;
                fetch('/useSkill', {
                    method: 'POST',
                    headers: {'Content-Type': 'application/x-www-form-urlencoded'},
                    body: data
                })
                .then(response => response.text())
                .then(text => {
                    // Only show alert if there is a problem or specific condition
                    if (!text.includes(""successfully"")) { // Change this condition based on actual success message
                        alert('Response: ' + text);
                    }
                    // Optionally update the UI here to reflect changes
                })
                .catch(error => {
                    console.error('Error using skill:', error);
                    alert('Error using skill: ' + error); // Show alert on errors
                });
            }

                function toggleExpander(expanderId) {
                var content = document.getElementById(expanderId);
                if (content.style.display === 'block') {
                    content.style.display = 'none';
                } else {
                    content.style.display = 'block';
                    if (content.innerHTML.trim() === '') { // Check if content is empty and adjust accordingly
                        content.innerHTML = 'No description available.'; // Provide a default message
                        content.style.minHeight = '20px'; // Ensure it's visible even if empty
                    }
                }
            }

            function upHP(characterName, amount) {
                const data = `CharacterName=${encodeURIComponent(characterName)}&amount=${encodeURIComponent(amount)}`;
                fetch('/upHP', {
                    method: 'POST',
                    headers: {'Content-Type': 'application/x-www-form-urlencoded'},
                    body: data
                })
                .then(response => response.text())
                .then(text => {
                    if (!text.includes(""successfully"")) {
                        alert('Response: ' + text);
                    }
                    // Optionally update the UI here to reflect changes
                })
                .catch(error => {
                    console.error('Error healing:', error);
                    alert('Error healing: ' + error);
                });
            }
            function downHP(characterName, amount) {
                const data = `CharacterName=${encodeURIComponent(characterName)}&amount=${encodeURIComponent(amount)}`;
                fetch('/downHP', {
                    method: 'POST',
                    headers: {'Content-Type': 'application/x-www-form-urlencoded'},
                    body: data
                })
                .then(response => response.text())
                .then(text => {
                    if (!text.includes(""successfully"")) {
                        alert('Response: ' + text);
                    }
                    // Optionally update the UI here to reflect changes
                })
                .catch(error => {
                    console.error('Error healing:', error);
                    alert('Error healing: ' + error);
                });
            }
            function upMP(characterName, amount) {
                const data = `CharacterName=${encodeURIComponent(characterName)}&amount=${encodeURIComponent(amount)}`;
                fetch('/upMP', {
                    method: 'POST',
                    headers: {'Content-Type': 'application/x-www-form-urlencoded'},
                    body: data
                })
                .then(response => response.text())
                .then(text => {
                    if (!text.includes(""successfully"")) {
                        alert('Response: ' + text);
                    }
                    // Optionally update the UI here to reflect changes
                })
                .catch(error => {
                    console.error('Error healing:', error);
                    alert('Error healing: ' + error);
                });
            }
            function downMP(characterName, amount) {
                const data = `CharacterName=${encodeURIComponent(characterName)}&amount=${encodeURIComponent(amount)}`;
                fetch('/downMP', {
                    method: 'POST',
                    headers: {'Content-Type': 'application/x-www-form-urlencoded'},
                    body: data
                })
                .then(response => response.text())
                .then(text => {
                    if (!text.includes(""successfully"")) {
                        alert('Response: ' + text);
                    }
                    // Optionally update the UI here to reflect changes
                })
                .catch(error => {
                    console.error('Error healing:', error);
                    alert('Error healing: ' + error);
                });
            }
                
            ");
            stringBuilder.Append("</script>");
            stringBuilder.Append("</head><body>");
            stringBuilder.AppendFormat("<h2>{0}</h2><br>", character.Name);
            stringBuilder.Append("<form action='/updateCharacter' method='post'>");
            
            stringBuilder.AppendFormat("<img src='/images/{0}' class='character-image' alt='Character Image'/>", HttpUtility.UrlEncode(character.safeName));

            // Add hidden input for character name to identify the character on submission
            stringBuilder.AppendFormat("<input type='hidden' name='Name' value='{0}'/>", character.safeName);

            stringBuilder.AppendFormat("\r\n<div class=\"bar-container\">\r\n    " +
                "<button type=\"button\" onclick=\"downHP({{characterName}}, 1)\" class=\"adjust-btn\">&lt;</button>\r\n    " +
                "<div class=\"hp-bar bar\" style=\"width: calc(100% * {1} / {2});\"></div>\r\n    " +
                "<button type=\"button\" onclick=\"upHP('{0}', 1)\" class=\"adjust-btn\">&gt;</button>\r\n</div>\r\n\r\n<div class=\"bar-container\">\r\n    " +
                "<button type=\"button\" onclick=\"downMP({0}, 1)\" class=\"adjust-btn\">&lt;</button>\r\n    " +
                "<div class=\"mp-bar bar\" style=\"width: calc(100% * {{currentMP}} / {{maxMP}});\"></div>\r\n    " +
                "<button type=\"button\" onclick=\"upMP({0}, 1)\" class=\"adjust-btn\">&gt;</button>\r\n</div>", 
                character.safeName, character.CurrentHP, character.MaxHP);
            // For each editable property, create an appropriate input
            
            stringBuilder.AppendFormat("<textarea rows='5' cols='40' name='Description'>{0}</textarea></p>", character.Description);
            stringBuilder.AppendFormat("<p>Age: <input type='number' name='Age' value='{0}' /></p>", character.Age);
            stringBuilder.AppendFormat("<p>Gender: <input type='text' name='Gender' value='{0}' /></p>", character.Gender);
            stringBuilder.AppendFormat("<p>Race: <input type='text' name='Race' value='{0}' /></p>", character.Race);
            stringBuilder.AppendFormat("<p>Title: <input type='text' name='Title' value='{0}' /></p>", character.Title);
            stringBuilder.Append("<input type='submit' value='Update Character'/>");


            // Skills section
            stringBuilder.Append("<h3>Skills</h3>");
            foreach (var skill in character.Skills)
            {
                stringBuilder.AppendFormat("<div class='expander-label' onclick=\"toggleExpander('expander_{0}')\">{1}</div>", skill.safeName, skill.Name);
                stringBuilder.AppendFormat("<div id='expander_{0}' class='expander-content'>{1}\n(Uses: {4}/{5})<br><button type=\"button\" onclick=\"useSkill('{2}', '{3}')\">Use Skill</button></div>",
                skill.safeName, skill.Description, character.safeName, skill.safeName, skill.RemainingUses, skill.MaxUses);
            }

            // Equipped Items Section
            stringBuilder.Append("<h3>Equipment</h3>");
            foreach (var item in character.EquippedItems)
            {
                stringBuilder.AppendFormat("<div class='expander-label' onclick=\"toggleExpander('expander_{0}')\">{1}</div>", item.safeName, item.Name);
                stringBuilder.AppendFormat("<div id='expander_{0}' class='expander-content'>{1}</div>", item.safeName, item.Description);
            }

            // Inventory section
            stringBuilder.Append("<h3>Inventory</h3>");
            foreach (var item in character.Inventory)
            {
                stringBuilder.AppendFormat("<div class='expander-label' onclick=\"toggleExpander('expander_{0}')\">{1}</div>", item.safeName, item.Name);
                stringBuilder.AppendFormat("<div id='expander_{0}' class='expander-content'>{1}\n(Count: {2}, Uses: {3}/{4})</div>",
                item.safeName, item.Description, item.Count, item.Uses, item.MaxUses);
            }
            stringBuilder.Append("</ul>");


            stringBuilder.Append("</form>");
            stringBuilder.Append("</body></html>");

            return stringBuilder.ToString();
        }
    }

}
