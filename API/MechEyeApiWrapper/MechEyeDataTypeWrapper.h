#pragma once
#include "comdef.h"

struct ErrorStatus
{
	int errorCode;
	char* errorDescription;
};

struct MechEyeDeviceInfo
{
	char* model;
	char* id;
	char* hardwareVersion;
	char* firmwareVersion;
	char* ipAddress;
	uint16_t port;
};

struct DeviceIntri
{
	double k1;
	double k2;
	double p1;
	double p2;
	double k3;
	double fx;
	double fy;
	double cx;
	double cy;
};
