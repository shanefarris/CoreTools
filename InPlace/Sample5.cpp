#include "Sample5.h"
#include "Vector.h"
#include "Renderable.h"

class Sphere: public Renderable
{
public:

	CLASSID(600)

	Sphere(float) {}

	void Render() const {}

	Sphere(const SerializationManager &) {}
	void CollectPtrs(SerializationManager &) const {}
};


class Cube: public Renderable
{
public:

	CLASSID(601)

	Cube(float) {}

	void Render() const {}

	Cube(const SerializationManager &) {}
	void CollectPtrs(SerializationManager &) const {}
};

void Sample5(SerializationManager &manager)
{
	manager.GetFactory().RegisterClass<Sphere>();
	manager.GetFactory().RegisterClass<Cube>();

	Vector<Renderable *> renderables(3);

	renderables[0] = new Sphere(10.0f);
	renderables[1] = 0;		
	renderables[2] = new Cube(10.0f);	

	manager.Save("sample5.class", renderables);

	delete renderables[0];	
	delete renderables[2];

	Resource<Vector<Renderable *> > renderables_ip;
	manager.Load("sample5.class", renderables_ip);
	renderables_ip.Release();	
}