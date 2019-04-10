#if COMPILE_DOTSCENE
// Converts from the DotScene format to the Ogre format.

#ifndef __CDOTSCENECONVERTER_H__
#define __CDOTSCENECONVERTER_H__

#include "IConverter.h"

class CDotSceneConverter : public IConverter
{
public:
	CDotSceneConverter();
	~CDotSceneConverter();

	bool Convert(std::string FileName);

private:
	void ParseEnvironment(IO::IXMLReader* xml);
	void ParseNode(IO::IXMLReader* xml);
	void ParseSubEntities(IO::IXMLReader* xml, ENTITY &Entity);
	bool SaveData();

};

#endif // __CDOTSCENECONVERTER_H__

#endif