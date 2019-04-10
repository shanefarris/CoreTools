#include "Sample3.h"

class N0;
class N1;
class N2;
class N3;

class N3
{
public:

	float a, b, c;

	N3() {}

	N3(const SerializationManager &)
	{
	}

	void CollectPtrs(SerializationManager &) const
	{
	}
};


class N0
{
public:

	N2 *n2;
	N1 *n1;

	N0() {}

	N0(const SerializationManager &sm)
	{
		sm.RelocatePointer(n2);		
		sm.RelocatePointer(n1);
	}

	void CollectPtrs(SerializationManager &sm) const
	{
		sm.CollectPtrs(n2);
		sm.CollectPtrs(n1);
	}	
};

class N2
{
public:

	N2() {}

	N1 *n1;
	N3 *n3;

	N2(const SerializationManager &sm)
	{
		sm.RelocatePointer(n1);
		sm.RelocatePointer(n3);
	}

	void CollectPtrs(SerializationManager &sm) const
	{			
		sm.CollectPtrs(n1);
		sm.CollectPtrs(n3);

	}
};


class N1
{
public:

	float val;
	N2 n2;

	N1() {}

	N1(const SerializationManager &sm): n2(sm)
	{
	}

	void CollectPtrs(SerializationManager &sm) const
	{
		sm.CollectPtrs(n2);		
	}
};

void Sample3(SerializationManager &manager)
{
	N0 n0;
	N1 n1;
	N3 n3;

	n1.val = 1.0f;
	n0.n1 = &n1;
	n0.n2 = &n1.n2;
	n0.n2->n1 = n0.n1;
	n0.n2->n3 = &n3;

	manager.Save("sample3.class", n0);

	Resource<N0> nn0;
	manager.Load("sample3.class", nn0);
	nn0.Release();
}