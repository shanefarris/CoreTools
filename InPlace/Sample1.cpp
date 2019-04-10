#include "Sample1.h"
#include "Vector.h"

class Cosa
{
public:

	float x, y, z;
	Cosa *next;

	Cosa(): next(0)
	{
	}

	Cosa(float xx, float yy, float zz): next(0), x(xx), y(yy), z(zz)
	{
	}

	Cosa(const SerializationManager &sm)
	{
		sm.RelocatePointer(next);		
	}

	void CollectPtrs(SerializationManager &sm) const
	{
		sm.CollectPtrs(next);		
	}
};

void Sample1(SerializationManager &manager)
{	
	Cosa p0, p1, p2;

	p0.x = p0.y = p0.z = 1.0f;
	p0.next = &p1;	

	p1.x = p1.y = p1.z = 2.0f;
	p1.next = &p2;

	p2.x = p2.y = p2.z = 3.0f;
	p2.next = &p0;

	manager.Save("sample10.class", p0);

	Resource<Cosa> dd0;
	manager.Load("sample10.class", dd0);
	dd0.Release();

	////////////////

	Vector<Cosa> vec(5);

	vec[0] = Cosa(0.0f, 0.0f, 0.0f);
	vec[1] = Cosa(1.0f, 1.0f, 1.0f);
	vec[2] = Cosa(2.0f, 2.0f, 2.0f);
	vec[3] = Cosa(3.0f, 3.0f, 3.0f);
	vec[4] = Cosa(4.0f, 4.0f, 4.0f);

	manager.Save("sample11.class", vec);

	Resource<Vector<Cosa> > vv;
	manager.Load("sample11.class", vv);	
	vv.Release();
}