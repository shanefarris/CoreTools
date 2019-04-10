#ifndef __RENDERABLE_H__
#define __RENDERABLE_H__

#include "BaseObject.h"

class Renderable: public BaseObject
{
public:
	
	virtual void Render() const = 0;	
};

#endif