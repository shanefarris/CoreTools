#ifndef __STATICVECTOR_H__
#define __STATICVECTOR_H__

#include "SerializationManager.h"

template<class T, size_t N> 
class StaticVector
{
public:

	explicit StaticVector(const T& v)
	{
		for(int i = 0; i < N; i++)
		{
			new ((reinterpret_cast<T *>(m_data)) + i) T(v);
		}		
	}

	StaticVector(const StaticVector &v)
	{
		for(int i = 0; i < N; i++)
		{
			operator[](i) = v[i];
		}		
	}

	const StaticVector &operator=(const StaticVector &v)
	{
		Clear();

		for(int i = 0; i < N; i++)
		{
			operator[](i) = v[i];
		}		
	
		return *this;
	}

	~StaticVector()
	{
		Clear();
	}

	// LoadInPlace ctor

	StaticVector(const SerializationManager &sm)
	{
		for(int i = 0; i < N; i++)
		{
			new ((reinterpret_cast<T *>(m_data)) + i) T(sm);
		}		
	}

	// LoadInPlace pointer recollector

	void CollectPtrs(SerializationManager &sm) const
	{
		for(int i = 0; i < N; i++)
		{
			sm.CollectPtrs((reinterpret_cast<const T *>(m_data))[i]);
		}				
	}

	T &operator[](size_t i)
	{			
		assert(i < N);

		return (reinterpret_cast<T *>(m_data))[i];
	}

	const T &operator[](size_t i) const
	{			
		assert(i < N);

		return (reinterpret_cast<T *>(m_data))[i];
	}

private:

	void Clear()
	{
		for(int i = 0; i < N; i++)
		{
			(reinterpret_cast<T *>(m_data))->~T();
		}				
	}

	char m_data[N * sizeof(T)];
};

#endif
