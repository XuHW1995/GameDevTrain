#pragma once
#include<math.h>
#include<string.h>
#include<iostream>
#define _DllExport _declspec(dllexport) //ʹ�ú궨����д��


extern "C"
{
	float _DllExport GetDistance(float x1, float y1, float x2, float y2);
	int _DllExport AddInt(int a, int b);
}