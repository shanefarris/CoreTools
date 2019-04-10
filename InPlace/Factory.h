#ifndef __FACTORY_H__
#define __FACTORY_H__

#include <map>
#include <assert.h>
#include "BaseObject.h"

class SerializationManager;

// Factory. An object factory of BaseObject's.
// For this simple example, all the classes must be manually registered:
//
//		factory.RegisterClass<ClassA>();		
//		factory.RegisterClass<ClassB>();		
//		factory.RegisterClass<ClassC>();		

class Factory
{
public:
	
	template<class T> void RegisterClass()
	{
		// Assert: T has not been previously registered
		assert(m_registeredClasses.find(T::ID) == m_registeredClasses.end());

		m_registeredClasses[T::ID] = (ObjectCreatorPtr)&ObjectCreator<T>;
	}

	BaseObject *CreateObject(CID id, void *inPlacePtr, const SerializationManager &sm) const
	{
		// Assert: id is registered
		assert(m_registeredClasses.find(id) != m_registeredClasses.end());

		return (*m_registeredClasses.find(id)->second)(inPlacePtr, sm);    		
	}

private:

	template<class T> static BaseObject *ObjectCreator(void *inPlacePtr, const SerializationManager &sm)
	{
		return new(inPlacePtr) T(sm);    
	}

	typedef BaseObject *(*ObjectCreatorPtr)(void *inPlacePtr, const SerializationManager &sm);

	std::map<CID, ObjectCreatorPtr> m_registeredClasses;
};


#endif