#ifndef __BASEOBJECT_H__
#define __BASEOBJECT_H__

#include <stddef.h>

typedef unsigned int CID;

class SerializationManager;

// BaseObject. Root class for all the classes being registered in the Factory

class BaseObject
{
public:

	virtual CID GetClassID() const = 0;
	virtual size_t GetSize() const = 0;	

	virtual void CollectPtrs(SerializationManager &sm) const = 0;

	virtual ~BaseObject() = 0 {};
};

// Macro for automatically implement the virtual functions, given an ID

#define CLASSID(id)										\
	static const CID ID = id;							\
	CID GetClassID() const { return id; }				\
	size_t GetSize() const { return sizeof(*this); }

#endif