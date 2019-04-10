#include "Sample4.h"
#include "Vector.h"
#include "Point3f.h"

void Sample4(SerializationManager &manager)
{
	Vector<Point3f> pts(5, Point3f(0.0f, 0.0f, 0.0f));
	
	pts[0] = Point3f(0.0f, 0.0f, 0.0f);
	pts[1] = Point3f(1.0f, 1.0f, 1.0f);
	pts[2] = Point3f(2.0f, 2.0f, 2.0f);
	pts[3] = Point3f(3.0f, 3.0f, 3.0f);
	pts[4] = Point3f(4.0f, 4.0f, 4.0f);

	manager.Save("sample4.class", pts);

	Resource<Vector<Point3f> > pts_l;
	manager.Load("sample4.class", pts_l);	
	pts_l.Release();
}