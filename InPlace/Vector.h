#ifndef __VECTOR_H__
#define __VECTOR_H__

#include <assert.h>
#include <stddef.h>
#include <memory.h>

#include "SerializationManager.h"

template<class T>
class Vector
{
public:

	explicit Vector(size_t size)
	{
		m_size = size;
		m_data = reinterpret_cast<T *>(new char[size * sizeof(T)]);

		for(size_t i = 0; i < size; i++)
		{
			new(m_data + i) T;
		}	
	}
	
	Vector(size_t size, const T &v)
	{
		m_size = size;
		m_data = reinterpret_cast<T *>(new char[size * sizeof(T)]);

		for(size_t i = 0; i < size; i++)
		{
			new(m_data + i) T(v);
		}		
	}

	Vector(const Vector &v)
	{
		m_size = v.m_size;
		m_data = reinterpret_cast<T *>(new char[m_size * sizeof(T)]);

		for(size_t i = 0; i < m_size; i++)
		{
			new(m_data + i) T(v[i]);
		}				
	}

	const Vector &operator=(const Vector &v)
	{
		Clear();

		m_size = v.m_size;
		m_data = reinterpret_cast<T *>(new char[m_size * sizeof(T)]);

		for(size_t i = 0; i < m_size; i++)
		{
			new(m_data + i) T(v[i]);
		}		

		return *this;
	}

	~Vector()
	{
		Clear();
	}

	void Resize(size_t size, const T &v)
	{
		Clear();

		m_size = size;

		m_data = reinterpret_cast<T *>(new char[size * sizeof(T)]);

		for(size_t i = 0; i < m_size; i++)
		{
			new(m_data + i) T(v);
		}	
	}

	// LoadInPlace ctor

	Vector(const SerializationManager &sm)
	{
		sm.RelocatePointer(m_data, m_size);
	}

	// LoadInPlace pointer recollector

	void CollectPtrs(SerializationManager &sm) const
	{
		sm.CollectPtrs(m_data, m_size);		
	}

	// Accessors

	T &operator[](size_t i)
	{
		assert(m_data);		
		assert(i < m_size);

		return m_data[i];
	}

	const T &operator[](size_t i) const
	{
		assert(m_data);		
		assert(i < m_size);

		return m_data[i];
	}

private:

	void Clear()
	{
		if(m_data)
		{			
			for(size_t i = 0; i < m_size; i++)
			{
				m_data[i].~T();
			}

			delete[] (char *)m_data;

			m_data = 0;
			m_size = 0;
		}
	}
	
	size_t m_size;
	T *m_data;

};

#endif