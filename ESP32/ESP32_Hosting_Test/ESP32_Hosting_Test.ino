#include <WiFi.h>
#include <WebServer.h>

WebServer server(80);
String wifiList = "";
bool isConnected = false;
String connectedSSID = "";

void handleRoot() {
  String page = "<html><body>";
  page += "<h2>WiFi Setup</h2>";

  if (isConnected) {
    page += "<p style='color:green;'>✅ Connected to " + connectedSSID + "</p>";
  } else {
    page += "<form action='/connect' method='POST'>";
    page += "<label>SSID:</label><select name='ssid'>";
    page += wifiList;
    page += "</select><br><br>";
    page += "<label>Password:</label><input type='password' name='pass'><br><br>";
    page += "<input type='submit' value='Connect'>";
    page += "</form>";
  }

  page += "</body></html>";
  server.send(200, "text/html", page);
}

void handleConnect() {
  String ssid = server.arg("ssid");
  String pass = server.arg("pass");

  server.send(200, "text/html", "<html><body><h3>Connecting to " + ssid + "...</h3><meta http-equiv='refresh' content='3;url=/' /></body></html>");

  WiFi.begin(ssid.c_str(), pass.c_str());
  connectedSSID = ssid;
}

void setup() {
  Serial.begin(115200);

  // Scan networks
  int n = WiFi.scanNetworks();
  for (int i = 0; i < n; ++i) {
    wifiList += "<option value='" + WiFi.SSID(i) + "'>" + WiFi.SSID(i) + "</option>";
  }

  // Start AP
  WiFi.softAP("ESP32-Setup", "12345678");
  Serial.print("AP IP: ");
  Serial.println(WiFi.softAPIP());

  server.on("/", handleRoot);
  server.on("/connect", HTTP_POST, handleConnect);
  server.begin();
}

void loop() {
  server.handleClient();

  // Check connection status
  if (WiFi.status() == WL_CONNECTED && !isConnected) {
    isConnected = true;
    Serial.println("✅ Connected to " + connectedSSID);
  }
}
