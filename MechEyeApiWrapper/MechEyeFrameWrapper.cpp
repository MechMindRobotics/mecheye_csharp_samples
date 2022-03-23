#include "MechEyeFrame.hpp"

extern "C" __declspec(dllexport) mmind::api::Frame<mmind::api::ElementColor> * CreateColorMap() {
	return new mmind::api::Frame<mmind::api::ElementColor>();
}

extern "C" __declspec(dllexport) mmind::api::Frame<mmind::api::ElementDepth> *CreateDepthMap() {
	return new mmind::api::Frame<mmind::api::ElementDepth>();
}

extern "C" __declspec(dllexport) mmind::api::Frame<mmind::api::ElementPointXYZ> *CreatePointXYZMap() {
	return new mmind::api::Frame<mmind::api::ElementPointXYZ>();
}

extern "C" __declspec(dllexport) mmind::api::Frame<mmind::api::ElementPointXYZBGR> *CreatePointXYZBGRMap() {
	return new mmind::api::Frame<mmind::api::ElementPointXYZBGR>();
}

extern "C" __declspec(dllexport) void DeleteColorMap(mmind::api::Frame<mmind::api::ElementColor> * mapPtr) {
	if (mapPtr != nullptr)
		delete mapPtr;
}

extern "C" __declspec(dllexport) void DeleteDepthMap(mmind::api::Frame<mmind::api::ElementDepth> *mapPtr) {
	if (mapPtr != nullptr)
		delete mapPtr;
}

extern "C" __declspec(dllexport) void DeletePointXYZMap(mmind::api::Frame<mmind::api::ElementPointXYZ> *mapPtr) {
	if (mapPtr != nullptr)
		delete mapPtr;
}

extern "C" __declspec(dllexport) void DeletePointXYZBGRMap(mmind::api::Frame<mmind::api::ElementPointXYZBGR> *mapPtr) {
	if (mapPtr != nullptr)
		delete mapPtr;
}

extern "C" __declspec(dllexport) uint32_t ColorMapWidth(mmind::api::Frame<mmind::api::ElementColor> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->width();
	else
		return 0;
}

extern "C" __declspec(dllexport) uint32_t DepthMapWidth(mmind::api::Frame<mmind::api::ElementDepth> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->width();
	else
		return 0;
}

extern "C" __declspec(dllexport) uint32_t PointXYZMapWidth(mmind::api::Frame<mmind::api::ElementPointXYZ> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->width();
	else
		return 0;
}

extern "C" __declspec(dllexport) uint32_t PointXYZBGRMapWidth(mmind::api::Frame<mmind::api::ElementPointXYZBGR> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->width();
	else
		return 0;
}

extern "C" __declspec(dllexport) uint32_t ColorMapHeight(mmind::api::Frame<mmind::api::ElementColor> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->height();
	else
		return 0;
}

extern "C" __declspec(dllexport) uint32_t DepthMapHeight(mmind::api::Frame<mmind::api::ElementDepth> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->height();
	else
		return 0;
}

extern "C" __declspec(dllexport) uint32_t PointXYZMapHeight(mmind::api::Frame<mmind::api::ElementPointXYZ> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->height();
	else
		return 0;
}

extern "C" __declspec(dllexport) uint32_t PointXYZBGRMapHeight(mmind::api::Frame<mmind::api::ElementPointXYZBGR> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->height();
	else
		return 0;
}

extern "C" __declspec(dllexport) bool ColorMapEmpty(mmind::api::Frame<mmind::api::ElementColor> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->empty();
	else
		return true;
}

extern "C" __declspec(dllexport) bool DepthMapEmpty(mmind::api::Frame<mmind::api::ElementDepth> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->empty();
	else
		return true;
}

extern "C" __declspec(dllexport) bool PointXYZMapEmpty(mmind::api::Frame<mmind::api::ElementPointXYZ> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->empty();
	else
		return true;
}

extern "C" __declspec(dllexport) bool PointXYZBGRMapEmpty(mmind::api::Frame<mmind::api::ElementPointXYZBGR> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->empty();
	else
		return true;
}

