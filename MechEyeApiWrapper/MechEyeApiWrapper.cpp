#include <comutil.h>
#include <wtypes.h>
#include <strsafe.h>
#include "MechEyeApi.h"
#include "MechEyeDataTypeWrapper.h"

std::string bstrToString(const BSTR & bstr)
{
	_bstr_t bstr_t(bstr, true);
	std::string str(bstr_t.operator const char *());
	return str;
}

BSTR stringToBSTR(const std::string& str)
{
	if (str.size()) {
		int wslen = ::MultiByteToWideChar(CP_ACP, 0 /* no flags */,
			str.data(), str.length(),
			NULL, 0);

		BSTR wsdata = ::SysAllocStringLen(NULL, wslen);
		::MultiByteToWideChar(CP_ACP, 0 /* no flags */,
			str.data(), str.length(),
			wsdata, wslen);
		return wsdata;
	}
	else
		return NULL;
}

MechEyeDeviceInfoWrapper infoToWrapper(const mmind::api::MechEyeDeviceInfo & info)
{
	MechEyeDeviceInfoWrapper wrapper;
	wrapper.model = stringToBSTR(info.model);
	wrapper.id = stringToBSTR(info.id);
	wrapper.hardwareVersion = stringToBSTR(info.hardwareVersion);
	wrapper.firmwareVersion = stringToBSTR(info.firmwareVersion);
	wrapper.ipAddress = stringToBSTR(info.ipAddress);
	wrapper.port = info.port;
	return wrapper;
}

mmind::api::MechEyeDeviceInfo wrapperToInfo(const MechEyeDeviceInfoWrapper & wrapper)
{
	mmind::api::MechEyeDeviceInfo info;
	info.model = bstrToString(wrapper.model);
	info.id = bstrToString(wrapper.id);
	info.hardwareVersion = bstrToString(wrapper.hardwareVersion);
	info.firmwareVersion = bstrToString(wrapper.firmwareVersion);
	info.ipAddress = bstrToString(wrapper.ipAddress);
	info.port = wrapper.port;
	return info;
}

