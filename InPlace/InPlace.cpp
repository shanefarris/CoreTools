#include <windows.h>

#include "Factory.h"
#include "SerializationManager.h"

#include "Sample0.h"
#include "Sample1.h"
#include "Sample2.h"
#include "Sample3.h"
#include "Sample4.h"
#include "Sample5.h"
#include "Sample6.h"
#include "Sample7.h"
#include "Sample8.h"

int main()
{ 	
	Factory factory;	
	SerializationManager manager(factory);	

	printf("\n-------------------------------------------------------------");
	printf("\n                  Load-In-Place Tests                        ");
	printf("\n-------------------------------------------------------------\n\n");

	Sample0(manager);
	printf("\n\n");

	Sample1(manager);
	printf("\n\n");

	Sample2(manager);
	printf("\n\n");

	Sample3(manager);
	printf("\n\n");

	Sample4(manager);
	printf("\n\n");

	Sample5(manager);
	printf("\n\n");

	Sample6(manager);
	printf("\n\n");

	Sample7(manager);
	printf("\n\n");

	Sample8(manager);
	printf("\n\n");

	printf("\n-------------------------------------------------------------\n\n");

	return 0;
}

