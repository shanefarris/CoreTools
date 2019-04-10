#include "Sample2.h"
#include "Vector.h"

class Grob: public BaseObject
{
public:

	CLASSID(500)

	Grob()
	{
	}

	Grob(const SerializationManager &)
	{			
	}

	void CollectPtrs(SerializationManager &) const
	{		
	}

	virtual ~Grob() {}
};

void Sample2(SerializationManager &manager)
{
	manager.GetFactory().RegisterClass<Grob>();

	Grob *grob = new Grob;
	manager.Save("sample20.class", grob);
	delete grob;

	Resource<Grob> grob0;
	manager.Load("sample20.class", grob0);
	grob0.Release();

	////////////////////

	Vector<Grob> _grobs(3);

	_grobs[0] = Grob();
	_grobs[1] = Grob();
	_grobs[2] = Grob();

	manager.Save("sample21.class", _grobs);

	Resource<Vector<Grob> > _grobbys;
	manager.Load("sample21.class", _grobbys);
	_grobbys.Release();

}