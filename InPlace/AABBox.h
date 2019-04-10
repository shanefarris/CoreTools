#ifndef __AABBOX_H__
#define __AABBOX_H__

#include "SerializationManager.h"
#include "Point3f.h"

class AABBox
{
public:

	AABBox(const Point3f &min, const Point3f &max): m_min(min), m_max(max)
	{
	}

	// LoadInPlace ctor

	AABBox(const SerializationManager &sm): m_min(sm), m_max(sm)
	{	
	}

	// LoadInPlace pointer recollector

	void CollectPtrs(SerializationManager &) const
	{	
	}

	Point3f m_min, m_max;	
};

#endif