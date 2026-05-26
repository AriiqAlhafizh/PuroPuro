#include <WiFi.h>
#include <WiFiUdp.h>
#include <Wire.h>

#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"

MPU6050 mpu;
WiFiUDP udp;

// =========================
// WIFI SETTINGS
// =========================
// const char* ssid = "*";
// const char* password = "HapeBintang";
const char* ssid = "ZTE_2.4G_EdXPE4";
const char* password = "BayiCantik,TapiBohong";

// Your PC IP Address
const char* udpAddress = "192.168.1.13";
const int udpSendPort = 4210;
const int udpReceivePort = 4211;

// =========================
// BUTTON PINS
// =========================
#define TRIGGER_BUTTON 4
#define CAL_BUTTON 5

// =========================
// MPU VARIABLES
// =========================
bool dmpReady = false;
uint8_t devStatus;
uint16_t packetSize;
uint16_t fifoCount;
uint8_t fifoBuffer[64];

Quaternion q;
VectorFloat gravity;
float ypr[3];

int ammo = 6;

// Calibration offset
float yawOffset = 0;
float pitchOffset = 0;

void setup()
{
    Serial.begin(115200);

    // Buttons
    pinMode(TRIGGER_BUTTON, INPUT_PULLUP);
    pinMode(CAL_BUTTON, INPUT_PULLUP);

    // I2C
    Wire.begin(21, 22);
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
            pitchOffset = pitch;
            delay(300);
        }

        // Apply offset
        yaw -= yawOffset;
        pitch -= pitchOffset;

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
        Serial.println(packet);

        receiveAmmo();
    }
    delay(5);
}

void receiveAmmo()
{
    int packetSize = udp.parsePacket();

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

        ammo = atoi(incomingPacket);

        Serial.print("Ammo: ");
        Serial.println(ammo);

        // TODO:
        // update OLED / LED display here
    }
}
