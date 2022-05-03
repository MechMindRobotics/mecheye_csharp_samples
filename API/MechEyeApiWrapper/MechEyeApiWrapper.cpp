#include <comutil.h>
#include <wtypes.h>
#include <strsafe.h>
#include <string>
#include "MechEyeApi.h"
#include "MechEyeDataTypeWrapper.h"

std::string charArrToString(const char* charPtr)
{
	std::string str = charPtr;
	return str;
}

char* stringToCharArr(const std::string& str)
{
	char* charArr = new char[str.length() + 1];
	strcpy_s(charArr, str.length() + 1, str.c_str());
	return charArr;
}

MechEyeDeviceInfo infoToWrapper(const mmind::api::MechEyeDeviceInfo & info)
{
	MechEyeDeviceInfo wrapper;
	wrapper.model = stringToCharArr(info.model);
	wrapper.id = stringToCharArr(info.id);
	wrapper.hardwareVersion = stringToCharArr(info.hardwareVersion);
	wrapper.firmwareVersion = stringToCharArr(info.firmwareVersion);
	wrapper.ipAddress = stringToCharArr(info.ipAddress);
	wrapper.port = info.port;
	return wrapper;
}

mmind::api::MechEyeDeviceInfo wrapperToInfo(const MechEyeDeviceInfo & wrapper)
{
	mmind::api::MechEyeDeviceInfo info;
	info.model = charArrToString(wrapper.model);
	info.id = charArrToString(wrapper.id);
	info.hardwareVersion = charArrToString(wrapper.hardwareVersion);
	info.firmwareVersion = charArrToString(wrapper.firmwareVersion);
	info.ipAddress = charArrToString(wrapper.ipAddress);
	info.port = wrapper.port;
	return info;
}

DeviceIntri & intriToWrapper(const mmind::api::DeviceIntri & intri)
{
	DeviceIntri wrapper = {
	intri.distCoeffs[0],
	intri.distCoeffs[1],
	intri.distCoeffs[2],
	intri.distCoeffs[3],
	intri.distCoeffs[4],
	intri.cameraMatrix[0],
	intri.cameraMatrix[1],
	intri.cameraMatrix[2],
	intri.cameraMatrix[3]
	};
	return wrapper;
}

mmind::api::DeviceIntri& wrapperToIntri(const DeviceIntri & wrapper)
{
	mmind::api::DeviceIntri intri = {
		{wrapper.k1, wrapper.k2, wrapper.k3, wrapper.p1, wrapper.p2},
		{wrapper.fx, wrapper.fy, wrapper.cx, wrapper.cy}
	};
	return intri;
}

ErrorStatus errorStatusToWrapper(mmind::api::ErrorStatus status)
{
	ErrorStatus wrapper;
	wrapper.errorCode = status.errorCode;
	wrapper.errorDescription = stringToCharArr(status.errorDescription);
	return wrapper;
}

mmind::api::ErrorStatus wrapperToErrorStatus(ErrorStatus wrapper)
{
	mmind::api::ErrorStatus status(static_cast<mmind::api::ErrorStatus::ErrorCode>(wrapper.errorCode), charArrToString(wrapper.errorDescription));
	return status;
}

extern "C" __declspec(dllexport) mmind::api::MechEyeDevice * CreateMechEyeDevice()
{
	return new mmind::api::MechEyeDevice();
}

extern "C" __declspec(dllexport) int GetMechEyeDeviceListSize()
{
	std::vector<mmind::api::MechEyeDeviceInfo> infoVec = mmind::api::MechEyeDevice::enumerateMechEyeDeviceList();
	return infoVec.size();
}

extern "C" __declspec(dllexport) void EnumerateMechEyeDeviceList(MechEyeDeviceInfo ** ppArray, int* pSize)
{
	std::vector<mmind::api::MechEyeDeviceInfo> infoVec = mmind::api::MechEyeDevice::enumerateMechEyeDeviceList();
	MechEyeDeviceInfo* newArr = (MechEyeDeviceInfo*)CoTaskMemAlloc(sizeof(MechEyeDeviceInfo) * infoVec.size());
	for (int i = 0; i < infoVec.size(); ++i)
		newArr[i] = infoToWrapper(infoVec[i]);
	CoTaskMemFree(*ppArray);
	*ppArray = newArr;
	*pSize = infoVec.size();
}

extern "C" __declspec(dllexport) ErrorStatus Connect(mmind::api::MechEyeDevice * devicePtr, MechEyeDeviceInfo * info)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->connect(wrapperToInfo(*info)));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) void Disconnect(mmind::api::MechEyeDevice * devicePtr)
{
	if (devicePtr != nullptr)
		devicePtr->disconnect();
}

