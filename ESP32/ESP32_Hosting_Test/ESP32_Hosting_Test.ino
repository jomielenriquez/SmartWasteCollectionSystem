#include <WiFi.h>
#include <WebServer.h>
#include <Preferences.h>

Preferences preferences;
WebServer server(80);

bool isConnected = false;
String connectedSSID = "";
String wifiList = "";

// -------- HTML Page --------
String buildPage() {
  String page = R"rawliteral(
  <!DOCTYPE html><html><head>
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <style>
    body{font-family:Arial;margin:0;padding:20px;background:#f4f4f4}
    .container{max-width:420px;margin:auto;background:#fff;padding:18px;border-radius:12px;box-shadow:0 4px 6px rgba(0,0,0,.08)}
    h2{text-align:center;color:#333}
    label{display:block;margin:12px 0 6px;font-weight:600}
    select,input[type=password],button{width:100%;padding:12px;margin-bottom:12px;border:1px solid #ccc;border-radius:8px;font-size:16px}
    .btn-primary{background:#28a745;color:#fff;border:none}
    .btn-danger{background:#dc3545;color:#fff;border:none}
    .status{text-align:center;font-size:16px;color:green;margin-bottom:10px}
    .small{font-size:13px;color:#666;text-align:center}
  </style>
  </head><body><div class="container">
  <h2>WiFi Setup</h2>
  )rawliteral";

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

  page += "</div></body></html>";
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
  preferences.end();

  if (savedSSID != "") {
    Serial.println("Found saved Wi-Fi. Attempting to connect...");
    WiFi.begin(savedSSID.c_str(), savedPASS.c_str());
    connectedSSID = savedSSID;
  } else {
    Serial.println("No saved Wi-Fi. Waiting for config.");
  }

  // Scan networks
  int n = WiFi.scanNetworks();
  for (int i = 0; i < n; ++i) {
    wifiList += "<option value='" + WiFi.SSID(i) + "'>" + WiFi.SSID(i) + "</option>";
  }

  // Webserver
  server.on("/", handleRoot);
  server.on("/connect", HTTP_POST, handleConnect);
  server.on("/reset", HTTP_POST, handleReset);
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
