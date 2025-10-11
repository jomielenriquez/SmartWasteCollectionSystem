// References
// Ultrasonic with esp32 : https://randomnerdtutorials.com/esp32-hc-sr04-ultrasonic-arduino/
// motion sensor: https://projecthub.arduino.cc/electronicsfan123/interfacing-arduino-uno-with-pir-motion-sensor-593b6b
// ESP32 Tutorial: https://randomnerdtutorials.com/getting-started-with-esp32/
// ESP32 Pinouts: https://lastminuteengineers.com/esp32-pinout-reference/
// 

#include <Arduino.h>
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
String SaveBinSettingsMessage = "";
String connectedSSID = "";
String wifiList = "";
String apiKey = "";

// Utrasonic
const int trigPin = 5;
const int echoPin = 18;
#define SOUND_SPEED 0.0343 //define sound speed in cm/uS
#define CM_TO_INCH 0.393701
long duration;
float distanceCm;
float distanceInch;

// Motion Sensor
#define PIR_PIN 27   // GPIO pin where PIR OUT is connected
// int motionCount = 0;
bool isBinOpen = false;
bool isBinOpenLogged = true;
float binOpenTravelCounter = 0;
float binOpenTravelCounterLimit = 7; // 7 sec travel limit
float binOpenCounter = 0;
float binOpenCounterLimit = 6; // 6 sec bin will be open
float binLock = 100;
float loopDelay = 0.5;
float binHeight = 30.0;

// Relay Module
#define RELAY1_PIN 26
#define RELAY2_PIN 25

// MQ3
#define MQ3_PIN 34

int readMQ3(){
  int samples = 10;
  long sum = 0;
  for (int i = 0; i < samples; i++) {
    sum += analogRead(MQ3_PIN); // 0 - 4095 on ESP32
    delay(10);
  }
  int sensorValue = sum / samples;
  float voltage = sensorValue * (3.3 / 4095.0);

  Serial.print("ADC: ");
  Serial.print(sensorValue);
  Serial.print(" | Voltage: ");
  Serial.print(voltage);
  
  if (sensorValue < 1000) {
    Serial.println(" -> Air is clean ✅");
  } else if (sensorValue < 2000) {
    Serial.println(" -> Mild odor ⚠️");
  } else {
    Serial.println(" -> Strong foul odor ❌");
  }
  return sensorValue;
}

float readUltraSonic(){
  // Ultrasonic start with 5 sec delay
  // Clears the trigPin
  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  // Sets the trigPin on HIGH state for 10 micro seconds
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);
  // Reads the echoPin, returns the sound wave travel time in microseconds
  duration = pulseIn(echoPin, HIGH, 30000);
  // Calculate the distance
  distanceCm = duration * SOUND_SPEED/2;
  distanceInch = distanceCm * CM_TO_INCH;

  // Prevent negative values
  if (distanceInch > binHeight) distanceInch = binHeight;
  if (distanceInch < 0) distanceInch = 0;

  // Compute percentage
  float fillPercent = ((binHeight - distanceInch) / binHeight) * 100.0;

  return fillPercent;
}

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
    page += R"rawliteral(
      <form action="/binSettings" method="GET">
        <button type="submit" class="btn-primary">Bin Settings</button>
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
  page += "<button type='submit' class='btn-primary'>Send</button></form>";
  page += pageFooter();
  SaveApiMessage = "";
  server.send(200, "text/html", page);
}

void PostSendBinStatus() {
  if(apiKey == ""){
    preferences.begin("wifi", true);
    apiKey = preferences.getString("apiKey", "");
    preferences.end();
  }

  String perc = String(readUltraSonic(), 0);
  int mq3 = readMQ3();

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

    String payload = "apiKey=" + apiKey + "&percentage=" + perc + "&mq3=" + mq3;
    int httpCode = http.POST(payload);

    if (httpCode > 0) {
      String response = http.getString();
      SaveApiMessage = "POST Sent!";
    } else {
      SaveApiMessage = "Error sending POST";
    }
    http.end();
  } else {
    SaveApiMessage = "Not connected to WiFi!";
  }
}

