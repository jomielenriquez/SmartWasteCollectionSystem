#include <WiFi.h>
#include <WebServer.h>
#include <Preferences.h>

Preferences preferences;
WebServer server(80);

bool isConnected = false;
String connectedSSID = "";
String wifiList = "";

// ---------- HTML Pages ----------
String buildPage() {
  String page = R"rawliteral(
  <!DOCTYPE html>
  <html>
  <head>
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <style>
      body {
        font-family: Arial, sans-serif;
        margin: 0; padding: 20px;
        background: #f4f4f4;
      }
      .container {
        max-width: 400px;
        margin: auto;
        background: #fff;
        padding: 20px;
        border-radius: 12px;
        box-shadow: 0 4px 6px rgba(0,0,0,0.1);
      }
      h2 {
        text-align: center;
        color: #333;
      }
      label {
        display: block;
        margin: 12px 0 6px;
        font-weight: bold;
      }
      select, input[type=password], input[type=submit] {
        width: 100%;
        padding: 10px;
        margin-bottom: 15px;
        border: 1px solid #ccc;
        border-radius: 8px;
        font-size: 16px;
      }
      input[type=submit] {
        background: #28a745;
        color: white;
        border: none;
        cursor: pointer;
      }
      input[type=submit]:hover {
        background: #218838;
      }
      .status {
        text-align: center;
        font-size: 18px;
        color: green;
        margin-bottom: 15px;
      }
    </style>
  </head>
  <body>
    <div class="container">
      <h2>WiFi Setup</h2>
  )rawliteral";

  if (isConnected) {
    page += "<div class='status'>Connected to " + connectedSSID + "</div>";
  } else {
    page += "<form action='/connect' method='POST'>";
    page += "<label for='ssid'>SSID:</label>";
    page += "<select name='ssid' id='ssid'>";
    page += wifiList;
    page += "</select>";
    page += "<label for='pass'>Password:</label>";
    page += "<input type='password' name='pass' id='pass'>";
    page += "<input type='submit' value='Connect'>";
    page += "</form>";
  }

  page += "</div></body></html>";
  return page;
}


// ---------- Handlers ----------
void handleRoot() {
  server.send(200, "text/html", buildPage());
}

void handleConnect() {
  String ssid = server.arg("ssid");
  String pass = server.arg("pass");

  server.send(200, "text/html",
              "<html><body><h3>Connecting to " + ssid +
              "...</h3><meta http-equiv='refresh' content='5;url=/' /></body></html>");

  // Save credentials in flash
  preferences.begin("wifi", false);
  preferences.putString("ssid", ssid);
  preferences.putString("pass", pass);
  preferences.end();

  connectedSSID = ssid;
  WiFi.begin(ssid.c_str(), pass.c_str());
}

// ---------- Setup ----------
void setup() {
  Serial.begin(115200);

  // Load saved credentials
  preferences.begin("wifi", true);
  String savedSSID = preferences.getString("ssid", "");
  String savedPASS = preferences.getString("pass", "");
  preferences.end();

  if (savedSSID != "") {
    Serial.println("Found saved credentials. Connecting...");
    WiFi.begin(savedSSID.c_str(), savedPASS.c_str());
    connectedSSID = savedSSID;
  } else {
    Serial.println("No saved WiFi. Starting AP mode.");
    WiFi.softAP("ESP32-Setup", "12345678");
  }

  // Prepare WiFi list
  int n = WiFi.scanNetworks();
  for (int i = 0; i < n; ++i) {
    wifiList += "<option value='" + WiFi.SSID(i) + "'>" + WiFi.SSID(i) + "</option>";
  }

  // Setup webserver
  server.on("/", handleRoot);
  server.on("/connect", HTTP_POST, handleConnect);
  server.begin();
}

// ---------- Loop ----------
void loop() {
  server.handleClient();

  if (WiFi.status() == WL_CONNECTED && !isConnected) {
    isConnected = true;
    Serial.println("✅ Connected to " + connectedSSID);
    Serial.println("IP Address: " + WiFi.localIP().toString());
  }
}
