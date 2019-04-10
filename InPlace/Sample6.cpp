#include "Sample6.h"
#include "String.h"

void Sample6(SerializationManager &manager)
{
	String string("ent^Incognita");

	manager.Save("sample6.class", string);

	Resource<String> ss_d;
	manager.Load("sample6.class", ss_d);
	ss_d.Release();
}