#ifndef __STRING_H__
#define __STRING_H__

#include <string.h>
#include "SerializationManager.h"

class String
{
public:

	explicit String(const char *szString)
	{		
		m_len = strlen(szString);
		m_szString = new char[m_len + 1];
		strcpy(m_szString, szString);			
	}

	String(const String &string)
	{
		m_len = string.m_len;
		m_szString = new char[m_len + 1];
		strcpy(m_szString, string.m_szString);
	}

	const String &operator=(const String &string)
	{
		Clear();

		m_len = string.m_len;
		m_szString = new char[m_len + 1];
		strcpy(m_szString, string.m_szString);

		return *this;
	}

	~String()
	{
		Clear();
	}

	// LoadInPlace ctor

	String(const SerializationManager &sm)
	{
		sm.RelocatePointer(m_szString, m_len + 1);
	}

	// LoadInPlace pointer recollector

	void CollectPtrs(SerializationManager &sm) const
	{
		sm.CollectPtrs(m_szString, m_len + 1);		
	}

	// Accessors

	const char *c_str() const
	{
		return m_szString;
	}

private:

	void Clear()
	{
		if(m_szString)
		{
			delete[] m_szString;
			m_szString = 0;
			m_len = 0;
		}
	}

	size_t m_len;
	char *m_szString;
};

#endif