extern "C" __declspec(dllexport) ErrorStatus GetDeviceInfo(mmind::api::MechEyeDevice * devicePtr, MechEyeDeviceInfo * wrapper)
{
	if (devicePtr != nullptr) {
		mmind::api::MechEyeDeviceInfo info = wrapperToInfo(*wrapper);
		mmind::api::ErrorStatus status = devicePtr->getDeviceInfo(info);
		*wrapper = infoToWrapper(info);
		return errorStatusToWrapper(status);
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetDeviceIntri(mmind::api::MechEyeDevice * devicePtr, DeviceIntri * wrapper)
{
	if (devicePtr != nullptr) {
		mmind::api::DeviceIntri intri = wrapperToIntri(*wrapper);
		mmind::api::ErrorStatus status = devicePtr->getDeviceIntri(intri);
		*wrapper = intriToWrapper(intri);
		return errorStatusToWrapper(status);
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetDeviceResolution(mmind::api::MechEyeDevice * devicePtr, mmind::api::DeviceResolution * resolution)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->getDeviceResolution(*resolution));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus CaptureColorMap(mmind::api::MechEyeDevice * devicePtr, mmind::api::ColorMap * mapPtr)
{
	if (devicePtr != nullptr && mapPtr != nullptr)
		return errorStatusToWrapper(devicePtr->captureColorMap(*mapPtr));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus CaptureDepthMap(mmind::api::MechEyeDevice * devicePtr, mmind::api::DepthMap * mapPtr)
{
	if (devicePtr != nullptr && mapPtr != nullptr)
		return errorStatusToWrapper(devicePtr->captureDepthMap(*mapPtr));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus CapturePointXYZMap(mmind::api::MechEyeDevice * devicePtr, mmind::api::PointXYZMap * mapPtr)
{
	if (devicePtr != nullptr && mapPtr != nullptr)
		return errorStatusToWrapper(devicePtr->capturePointXYZMap(*mapPtr));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus CapturePointXYZBGRMap(mmind::api::MechEyeDevice * devicePtr, mmind::api::PointXYZBGRMap * mapPtr)
{
	if (devicePtr != nullptr && mapPtr != nullptr)
		return errorStatusToWrapper(devicePtr->capturePointXYZBGRMap(*mapPtr));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetScan2DExposureMode(mmind::api::MechEyeDevice * devicePtr, int value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setScan2DExposureMode(static_cast<mmind::api::Scanning2DSettings::Scan2DExposureMode>(value)));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetScan2DExposureMode(mmind::api::MechEyeDevice * devicePtr, int* value)
{
	if (devicePtr != nullptr) {
		mmind::api::Scanning2DSettings::Scan2DExposureMode mode;
		ErrorStatus wrapper = errorStatusToWrapper(devicePtr->getScan2DExposureMode(mode));
		*value = mode;
		return wrapper;
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetScan2DExposureTime(mmind::api::MechEyeDevice * devicePtr, double value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setScan2DExposureTime(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetScan2DExposureTime(mmind::api::MechEyeDevice * devicePtr, double* value)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getScan2DExposureTime(*value));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetScan2DHDRExposureSequence(mmind::api::MechEyeDevice* devicePtr, double* values, int size)
{
	if (devicePtr != nullptr) {
		std::vector<double> sequence(values, values+size);
		return errorStatusToWrapper(devicePtr->setScan2DHDRExposureSequence(sequence));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetScan2DHDRExposureSequence(mmind::api::MechEyeDevice * devicePtr, double* values)
{
	if (devicePtr != nullptr) {
		std::vector<double> sequence;
		ErrorStatus status = errorStatusToWrapper(devicePtr->getScan2DHDRExposureSequence(sequence));
		values = &sequence[0];
		return status;
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetScan2DExpectedGrayValue(mmind::api::MechEyeDevice * devicePtr, int value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setScan2DExpectedGrayValue(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetScan2DExpectedGrayValue(mmind::api::MechEyeDevice * devicePtr, int* value)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getScan2DExpectedGrayValue(*value));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetScan2DToneMappingEnable(mmind::api::MechEyeDevice * devicePtr, bool value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setScan2DToneMappingEnable(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetScan2DToneMappingEnable(mmind::api::MechEyeDevice * devicePtr, bool* value)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getScan2DToneMappingEnable(*value));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetScan2DSharpenFactor(mmind::api::MechEyeDevice * devicePtr, double value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setScan2DSharpenFactor(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetScan2DSharpenFactor(mmind::api::MechEyeDevice * devicePtr, double* value)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getScan2DSharpenFactor(*value));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetScan2DROI(mmind::api::MechEyeDevice * devicePtr, mmind::api::ROI * roi)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setScan2DROI(*roi));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetScan2DROI(mmind::api::MechEyeDevice * devicePtr, mmind::api::ROI* roi)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getScan2DROI(*roi));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetScan3DExposure(mmind::api::MechEyeDevice* devicePtr, double* values, int size)
{
	if (devicePtr != nullptr) {
		std::vector<double> sequence(values, values+size);
		return errorStatusToWrapper(devicePtr->setScan3DExposure(sequence));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetScan3DExposure(mmind::api::MechEyeDevice* devicePtr, double** ppArray, int* pSize)
{
	if (devicePtr != nullptr) {
		std::vector<double> sequence;
		ErrorStatus status = errorStatusToWrapper(devicePtr->getScan3DExposure(sequence));
		double* newArr = (double*)CoTaskMemAlloc(sizeof(double) * sequence.size());
		for (int i = 0; i < sequence.size(); ++i)
			newArr[i] = sequence[i];
		CoTaskMemFree(*ppArray);
		*ppArray = newArr;
		*pSize = sequence.size();
		return status;
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetScan3DGain(mmind::api::MechEyeDevice * devicePtr, double value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setScan3DGain(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetScan3DGain(mmind::api::MechEyeDevice * devicePtr, double* value)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getScan3DGain(*value));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetScan3DROI(mmind::api::MechEyeDevice * devicePtr, mmind::api::ROI * roi)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setScan3DROI(*roi));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetScan3DROI(mmind::api::MechEyeDevice * devicePtr, mmind::api::ROI* roi)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getScan3DROI(*roi));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetDepthRange(mmind::api::MechEyeDevice * devicePtr, mmind::api::DepthRange * depthRange)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setDepthRange(*depthRange));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetDepthRange(mmind::api::MechEyeDevice * devicePtr, mmind::api::DepthRange* depthRange)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getDepthRange(*depthRange));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetFringeContrastThreshold(mmind::api::MechEyeDevice * devicePtr, int value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setFringeContrastThreshold(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetFringeContrastThreshold(mmind::api::MechEyeDevice * devicePtr, int* value)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getFringeContrastThreshold(*value));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetFringeMinThreshold(mmind::api::MechEyeDevice * devicePtr, int value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setFringeMinThreshold(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetFringeMinThreshold(mmind::api::MechEyeDevice * devicePtr, int* value)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getFringeMinThreshold(*value));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetCloudOutlierFilterMode(mmind::api::MechEyeDevice * devicePtr, int value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setCloudOutlierFilterMode(static_cast<mmind::api::PointCloudProcessingSettings::CloudOutlierFilterMode>(value)));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetCloudOutlierFilterMode(mmind::api::MechEyeDevice * devicePtr, int* value)
{
	if (devicePtr != nullptr) {
		mmind::api::PointCloudProcessingSettings::CloudOutlierFilterMode mode;
		ErrorStatus wrapper = errorStatusToWrapper(devicePtr->getCloudOutlierFilterMode(mode));
		*value = static_cast<int>(mode);
		return wrapper;
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetCloudSmoothMode(mmind::api::MechEyeDevice * devicePtr, int value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setCloudSmoothMode(static_cast<mmind::api::PointCloudProcessingSettings::CloudSmoothMode>(value)));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetCloudSmoothMode(mmind::api::MechEyeDevice * devicePtr, int* value)
{
	if (devicePtr != nullptr) {
		mmind::api::PointCloudProcessingSettings::CloudSmoothMode mode;
		ErrorStatus wrapper = errorStatusToWrapper(devicePtr->getCloudSmoothMode(mode));
		*value = static_cast<int>(mode);
		return wrapper;
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetLaserSettings(mmind::api::MechEyeDevice * devicePtr, mmind::api::LaserSettings value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setLaserSettings(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetLaserSettings(mmind::api::MechEyeDevice * devicePtr, mmind::api::LaserSettings* value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->getLaserSettings(*value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SaveAllSettingsToUserSets(mmind::api::MechEyeDevice * devicePtr)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->saveAllSettingsToUserSets());
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus SetCurrentUserSet(mmind::api::MechEyeDevice * devicePtr, const char * value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setCurrentUserSet(std::string(value)));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus GetCurrentUserSet(mmind::api::MechEyeDevice * devicePtr, char** value)
{
	if (devicePtr != nullptr) {
		std::string name;
		ErrorStatus status = errorStatusToWrapper(devicePtr->getCurrentUserSet(name));
		*value = stringToCharArr(name);
		return status;
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) int GetUserSetsCount(mmind::api::MechEyeDevice * devicePtr)
{
	if (devicePtr != nullptr) {
		std::vector<std::string> sets;
		devicePtr->getAllUserSets(sets);
		return sets.size();
	}
	else
		return 0;
}

extern "C" __declspec(dllexport) ErrorStatus GetAllUserSets(mmind::api::MechEyeDevice * devicePtr, char * names[])
{
	if (devicePtr != nullptr) {
		std::vector<std::string> sets;
		ErrorStatus status = errorStatusToWrapper(devicePtr->getAllUserSets(sets));
		STRSAFE_LPSTR temp;
		for (int i = 0; i < sets.size(); ++i)
		{
			temp = (STRSAFE_LPSTR)CoTaskMemAlloc(sets[i].size());
			StringCchCopyA(temp, sets[i].size() + 1, (STRSAFE_LPSTR)sets[i].c_str());
			CoTaskMemFree(names[i]);
			names[i] = (char*)temp;
		}
		return status;
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus DeleteUserSet(mmind::api::MechEyeDevice * devicePtr, const char * value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->deleteUserSet(std::string(value)));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatus AddUserSet(mmind::api::MechEyeDevice * devicePtr, const char * value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->addUserSet(std::string(value)));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}
