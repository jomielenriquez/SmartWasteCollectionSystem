#include <WiFi.h>
#include <WebServer.h>
#include <Preferences.h>
#include <HTTPClient.h>
#include <vector>
#include <algorithm>

std::vector<String> wifiVectorList;

Preferences preferences;
WebServer server(80);

bool isConnected = false;
String SaveApiMessage = "";
String SaveApiKeyMessage = "";
String connectedSSID = "";
String wifiList = "";
String apiKey = "";

String PageHeader(String title){
  String html = R"rawliteral(
  <!DOCTYPE html><html><head>
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <style>
    body{font-family:Arial;margin:0;padding:20px;background:#f4f4f4}
    .container{max-width:420px;margin:auto;background:#fff;padding:18px;border-radius:12px;box-shadow:0 4px 6px rgba(0,0,0,.08)}
    h2{text-align:center;color:#333}
    label{display:block;margin:12px 0 6px;font-weight:600}
    select,input,a,button{width:100%;padding:12px;margin-bottom:12px;border:1px solid #ccc;border-radius:8px;font-size:16px}
    input{width:95% !important}
    .btn-primary{background:#28a745;color:#fff;border:none}
    .btn-danger{background:#dc3545;color:#fff;border:none}
    .status{text-align:center;font-size:16px;color:green;margin-bottom:10px}
    .small{font-size:13px;color:#666;text-align:center}
  </style>
  </head><body><div class="container">
  )rawliteral";
  html += "<h2>" + title + "</h2>";
  return html;
}

String pageFooter() {
  return "</div></body></html>";
}

// -------- HTML Page --------
String buildPage() {
  String page = PageHeader("Wifi Setup");
  if (isConnected) {
    page += "<div class='status'>Connected to " + connectedSSID + "</div>";
    page += "<div class='small'>IP on Wi-Fi: " + WiFi.localIP().toString() + "</div>";
    // Reset button visible only when connected
    page += R"rawliteral(
      <form action="/reset" method="POST" 
            onsubmit="return confirm('Reset Wi-Fi credentials? This will reboot the device.');">
        <button type="submit" class="btn-danger">Reset Wi-Fi</button>
      </form>
    )rawliteral";

    page += R"rawliteral(
      <form action="/postpage" method="GET">
        <button type="submit" class="btn-primary">Go to Post Page</button>
      </form>
    )rawliteral";
    page += R"rawliteral(
      <form action="/apikey" method="GET">
        <button type="submit" class="btn-primary">Set API Key</button>
      </form>
    )rawliteral";
  } else {
    page += "<form action='/connect' method='POST'>";
    page += "<label for='ssid'>SSID:</label>";
    page += "<select name='ssid' id='ssid'>";
    page += wifiList;
    page += "</select>";
    page += "<label for='pass'>Password:</label>";
    page += "<input type='password' name='pass' id='pass'>";
    page += "<button type='submit' class='btn-primary'>Connect</button></form>";
  }

  page += pageFooter();
  return page;
}

// -------- Handlers --------
void handleRoot() { server.send(200, "text/html", buildPage()); }

void handleConnect() {
  String ssid = server.arg("ssid");
  String pass = server.arg("pass");
  // Save to flash
  preferences.begin("wifi", false);
  preferences.putString("ssid", ssid);
  preferences.putString("pass", pass);
  preferences.end();
  server.send(200, "text/html",
              "<html><body><h3>Connecting to " + ssid +
              "...</h3><meta http-equiv='refresh' content='5;url=/' /></body></html>");
  connectedSSID = ssid;
  WiFi.begin(ssid.c_str(), pass.c_str());
}

void handleReset() {
  preferences.begin("wifi", false);
  preferences.clear();
  preferences.end();
  WiFi.disconnect(true, true);
  server.send(200, "text/html", "<html><body><h3>Wi-Fi credentials cleared. Rebooting...</h3></body></html>");
  delay(1000);
  ESP.restart();
}

// Page with POST button
void handlePostPage() {
  String page = PageHeader("Send Bin Status");
  if (SaveApiMessage != "") {page += "<div class='status'>" + SaveApiMessage + "</div>";}
  page += "<form action='/sendpost' method='POST'>";
  page += "<label for='apiKey'>API Key:</label><input type='text' name='apiKey' id='apiKey' value='" + apiKey + "'>";
  page += "<label for='percentage'>Percentage:</label><input type='text' name='percentage' id='percentage'>";
  page += "<button type='submit' class='btn-primary'>Send</button></form>";
  page += pageFooter();
  SaveApiMessage = "";
  server.send(200, "text/html", page);
}

void handleSendPost() {
  apiKey = server.arg("apiKey");
  String perc = server.arg("percentage");

  if (apiKey == "" || perc == "") {
    server.send(200, "text/html", "API Key or Percentage missing!");
    return;
  }

  preferences.begin("wifi", false);
  preferences.putString("apiKey", apiKey);
  preferences.end();

  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;
    http.begin("https://ecokonek.somee.com/BinLog/UpdateBinStatus");
    http.addHeader("Content-Type", "application/x-www-form-urlencoded");

    String payload = "apiKey=" + apiKey + "&percentage=" + perc;
    int httpCode = http.POST(payload);

    if (httpCode > 0) {
      String response = http.getString();
      SaveApiMessage = "POST Sent!";
      handlePostPage();
    } else {
      SaveApiMessage = "Error sending POST";
      handlePostPage();
    }
    http.end();
  } else {
    SaveApiMessage = "Not connected to WiFi!";
    handlePostPage();
  }
}

void handleApiKeyPage() {
  String page = PageHeader("Save API Key");
  if (SaveApiKeyMessage != "") {page += "<div class='status'>" + SaveApiKeyMessage + "</div>";}
  page += "<form action='/saveApiKey' method='POST'>";
  page += "<label for='apiKey'>API Key:</label><input type='text' name='apiKey' id='apiKey' value='" + apiKey + "'>";
  page += "<button type='submit' class='btn-primary'>Save</button></form>";
  page += pageFooter();
  SaveApiKeyMessage = "";
  server.send(200, "text/html", page);
}

void handleSaveApiKey() {
  apiKey = server.arg("apiKey");
  if (apiKey != "") {
    preferences.begin("wifi", false);
    preferences.putString("apiKey", apiKey);
    preferences.end();
    SaveApiKeyMessage = "API Key saved!";
    handleApiKeyPage();
  } else {
    SaveApiKeyMessage = "API Key Missing";
    handleApiKeyPage();
  }
}

// -------- Setup --------
void setup() {
  Serial.begin(115200);

  // Always enable AP
  WiFi.mode(WIFI_AP_STA); // dual mode
  WiFi.softAP("ESP32-Setup", "12345678");
  Serial.print("AP IP: "); Serial.println(WiFi.softAPIP());

  // Load saved credentials
  preferences.begin("wifi", true);
  String savedSSID = preferences.getString("ssid", "");
  String savedPASS = preferences.getString("pass", "");
  apiKey = preferences.getString("apiKey", "");
  Serial.print("API Key: "); Serial.println(apiKey);
  preferences.end();
  
  // Scan networks
  int n = WiFi.scanNetworks();
  for (int i = 0; i < n; ++i) {
    wifiVectorList.push_back(WiFi.SSID(i));
    wifiList += "<option value='" + WiFi.SSID(i) + "'>" + WiFi.SSID(i) + "</option>";
  }

  if (savedSSID != "") {
    if (std::find(wifiVectorList.begin(), wifiVectorList.end(), savedSSID) != wifiVectorList.end()) {
      Serial.println("Found saved Wi-Fi. Attempting to connect...");
      WiFi.begin(savedSSID.c_str(), savedPASS.c_str());
      connectedSSID = savedSSID;
    } else {
      Serial.println("Saved Wifi is not in range.");
    }
  } else {
    Serial.println("No saved Wi-Fi. Waiting for config.");
  }

  // Webserver
  server.on("/", handleRoot);
  server.on("/connect", HTTP_POST, handleConnect);
  server.on("/reset", HTTP_POST, handleReset);
  server.on("/postpage", handlePostPage);
  server.on("/sendpost", HTTP_POST, handleSendPost);
  server.on("/apikey", handleApiKeyPage);
  server.on("/saveApiKey", HTTP_POST, handleSaveApiKey);
  server.begin();
}

// -------- Loop --------
void loop() {
  server.handleClient();

  if (WiFi.status() == WL_CONNECTED && !isConnected) {
    isConnected = true;
    Serial.println("✅ Connected to " + connectedSSID);
    Serial.println("STA IP: " + WiFi.localIP().toString());
  }
}