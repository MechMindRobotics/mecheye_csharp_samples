#pragma once
#include "comdef.h"

struct ErrorStatusWrapper
{
	int errorCode;
	BSTR errorDescription;
};

struct MechEyeDeviceInfoWrapper
{
	BSTR model;
	BSTR id;
	BSTR hardwareVersion;
	BSTR firmwareVersion;
	BSTR ipAddress;
	uint16_t port;
};

struct DeviceIntriWrapper
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
