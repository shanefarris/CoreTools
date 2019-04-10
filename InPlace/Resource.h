#ifndef __RESOURCE_H__
#define __RESOURCE_H__

// Resource. SerializationManager returns loaded data inside this SmartPointer. When you have finished
//	with the resource, you should free it with the Release() function.

template<class T> class Resource
{
public:

	Resource(): m_data(0)
	{
	}

	void Release()
	{
		delete[] m_data;
		m_data = 0;
	}

	T *operator->() const
	{
		assert(m_rootObj != 0);
		return m_rootObj;
	}
	
private:

	friend class SerializationManager;

	union
	{
		char *m_data;
		T *m_rootObj;
	};	
};

#endif