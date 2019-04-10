#ifdef COMPILE_ARTIFEX
// Generated DAL code.
#include "CArtifexDal.h"

namespace DAL {

/**
 **  Begin class 'Masks'
 **/

Masks::Masks(Database *db)
{
	database = db;
	new_object = 1;
	clear();
}


Masks::Masks(Database *db,const std::string& sql)
{
	database = db;
	new_object = 1;
	spawn(sql);
}


Masks::Masks(Database *db,Query *qd,int offset)
{
	database = db;
	new_object = 0;
	spawn(qd, offset);
}


Masks::Masks(Database& db,long i_id):database(&db),new_object(1)
{
	Query q(*database);
	std::string sql = "select * from masks where ";
	{
		char slask[100];
		sprintf(slask,"id='%ld'",i_id);
		sql += slask;
	}
	spawn(sql);
}


Masks::~Masks()
{
}


void Masks::select(const std::string& sql)
{
	spawn(sql);
}


unsigned __int64 Masks::insert()
{
	Query q(*database);
	std::string sql;

	sql = "insert into masks(mask_name,mask)";
	sql += " values('" + q.GetDatabase().safestr(this -> mask_name) + "'";
	sql += ", '" + q.GetDatabase().safestr(this -> mask) + "'";
	sql += ")";
	q.execute(sql);
	new_object = 0;
	unsigned __int64 inserted_id = q.insert_id();
	id = inserted_id;
	return inserted_id;
}


void Masks::update()
{
	update(this -> id);
}


void Masks::update(long i_id)
{
	Query q(*database);
	std::string sql;
	sql += "update masks set mask_name='" + q.GetDatabase().safestr(this -> mask_name) + "'";
	sql += ", mask='" + q.GetDatabase().safestr(this -> mask) + "'";
	{
		char slask[200];
		sprintf(slask," where id='%ld'",i_id);
		sql += slask;
	}
	q.execute(sql);
}


void Masks::save()
{
	if (new_object)
		insert();
	else
		update();
}


void Masks::erase()
{
	if (!new_object)
	{
		std::string sql = "delete from masks where";
		Query q(*database);
		{
			char slask[200];
			sprintf(slask," id='%ld'",this -> id);
			sql += slask;
		}
		q.execute(sql);
	}
}


std::string Masks::xml()
{
	Query q(*database);
	std::string dest;
	char slask[200];
	dest = "<MASKS>";
	sprintf(slask,"<ID>%ld</ID>",this -> id);
	dest += slask;
	dest += "<MASK_NAME>" + q.GetDatabase().xmlsafestr(this -> mask_name) + "</MASK_NAME>";
	dest += "<MASK>" + q.GetDatabase().xmlsafestr(this -> mask) + "</MASK>";
	dest += "</MASKS>";
	return dest;
}


std::string Masks::xml(const std::string& tag,const std::string& xvalx)
{
	Query q(*database);
	std::string dest;
	char slask[200];
	dest = "<MASKS " + tag + "=\"" + xvalx + "\">";
	sprintf(slask,"<ID>%ld</ID>",this -> id);
	dest += slask;
	dest += "<MASK_NAME>" + q.GetDatabase().xmlsafestr(this -> mask_name) + "</MASK_NAME>";
	dest += "<MASK>" + q.GetDatabase().xmlsafestr(this -> mask) + "</MASK>";
	dest += "</MASKS>";
	return dest;
}


size_t Masks::num_cols()
{
	return 3;
}


void Masks::clear()
{
	this -> id = 0;
	this -> mask_name = "";
	this -> mask = "";
}


void Masks::spawn(const std::string& sql)
{
	Query q(*database);
	std::string temp;

	clear();

	if (!strncasecmp(sql.c_str(),"select * ",9))
	{
		temp = "select id,mask_name,mask " + sql.substr(9);
	} else
		temp = sql;
	q.get_result(temp);
	if (q.fetch_row())
	{
		this -> id = q.getval(0);																				// 0 - id INTEGER
		this -> mask_name = q.getstr(1);																				// 1 - mask_name VARCHAR(255)
		this -> mask = q.getstr(2);																				// 2 - mask VARCHAR(255)
		new_object = 0;
	} else
		clear();
	q.free_result();
}


void Masks::spawn(Query *qd,int offset)
{
	clear();

	this -> id = qd -> getval(0 + offset);																				// 0 - id INTEGER
	this -> mask_name = qd -> getstr(1 + offset);																				// 1 - mask_name VARCHAR(255)
	this -> mask = qd -> getstr(2 + offset);																				// 2 - mask VARCHAR(255)
}


// End of implementation of class 'Masks'

} // End of namespace
namespace DAL {

/**
 **  Begin class 'Info'
 **/

Info::Info(Database *db)
{
	database = db;
	new_object = 1;
	clear();
}


Info::Info(Database *db,const std::string& sql)
{
	database = db;
	new_object = 1;
	spawn(sql);
}


Info::Info(Database *db,Query *qd,int offset)
{
	database = db;
	new_object = 0;
	spawn(qd, offset);
}


Info::Info(Database& db,long i_id):database(&db),new_object(1)
{
	Query q(*database);
	std::string sql = "select * from info where ";
	{
		char slask[100];
		sprintf(slask,"id='%ld'",i_id);
		sql += slask;
	}
	spawn(sql);
}


Info::~Info()
{
}


void Info::select(const std::string& sql)
{
	spawn(sql);
}


unsigned __int64 Info::insert()
{
	Query q(*database);
	std::string sql;

	sql = "insert into info(setting,value)";
	sql += " values('" + q.GetDatabase().safestr(this -> setting) + "'";
	sql += ", '" + q.GetDatabase().safestr(this -> value) + "'";
	sql += ")";
	q.execute(sql);
	new_object = 0;
	unsigned __int64 inserted_id = q.insert_id();
	id = inserted_id;
	return inserted_id;
}


void Info::update()
{
	update(this -> id);
}


void Info::update(long i_id)
{
	Query q(*database);
	std::string sql;
	sql += "update info set setting='" + q.GetDatabase().safestr(this -> setting) + "'";
	sql += ", value='" + q.GetDatabase().safestr(this -> value) + "'";
	{
		char slask[200];
		sprintf(slask," where id='%ld'",i_id);
		sql += slask;
	}
	q.execute(sql);
}


void Info::save()
{
	if (new_object)
		insert();
	else
		update();
}


void Info::erase()
{
	if (!new_object)
	{
		std::string sql = "delete from info where";
		Query q(*database);
		{
			char slask[200];
			sprintf(slask," id='%ld'",this -> id);
			sql += slask;
		}
		q.execute(sql);
	}
}


std::string Info::xml()
{
	Query q(*database);
	std::string dest;
	char slask[200];
	dest = "<INFO>";
	sprintf(slask,"<ID>%ld</ID>",this -> id);
	dest += slask;
	dest += "<SETTING>" + q.GetDatabase().xmlsafestr(this -> setting) + "</SETTING>";
	dest += "<VALUE>" + q.GetDatabase().xmlsafestr(this -> value) + "</VALUE>";
	dest += "</INFO>";
	return dest;
}


std::string Info::xml(const std::string& tag,const std::string& xvalx)
{
	Query q(*database);
	std::string dest;
	char slask[200];
	dest = "<INFO " + tag + "=\"" + xvalx + "\">";
	sprintf(slask,"<ID>%ld</ID>",this -> id);
	dest += slask;
	dest += "<SETTING>" + q.GetDatabase().xmlsafestr(this -> setting) + "</SETTING>";
	dest += "<VALUE>" + q.GetDatabase().xmlsafestr(this -> value) + "</VALUE>";
	dest += "</INFO>";
	return dest;
}


size_t Info::num_cols()
{
	return 3;
}


void Info::clear()
{
	this -> id = 0;
	this -> setting = "";
	this -> value = "";
}


void Info::spawn(const std::string& sql)
{
	Query q(*database);
	std::string temp;

	clear();

	if (!strncasecmp(sql.c_str(),"select * ",9))
	{
		temp = "select id,setting,value " + sql.substr(9);
	} else
		temp = sql;
	q.get_result(temp);
	if (q.fetch_row())
	{
		this -> id = q.getval(0);																				// 0 - id INTEGER
		this -> setting = q.getstr(1);																				// 1 - setting VARCHAR(255)
		this -> value = q.getstr(2);																				// 2 - value VARCHAR(255)
		new_object = 0;
	} else
		clear();
	q.free_result();
}


void Info::spawn(Query *qd,int offset)
{
	clear();

	this -> id = qd -> getval(0 + offset);																				// 0 - id INTEGER
	this -> setting = qd -> getstr(1 + offset);																				// 1 - setting VARCHAR(255)
	this -> value = qd -> getstr(2 + offset);																				// 2 - value VARCHAR(255)
}


// End of implementation of class 'Info'

} // End of namespace
namespace DAL {

/**
 **  Begin class 'Objects'
 **/

Objects::Objects(Database *db)
{
	database = db;
	new_object = 1;
	clear();
}


Objects::Objects(Database *db,const std::string& sql)
{
	database = db;
	new_object = 1;
	spawn(sql);
}


Objects::Objects(Database *db,Query *qd,int offset)
{
	database = db;
	new_object = 0;
	spawn(qd, offset);
}


Objects::Objects(Database& db,long i_id):database(&db),new_object(1)
{
	Query q(*database);
	std::string sql = "select * from objects where ";
	{
		char slask[100];
		sprintf(slask,"id='%ld'",i_id);
		sql += slask;
	}
	spawn(sql);
}


Objects::~Objects()
{
}


void Objects::select(const std::string& sql)
{
	spawn(sql);
}


unsigned __int64 Objects::insert()
{
	Query q(*database);
	std::string sql;

	sql = "insert into objects(entity,meshfile,x,y,z,rot_x,rot_y,rot_z,mask,scale_x,scale_y,scale_z,material,static,drop_to_ground,object_type)";
	sql += " values('" + q.GetDatabase().safestr(this -> entity) + "'";
	sql += ", '" + q.GetDatabase().safestr(this -> meshfile) + "'";
	{
		char slask[100];
		sprintf(slask,", %f",this -> x);
		sql += slask;
	}
	{
		char slask[100];
		sprintf(slask,", %f",this -> y);
		sql += slask;
	}
	{
		char slask[100];
		sprintf(slask,", %f",this -> z);
		sql += slask;
	}
	{
		char slask[100];
		sprintf(slask,", %f",this -> rot_x);
		sql += slask;
	}
	{
		char slask[100];
		sprintf(slask,", %f",this -> rot_y);
		sql += slask;
	}
	{
		char slask[100];
		sprintf(slask,", %f",this -> rot_z);
		sql += slask;
	}
	{
		char slask[100];
		sprintf(slask,", %ld",this -> mask);
		sql += slask;
	}
	{
		char slask[100];
		sprintf(slask,", %f",this -> scale_x);
		sql += slask;
	}
	{
		char slask[100];
		sprintf(slask,", %f",this -> scale_y);
		sql += slask;
	}
	{
		char slask[100];
		sprintf(slask,", %f",this -> scale_z);
		sql += slask;
	}
	sql += ", '" + q.GetDatabase().safestr(this -> material) + "'";
	sql += ", '" + (this -> isstatic ? q.GetDatabase().safestr(this -> isstatic ? "true" : "false") : "") + "'";
	sql += ", '" + (this -> drop_to_ground ? q.GetDatabase().safestr(this -> drop_to_ground ? "true" : "false") : "") + "'";
	sql += ", '" + q.GetDatabase().safestr(this -> object_type) + "'";
	sql += ")";
	q.execute(sql);
	new_object = 0;
	unsigned __int64 inserted_id = q.insert_id();
	id = inserted_id;
	return inserted_id;
}


void Objects::update()
{
	update(this -> id);
}


void Objects::update(long i_id)
{
	Query q(*database);
	std::string sql;
	sql += "update objects set entity='" + q.GetDatabase().safestr(this -> entity) + "'";
	sql += ", meshfile='" + q.GetDatabase().safestr(this -> meshfile) + "'";
	{
		char slask[200];
		sprintf(slask,", x='%f'",this -> x);
		sql += slask;
	}
	{
		char slask[200];
		sprintf(slask,", y='%f'",this -> y);
		sql += slask;
	}
	{
		char slask[200];
		sprintf(slask,", z='%f'",this -> z);
		sql += slask;
	}
	{
		char slask[200];
		sprintf(slask,", rot_x='%f'",this -> rot_x);
		sql += slask;
	}
	{
		char slask[200];
		sprintf(slask,", rot_y='%f'",this -> rot_y);
		sql += slask;
	}
	{
		char slask[200];
		sprintf(slask,", rot_z='%f'",this -> rot_z);
		sql += slask;
	}
	{
		char slask[200];
		sprintf(slask,", mask=%ld",this -> mask);
		sql += slask;
	}
	{
		char slask[200];
		sprintf(slask,", scale_x='%f'",this -> scale_x);
		sql += slask;
	}
	{
		char slask[200];
		sprintf(slask,", scale_y='%f'",this -> scale_y);
		sql += slask;
	}
	{
		char slask[200];
		sprintf(slask,", scale_z='%f'",this -> scale_z);
		sql += slask;
	}
	sql += ", material='" + q.GetDatabase().safestr(this -> material) + "'";
	sql += ", static='" + q.GetDatabase().safestr(this -> isstatic  ? "true" : "false") + "'";
	sql += ", drop_to_ground='" + q.GetDatabase().safestr(this -> drop_to_ground  ? "true" : "false") + "'";
	sql += ", object_type='" + q.GetDatabase().safestr(this -> object_type) + "'";
	{
		char slask[200];
		sprintf(slask," where id='%ld'",i_id);
		sql += slask;
	}
	q.execute(sql);
}


void Objects::save()
{
	if (new_object)
		insert();
	else
		update();
}


void Objects::erase()
{
	if (!new_object)
	{
		std::string sql = "delete from objects where";
		Query q(*database);
		{
			char slask[200];
			sprintf(slask," id='%ld'",this -> id);
			sql += slask;
		}
		q.execute(sql);
	}
}


std::string Objects::xml()
{
	Query q(*database);
	std::string dest;
	char slask[200];
	dest = "<OBJECTS>";
	sprintf(slask,"<ID>%ld</ID>",this -> id);
	dest += slask;
	dest += "<ENTITY>" + q.GetDatabase().xmlsafestr(this -> entity) + "</ENTITY>";
	dest += "<MESHFILE>" + q.GetDatabase().xmlsafestr(this -> meshfile) + "</MESHFILE>";
	sprintf(slask,"<X>%f</X>",this -> x);
	dest += slask;
	sprintf(slask,"<Y>%f</Y>",this -> y);
	dest += slask;
	sprintf(slask,"<Z>%f</Z>",this -> z);
	dest += slask;
	sprintf(slask,"<ROT_X>%f</ROT_X>",this -> rot_x);
	dest += slask;
	sprintf(slask,"<ROT_Y>%f</ROT_Y>",this -> rot_y);
	dest += slask;
	sprintf(slask,"<ROT_Z>%f</ROT_Z>",this -> rot_z);
	dest += slask;
	sprintf(slask,"<MASK>%ld</MASK>",this -> mask);
	dest += slask;
	sprintf(slask,"<SCALE_X>%f</SCALE_X>",this -> scale_x);
	dest += slask;
	sprintf(slask,"<SCALE_Y>%f</SCALE_Y>",this -> scale_y);
	dest += slask;
	sprintf(slask,"<SCALE_Z>%f</SCALE_Z>",this -> scale_z);
	dest += slask;
	dest += "<MATERIAL>" + q.GetDatabase().xmlsafestr(this -> material) + "</MATERIAL>";
	dest += "<STATIC>" + q.GetDatabase().xmlsafestr(this -> isstatic ? "true" : "false") + "</STATIC>";
	dest += "<DROP_TO_GROUND>" + q.GetDatabase().xmlsafestr(this -> drop_to_ground ? "true" : "false") + "</DROP_TO_GROUND>";
	dest += "<OBJECT_TYPE>" + q.GetDatabase().xmlsafestr(this -> object_type) + "</OBJECT_TYPE>";
	dest += "</OBJECTS>";
	return dest;
}


std::string Objects::xml(const std::string& tag,const std::string& xvalx)
{
	Query q(*database);
	std::string dest;
	char slask[200];
	dest = "<OBJECTS " + tag + "=\"" + xvalx + "\">";
	sprintf(slask,"<ID>%ld</ID>",this -> id);
	dest += slask;
	dest += "<ENTITY>" + q.GetDatabase().xmlsafestr(this -> entity) + "</ENTITY>";
	dest += "<MESHFILE>" + q.GetDatabase().xmlsafestr(this -> meshfile) + "</MESHFILE>";
	sprintf(slask,"<X>%f</X>",this -> x);
	dest += slask;
	sprintf(slask,"<Y>%f</Y>",this -> y);
	dest += slask;
	sprintf(slask,"<Z>%f</Z>",this -> z);
	dest += slask;
	sprintf(slask,"<ROT_X>%f</ROT_X>",this -> rot_x);
	dest += slask;
	sprintf(slask,"<ROT_Y>%f</ROT_Y>",this -> rot_y);
	dest += slask;
	sprintf(slask,"<ROT_Z>%f</ROT_Z>",this -> rot_z);
	dest += slask;
	sprintf(slask,"<MASK>%ld</MASK>",this -> mask);
	dest += slask;
	sprintf(slask,"<SCALE_X>%f</SCALE_X>",this -> scale_x);
	dest += slask;
	sprintf(slask,"<SCALE_Y>%f</SCALE_Y>",this -> scale_y);
	dest += slask;
	sprintf(slask,"<SCALE_Z>%f</SCALE_Z>",this -> scale_z);
	dest += slask;
	dest += "<MATERIAL>" + q.GetDatabase().xmlsafestr(this -> material) + "</MATERIAL>";
	dest += "<STATIC>" + q.GetDatabase().xmlsafestr(this -> isstatic ? "true" : "false") + "</STATIC>";
	dest += "<DROP_TO_GROUND>" + q.GetDatabase().xmlsafestr(this -> drop_to_ground ? "true" : "false") + "</DROP_TO_GROUND>";
	dest += "<OBJECT_TYPE>" + q.GetDatabase().xmlsafestr(this -> object_type) + "</OBJECT_TYPE>";
	dest += "</OBJECTS>";
	return dest;
}


size_t Objects::num_cols()
{
	return 17;
}


void Objects::clear()
{
	this -> id = 0;
	this -> entity = "";
	this -> meshfile = "";
	this -> x = 0;
	this -> y = 0;
	this -> z = 0;
	this -> rot_x = 0;
	this -> rot_y = 0;
	this -> rot_z = 0;
	this -> mask = 0;
	this -> scale_x = 0;
	this -> scale_y = 0;
	this -> scale_z = 0;
	this -> material = "";
	this -> isstatic = "";
	this -> drop_to_ground = "";
	this -> object_type = "";
}


void Objects::spawn(const std::string& sql)
{
	Query q(*database);
	std::string temp;

	clear();

	if (!strncasecmp(sql.c_str(),"select * ",9))
	{
		temp = "select id,entity,meshfile,x,y,z,rot_x,rot_y,rot_z,mask,scale_x,scale_y,scale_z,material,static,drop_to_ground,object_type " + sql.substr(9);
	} else
		temp = sql;
	q.get_result(temp);
	if (q.fetch_row())
	{
		this -> id = q.getval(0);																				// 0 - id INTEGER
		this -> entity = q.getstr(1);																				// 1 - entity VARCHAR(255)
		this -> meshfile = q.getstr(2);																				// 2 - meshfile VARCHAR(255)
		this -> x = q.getnum(3);																				// 3 - x FLOAT
		this -> y = q.getnum(4);																				// 4 - y FLOAT
		this -> z = q.getnum(5);																				// 5 - z FLOAT
		this -> rot_x = q.getnum(6);																				// 6 - rot_x FLOAT
		this -> rot_y = q.getnum(7);																				// 7 - rot_y FLOAT
		this -> rot_z = q.getnum(8);																				// 8 - rot_z FLOAT
		this -> mask = q.getval(9);																				// 9 - mask INTEGER
		this -> scale_x = q.getnum(10);																				// 10 - scale_x FLOAT
		this -> scale_y = q.getnum(11);																				// 11 - scale_y FLOAT
		this -> scale_z = q.getnum(12);																				// 12 - scale_z FLOAT
		this -> material = q.getstr(13);																				// 13 - material VARCHAR(255)
		this -> isstatic = q.getstr(14);																				// 14 - static BOOLEAN
		this -> drop_to_ground = q.getstr(15);																				// 15 - drop_to_ground BOOLEAN
		this -> object_type = q.getstr(16);																				// 16 - object_type VARCHAR(255)
		new_object = 0;
	} else
		clear();
	q.free_result();
}


void Objects::spawn(Query *qd,int offset)
{
	clear();

	this -> id = qd -> getval(0 + offset);																				// 0 - id INTEGER
	this -> entity = qd -> getstr(1 + offset);																				// 1 - entity VARCHAR(255)
	this -> meshfile = qd -> getstr(2 + offset);																				// 2 - meshfile VARCHAR(255)
	this -> x = qd -> getnum(3 + offset);																				// 3 - x FLOAT
	this -> y = qd -> getnum(4 + offset);																				// 4 - y FLOAT
	this -> z = qd -> getnum(5 + offset);																				// 5 - z FLOAT
	this -> rot_x = qd -> getnum(6 + offset);																				// 6 - rot_x FLOAT
	this -> rot_y = qd -> getnum(7 + offset);																				// 7 - rot_y FLOAT
	this -> rot_z = qd -> getnum(8 + offset);																				// 8 - rot_z FLOAT
	this -> mask = qd -> getval(9 + offset);																				// 9 - mask INTEGER
	this -> scale_x = qd -> getnum(10 + offset);																				// 10 - scale_x FLOAT
	this -> scale_y = qd -> getnum(11 + offset);																				// 11 - scale_y FLOAT
	this -> scale_z = qd -> getnum(12 + offset);																				// 12 - scale_z FLOAT
	this -> material = qd -> getstr(13 + offset);																				// 13 - material VARCHAR(255)
	this -> isstatic = qd -> getstr(14 + offset);																				// 14 - static BOOLEAN
	this -> drop_to_ground = qd -> getstr(15 + offset);																				// 15 - drop_to_ground BOOLEAN
	this -> object_type = qd -> getstr(16 + offset);																				// 16 - object_type VARCHAR(255)
}


// End of implementation of class 'Objects'

} // End of namespace
namespace DAL {

/**
 **  Begin class 'Attributes'
 **/

Attributes::Attributes(Database *db)
{
	database = db;
	new_object = 1;
	clear();
}


Attributes::Attributes(Database *db,const std::string& sql)
{
	database = db;
	new_object = 1;
	spawn(sql);
}


Attributes::Attributes(Database *db,Query *qd,int offset)
{
	database = db;
	new_object = 0;
	spawn(qd, offset);
}


Attributes::Attributes(Database& db,long i_id):database(&db),new_object(1)
{
	Query q(*database);
	std::string sql = "select * from attributes where ";
	{
		char slask[100];
		sprintf(slask,"id='%ld'",i_id);
		sql += slask;
	}
	spawn(sql);
}


Attributes::~Attributes()
{
}


void Attributes::select(const std::string& sql)
{
	spawn(sql);
}


unsigned __int64 Attributes::insert()
{
	Query q(*database);
	std::string sql;

	sql = "insert into attributes(object_name,attribute,value)";
	sql += " values('" + q.GetDatabase().safestr(this -> object_name) + "'";
	sql += ", '" + q.GetDatabase().safestr(this -> attribute) + "'";
	sql += ", '" + q.GetDatabase().safestr(this -> value) + "'";
	sql += ")";
	q.execute(sql);
	new_object = 0;
	unsigned __int64 inserted_id = q.insert_id();
	id = inserted_id;
	return inserted_id;
}


void Attributes::update()
{
	update(this -> id);
}


void Attributes::update(long i_id)
{
	Query q(*database);
	std::string sql;
	sql += "update attributes set object_name='" + q.GetDatabase().safestr(this -> object_name) + "'";
	sql += ", attribute='" + q.GetDatabase().safestr(this -> attribute) + "'";
	sql += ", value='" + q.GetDatabase().safestr(this -> value) + "'";
	{
		char slask[200];
		sprintf(slask," where id='%ld'",i_id);
		sql += slask;
	}
	q.execute(sql);
}


void Attributes::save()
{
	if (new_object)
		insert();
	else
		update();
}


void Attributes::erase()
{
	if (!new_object)
	{
		std::string sql = "delete from attributes where";
		Query q(*database);
		{
			char slask[200];
			sprintf(slask," id='%ld'",this -> id);
			sql += slask;
		}
		q.execute(sql);
	}
}


std::string Attributes::xml()
{
	Query q(*database);
	std::string dest;
	char slask[200];
	dest = "<ATTRIBUTES>";
	sprintf(slask,"<ID>%ld</ID>",this -> id);
	dest += slask;
	dest += "<OBJECT_NAME>" + q.GetDatabase().xmlsafestr(this -> object_name) + "</OBJECT_NAME>";
	dest += "<ATTRIBUTE>" + q.GetDatabase().xmlsafestr(this -> attribute) + "</ATTRIBUTE>";
	dest += "<VALUE>" + q.GetDatabase().xmlsafestr(this -> value) + "</VALUE>";
	dest += "</ATTRIBUTES>";
	return dest;
}


std::string Attributes::xml(const std::string& tag,const std::string& xvalx)
{
	Query q(*database);
	std::string dest;
	char slask[200];
	dest = "<ATTRIBUTES " + tag + "=\"" + xvalx + "\">";
	sprintf(slask,"<ID>%ld</ID>",this -> id);
	dest += slask;
	dest += "<OBJECT_NAME>" + q.GetDatabase().xmlsafestr(this -> object_name) + "</OBJECT_NAME>";
	dest += "<ATTRIBUTE>" + q.GetDatabase().xmlsafestr(this -> attribute) + "</ATTRIBUTE>";
	dest += "<VALUE>" + q.GetDatabase().xmlsafestr(this -> value) + "</VALUE>";
	dest += "</ATTRIBUTES>";
	return dest;
}


size_t Attributes::num_cols()
{
	return 4;
}


void Attributes::clear()
{
	this -> id = 0;
	this -> object_name = "";
	this -> attribute = "";
	this -> value = "";
}


void Attributes::spawn(const std::string& sql)
{
	Query q(*database);
	std::string temp;

	clear();

	if (!strncasecmp(sql.c_str(),"select * ",9))
	{
		temp = "select id,object_name,attribute,value " + sql.substr(9);
	} else
		temp = sql;
	q.get_result(temp);
	if (q.fetch_row())
	{
		this -> id = q.getval(0);																				// 0 - id INTEGER
		this -> object_name = q.getstr(1);																				// 1 - object_name VARCHAR(255)
		this -> attribute = q.getstr(2);																				// 2 - attribute VARCHAR(255)
		this -> value = q.getstr(3);																				// 3 - value TEXT
		new_object = 0;
	} else
		clear();
	q.free_result();
}


void Attributes::spawn(Query *qd,int offset)
{
	clear();

	this -> id = qd -> getval(0 + offset);																				// 0 - id INTEGER
	this -> object_name = qd -> getstr(1 + offset);																				// 1 - object_name VARCHAR(255)
	this -> attribute = qd -> getstr(2 + offset);																				// 2 - attribute VARCHAR(255)
	this -> value = qd -> getstr(3 + offset);																				// 3 - value TEXT
}


// End of implementation of class 'Attributes'

} // End of namespace

#endif