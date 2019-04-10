#ifndef __COMPILETOOLS_H__
#define __COMPILETOOLS_H__

// CompileTime utilities taken from Alexandrescu's Modern C++ Design

template<class T, class U>
class Conversion
{
	typedef char Small;
	class Big { char dummy[2]; };
	static Small Test(U);
	static Big Test(...);
	static T MakeT();

public:

	enum { exists = sizeof(Test(MakeT())) == sizeof(Small) };
};

// SUPERSUBCLASS(T, U) evaluates to true if U inherits from T publicly

#define SUPERSUBCLASS(T, U)	Conversion<const U *, const T *>::exists

#endif