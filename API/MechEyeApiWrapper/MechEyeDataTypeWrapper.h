#pragma once
#include "comdef.h"

struct ErrorStatus
{
	int errorCode;
	BSTR errorDescription;
};

struct MechEyeDeviceInfo
{
	BSTR model;
	BSTR id;
	BSTR hardwareVersion;
	BSTR firmwareVersion;
	BSTR ipAddress;
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