extern "C" __declspec(dllexport) mmind::api::ElementColor * ColorMapData(mmind::api::Frame<mmind::api::ElementColor> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->data();
	else
		return nullptr;
}

extern "C" __declspec(dllexport) mmind::api::ElementDepth * DepthMapData(mmind::api::Frame<mmind::api::ElementDepth> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->data();
	else
		return nullptr;
}

extern "C" __declspec(dllexport) mmind::api::ElementPointXYZ * PointXYZMapData(mmind::api::Frame<mmind::api::ElementPointXYZ> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->data();
	else
		return nullptr;
}

extern "C" __declspec(dllexport) mmind::api::ElementPointXYZBGR * PointXYZBGRMapData(mmind::api::Frame<mmind::api::ElementPointXYZBGR> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->data();
	else
		return nullptr;
}

extern "C" __declspec(dllexport) mmind::api::ElementColor & ColorMapAt(mmind::api::Frame<mmind::api::ElementColor> *mapPtr, uint32_t row, uint32_t col) {
	if (mapPtr != nullptr)
		return mapPtr->at(row, col);
	else {
		mmind::api::ElementColor emptyElem;
		return emptyElem;
	}
}

extern "C" __declspec(dllexport) mmind::api::ElementDepth & DepthMapAt(mmind::api::Frame<mmind::api::ElementDepth> *mapPtr, uint32_t row, uint32_t col) {
	if (mapPtr != nullptr)
		return mapPtr->at(row, col);
	else {
		mmind::api::ElementDepth emptyElem;
		return emptyElem;
	}
}

extern "C" __declspec(dllexport) mmind::api::ElementPointXYZ & PointXYZMapAt(mmind::api::Frame<mmind::api::ElementPointXYZ> *mapPtr, uint32_t row, uint32_t col) {
	if (mapPtr != nullptr)
		return mapPtr->at(row, col);
	else {
		mmind::api::ElementPointXYZ emptyELem;
		return emptyELem;
	}
}

extern "C" __declspec(dllexport) mmind::api::ElementPointXYZBGR & PointXYZBGRMapAt(mmind::api::Frame<mmind::api::ElementPointXYZBGR> *mapPtr, uint32_t row, uint32_t col) {
	if (mapPtr != nullptr)
		return mapPtr->at(row, col);
	else {
		mmind::api::ElementPointXYZBGR emptyElem;
		return emptyElem;
	}
}

extern "C" __declspec(dllexport) void ColorMapResize(mmind::api::Frame<mmind::api::ElementColor> *mapPtr, uint32_t width, uint32_t height) {
	if (mapPtr != nullptr)
		return mapPtr->resize(width, height);
}

extern "C" __declspec(dllexport) void DepthMapResize(mmind::api::Frame<mmind::api::ElementDepth> *mapPtr, uint32_t width, uint32_t height) {
	if (mapPtr != nullptr)
		return mapPtr->resize(width, height);
}

extern "C" __declspec(dllexport) void PointXYZMapResize(mmind::api::Frame<mmind::api::ElementPointXYZ> *mapPtr, uint32_t width, uint32_t height) {
	if (mapPtr != nullptr)
		return mapPtr->resize(width, height);
}

extern "C" __declspec(dllexport) void PointXYZBGRMapResize(mmind::api::Frame<mmind::api::ElementPointXYZBGR> *mapPtr, uint32_t width, uint32_t height) {
	if (mapPtr != nullptr)
		return mapPtr->resize(width, height);
}

extern "C" __declspec(dllexport) void ColorMapRelease(mmind::api::Frame<mmind::api::ElementColor> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->release();
}

extern "C" __declspec(dllexport) void DepthMapRelease(mmind::api::Frame<mmind::api::ElementDepth> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->release();
}

extern "C" __declspec(dllexport) void PointXYZMapRelease(mmind::api::Frame<mmind::api::ElementPointXYZ> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->release();
}

extern "C" __declspec(dllexport) void PointXYZBGRMapRelease(mmind::api::Frame<mmind::api::ElementPointXYZBGR> *mapPtr) {
	if (mapPtr != nullptr)
		return mapPtr->release();
}