#include "Sample0.h"

class A0;
class B0;

class C0
{
public:

	A0 *a0;
	B0 *b0;

	C0()
	{
	}
	
	C0(const SerializationManager &sm)
	{
		sm.RelocatePointer(a0);
		sm.RelocatePointer(b0);
	}

	void CollectPtrs(SerializationManager &sm) const
	{
		sm.CollectPtrs(a0);
		sm.CollectPtrs(b0);
	}
};

class B0
{
public:

	float val;
	C0 c0;

	B0()
	{
	}

	B0(const SerializationManager &sm): c0(sm)
	{
	}

	void CollectPtrs(SerializationManager &sm) const
	{
		sm.CollectPtrs(c0);
	}

};

class A0
{
public:

	float val;
	
	C0 *c0;
	B0 b0;

	A0()
	{
	}

	A0(const SerializationManager &sm): b0(sm)
	{
		sm.RelocatePointer(c0);		
	}

	void CollectPtrs(SerializationManager &sm) const
	{
		sm.CollectPtrs(c0);
		sm.CollectPtrs(b0);
	}
};


void Sample0(SerializationManager &manager)
{	
	A0 a0;

	a0.val = 1.0f;
	a0.b0.val = 1.0f;
	a0.c0 = &a0.b0.c0;
	a0.b0.c0.a0 = &a0;
	a0.b0.c0.b0 = &a0.b0;

	manager.Save("sample0.class", a0);

	Resource<A0> dd;
	manager.Load("sample0.class", dd);	
	dd.Release();
}