DeviceIntriWrapper & intriToWrapper(const mmind::api::DeviceIntri & intri)
{
	DeviceIntriWrapper wrapper = {
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

mmind::api::DeviceIntri& wrapperToIntri(const DeviceIntriWrapper& wrapper)
{
	mmind::api::DeviceIntri intri = {
		{wrapper.k1, wrapper.k2, wrapper.k3, wrapper.p1, wrapper.p2},
		{wrapper.fx, wrapper.fy, wrapper.cx, wrapper.cy}
	};
	return intri;
}

ErrorStatusWrapper errorStatusToWrapper(mmind::api::ErrorStatus status)
{
	ErrorStatusWrapper wrapper;
	wrapper.errorCode = status.errorCode;
	wrapper.errorDescription = stringToBSTR(status.errorDescription);
	return wrapper;
}

mmind::api::ErrorStatus wrapperToErrorStatus(ErrorStatusWrapper wrapper)
{
	mmind::api::ErrorStatus status(static_cast<mmind::api::ErrorStatus::ErrorCode>(wrapper.errorCode), bstrToString(wrapper.errorDescription));
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

extern "C" __declspec(dllexport) void EnumerateMechEyeDeviceList(MechEyeDeviceInfoWrapper * list)
{
	std::vector<mmind::api::MechEyeDeviceInfo> infoVec = mmind::api::MechEyeDevice::enumerateMechEyeDeviceList();
	for (int i = 0; i < infoVec.size(); ++i)
		list[i] = infoToWrapper(infoVec[i]);
}

extern "C" __declspec(dllexport) ErrorStatusWrapper  Connect(mmind::api::MechEyeDevice * devicePtr, MechEyeDeviceInfoWrapper info)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->connect(wrapperToInfo(info)));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) void Disconnect(mmind::api::MechEyeDevice * devicePtr)
{
	if (devicePtr != nullptr)
		devicePtr->disconnect();
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetDeviceInfo(mmind::api::MechEyeDevice * devicePtr, MechEyeDeviceInfoWrapper & wrapper)
{
	if (devicePtr != nullptr) {
		mmind::api::MechEyeDeviceInfo info = wrapperToInfo(wrapper);
		mmind::api::ErrorStatus status = devicePtr->getDeviceInfo(info);
		wrapper = infoToWrapper(info);
		return errorStatusToWrapper(status);
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetDeviceIntri(mmind::api::MechEyeDevice * devicePtr, DeviceIntriWrapper & wrapper)
{
	if (devicePtr != nullptr) {
		mmind::api::DeviceIntri intri = wrapperToIntri(wrapper);
		mmind::api::ErrorStatus status = devicePtr->getDeviceIntri(intri);
		wrapper = intriToWrapper(intri);
		return errorStatusToWrapper(status);
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetDeviceResolution(mmind::api::MechEyeDevice * devicePtr, mmind::api::DeviceResolution & resolution)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->getDeviceResolution(resolution));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper CaptureColorMap(mmind::api::MechEyeDevice * devicePtr, mmind::api::ColorMap * mapPtr)
{
	if (devicePtr != nullptr && mapPtr != nullptr)
		return errorStatusToWrapper(devicePtr->captureColorMap(*mapPtr));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper CaptureDepthMap(mmind::api::MechEyeDevice * devicePtr, mmind::api::DepthMap * mapPtr)
{
	if (devicePtr != nullptr && mapPtr != nullptr)
		return errorStatusToWrapper(devicePtr->captureDepthMap(*mapPtr));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper CapturePointXYZMap(mmind::api::MechEyeDevice * devicePtr, mmind::api::PointXYZMap * mapPtr)
{
	if (devicePtr != nullptr && mapPtr != nullptr)
		return errorStatusToWrapper(devicePtr->capturePointXYZMap(*mapPtr));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper CapturePointXYZBGRMap(mmind::api::MechEyeDevice * devicePtr, mmind::api::PointXYZBGRMap * mapPtr)
{
	if (devicePtr != nullptr && mapPtr != nullptr)
		return errorStatusToWrapper(devicePtr->capturePointXYZBGRMap(*mapPtr));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetScan2DExposureMode(mmind::api::MechEyeDevice * devicePtr, int value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setScan2DExposureMode(static_cast<mmind::api::Scanning2DSettings::Scan2DExposureMode>(value)));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetScan2DExposureMode(mmind::api::MechEyeDevice * devicePtr, int& value)
{
	if (devicePtr != nullptr) {
		mmind::api::Scanning2DSettings::Scan2DExposureMode mode;
		ErrorStatusWrapper wrapper = errorStatusToWrapper(devicePtr->getScan2DExposureMode(mode));
		value = mode;
		return wrapper;
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetScan2DExposureTime(mmind::api::MechEyeDevice * devicePtr, double value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setScan2DExposureTime(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetScan2DExposureTime(mmind::api::MechEyeDevice * devicePtr, double& value)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getScan2DExposureTime(value));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetScan2DHDRExposureSequence(mmind::api::MechEyeDevice* devicePtr, double* values, int size)
{
	if (devicePtr != nullptr) {
		std::vector<double> sequence(values, values+size);
		return errorStatusToWrapper(devicePtr->setScan2DHDRExposureSequence(sequence));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetScan2DHDRExposureSequence(mmind::api::MechEyeDevice * devicePtr, double* values)
{
	if (devicePtr != nullptr) {
		std::vector<double> sequence;
		ErrorStatusWrapper status = errorStatusToWrapper(devicePtr->getScan2DHDRExposureSequence(sequence));
		values = &sequence[0];
		return status;
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetScan2DExpectedGrayValue(mmind::api::MechEyeDevice * devicePtr, int value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setScan2DExpectedGrayValue(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetScan2DExpectedGrayValue(mmind::api::MechEyeDevice * devicePtr, int& value)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getScan2DExpectedGrayValue(value));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetScan2DToneMappingEnable(mmind::api::MechEyeDevice * devicePtr, bool value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setScan2DToneMappingEnable(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetScan2DToneMappingEnable(mmind::api::MechEyeDevice * devicePtr, bool& value)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getScan2DToneMappingEnable(value));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetScan2DSharpenFactor(mmind::api::MechEyeDevice * devicePtr, double value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setScan2DSharpenFactor(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetScan2DSharpenFactor(mmind::api::MechEyeDevice * devicePtr, double& value)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getScan2DSharpenFactor(value));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetScan2DROI(mmind::api::MechEyeDevice * devicePtr, mmind::api::ROI * roi)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setScan2DROI(*roi));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetScan2DROI(mmind::api::MechEyeDevice * devicePtr, mmind::api::ROI* roi)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getScan2DROI(*roi));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetScan3DExposure(mmind::api::MechEyeDevice* devicePtr, double* values, int size)
{
	if (devicePtr != nullptr) {
		std::vector<double> sequence(values, values+size);
		return errorStatusToWrapper(devicePtr->setScan3DExposure(sequence));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) int GetScan3DExposureSize(mmind::api::MechEyeDevice * devicePtr)
{
	if (devicePtr != nullptr) {
		std::vector<double> sequence;
		devicePtr->getScan3DExposure(sequence);
		return sequence.size();
	}
	else
		return 0;
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetScan3DExposure(mmind::api::MechEyeDevice * devicePtr, double* values)
{
	if (devicePtr != nullptr) {
		std::vector<double> sequence;
		ErrorStatusWrapper status = errorStatusToWrapper(devicePtr->getScan3DExposure(sequence));
		for (int i = 0; i < sequence.size(); ++i)
			values[i] = sequence[i];
		return status;
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetScan3DGain(mmind::api::MechEyeDevice * devicePtr, double value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setScan3DGain(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetScan3DGain(mmind::api::MechEyeDevice * devicePtr, double& value)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getScan3DGain(value));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetScan3DROI(mmind::api::MechEyeDevice * devicePtr, mmind::api::ROI * roi)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setScan3DROI(*roi));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetScan3DROI(mmind::api::MechEyeDevice * devicePtr, mmind::api::ROI* roi)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getScan3DROI(*roi));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetDepthRange(mmind::api::MechEyeDevice * devicePtr, mmind::api::DepthRange * depthRange)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setDepthRange(*depthRange));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetDepthRange(mmind::api::MechEyeDevice * devicePtr, mmind::api::DepthRange* depthRange)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getDepthRange(*depthRange));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetFringeContrastThreshold(mmind::api::MechEyeDevice * devicePtr, int value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setFringeContrastThreshold(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetFringeContrastThreshold(mmind::api::MechEyeDevice * devicePtr, int& value)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getFringeContrastThreshold(value));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetFringeMinThreshold(mmind::api::MechEyeDevice * devicePtr, int value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setFringeMinThreshold(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetFringeMinThreshold(mmind::api::MechEyeDevice * devicePtr, int& value)
{
	if (devicePtr != nullptr) {
		return errorStatusToWrapper(devicePtr->getFringeMinThreshold(value));
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetCloudOutlierFilterMode(mmind::api::MechEyeDevice * devicePtr, int value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setCloudOutlierFilterMode(static_cast<mmind::api::PointCloudProcessingSettings::CloudOutlierFilterMode>(value)));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetCloudOutlierFilterMode(mmind::api::MechEyeDevice * devicePtr, int& value)
{
	if (devicePtr != nullptr) {
		mmind::api::PointCloudProcessingSettings::CloudOutlierFilterMode mode;
		ErrorStatusWrapper wrapper = errorStatusToWrapper(devicePtr->getCloudOutlierFilterMode(mode));
		value = static_cast<int>(mode);
		return wrapper;
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetCloudSmoothMode(mmind::api::MechEyeDevice * devicePtr, int value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setCloudSmoothMode(static_cast<mmind::api::PointCloudProcessingSettings::CloudSmoothMode>(value)));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetCloudSmoothMode(mmind::api::MechEyeDevice * devicePtr, int& value)
{
	if (devicePtr != nullptr) {
		mmind::api::PointCloudProcessingSettings::CloudSmoothMode mode;
		ErrorStatusWrapper wrapper = errorStatusToWrapper(devicePtr->getCloudSmoothMode(mode));
		value = static_cast<int>(mode);
		return wrapper;
	}
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetLaserSettings(mmind::api::MechEyeDevice * devicePtr, mmind::api::LaserSettings value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setLaserSettings(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetLaserSettings(mmind::api::MechEyeDevice * devicePtr, mmind::api::LaserSettings& value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->getLaserSettings(value));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SaveAllSettingsToUserSets(mmind::api::MechEyeDevice * devicePtr)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->saveAllSettingsToUserSets());
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper SetCurrentUserSet(mmind::api::MechEyeDevice * devicePtr, const char * value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->setCurrentUserSet(std::string(value)));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper GetCurrentUserSet(mmind::api::MechEyeDevice * devicePtr, const char * value)
{
	if (devicePtr != nullptr) {
		std::string name;
		ErrorStatusWrapper status = errorStatusToWrapper(devicePtr->getCurrentUserSet(name));
		value = name.c_str();
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

extern "C" __declspec(dllexport) ErrorStatusWrapper GetAllUserSets(mmind::api::MechEyeDevice * devicePtr, char * names[])
{
	if (devicePtr != nullptr) {
		std::vector<std::string> sets;
		ErrorStatusWrapper status = errorStatusToWrapper(devicePtr->getAllUserSets(sets));
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

extern "C" __declspec(dllexport) ErrorStatusWrapper DeleteUserSet(mmind::api::MechEyeDevice * devicePtr, const char * value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->deleteUserSet(std::string(value)));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}

extern "C" __declspec(dllexport) ErrorStatusWrapper AddUserSet(mmind::api::MechEyeDevice * devicePtr, const char * value)
{
	if (devicePtr != nullptr)
		return errorStatusToWrapper(devicePtr->addUserSet(std::string(value)));
	else
		return errorStatusToWrapper(mmind::api::ErrorStatus(mmind::api::ErrorStatus::ErrorCode::MMIND_STATUS_INVALID_DEVICE, "Invalid Device!"));
}
