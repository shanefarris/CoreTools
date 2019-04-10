// Converts from the Ogitor format that it exports as xml and converts it to the Core format.

#ifndef __COGSCENECONVERTER_H__
#define __COGSCENECONVERTER_H__

#include "IConverter.h"

class COgSceneConverter : public IConverter
{
public:
	COgSceneConverter();
	~COgSceneConverter();

	bool Convert(std::string FileName);

private:
	void ParseSceneManager(IO::IXMLReader* xml);
	void ParseViewport(IO::IXMLReader* xml);
	void ParseLight(IO::IXMLReader* xml);
	void ParseEntity(IO::IXMLReader* xml);
	void ParseTerrain(IO::IXMLReader* xml);
	void ParseCamera(IO::IXMLReader* xml);
	void ParseCaelum(IO::IXMLReader* xml);
	void ParseHydrax(IO::IXMLReader* xml);
	void ParseMarker(IO::IXMLReader* xml);
	void ParseCustomProperty(IO::IXMLReader* xml, reVector3Df& pos);
	void ParseTerrainName(IO::IXMLReader* xml);

	bool Validate();
	bool SaveData();

	// Used for the custom properties avaiable in Ogitor
	struct CustomProperty
	{
		String Type;
		String Value;
	};

};

#endif // __COGSCENECONVERTER_H__