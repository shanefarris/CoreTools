#ifdef COMPILE_ARTIFEX
// Artifex does not have everything needed for a complete level in Core so it must be parsed into Core's xml
// format and possibly loaded into another editor to complete the pieces that it's missing.
// Example: physics, sound, custom nodes.

#ifndef __CARTIFEXSCENECONVERTER_H__
#define __CARTIFEXSCENECONVERTER_H__

#include "IConverter.h"
#include "CArtifexDal.h"

class CArtifexSceneConverter : public IConverter
{
public:
	CArtifexSceneConverter();
	~CArtifexSceneConverter();

	bool Convert(std::string FileName);
	bool Convert(std::string FileName, std::string Terrain, std::string Zone);
	bool ReadTerrainConfig(std::string FileName);
	bool ReadZoneConfig(std::string FileName);

protected:
	bool SaveData();

private:
	Database* db;

	bool ParseEntity(DAL::Objects* tbl);
};

#endif // __CARTIFEXSCENECONVERTER_H__

#endif