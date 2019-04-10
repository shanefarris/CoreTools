#ifdef COMPILE_ARTIFEX
// Generated DAL code for the artifex terrain editor

#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include "sqlite3.h"
#include "IError.h"
#include "StderrLog.h"
#include "Database.h"
#include "Query.h"
#include <map>

#include <vector>
#include <string>

#ifndef __CLEVELDAL_H__
#define __CLEVELDAL_H__

#ifdef _WIN32
#define strncasecmp strnicmp
#define strcasecmp stricmp
#endif // _WIN32

/**
 **  Class 'Masks' and 'cMasks'
 **/

namespace DAL
{
class Masks {
public:
	Masks(Database *);
	Masks(Database *,const std::string& );
	Masks(Database *,Query *,int = 0 /* offset */);
	Masks(Database&,long id);
	~Masks();
	Database& GetDatabase() { return *database; }

	unsigned __int64 insert();
	void update();
	void save();
	void erase();
	std::string xml();
	std::string xml(const std::string& ,const std::string& );
	size_t num_cols();

	long GetId() { return this -> id; }
	void SetId(long x) { this -> id = x; }
	const char *GetMask_name() { return this -> mask_name.c_str(); }
	void SetMask_name(const std::string&  x) { this -> mask_name = x; }
	const char *GetMask() { return this -> mask.c_str(); }
	void SetMask(const std::string&  x) { this -> mask = x; }

	// table columns
private:
	long                     id; // INTEGER
	std::string              mask_name; // VARCHAR(255)
	std::string              mask; // VARCHAR(255)
	//
	void clear();
	void spawn(const std::string& );
	void spawn(Query *,int = 0 /* offset */);
	void select(const std::string& );
	void update(long id);
	//
	Database *database;
	short new_object;
}; // End of class 'Masks'

} // end of namespace

/**
 **  Class 'Info' and 'cInfo'
 **/

namespace DAL
{
class Info {
public:
	Info(Database *);
	Info(Database *,const std::string& );
	Info(Database *,Query *,int = 0 /* offset */);
	Info(Database&,long id);
	~Info();
	Database& GetDatabase() { return *database; }

	unsigned __int64 insert();
	void update();
	void save();
	void erase();
	std::string xml();
	std::string xml(const std::string& ,const std::string& );
	size_t num_cols();

	long GetId() { return this -> id; }
	void SetId(long x) { this -> id = x; }
	const char *GetSetting() { return this -> setting.c_str(); }
	void SetSetting(const std::string&  x) { this -> setting = x; }
	const char *GetValue() { return this -> value.c_str(); }
	void SetValue(const std::string&  x) { this -> value = x; }

	// table columns
private:
	long                     id; // INTEGER
	std::string              setting; // VARCHAR(255)
	std::string              value; // VARCHAR(255)
	//
	void clear();
	void spawn(const std::string& );
	void spawn(Query *,int = 0 /* offset */);
	void select(const std::string& );
	void update(long id);
	//
	Database *database;
	short new_object;
}; // End of class 'Info'

} // end of namespace

/**
 **  Class 'Objects' and 'cObjects'
 **/

namespace DAL
{
class Objects {
public:
	Objects(Database *);
	Objects(Database *,const std::string& );
	Objects(Database *,Query *,int = 0 /* offset */);
	Objects(Database&,long id);
	~Objects();
	Database& GetDatabase() { return *database; }

	unsigned __int64 insert();
	void update();
	void save();
	void erase();
	std::string xml();
	std::string xml(const std::string& ,const std::string& );
	size_t num_cols();