void handleSendPost(){
  PostSendBinStatus();
  handlePostPage();
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

void handleBinSettingsPage() {
  String page = PageHeader("Bin Settings Page");
  if (SaveBinSettingsMessage != "") {page += "<div class='status'>" + SaveBinSettingsMessage + "</div>";}
  page += "<form action='/saveBinSettings' method='POST'>";
  page += "<label for='binOpenTravelCounterLimit'>Bin Open Travel Counter Limit (sec):</label><input type='text' name='binOpenTravelCounterLimit' id='binOpenTravelCounterLimit' value='" + String(binOpenTravelCounterLimit, 1) + "'>";
  page += "<label for='binOpenCounterLimit'>Bin Open Counter Limit (sec):</label><input type='text' name='binOpenCounterLimit' id='binOpenCounterLimit' value='" + String(binOpenCounterLimit, 1) + "'>";
  page += "<label for='binHeight'>Bin Height (inch):</label><input type='text' name='binHeight' id='binHeight' value='" + String(binHeight, 1) + "'>";
  page += "<label for='binLock'>Bin Lock (%):</label><input type='text' name='binLock' id='binLock' value='" + String(binLock, 1) + "'>";
  page += "<button type='submit' class='btn-primary'>Save</button></form>";
  page += pageFooter();
  SaveBinSettingsMessage = "";
  server.send(200, "text/html", page);
}

void handleSaveBinSettings() {
  binOpenTravelCounterLimit = server.arg("binOpenTravelCounterLimit").toFloat();
  binOpenCounterLimit = server.arg("binOpenCounterLimit").toFloat();
  binHeight = server.arg("binHeight").toFloat();
  binLock = server.arg("binLock").toFloat();
  
  preferences.begin("wifi", false);
  preferences.putFloat("binOpening", binOpenTravelCounterLimit);
  preferences.putFloat("binOpen", binOpenCounterLimit);
  preferences.putFloat("binHeight", binHeight);
  preferences.putFloat("binLock", binLock);
  preferences.end();
  SaveBinSettingsMessage = "Bin Settings saved!";
  handleBinSettingsPage();
}
// // put function declarations here:
// int myFunction(int, int);

void closeBin(){
  digitalWrite(RELAY1_PIN, HIGH);
  digitalWrite(RELAY2_PIN, LOW);
}

void openBin(){
  digitalWrite(RELAY1_PIN, LOW);
  digitalWrite(RELAY2_PIN, HIGH);
}

void standbyBin(){
  digitalWrite(RELAY1_PIN, LOW);
  digitalWrite(RELAY2_PIN, LOW);
}

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
  binOpenTravelCounterLimit = preferences.getFloat("binOpening", 7);
  binOpenCounterLimit = preferences.getFloat("binOpen", 6);
  binHeight = preferences.getFloat("binHeight", 6);
  binLock = preferences.getFloat("binLock", 100);
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
  server.on("/binSettings", handleBinSettingsPage);
  server.on("/saveBinSettings", HTTP_POST, handleSaveBinSettings);
  server.begin();

  //UltraSonic
  pinMode(trigPin, OUTPUT); // Sets the trigPin as an Output
  pinMode(echoPin, INPUT); // Sets the echoPin as an Input

  //Motion Sensor
  pinMode(PIR_PIN, INPUT);  // PIR sensor as input

  //Relay
  pinMode(RELAY1_PIN, OUTPUT);
  pinMode(RELAY2_PIN, OUTPUT);
  digitalWrite(RELAY1_PIN, LOW);
  digitalWrite(RELAY2_PIN, LOW);

  closeBin();
}

void loop() {
  server.handleClient();

  if (WiFi.status() == WL_CONNECTED && !isConnected) {
    isConnected = true;
    Serial.println("✅ Connected to " + connectedSSID);
    Serial.println("STA IP: " + WiFi.localIP().toString());
  }

  int motion = digitalRead(PIR_PIN);

  Serial.print("binOpenTravelCounter: "); Serial.println(binOpenTravelCounter);
  Serial.print("binOpenCounter: "); Serial.println(binOpenCounter);

  if (motion == HIGH) {
    int mq3Reading = readMQ3();

    Serial.println(" Motion Detected!");
    if (!isBinOpen && mq3Reading < 2000 && readUltraSonic() < binLock){
      isBinOpen = true;
    }
    else{
      PostSendBinStatus();
    }
  } 
  else if (binOpenCounter >= binOpenCounterLimit) {
    Serial.println("No Motion");
    isBinOpen = false;
    binOpenCounter = 0;
  }

  if (isBinOpen && binOpenTravelCounter < binOpenTravelCounterLimit){
    Serial.println("openBin");
    openBin();
    binOpenTravelCounter += loopDelay;
  }
  else if (isBinOpen && binOpenTravelCounter >= binOpenTravelCounterLimit){
    Serial.println("standbyBin");
    standbyBin();
    binOpenCounter += loopDelay;
  }
  else if (!isBinOpen){
    Serial.println("closeBin");
    closeBin();
    if (binOpenTravelCounter > 0){
      binOpenTravelCounter -= loopDelay;
      isBinOpenLogged = false;
    }
    else if(binOpenTravelCounter <=0 && !isBinOpenLogged){
      isBinOpenLogged = true;
      PostSendBinStatus();
    }
  }
  delay((loopDelay * 1000));
}

// // put function definitions here:
// int myFunction(int x, int y) {
//   return x + y;
// }