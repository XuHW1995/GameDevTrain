#include <DllForUnity.h>

float GetDistance(float x1, float y1, float x2, float y2)
{
    return sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
}

int AddInt(int a, int b)
{
    return a + b;
}