	long GetId() { return this -> id; }
	void SetId(long x) { this -> id = x; }
	const char *GetEntity() { return this -> entity.c_str(); }
	void SetEntity(const std::string&  x) { this -> entity = x; }
	const char *GetMeshfile() { return this -> meshfile.c_str(); }
	void SetMeshfile(const std::string&  x) { this -> meshfile = x; }
	double GetX() { return this -> x; }
	void SetX(double x) { this -> x = x; }
	double GetY() { return this -> y; }
	void SetY(double x) { this -> y = x; }
	double GetZ() { return this -> z; }
	void SetZ(double x) { this -> z = x; }
	double GetRot_x() { return this -> rot_x; }
	void SetRot_x(double x) { this -> rot_x = x; }
	double GetRot_y() { return this -> rot_y; }
	void SetRot_y(double x) { this -> rot_y = x; }
	double GetRot_z() { return this -> rot_z; }
	void SetRot_z(double x) { this -> rot_z = x; }
	long GetMask() { return this -> mask; }
	void SetMask(long x) { this -> mask = x; }
	double GetScale_x() { return this -> scale_x; }
	void SetScale_x(double x) { this -> scale_x = x; }
	double GetScale_y() { return this -> scale_y; }
	void SetScale_y(double x) { this -> scale_y = x; }
	double GetScale_z() { return this -> scale_z; }
	void SetScale_z(double x) { this -> scale_z = x; }
	const char *GetMaterial() { return this -> material.c_str(); }
	void SetMaterial(const std::string&  x) { this -> material = x; }
	bool GetStatic() { return this -> isstatic; }
	void SetStatic(bool x) { this -> isstatic = x; }
	bool GetDrop_to_ground() { return this -> drop_to_ground; }
	void SetDrop_to_ground(bool x) { this -> drop_to_ground = x; }
	const char *GetObject_type() { return this -> object_type.c_str(); }
	void SetObject_type(const std::string&  x) { this -> object_type = x; }

	// table columns
private:
	long                     id; // INTEGER
	std::string              entity; // VARCHAR(255)
	std::string              meshfile; // VARCHAR(255)
	double                   x; // FLOAT
	double                   y; // FLOAT
	double                   z; // FLOAT
	double                   rot_x; // FLOAT
	double                   rot_y; // FLOAT
	double                   rot_z; // FLOAT
	long                     mask; // INTEGER
	double                   scale_x; // FLOAT
	double                   scale_y; // FLOAT
	double                   scale_z; // FLOAT
	std::string              material; // VARCHAR(255)
	bool                     isstatic; // BOOLEAN
	bool                     drop_to_ground; // BOOLEAN
	std::string              object_type; // VARCHAR(255)
	//
	void clear();
	void spawn(const std::string& );
	void spawn(Query *,int = 0 /* offset */);
	void select(const std::string& );
	void update(long id);
	//
	Database *database;
	short new_object;
}; // End of class 'Objects'

} // end of namespace

/**
 **  Class 'Attributes' and 'cAttributes'
 **/

namespace DAL
{
class Attributes {
public:
	Attributes(Database *);
	Attributes(Database *,const std::string& );
	Attributes(Database *,Query *,int = 0 /* offset */);
	Attributes(Database&,long id);
	~Attributes();
	Database& GetDatabase() { return *database; }

	unsigned __int64 insert();
	void update();
	void save();
	void erase();
	std::string xml();
	std::string xml(const std::string& ,const std::string& );
	size_t num_cols();

	long GetId() { return this -> id; }
	void SetId(long x) { this -> id = x; }
	const char *GetObject_name() { return this -> object_name.c_str(); }
	void SetObject_name(const std::string&  x) { this -> object_name = x; }
	const char *GetAttribute() { return this -> attribute.c_str(); }
	void SetAttribute(const std::string&  x) { this -> attribute = x; }
	const char *GetValue() { return this -> value.c_str(); }
	void SetValue(const std::string&  x) { this -> value = x; }

	// table columns
private:
	long                     id; // INTEGER
	std::string              object_name; // VARCHAR(255)
	std::string              attribute; // VARCHAR(255)
	std::string              value; // TEXT
	//
	void clear();
	void spawn(const std::string& );
	void spawn(Query *,int = 0 /* offset */);
	void select(const std::string& );
	void update(long id);
	//
	Database *database;
	short new_object;
}; // End of class 'Attributes'

} // end of namespace
#endif // __CLEVELDAL_H__

#endif