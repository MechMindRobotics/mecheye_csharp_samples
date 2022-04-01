#include "MechEyeDataType.h"

extern "C" __declspec(dllexport) mmind::api::ROI * CreateROIWithoutParameter() {
	return new mmind::api::ROI();
}

extern "C" __declspec(dllexport) mmind::api::ROI * CreateROIWithParameter(int x, int y, int width, int height) {
	return new mmind::api::ROI(x, y, width, height);
}

extern "C" __declspec(dllexport) void DeleteROI(mmind::api::ROI * roiPtr) {
	if (roiPtr != nullptr)
		delete roiPtr;
}

extern "C" __declspec(dllexport) unsigned GetROIX(mmind::api::ROI * roiPtr)
{
	if (roiPtr != nullptr)
		return roiPtr->x;
	else
		return 0;
}

extern "C" __declspec(dllexport) unsigned GetROIY(mmind::api::ROI * roiPtr)
{
	if (roiPtr != nullptr)
		return roiPtr->y;
	else
		return 0;
}

extern "C" __declspec(dllexport) unsigned GetROIWidth(mmind::api::ROI * roiPtr)
{
	if (roiPtr != nullptr)
		return roiPtr->width;
	else
		return 0;
}

extern "C" __declspec(dllexport) unsigned GetROIHeight(mmind::api::ROI * roiPtr)
{
	if (roiPtr != nullptr)
		return roiPtr->height;
	else
		return 0;
}

extern "C" __declspec(dllexport) void SetROIX(mmind::api::ROI * roiPtr, unsigned value)
{
	if (roiPtr != nullptr)
		roiPtr->x = value;
}

extern "C" __declspec(dllexport) void SetROIY(mmind::api::ROI * roiPtr, unsigned value)
{
	if (roiPtr != nullptr)
		roiPtr->y = value;
}

extern "C" __declspec(dllexport) void SetROIWidth(mmind::api::ROI * roiPtr, unsigned value)
{
	if (roiPtr != nullptr)
		roiPtr->width = value;
}

extern "C" __declspec(dllexport) void SetROIHeight(mmind::api::ROI * roiPtr, unsigned value)
{
	if (roiPtr != nullptr)
		roiPtr->height = value;
}

extern "C" __declspec(dllexport) mmind::api::DepthRange * CreateDepthRangeWithoutParameter() {
	return new mmind::api::DepthRange();
}

extern "C" __declspec(dllexport) mmind::api::DepthRange * CreateDepthRangeWithParameter(int lower, int upper) {
	return new mmind::api::DepthRange(lower, upper);
}

extern "C" __declspec(dllexport) void DeleteDepthRange(mmind::api::DepthRange * depthRangePtr) {
	if (depthRangePtr != nullptr)
		delete depthRangePtr;
}

extern "C" __declspec(dllexport) int GetLower(mmind::api::DepthRange * depthRangePtr)
{
	if (depthRangePtr != nullptr)
		return depthRangePtr->lower;
	else
		return -1;
}

extern "C" __declspec(dllexport) void SetLower(mmind::api::DepthRange * depthRangePtr, int value)
{
	if (depthRangePtr != nullptr)
		depthRangePtr->lower = value;
}

extern "C" __declspec(dllexport) int GetUpper(mmind::api::DepthRange * depthRangePtr)
{
	if (depthRangePtr != nullptr)
		return depthRangePtr->upper;
	else
		return -1;
}

extern "C" __declspec(dllexport) void SetUpper(mmind::api::DepthRange * depthRangePtr, int value)
{
	if (depthRangePtr != nullptr)
		depthRangePtr->upper = value;
}
