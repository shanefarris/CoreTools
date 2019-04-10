#include "Sample7.h"
#include "StaticVector.h"
#include "Point3f.h"

void Sample7(SerializationManager &manager)
{
	StaticVector<Point3f, 5> sv(Point3f(1.0f, 2.0f, 3.0f));

	sv[0] = Point3f(0.0f, 0.0f, 0.0f);

	manager.Save("sample7.class", sv);

	Resource<StaticVector<Point3f, 5> > sv_d;
	manager.Load("sample7.class", sv_d);
	sv_d.Release();
}