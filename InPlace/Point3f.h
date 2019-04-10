#ifndef __POINT3F_H__
#define __POINT3F_H__

#include "SerializationManager.h"

class Point3f
{
public:

	Point3f(float xx, float yy, float zz): x(xx), y(yy), z(zz) 
	{
	}

	// LoadInPlace ctor

	Point3f(const SerializationManager &)
	{	
	}

	// LoadInPlace pointer recollector

	void CollectPtrs(SerializationManager &) const
	{	
	}

	float x, y, z;
};

#endif