#include <WiFi.h>
#include <WiFiUdp.h>
#include <Wire.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>

#include "config.h"
#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"

MPU6050 mpu;
WiFiUDP udp;

// Your PC IP Address
const char* udpAddress = "10.32.110.222";
const int udpSendPort = 4210;
const int udpReceivePort = 4211;

// =========================
// BUTTON PINS
// =========================
#define TRIGGER_BUTTON 32
#define CAL_BUTTON 33

// =========================
// MPU VARIABLES
// =========================
bool dmpReady = false;
uint8_t devStatus;
uint16_t packetSize;
uint16_t fifoCount;
uint8_t fifoBuffer[64];

// =========================
// DISPLAY VARIABLES
// =========================
#define SCREEN_WIDTH 128
#define SCREEN_HEIGHT 64

// =========================
// HP LEDs
// =========================
#define HP_LED_1 16
#define HP_LED_2 17
#define HP_LED_3 18
int hp = 3;

#define OLED_RESET -1

Adafruit_SSD1306 display(
    SCREEN_WIDTH,
    SCREEN_HEIGHT,
    &Wire,
    OLED_RESET
);

Quaternion q;
VectorFloat gravity;
float ypr[3];

int ammo = 6;

// Calibration offset
float yawOffset = 0;
float rollOffset = 0;

void setup()
{
    Serial.begin(115200);

    // LEDs
    pinMode(HP_LED_1, OUTPUT);
    pinMode(HP_LED_2, OUTPUT);
    pinMode(HP_LED_3, OUTPUT);

    // Buttons
    pinMode(TRIGGER_BUTTON, INPUT_PULLUP);
    pinMode(CAL_BUTTON, INPUT_PULLUP);

    // I2C
    Wire.begin(21, 22);

    if(!display.begin(
        SSD1306_SWITCHCAPVCC,
        0x3C
    ))
    {
        Serial.println("OLED failed");

        while(1);
    }

    display.clearDisplay();

    display.setTextColor(SSD1306_WHITE);

    display.display();

    Wire.setClock(400000);

    // WiFi
    WiFi.begin(ssid, password);

    Serial.print("Connecting to WiFi");

    while (WiFi.status() != WL_CONNECTED)
    {
        delay(500);
        Serial.print(".");
    }

    Serial.println("\nWiFi connected!");
    Serial.print("IP Address: ");
    Serial.println(WiFi.localIP());

    // UDP LISTENER
    udp.begin(udpReceivePort);

    Serial.print("Listening UDP on ");
    Serial.println(udpReceivePort);

    delay(1000);

    // MPU6050
    mpu.initialize();

    if (mpu.dmpInitialize() == 0)
    {
        mpu.setDMPEnabled(true);
        dmpReady = true;

        Serial.println("DMP Ready!");
    }
    else
    {
        Serial.println("DMP Initialization Failed!");
    }

    updateOLED();
    updateHPLEDs();
}

void loop()
{
    if (!dmpReady)
        return;

    // Read DMP packet
    if (mpu.dmpGetCurrentFIFOPacket(fifoBuffer))
    {
        mpu.dmpGetQuaternion(&q, fifoBuffer);
        mpu.dmpGetGravity(&gravity, &q);
        mpu.dmpGetYawPitchRoll(ypr, &q, &gravity);

        // Convert radians to degrees
        float yaw =
            ypr[0] * 180.0 / M_PI;

        float pitch =
            ypr[1] * 180.0 / M_PI;

        float roll =
            ypr[2] * 180.0 / M_PI;

        // =========================
        // CALIBRATION BUTTON
        // =========================
        if (digitalRead(CAL_BUTTON) == LOW)
        {
            yawOffset = yaw;
            rollOffset = roll;
            delay(300);
        }

        // Apply offset
        yaw -= yawOffset;
        roll -= rollOffset;

        // =========================
        // TRIGGER BUTTON
        // =========================
        int trigger =
            (digitalRead(TRIGGER_BUTTON) == LOW) ? 1 : 0;

        // =========================
        // CREATE UDP PACKET
        // =========================
        char packet[128];

        sprintf(packet,
                "%.2f,%.2f,%d",
                yaw,
                roll,
                trigger);

        // Send UDP packet
        udp.beginPacket(udpAddress, udpSendPort);
        udp.write((uint8_t*)packet, strlen(packet));
        udp.endPacket();

        // Debug
        // Serial.println(packet);
        // Serial.print("Trigger: ");
        // Serial.print(digitalRead(TRIGGER_BUTTON));

        // Serial.print(" | Cal: ");
        // Serial.println(digitalRead(CAL_BUTTON));

        receiveHUDData();
    }
    delay(5);
}

void receiveHUDData()
{
    int packetSize =
        udp.parsePacket();

    if(packetSize)
    {
        char incomingPacket[255];

        int len =
            udp.read(
                incomingPacket,
                255
            );

        if(len > 0)
        {
            incomingPacket[len] = 0;
        }

        Serial.print("Received: ");
        Serial.println(incomingPacket);

        // PARSE CSV
        int receivedAmmo;
        int receivedHP;

        sscanf(
            incomingPacket,
            "%d,%d",
            &receivedAmmo,
            &receivedHP
        );

        ammo = receivedAmmo;
        hp = receivedHP;

        // UPDATE DEVICES
        updateOLED();
        updateHPLEDs();

        Serial.print("Ammo: ");
        Serial.println(ammo);

        Serial.print("HP: ");
        Serial.println(hp);
    }
}

void updateOLED()
{
    display.clearDisplay();

    // TITLE
    display.setTextSize(1);

    display.setCursor(0,0);

    display.println("AMMO");

    // BIG NUMBER
    display.setTextSize(4);

    display.setCursor(20,20);

    display.println(ammo);

    display.display();
}

void updateHPLEDs()
{
    digitalWrite(
        HP_LED_1,
        hp >= 1
    );

    digitalWrite(
        HP_LED_2,
        hp >= 2
    );

    digitalWrite(
        HP_LED_3,
        hp >= 3
    );
}