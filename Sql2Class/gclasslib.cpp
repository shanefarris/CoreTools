#ifdef _WIN32
#pragma warning(disable:4786)
#endif

#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <string.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <fcntl.h>
#ifndef _WIN32
#include <unistd.h>
#include <stdint.h>
#else
#include <direct.h>
typedef unsigned __int64 uint64_t;
#endif

#include "sql2class.h"
#include "utils.h"
#include "Parse.h"

#include "gclasslib.h"

bool isPrimaryField(TBLS* tbl, std::string fldName)
{
	INDEX* idx = tbl->primary;
	if(idx)
	{
		std::vector<FIELDS *> flds = idx->fields;
		for(unsigned int i = 0; i < flds.size(); i++)
		{
			if(flds[i]->sqlname == fldName)
				return true;
		}
	}
	return false;
}

void generate_lib(char *dbname,short global,char *defdir,char *libname,char *prefix)
{
	FILE *fil;
	time_t ti = time(NULL);
	struct tm *tp = localtime(&ti);
	char filename[200];
	char define[200];

	if (global)
	{
		sprintf(filename,"%s/include/",prefix);
	}
	else
	{
		*filename = 0;
	}
	sprintf(filename + strlen(filename),"%s.h",libname);
	sprintf(define,"_%s_H",libname);
	strupr(define);

	if ((fil = fopen(filename,"wt")) == NULL)
	{
		printf("Couldn't create '%s'...\n",filename);
		return;
	}

	fprintf(fil, "// Generated DAL code. \n");
	fprintf(fil, "\n");

	if (use_wrapped)
	{
		fprintf(fil, "#include <Wrapped/IDatabase.h>\n");
		fprintf(fil, "#include <Wrapped/Query.h>\n");
		fprintf(fil, "#include <Wrapped/Mysql/enum_t.h>\n");
		fprintf(fil, "#include <Wrapped/Mysql/set_t.h>\n");
		fprintf(fil, "using Wrapped::Mysql::enum_t;\n");
		fprintf(fil, "using Wrapped::Mysql::set_t;\n");
	}
	else
	{
		fprintf(fil, "#include <stdio.h>\n");
		fprintf(fil, "#include <stdlib.h>\n");
		fprintf(fil, "#include <string.h>\n");
		if (use_odbc)
		{
			fprintf(fil, "#include <afxdb.h>\n");
			fprintf(fil, "#include <sql.h>\n");
			fprintf(fil, "#include <Database.h>\n");
			fprintf(fil, "#include <Query.h>\n");
		}
		else
		if (use_sqlite)
		{
			//fprintf(fil, "#include <sqlite3.h>\n");
		}
		else
		{
			fprintf(fil, "#include <mysql/mysql.h>\n");
		}

		fprintf(fil, "#include \"sqlite3.h\"\n");
		fprintf(fil, "#include \"IError.h\"\n");
		fprintf(fil, "#include \"StderrLog.h\"\n");
		fprintf(fil, "#include \"Database.h\"\n");
		fprintf(fil, "#include \"Query.h\"\n");
	}

	fprintf(fil, "#include <map>\n");
	fprintf(fil, "\n");
	{
		fprintf(fil, "#include <vector>\n"
			"#include <string>\n"
			"\n");
	}
	fprintf(fil, "#ifndef %s\n",define);
	fprintf(fil, "#define %s\n",define);
	fprintf(fil, "\n");
	fprintf(fil, "#ifdef _WIN32\n");
	fprintf(fil, "#define strncasecmp _strnicmp\n");
	fprintf(fil, "#define strcasecmp stricmp\n");
	fprintf(fil, "#pragma warning(push)\n");
	fprintf(fil, "#pragma warning(disable:4251)\n");
	fprintf(fil, "\n");
	fprintf(fil, "#ifdef CORE_DLL\n");
	fprintf(fil, "	#define CORE_EXPORT __declspec(dllexport)\n");
	fprintf(fil, "#else\n");
	fprintf(fil, "	#define CORE_EXPORT __declspec(dllimport)\n");
	fprintf(fil, "#endif\n");
	fprintf(fil, "#endif // _WIN32\n");
	if (container)
	{
		fprintf(fil, "#ifndef _LISTTYPE\n");
		fprintf(fil, "#define _LISTTYPE\n");
		fprintf(fil, "typedef enum { Direct, InDirect } ListType;\n");
		fprintf(fil, "#endif\n");
		fprintf(fil, "\n");
	}
	if (baseclass.size())
	{
		fprintf(fil, "#include \"%s.h\"\n\n",baseclass.c_str());
	}

	// Create header file
	if (namespc.size())
	{
		fprintf(fil, "\n");
		if(namespc == "Core")
		{
			fprintf(fil, "namespace Core\n{\n");
			fprintf(fil, "namespace DAL\n{\n");
		}
		else
		{
		fprintf(fil, "namespace %s\n{\n",namespc.c_str());
		}
	}

	for (auto it = tblsbase.begin(); it != tblsbase.end(); it++)
	{
		TBLS *t = *it;
		generate_libh(fil,t);
	}

	if (namespc.size())
	{
		if(namespc == "Core")
		{
			fprintf(fil, "} // end of DAL namespace\n");
			fprintf(fil, "} // end of Core namespace\n");
		}
		else
		{
			fprintf(fil, "} // end of namespace\n");
		}
	}
	// End of header file

	fprintf(fil, "#pragma warning(pop)\n");
	fprintf(fil, "#endif // %s\n",define);
	fclose(fil);


// make .cpp file

	if (global)
	{
		sprintf(filename,"%s/src/%s",prefix,libname);
#ifdef _WIN32
		_mkdir(filename);
#else
		mkdir(filename,0755);
#endif
		strcat(filename,"/");
	}
	else
	{
		*filename = 0;
	}
	sprintf(filename + strlen(filename),"%s.cpp",libname);

	if ((fil = fopen(filename,"wt")) == NULL)
	{
		printf("Couldn't create '%s'...\n",filename);
		return;
	}

	fprintf(fil, "// Generated DAL code.\n");
	
	if (use_wrapped)
	{
	}
	else
	if (!use_sqlite && !use_odbc)
	{
		fprintf(fil, "#ifdef WIN32\n"
			"#include <Config-win.h>\n"
			"#endif\n");
	}

	if (global)
	{
		fprintf(fil, "#include <%s.h>\n",libname);
	}
	else
	{
		fprintf(fil, "#include \"%s.h\"\n",libname);
	}
	fprintf(fil, "\n");

	// Create source file
	if (namespc.size())
	{
		if(namespc == "Core")
		{
			fprintf(fil, "namespace Core {\n");
			fprintf(fil, "namespace DAL {\n");
		}
		else
		{
			fprintf(fil, "namespace %s {\n", namespc.c_str());
		}
	}

	for (tbls_l::iterator it = tblsbase.begin(); it != tblsbase.end(); it++)
	{
		TBLS *t = *it;
		generate_libc(fil,t);
	}

	if (namespc.size())
	{
		if(namespc == "Core")
		{
			fprintf(fil, "} // End of Core namespace\n");
			fprintf(fil, "} // End of DAL namespace\n");
		}
		else
		{
			fprintf(fil, "} // End of namespace\n");
		}
	}
	// End of source file

	fclose(fil);

}


void generate_libh(FILE *fil,TBLS *table)
{
	char *ptrtotable;
	char classname[200];
	char slask[200];
	char cclassname[200];	// container class name
	char cstruct[200];	// container struct
	char lwrclassname[200];

	strcpy(classname,table -> name.c_str());
	strcpy(lwrclassname,classname);
	if (mangle_names)
	{
		if (*classname >= 'a' && *classname <= 'z')
		{
			*classname -= 32;
		}
	}

	sprintf(cclassname,"c%s",classname);	// container class name
	if (!strcmp(classname + strlen(classname) - 2,"ss"))
	{
		sprintf(cstruct,"c%struct",classname);
	}
	else
	{
		sprintf(cstruct,"c%sstruct",classname); // container struct
	}

	fprintf(fil, "//  Class '%s'\n",classname);

	if (container)
	{
		fprintf(fil, "typedef struct %s\n",cstruct);
		fprintf(fil, "{\n");
		fprintf(fil, "\tstruct %s *next;\n",cstruct);
		fprintf(fil, "\tclass %s *%s;\n",classname,lwrclassname);
		fprintf(fil, "};\n");
		fprintf(fil, "\n");
	}

	if (baseclass.size())
	{
		fprintf(fil, "class __declspec(dllexport) %s : public %s {\n",classname,baseclass.c_str());
	}
	else
	{
		fprintf(fil, "class __declspec(dllexport) %s {\n",classname);
	}
	// enum, set
	for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
	{
		FIELDS *t = *it;
		// long, float, char, text, mediumtext
		if (t -> enum_set)
		{
			fprintf(fil, "\t// map for %s column %s\n",t -> set ? "SET" : "ENUM",t -> column);
			fprintf(fil, "\tstd::map<std::string, uint64_t> mmap_%s;\n",t -> column);
			fprintf(fil, "\n");
		}
	}

	fprintf(fil, "public:\n");

	if (use_wrapped)
	{
		fprintf(fil, "\t%s(Wrapped::IDatabase *);\n", classname);
		fprintf(fil, "\t%s(Wrapped::IDatabase *,const std::string&);\n", classname);
		fprintf(fil, "\t%s(Wrapped::IDatabase *,Wrapped::IQuery *,int offset = 0);\n", classname);
	}
	else
	{
		// empty constructor + database
		fprintf(fil, "\t%s(Database *);\n",classname);

		// data fetch constructor + database
		fprintf(fil, "\t%s(Database *,const std::string& );\n",classname);

		// data fetch constructor from query
		fprintf(fil, "\t%s(Database *,Query *,int = 0 /* offset */);\n",classname);
	}

	// constructors from primary index
	{
	for (index_v::iterator it = table -> index.begin(); it != table -> index.end(); it++)
	{
		INDEX *p = *it;
		if (p -> primary || p -> unique)
		{
			if (use_wrapped)
				fprintf(fil, "\t%s(Wrapped::IDatabase&", classname);
			else
				fprintf(fil, "\t%s(Database&",classname);
			for (fields_v::iterator it = p -> fields.begin(); it != p -> fields.end(); it++)
			{
				FIELDS *f = *it;
				std::string intyp;
				std::string suffix;
				std::string ctype = f -> GetCType(intyp, suffix);
				// long, float, char, text, mediumtext
				fprintf(fil, ",%s %s",intyp.c_str(),f -> column);
			}
			fprintf(fil, ");\n");
		}
	}
	}

	// destructor
	fprintf(fil, "\t~%s();\n",classname);

	//
	if (use_wrapped)
		fprintf(fil, "\tWrapped::IDatabase& GetDatabase() { return *database; }\n\n");
	else
		fprintf(fil, "\tDatabase& GetDatabase() { return *database; }\n\n");

	// long Class::insert()
	//	'insert into ...(...) values(...)'
#ifdef _WIN32
	fprintf(fil, "\tunsigned __int64 insert();\n");
#else
	if (use_odbc)
		fprintf(fil, "\tunsigned __int64 insert();\n");
	else
		fprintf(fil, "\tunsigned long long int insert();\n");
#endif

	// void Class::update()
	if (table -> primary)
	{
		fprintf(fil, "\tvoid update();\n");
	}


	// void Class::save()
	fprintf(fil, "\tvoid save();\n");

	// void Class::erase()
	if (table -> primary)
	{
		fprintf(fil, "\tvoid erase();\n");
	}

	// void Class::xml()
	if (!use_wrapped)
	{
		fprintf(fil, "\tstd::string xml();\n");
		fprintf(fil, "\tstd::string xml(const std::string& ,const std::string& );\n");
	}
	// num_cols()
	fprintf(fil, "\tsize_t num_cols();\n");
	fprintf(fil, "\n");

	// get/set
	if (make_getset)
	{
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			// long, float, char, text, mediumtext
			if (*t -> ctype)
			{
				std::string intyp;
				std::string suffix;
				std::string type = t -> GetCType(intyp, suffix);
				char tmp[200];
				strcpy(tmp, t -> column);
				if (*tmp >= 'a')
				{
					*tmp -= 32;
				}
				if (type == "enum_t")
				{
					fprintf(fil, "\t%s Get%s() { return this -> %s.String(); }\n",
						intyp.c_str(), tmp, t -> column);
					fprintf(fil, "\tvoid Set%s(%s x) { this -> %s = x; }\n",
						tmp, intyp.c_str(), t -> column);
				}
				else
				if (type == "set_t")
				{
					fprintf(fil, "\t%s Get%s() { return this -> %s.String(); }\n",
						intyp.c_str(), tmp, t -> column);
					fprintf(fil, "\tvoid Set%s(%s x) { this -> %s = x; }\n",
						tmp, intyp.c_str(), t -> column);
				}
				else
				if (type == "std::string")
				{
					fprintf(fil, "\tconst char *Get%s() { return this -> %s.c_str(); }\n",
						tmp, t -> column);
					fprintf(fil, "\tvoid Set%s(%s x) { this -> %s = x; }\n",
						tmp, intyp.c_str(), t -> column);
				}
				else
				{
					fprintf(fil, "\t%s Get%s() { return this -> %s; }\n",
						intyp.c_str(), tmp, t -> column);
					fprintf(fil, "\tvoid Set%s(%s x) { this -> %s = x; }\n",
						tmp, intyp.c_str(), t -> column);
				}
			}
			else
			{
				fprintf(fil, "\t// %d - %s %s",t -> num,t -> column,t -> typ);
			}
		}
		fprintf(fil, "\n");
	}

	// variables
	fprintf(fil, "\t// table columns\n");
	{
		if (make_getset)
			fprintf(fil, "private:\n");
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			// long, float, char, text, mediumtext
			if (*t -> ctype)
			{
				std::string intyp;
				std::string suffix;
				std::string type = t -> GetCType(intyp, suffix);
				while (type.size() < 12)
					type += " ";
				fprintf(fil, "\t%s %s%s; // %s",type.c_str(),t -> column,suffix.c_str(),t -> typ);
			}
			else
			{
				fprintf(fil, "\t// %d - %s %s",t -> num,t -> column,t -> typ);
			}
 			if (*t -> comment)
 			{
				fprintf(fil, " //!< %s ",t -> comment);
			}
			fprintf(fil, "\n");

			// Comment from dbdesigner generates a class ptr as member variable
			if (*t -> comment && (ptrtotable = strstr(t -> comment,"ptrtotable: ")) != NULL)
			{
				strcpy(slask,ptrtotable + 12);	// ptrtoclass
				strlwr(slask);
				if (*slask >= 'a' && *slask <= 'z')
				{
					*slask -= 32;
				}
				fprintf(fil, "\tclass %s *_%s;\n",slask,t -> column);
			}
		}
	}
	fprintf(fil, "\t//\n");


	// private
	if (!make_getset)
		fprintf(fil, "private:\n");

	// clear fields
	fprintf(fil, "\tvoid clear();\n");

	// spawn from query
	fprintf(fil, "\tvoid spawn(const std::string& );\n");

	// spawn from query
	if (use_wrapped)
		fprintf(fil, "\tvoid spawn(Wrapped::IQuery *,int = 0 /* offset */);\n");
	else
		fprintf(fil, "\tvoid spawn(Query *,int = 0 /* offset */);\n");

	// void Class::select(const std::string& ) 'spawn_wrapper'
	//	numera ersatt av Class::Class(db,sql)...
	fprintf(fil, "\tvoid select(const std::string& );\n");

	// void Class::update(long num)
	//	'update ... set ... where ...'
	if (table -> primary)
	{
		fprintf(fil, "\tvoid update(");
		*slask = 0;
		for (fields_v::iterator it = table -> primary -> fields.begin(); it != table -> primary -> fields.end(); it++)
		{
			FIELDS *f = *it;
			std::string intyp;
			std::string suffix;
			std::string typ = f -> GetCType(intyp, suffix);
			// long, float, char, text, mediumtext
			if (*f -> ctype)
			{
				fprintf(fil, "%s%s %s",slask,intyp.c_str(),f -> column);
				strcpy(slask,",");
			}
			strcpy(slask,",");
		}
		fprintf(fil, ");\n");
	} // if (table -> primary)

	fprintf(fil, "\t//\n");
	if (use_wrapped)
		fprintf(fil, "\tWrapped::IDatabase *database;\n");
	else
		fprintf(fil, "\tDatabase *database;\n");
	fprintf(fil, "\tshort new_object;\n");

	fprintf(fil, "}; // End of class '%s'\n",classname);
	fprintf(fil, "\n");


	/*
	 * Create container class (see container.h for an example)
	 */
	if (container)
	{
		fprintf(fil, "class __declspec(dllexport) %s {\n",cclassname);
		fprintf(fil, "public:\n");

		// constructor cclassname(db) - empty list
		fprintf(fil, "\t%s::%s(Database *,ListType);\n",cclassname,cclassname);

		// constructor cclassname(db,sql)
		fprintf(fil, "\t%s::%s(Database *,char *,ListType);\n",cclassname,cclassname);

		// destructor
		fprintf(fil, "\t%s::~%s();\n",cclassname,cclassname);

		// save()
		fprintf(fil, "\tvoid %s::save();\n",cclassname);

		// baseptr()
		fprintf(fil, "\t%s *%s::baseptr();\n",cstruct,cclassname);

		// get...()
		fprintf(fil, "\t%s *%s::get%s(long);\n",classname,cclassname,lwrclassname);

		// add(classname *)
		fprintf(fil, "\tvoid %s::add(%s *);\n",cclassname,classname);

		// remove(classname *)
		fprintf(fil, "\tvoid %s::remove(%s *);\n",cclassname,classname);

		// find_...()
		fprintf(fil, "\n");
		fprintf(fil, "\t// find_xxx()\n");
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			strcpy(slask,typestring(t));
			if (*slask)
			{
				if (slask[strlen(slask) - 1] == ' ')
				{
					slask[strlen(slask) - 1] = 0;
				}
				fprintf(fil, "\t%s *%s::find_%s(%s);\n",classname,cclassname,t -> column,slask);
			}
		}

		// private
		fprintf(fil, "private:\n");
		fprintf(fil, "\tDatabase *database;\n");
		fprintf(fil, "\t%s *base;\n",cstruct);
		fprintf(fil, "\t%s **%ss;\n",classname,lwrclassname);
		fprintf(fil, "\tlong qty;\n");
		fprintf(fil, "\tListType _lt;\n");
		fprintf(fil, "}; // End of class '%s'\n",cclassname);
		fprintf(fil, "\n");
	} // if (container)
}


void generate_libc(FILE *fil,TBLS *table)
{
	char *ptrtotable;
	char classname[200];
	char slask[200];
	char cclassname[200];	// container class name
	char cstruct[200];	// container struct
	char lwrclassname[200];
	char lwrtablename[200];

	strcpy(classname,table -> name.c_str());
	strcpy(lwrclassname,classname);
	if (mangle_names)
	{
		if (*classname >= 'a' && *classname <= 'z')
		{
			*classname -= 32;
		}
	}

	strcpy(lwrtablename,table -> name.c_str());
	if (mangle_names)
	{
		if (*lwrtablename >= 'a' && *lwrtablename <= 'z')
		{
			*lwrtablename -= 32;
		}
	}

	sprintf(cclassname,"c%s",lwrclassname);	// container class name
	cclassname[1] -= 32; // (uppercase)
	if (!strcmp(classname + strlen(classname) - 2,"ss"))
	{
		sprintf(cstruct,"c%struct",classname);
	}
	else
	{
		sprintf(cstruct,"c%sstruct",classname); // container struct
	}
	
	fprintf(fil, "// Begin class '%s'\n",classname);

	// empty constructor + database
	if (use_wrapped)
		fprintf(fil, "%s::%s(Wrapped::IDatabase *db)",classname,lwrtablename);
	else
		fprintf(fil, "%s::%s(Database *db)",classname,lwrtablename);
	strcpy(slask,":");
	for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
	{
		FIELDS *t = *it;
		if (t -> enum_set)
		{
			fprintf(fil, "%s%s(mmap_%s)",slask,t -> column,t -> column);
			strcpy(slask,",");
		}
	}
	fprintf(fil, "\n{\n");
	fprintf(fil, "\tdatabase = db;\n");
	fprintf(fil, "\tnew_object = 1;\n");
	fprintf(fil, "\tclear();\n");
	fprintf(fil, "}\n\n");

	// data fetch constructor + database
	if (use_wrapped)
		fprintf(fil, "%s::%s(Wrapped::IDatabase *db,const std::string& sql)",classname,lwrtablename);
	else
		fprintf(fil, "%s::%s(Database *db,const std::string& sql)",classname,lwrtablename);
	strcpy(slask,":");
	{
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			if (t -> enum_set)
			{
				fprintf(fil, "%s%s(mmap_%s)",slask,t -> column,t -> column);
				strcpy(slask,",");
			}
		}
	}
	fprintf(fil, "\n{\n");
	fprintf(fil, "\tdatabase = db;\n");
	fprintf(fil, "\tnew_object = 1;\n");
	{
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			// long, float, char, text, mediumtext
			if (!strcmp(t -> ctype,"text") && !use_stl)
			{
				fprintf(fil, "\tthis -> %s = NULL;\n",t -> column);
			}
		}
	}
	// call spawn here
	fprintf(fil, "\tspawn(sql);\n");
	fprintf(fil, "}\n\n");

	// data fetch constructor from query
	if (use_wrapped)
		fprintf(fil, "%s::%s(Wrapped::IDatabase *db,Wrapped::IQuery *qd,int offset)",classname,lwrtablename);
	else
		fprintf(fil, "%s::%s(Database *db,Query *qd,int offset)",classname,lwrtablename);
	strcpy(slask,":");
	{
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			if (t -> enum_set)
			{
				fprintf(fil, "%s%s(mmap_%s)",slask,t -> column,t -> column);
				strcpy(slask,",");
			}
		}
	}
	fprintf(fil, "\n{\n");
	fprintf(fil, "\tdatabase = db;\n");
	fprintf(fil, "\tnew_object = 0;\n");
	{
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			// long, float, char, text, mediumtext
			if (!strcmp(t -> ctype,"text") && !use_stl)
			{
				fprintf(fil, "\tthis -> %s = NULL;\n",t -> column);
			}
		}
	}
	// call spawn here
	fprintf(fil, "\tspawn(qd, offset);\n");
	fprintf(fil, "}\n\n");

	// constructors from primary index
	{
	for (index_v::iterator it = table -> index.begin(); it != table -> index.end(); it++)
	{
		INDEX *p = *it;
		if (p -> primary || p -> unique)
		{
			if (use_wrapped)
				fprintf(fil, "%s::%s(Wrapped::IDatabase& db",classname,classname);
			else
				fprintf(fil, "%s::%s(Database& db",classname,classname);
			{
				for (fields_v::iterator it = p -> fields.begin(); it != p -> fields.end(); it++)
				{
					FIELDS *f = *it;
					std::string intyp;
					std::string suffix;
					std::string typ = f -> GetCType(intyp, suffix);
					// long, float, char, text, mediumtext
					if (*f -> ctype)
					{
						fprintf(fil, ",%s i_%s",intyp.c_str(),f -> column);
					}
				}
			}
			fprintf(fil, ")");
			strcpy(slask,":");
			for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
			{
				FIELDS *t = *it;
				if (t -> enum_set)
				{
					fprintf(fil, "%s%s(mmap_%s)",slask,t -> column,t -> column);
					strcpy(slask,",");
				}
			}
			fprintf(fil, "%sdatabase(&db),new_object(1)",slask);
			fprintf(fil, "\n{\n");
			if (use_wrapped)
				fprintf(fil, "\tQuery q(*database);\n");
			else
				fprintf(fil, "\tQuery q(*database);\n");
			fprintf(fil, "\tstd::string sql = \"select * from %s where \";\n",table -> name.c_str());
			*slask = 0;
			{
				for (fields_v::iterator it = p -> fields.begin(); it != p -> fields.end(); it++)
				{
					FIELDS *f = *it;
					// long, float, char, text, mediumtext
					if (!strcmp(f -> ctype,"float"))
					{
						fprintf(fil, "\t{\n");
						fprintf(fil, "\t\tchar slask[100];\n");
						fprintf(fil, "\t\tsprintf_s(slask, 100, \"%s%s='%%f'\",i_%s);\n",slask,f -> sqlname.c_str()/*column*/,f -> column);
						fprintf(fil, "\t\tsql += slask;\n");
						fprintf(fil, "\t}\n");
						strcpy(slask," and ");
					}
					else
					if (!strcmp(f -> ctype,"long"))
					{
						std::string l = (f -> bitsize >= 32) ? "l" : "";
						char c = f -> uns ? 'u' : 'd';
						// %! bigint
						if (f -> bitsize == 64)
							l = "ll";
						fprintf(fil, "\t{\n");
						fprintf(fil, "\t\tchar slask[100];\n");
						fprintf(fil, "\t\tsprintf_s(slask, 100, \"%s%s='%%%s%c'\",i_%s);\n",slask,f -> sqlname.c_str()/*column*/,l.c_str(),c,f -> column);
						fprintf(fil, "\t\tsql += slask;\n");
						fprintf(fil, "\t}\n");
						strcpy(slask," and ");
					}
					else
					if (*f -> ctype)
					{
						fprintf(fil, "\tsql += \"%s%s='\" + q.GetDatabase().safestr(i_%s) + \"'\";\n",slask,f -> sqlname.c_str()/*column*/,f -> column);
						strcpy(slask," and ");
					}
				}
			}
			{
				for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
				{
					FIELDS *t = *it;
					// long, float, char, text, mediumtext
					if (!strcmp(t -> ctype,"text") && !use_stl)
					{
						fprintf(fil, "\tthis -> %s = NULL;\n",t -> column);
					}
				}
			}
			// call spawn here
			fprintf(fil, "\tspawn(sql);\n");
			//
			fprintf(fil, "}\n\n");
		}
	}
	}

	// destructor
	fprintf(fil, "%s::~%s()\n{\n",classname,lwrtablename);
	{
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			// long, float, char, text, mediumtext
			if (!strcmp(t -> ctype,"text"))
			{
				if (!use_stl)
				{
					fprintf(fil, "\tif (this -> %s)\n",t -> column);
					fprintf(fil, "\t{\n");
					fprintf(fil, "\t\tdelete this -> %s;\n",t -> column);
					fprintf(fil, "\t}\n");
				}
			}
		}
	}
	fprintf(fil, "}\n\n");


	// void Class::select(const std::string& ) 'spawn_wrapper'
	//	numera ersatt av Class::Class(db,sql)...
	fprintf(fil, "void %s::select(const std::string& sql)\n{\n",classname);
	fprintf(fil, "\tspawn(sql);\n");
	fprintf(fil, "}\n\n");


	// long Class::insert()
	//	'insert into ...(...) values(...)'
#ifdef _WIN32
	fprintf(fil, "unsigned __int64 %s::insert()\n{\n",classname);
#else
	if (use_odbc)
		fprintf(fil, "unsigned __int64 %s::insert()\n{\n",classname);
	else
		fprintf(fil, "unsigned long long int %s::insert()\n{\n",classname);
#endif
	if (use_wrapped)
		fprintf(fil, "\tQuery q(*database);\n");
	else
		fprintf(fil, "\tQuery q(*database);\n");
	fprintf(fil, "\tstd::string sql;\n");
	fprintf(fil, "\n");

	fprintf(fil, "\tsql = \"insert into %s(",table -> name.c_str());
	*slask = 0;
	{
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			if (!t -> ignore && !isPrimaryField(table, t->sqlname))
			{
				fprintf(fil, "%s%s",slask,t -> sqlname.c_str()); //t -> column);
				strcpy(slask,",");
			} // !ignore
		}
	}
	fprintf(fil, ")\";\n");

	strcpy(slask," values(");
	{
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			if (!t -> ignore && !isPrimaryField(table, t->sqlname))
			{
				// long, float, char, text, mediumtext
				if (t -> enum_set)
				{
					fprintf(fil, "\tsql += \"%s'\" + q.GetDatabase().safestr(this -> %s.String()) + \"'\";\n",slask,t -> column);
				}
				else
				if (!strcmp(t -> ctype,"float"))
				{
					fprintf(fil, "\t{\n");
					fprintf(fil, "\t\tchar slask[200];\n");
					fprintf(fil, "\t\tsprintf_s(slask, 200, \"%s%%f\",this -> %s);\n",slask,t -> column);
					fprintf(fil, "\t\tsql += slask;\n");
					fprintf(fil, "\t}\n");
				}
				else
				if (!strcmp(t -> ctype,"long"))
				{
					std::string l = (t -> bitsize >= 32) ? "l" : "";
					char c = t -> uns ? 'u' : 'd';
					// %! bigint
					if (t -> bitsize == 64)
						l = "ll";
					fprintf(fil, "\t{\n");
					fprintf(fil, "\t\tchar slask[200];\n");
					fprintf(fil, "\t\tsprintf_s(slask, 200, \"%s%%%s%c\",this -> %s);\n",slask,l.c_str(),c,t -> column);
					fprintf(fil, "\t\tsql += slask;\n");
					fprintf(fil, "\t}\n");
				}
				else
				if (!strcmp(t -> ctype,"text") && !use_stl)
				{
					fprintf(fil, "\tsql += \"%s'\" + (this -> %s ? q.GetDatabase().safestr(this -> %s) : \"\") + \"'\";\n",slask,t -> column,t -> column);
				}
				else
				if (!strcmp(t -> ctype,"mediumtext"))
				{
					fprintf(fil, "\t{\n");
					fprintf(fil, "\t\tsize_t i = 1;\n");
					fprintf(fil, "\t\tsql += \"%s'\";\n",slask);
					fprintf(fil, "\t\tfor (std::vector<std::string>::iterator it = this -> %s.begin(); it != this -> %s.end(); it++,i++)\n",t -> column,t -> column);
					fprintf(fil, "\t\t{\n");
					fprintf(fil, "\t\t\tsql += q.GetDatabase().safestr(*it);\n");
					fprintf(fil, "\t\t\tif (i < this -> %s.size())\n",t -> column);
					fprintf(fil, "\t\t\t\tsql += \"\\n\";\n");
					fprintf(fil, "\t\t}\n");
					fprintf(fil, "\t\tsql += \"'\";\n");
					fprintf(fil, "\t}\n");
				}
				else
				if (!strcmp(t -> ctype,"bool"))
				{
					fprintf(fil, "\tsql += \"%s'\" + (this -> %s ? q.GetDatabase().safestr(this -> %s ? \"true\" : \"false\") : \"\") + \"'\";\n",slask,t -> column,t -> column);
				}
				else
				if (*t -> ctype)
				{
					fprintf(fil, "\tsql += \"%s'\" + q.GetDatabase().safestr(this -> %s) + \"'\";\n",slask,t -> column);
				}
				else
				{
					fprintf(fil, "\tsql += \"%s''\";\n",slask);
				}
				strcpy(slask,", ");
			} // !ignore
		}
	}
	fprintf(fil, "\tsql += \")\";\n");

	fprintf(fil, "\n");
	fprintf(fil, "\tif(!q.execute(sql))\n");
	fprintf(fil, "\t\treturn 0;\n");
	fprintf(fil, "\n");
	fprintf(fil, "\tnew_object = 0;\n");
#ifdef _WIN32
	fprintf(fil, "\tunsigned __int64 inserted_id = q.insert_id();\n");
#else
	if (use_odbc)
		fprintf(fil, "\tunsigned __int64 inserted_id = q.insert_id();\n");
	else
		fprintf(fil, "\tunsigned long long int inserted_id = q.insert_id();\n");
#endif
	{
		fields_l::iterator it = table -> fieldsbase.begin();
		if (it != table -> fieldsbase.end())
		{
			FIELDS *p = *it; //table -> fieldsbase; //table -> primary -> fields[0];
			// long, float, char, text, mediumtext
			if (p && !strcmp(p -> ctype,"long"))
			{
				// %! take care - if first column is an integer type, it will be
				//		set to insert_id()
				fprintf(fil, "\t%s = (long)inserted_id;\n",p -> column);
			}
		}
	}
	fprintf(fil, "\treturn inserted_id;\n");
	fprintf(fil, "}\n\n");


	// void Class::update()
	if (table -> primary)
	{
		fprintf(fil, "void %s::update()\n{\n",classname);
		fprintf(fil, "\tupdate("); //%s);\n",table -> fieldsbase -> column);
		*slask = 0;
		for (fields_v::iterator it = table -> primary -> fields.begin(); it != table -> primary -> fields.end(); it++)
		{
			FIELDS *f = *it;
			fprintf(fil, "%sthis -> %s",slask,f -> column);
			strcpy(slask,",");
		}
		fprintf(fil, ");\n");
		fprintf(fil, "}\n\n");
	} // if (table -> primary)

	// void Class::update(long num)
	//	'update ... set ... where ...'
	if (table -> primary)
	{
	fprintf(fil, "void %s::update(",classname);
	*slask = 0;
	for (fields_v::iterator it = table -> primary -> fields.begin(); it != table -> primary -> fields.end(); it++)
	{
		FIELDS *f = *it;
		std::string intyp;
		std::string suffix;
		std::string typ = f -> GetCType(intyp, suffix);
		// long, float, char, text, mediumtext
		fprintf(fil, "%s%s i_%s",slask,intyp.c_str(),f -> column);
		strcpy(slask,",");
	}
	fprintf(fil, ")\n{\n");
	if (use_wrapped)
		fprintf(fil, "\tQuery q(*database);\n");
	else
		fprintf(fil, "\tQuery q(*database);\n");
	fprintf(fil, "\tstd::string sql;\n");

	sprintf(slask,"update %s set ",table -> name.c_str());
	{
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			if (!t -> ignore)
			{
				// long, float, char, text, mediumtext
				if (t -> enum_set)
				{
					fprintf(fil, "\tsql += \"%s%s='\" + q.GetDatabase().safestr(this -> %s.String()) + \"'\";\n",
						slask,t -> sqlname.c_str()/*column*/,t -> column);
					strcpy(slask,", ");
				}
				else
				if (!strcmp(t -> ctype,"float"))
				{
					fprintf(fil, "\t{\n");
					fprintf(fil, "\t\tchar slask[200];\n");
					fprintf(fil, "\t\tsprintf_s(slask, 200, \"%s%s='%%f'\",this -> %s);\n",
						slask,t -> sqlname.c_str()/*column*/,t -> column);
					fprintf(fil, "\t\tsql += slask;\n");
					fprintf(fil, "\t}\n");
					strcpy(slask,", ");
				}
				else
				if (!strcmp(t -> ctype,"long"))
				{
					std::string l = (t -> bitsize >= 32) ? "l" : "";
					char c = t -> uns ? 'u' : 'd';
					// %! bigint
					if (t -> bitsize == 64)
						l = "ll";
					fprintf(fil, "\t{\n");
					fprintf(fil, "\t\tchar slask[200];\n");
					fprintf(fil, "\t\tsprintf_s(slask, 200, \"%s%s=%%%s%c\",this -> %s);\n",
						slask,t -> sqlname.c_str()/*column*/,l.c_str(),c,t -> column);
					fprintf(fil, "\t\tsql += slask;\n");
					fprintf(fil, "\t}\n");
					strcpy(slask,", ");
				}
				else
				if (!strcmp(t -> ctype,"text") && !use_stl)
				{
					fprintf(fil, "\tsql += \"%s%s='\" + (this -> %s ? q.GetDatabase().safestr(this -> %s) : \"\") + \"'\";\n",
						slask,t -> sqlname.c_str()/*column*/,t -> column,t -> column);
					strcpy(slask,", ");
				}
				else
				if (!strcmp(t -> ctype,"mediumtext"))
				{
					fprintf(fil, "\t{\n");
					fprintf(fil, "\t\tsize_t i = 1;\n");
					fprintf(fil, "\t\tsql += \"%s%s='\";\n",slask,t -> sqlname.c_str()/*column*/);
					fprintf(fil, "\t\tfor (std::vector<std::string>::iterator it = this -> %s.begin(); it != this -> %s.end(); it++,i++)\n",t -> column,t -> column);
					fprintf(fil, "\t\t{\n");
					fprintf(fil, "\t\t\tsql += q.GetDatabase().safestr(*it);\n");
					fprintf(fil, "\t\t\tif (i < this -> %s.size())\n",t -> column);
					fprintf(fil, "\t\t\t\tsql += \"\\n\";\n");
					fprintf(fil, "\t\t}\n");
					fprintf(fil, "\t\tsql += \"'\";\n");
					fprintf(fil, "\t}\n");
				}
				else
				if (!strcmp(t -> ctype, "bool"))
				{
					fprintf(fil, "\tsql += \"%s%s='\" + q.GetDatabase().safestr(this -> %s  ? \"true\" : \"false\") + \"'\";\n",
						slask,t -> sqlname.c_str()/*column*/,t -> column);
					strcpy(slask,", ");
				}
				else
				if (*t -> ctype)
				{
					fprintf(fil, "\tsql += \"%s%s='\" + q.GetDatabase().safestr(this -> %s) + \"'\";\n",
						slask,t -> sqlname.c_str()/*column*/,t -> column);
					strcpy(slask,", ");
				}
			} // !ignore
		}
	}
	strcpy(slask," where");
	{
	for (fields_v::iterator it = table -> primary -> fields.begin(); it != table -> primary -> fields.end(); it++)
	{
		FIELDS *p = *it;
		// long, float, char, text, mediumtext
		if (!strcmp(p -> ctype,"float"))
		{
			fprintf(fil, "\t{\n");
			fprintf(fil, "\t\tchar slask[200];\n");
			fprintf(fil, "\t\tsprintf_s(slask, 200, \"%s %s='%%f'\",i_%s);\n",slask,p -> sqlname.c_str()/*column*/,p -> column);
			fprintf(fil, "\t\tsql += slask;\n");
			fprintf(fil, "\t}\n");
		}
		else
		if (!strcmp(p -> ctype,"long"))
		{
			std::string l = (p -> bitsize >= 32) ? "l" : "";
			char c = p -> uns ? 'u' : 'd';
			// %! bigint
			if (p -> bitsize == 64)
				l = "ll";
			fprintf(fil, "\t{\n");
			fprintf(fil, "\t\tchar slask[200];\n");
			fprintf(fil, "\t\tsprintf_s(slask, 200, \"%s %s='%%%s%c'\",i_%s);\n",slask,p -> sqlname.c_str()/*column*/,l.c_str(),c,p -> column);
			fprintf(fil, "\t\tsql += slask;\n");
			fprintf(fil, "\t}\n");
		}
		else
		if (*p -> ctype)
		{
			fprintf(fil, "\tsql += \"%s %s='\" + q.GetDatabase().safestr(i_%s) + \"'\";\n",slask,p -> sqlname.c_str()/*column*/,p -> column);
		}
		//
		strcpy(slask," and");
	}
	}

	fprintf(fil, "\tq.execute(sql);\n");

	fprintf(fil, "}\n\n");
	} // if (table -> primary)

	// void Class::save()
	fprintf(fil, "void %s::save()\n{\n",classname);
	fprintf(fil, "\tif (new_object)\n");
	fprintf(fil, "\t\tinsert();\n");
	if (table -> primary) // update / erase only available on tables with primary key
	{
		fprintf(fil, "\telse\n");
		fprintf(fil, "\t\tupdate();\n");
	}
	fprintf(fil, "}\n\n");

	// void Class::erase()
	if (table -> primary)
	{
		fprintf(fil, "void %s::erase()\n{\n",classname);
		fprintf(fil, "\tif (!new_object)\n");
		fprintf(fil, "\t{\n");
		fprintf(fil, "\t\tstd::string sql = \"delete from %s where\";\n",table -> name.c_str());
		if (use_wrapped)
			fprintf(fil, "\t\tQuery q(*database);\n");
		else
			fprintf(fil, "\t\tQuery q(*database);\n");
		*slask = 0;
		for (fields_v::iterator it = table -> primary -> fields.begin(); it != table -> primary -> fields.end(); it++)
		{
			FIELDS *f = *it;
			// long, float, char, text, mediumtext
			if (f -> enum_set)
			{
				fprintf(fil, "\t\tsql += \"%s %s='\" + q.GetDatabase().safestr(this -> %s.String()) + \"'\";\n",slask,f -> sqlname.c_str()/*column*/,f -> column);
			}
			else
			if (!strcmp(f -> ctype,"float"))
			{
				fprintf(fil, "\t\t{\n");
				fprintf(fil, "\t\t\tchar slask[200];\n");
				fprintf(fil, "\t\t\tsprintf_s(slask, 200, \"%s %s='%%f'\",this -> %s);\n",slask,f -> sqlname.c_str()/*column*/,f -> column);
				fprintf(fil, "\t\t\tsql += slask;\n");
				fprintf(fil, "\t\t}\n");
			}
			else
			if (!strcmp(f -> ctype,"long"))
			{
				std::string l = (f -> bitsize >= 32) ? "l" : "";
				char c = f -> uns ? 'u' : 'd';
				// %! bigint
				if (f -> bitsize == 64)
					l = "ll";
				fprintf(fil, "\t\t{\n");
				fprintf(fil, "\t\t\tchar slask[200];\n");
				fprintf(fil, "\t\t\tsprintf_s(slask, 200, \"%s %s='%%%s%c'\",this -> %s);\n",slask,f -> sqlname.c_str()/*column*/,l.c_str(),c,f -> column);
				fprintf(fil, "\t\t\tsql += slask;\n");
				fprintf(fil, "\t\t}\n");
			}
			else
			{
				if(!strcmp(f->ctype, "bool"))
				{
					fprintf(fil, "\t\tsql += \"%s %s='\" + q.GetDatabase().safestr(this -> %s ? \"true\" : \"false\") + \"'\";\n",
						slask,f -> sqlname.c_str()/*column*/,f -> column);
				}
				else
				{
					fprintf(fil, "\t\tsql += \"%s %s='\" + q.GetDatabase().safestr(this -> %s) + \"'\";\n",
						slask,f -> sqlname.c_str()/*column*/,f -> column);
				}
			}
			strcpy(slask," and");
		}
		fprintf(fil, "\t\tq.execute(sql);\n");
		fprintf(fil, "\t}\n");
		fprintf(fil, "}\n\n");
	} // if (table -> primary)

	// void Class::xml()
	if (!use_wrapped)
	{
	fprintf(fil, "std::string %s::xml()\n{\n",classname);
	if (use_wrapped)
		fprintf(fil, "\tQuery q(*database);\n");
	else
		fprintf(fil, "\tQuery q(*database);\n");
	fprintf(fil, "\tstd::string dest;\n");
	{
		bool need_slask = false;
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end() && !need_slask; it++)
		{
			FIELDS *t = *it;
			if (!strcmp(t -> ctype,"float") || !strcmp(t -> ctype,"long"))
				need_slask = true;
		}
		if (need_slask)
			fprintf(fil, "\tchar slask[200];\n");
	}

	strcpy(slask,classname);
	strupr(slask);
	fprintf(fil, "\tdest = \"<%s>\";\n",slask);

	{
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			strcpy(slask,t -> column);
			strupr(slask);
			// long, float, char, text, mediumtext
			if (t -> enum_set)
			{
				fprintf(fil, "\tdest += \"<%s>\" + q.GetDatabase().xmlsafestr(this -> %s ? \"true\" : \"false\")) + \"</%s>\";\n",slask,t -> column,slask);
			}
			else
			if (!strcmp(t -> ctype,"float"))
			{
				fprintf(fil, "\tsprintf_s(slask, 200,\"<%s>%%f</%s>\",this -> %s);\n",slask,slask,t -> column);
				fprintf(fil, "\tdest += slask;\n");
			}
			else
			if (!strcmp(t -> ctype,"long"))
			{
				std::string l = (t -> bitsize >= 32) ? "l" : "";
				char c = t -> uns ? 'u' : 'd';
				// %! bigint
				if (t -> bitsize == 64)
					l = "ll";
				fprintf(fil, "\tsprintf_s(slask, 200, \"<%s>%%%s%c</%s>\",this -> %s);\n",slask,l.c_str(),c,slask,t -> column);
				fprintf(fil, "\tdest += slask;\n");
			}
			else
			if (!strcmp(t -> ctype,"text") && !use_stl)
			{
				fprintf(fil, "\tdest += \"<%s>\" + (this -> %s ? q.GetDatabase().xmlsafestr(this -> %s) : \"\") + \"</%s>\";\n",slask,t -> column,t -> column,slask);
			}
			else
			if (!strcmp(t -> ctype,"text"))
			{
				fprintf(fil, "\tdest += \"<%s>\" + q.GetDatabase().xmlsafestr(this -> %s) + \"</%s>\";\n",slask,t -> column,slask);
			}
			else
			if (!strcmp(t -> ctype,"bool"))
			{
				fprintf(fil, "\tdest += \"<%s>\" + q.GetDatabase().xmlsafestr(this -> %s ? \"true\" : \"false\") + \"</%s>\";\n",slask,t -> column,slask);
			}
			else
			if (!strcmp(t -> ctype,"mediumtext"))
			{
				// vector<string>
			}
			else
			if (*t -> ctype)
			{
				fprintf(fil, "\tdest += \"<%s>\" + q.GetDatabase().xmlsafestr(this -> %s) + \"</%s>\";\n",slask,t -> column,slask);
			}
		}
	}
	strcpy(slask,classname);
	strupr(slask);
	fprintf(fil, "\tdest += \"</%s>\";\n",slask);

	fprintf(fil, "\treturn dest;\n");
	fprintf(fil, "}\n\n");
	} // use_wrapped

	// void Class::xml()
	if (!use_wrapped)
	{
	fprintf(fil, "std::string %s::xml(const std::string& tag,const std::string& xvalx)\n{\n",classname);
	if (use_wrapped)
		fprintf(fil, "\tQuery q(*database);\n");
	else
		fprintf(fil, "\tQuery q(*database);\n");
	fprintf(fil, "\tstd::string dest;\n");
	{
		bool need_slask = false;
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end() && !need_slask; it++)
		{
			FIELDS *t = *it;
			if (!strcmp(t -> ctype,"float") || !strcmp(t -> ctype,"long"))
				need_slask = true;
		}
		if (need_slask)
			fprintf(fil, "\tchar slask[200];\n");
	}

	strcpy(slask,classname);
	strupr(slask);
	fprintf(fil, "\tdest = \"<%s \" + tag + \"=\\\"\" + xvalx + \"\\\">\";\n",slask);

	{
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			strcpy(slask,t -> column);
			strupr(slask);
			// long, float, char, text, mediumtext
			if (t -> enum_set)
			{
				fprintf(fil, "\tdest += \"<%s>\" + q.GetDatabase().xmlsafestr(this -> %s.String()) + \"</%s>\";\n",slask,t -> column,slask);
			}
			else
			if (!strcmp(t -> ctype,"float"))
			{
				fprintf(fil, "\tsprintf_s(slask, 200,\"<%s>%%f</%s>\",this -> %s);\n",slask,slask,t -> column);
				fprintf(fil, "\tdest += slask;\n");
			}
			else
			if (!strcmp(t -> ctype,"long"))
			{
				std::string l = (t -> bitsize >= 32) ? "l" : "";
				char c = t -> uns ? 'u' : 'd';
				// %! bigint
				if (t -> bitsize == 64)
					l = "ll";
				fprintf(fil, "\tsprintf_s(slask, 200, \"<%s>%%%s%c</%s>\",this -> %s);\n",slask,l.c_str(),c,slask,t -> column);
				fprintf(fil, "\tdest += slask;\n");
			}
			else
			if (!strcmp(t -> ctype,"text") && !use_stl)
			{
				fprintf(fil, "\tdest += \"<%s>\" + (this -> %s ? q.GetDatabase().xmlsafestr(this -> %s) : \"\") + \"</%s>\";\n",slask,t -> column,t -> column,slask);
			}
			else
			if (!strcmp(t -> ctype,"text"))
			{
				fprintf(fil, "\tdest += \"<%s>\" + q.GetDatabase().xmlsafestr(this -> %s) + \"</%s>\";\n",slask,t -> column,slask);
			}
			else
			if (!strcmp(t -> ctype,"bool"))
			{
				fprintf(fil, "\tdest += \"<%s>\" + q.GetDatabase().xmlsafestr(this -> %s ? \"true\" : \"false\") + \"</%s>\";\n",slask,t -> column,slask);
			}
			else
			if (!strcmp(t -> ctype,"mediumtext"))
			{
				// vector<string>
			}
			else
			if (*t -> ctype)
			{
				fprintf(fil, "\tdest += \"<%s>\" + q.GetDatabase().xmlsafestr(this -> %s) + \"</%s>\";\n",slask,t -> column,slask);
			}
		}
	}
	strcpy(slask,classname);
	strupr(slask);
	fprintf(fil, "\tdest += \"</%s>\";\n",slask);

	fprintf(fil, "\treturn dest;\n");
	fprintf(fil, "}\n\n");
	} // use_wrapped

	// int Class::num_cols()
	int num_cols = table -> fieldsbase.size();
	fprintf(fil, "size_t %s::num_cols()\n",classname);
	fprintf(fil, "{\n");
	fprintf(fil, "\treturn %d;\n",num_cols); //listlen(table -> fieldsbase));
	fprintf(fil, "}\n\n");


	// private

	// clear fields
	fprintf(fil, "void %s::clear()\n{\n",classname);
	{
		// setup map's for enum_t/set_t
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			if (t -> set)
			{
				uint64_t val = 1;
				for (std::vector<std::string>::iterator it = t -> mvec.begin(); it != t -> mvec.end(); it++)
				{
					std::string txt = *it;
					fprintf(fil, "\tmmap_%s[\"%s\"] = %lld;\n",t -> column,txt.c_str(),val);
					val = val << 1;
				}
			}
			else
			if (t -> enum_set) // enum
			{
				uint64_t val = 1;
				for (std::vector<std::string>::iterator it = t -> mvec.begin(); it != t -> mvec.end(); it++)
				{
					std::string txt = *it;
					fprintf(fil, "\tmmap_%s[\"%s\"] = %lld;\n",t -> column,txt.c_str(),val);
					val++;
				}
			}
		}
	}
	for (auto it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
	{
		FIELDS *t = *it;
		// long, float, char, text, mediumtext
		if (!strcmp(t -> ctype,"float"))
		{
			fprintf(fil, "\tthis -> %s = 0;\n",t -> column);
		}
		else
		if (!strcmp(t -> ctype,"long"))
		{
			fprintf(fil, "\tthis -> %s = 0;\n",t -> column);
		}
		else
		if (!strcmp(t -> ctype,"text"))
		{
			if (use_stl)
			{
				fprintf(fil, "\tthis -> %s = \"\";\n",t -> column);
			}
			else
			{
				fprintf(fil, "\tthis -> %s = NULL;\n",t -> column);
			}
		}
		else
		if (!strcmp(t -> ctype,"mediumtext"))
		{
			// a vector<string> is created empty
			fprintf(fil, "\twhile (this -> %s.size())\n", t -> column);
			fprintf(fil, "\t{\n");
			fprintf(fil, "\t\tstd::vector<std::string>::iterator it = this -> %s.begin();\n", t -> column);
			fprintf(fil, "\t\tthis -> %s.erase(it);\n", t -> column);
			fprintf(fil, "\t}\n");
		}
		else
		if (*t -> ctype)
		{
			if (use_stl)
			{
				fprintf(fil, "\tthis -> %s = \"\";\n",t -> column);
			}
			else
			{
				fprintf(fil, "\t*this -> %s = 0;\n",t -> column);
				if (t -> length == 1) // char
				{
					fprintf(fil, "\tthis -> %s[1] = 0;\n",t -> column);
				}
			}
		}
		else
		{
			fprintf(fil, "\t// %d - %s %s\n",t -> num,t -> column,t -> typ);
		}

		// more dbdesigner comment special
		if (*t -> comment && (ptrtotable = strstr(t -> comment,"ptrtotable: ")) != NULL)
		{
			strcpy(slask,ptrtotable + 12);	// ptrtoclass
			strlwr(slask);
			fprintf(fil, "\t_%s = NULL;\n",t -> column);
		}
	}

	fprintf(fil, "}\n\n");

	// spawn from query
	fprintf(fil, "void %s::spawn(const std::string& sql)\n{\n",classname);
	if (use_wrapped)
		fprintf(fil, "\tQuery q(*database);\n");
	else
		fprintf(fil, "\tQuery q(*database);\n");
	fprintf(fil, "\tstd::string temp;\n");
	fprintf(fil, "\n");
	fprintf(fil, "\tclear();\n");
	fprintf(fil, "\n");

	fprintf(fil, "\tif (!strncasecmp(sql.c_str(),\"select * \",9))\n");
	fprintf(fil, "\t{\n");
	fprintf(fil, "\t\ttemp = \"");
	strcpy(slask,"select ");
	{
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			fprintf(fil, "%s%s",slask,t -> sqlname.c_str()/*column*/);
			strcpy(slask,",");
		}
	}
	fprintf(fil, " \" + sql.substr(9);\n");
	fprintf(fil, "\t} else\n");
	fprintf(fil, "\t\ttemp = sql;\n");

	fprintf(fil, "\tq.get_result(temp);\n");
	fprintf(fil, "\tif (q.fetch_row())\n");
	fprintf(fil, "\t{\n");
	{
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			int i = 0;
			if (!strcmp(t -> ctype,"float"))
			{
				fprintf(fil, "\t\tthis -> %s = (float)q.getnum(%d);",t -> column,t -> num);
				sprintf(slask,"%d",t -> num);
				i = strlen(t -> column) + strlen(slask);
			}
			else if (!strcmp(t -> ctype,"bool"))
			{
				fprintf(fil, "\t\tthis -> %s = q.getnum(%d) > 0;",t -> column,t -> num);
				sprintf(slask,"%d",t -> num);
				i = strlen(t -> column) + strlen(slask);
			}
			else if (!strcmp(t -> ctype,"long") && t -> bitsize == 64)
			{
				// %! bigint
				if (t -> uns)
				{
					fprintf(fil, "\t\tthis -> %s = q.getubigint(%d);",t -> column,t -> num);
					sprintf(slask,"%d",t -> num);
					i = strlen(t -> column) + strlen(slask) + 4;
				}
				else
				{
					fprintf(fil, "\t\tthis -> %s = q.getbigint(%d);",t -> column,t -> num);
					sprintf(slask,"%d",t -> num);
					i = strlen(t -> column) + strlen(slask) + 3;
				}
			}
			else if (!strcmp(t -> ctype,"long"))
			{
				if (t -> uns)
				{
					fprintf(fil, "\t\tthis -> %s = q.getuval(%d);",t -> column,t -> num);
					sprintf(slask,"%d",t -> num);
					i = strlen(t -> column) + strlen(slask) + 1;
				}
				else
				{
					fprintf(fil, "\t\tthis -> %s = q.getval(%d);",t -> column,t -> num);
					sprintf(slask,"%d",t -> num);
					i = strlen(t -> column) + strlen(slask);
				}
			}
			else if (!strcmp(t -> ctype,"text"))
			{
				if (use_stl)
				{
					fprintf(fil, "\t\tthis -> %s = q.getstr(%d);",t -> column,t -> num);
					sprintf(slask,"%d",t -> num);
					i = strlen(t -> column) + strlen(slask);
				}
				else
				{
					fprintf(fil, "\t\tthis -> %s = new char[strlen(q.getstr(%d)) + 1000];\n",t -> column,t -> num);
					fprintf(fil, "\t\tstrcpy(this -> %s,q.getstr(%d));",t -> column,t -> num);
					sprintf(slask,"%d",t -> num);
					i = strlen(t -> column) + strlen(slask) + 6;
				}
			}
			else if (!strcmp(t -> ctype,"mediumtext"))
			{
				fprintf(fil, "\t\t{\n");
	//			fprintf(fil, "\t\t\tsize_t x = 0;\n");
				fprintf(fil, "\t\t\tstd::string s = q.getstr(%d);\n",t -> num);
				fprintf(fil, "\t\t\tstd::string tmp;\n"); //char tmp[10000];\n");
				fprintf(fil, "\t\t\tfor (size_t i = 0; i < s.size(); i++)\n");
				fprintf(fil, "\t\t\t{\n");
				fprintf(fil, "\t\t\t\tif (s[i] == '\\n')\n");
				fprintf(fil, "\t\t\t\t{\n");
	//			fprintf(fil, "\t\t\t\t\ttmp[x] = 0;\n");
				fprintf(fil, "\t\t\t\t\tthis -> %s.push_back(tmp);\n",t -> column);
				fprintf(fil, "\t\t\t\t\ttmp = \"\";\n");
				fprintf(fil, "\t\t\t\t}\n");
				fprintf(fil, "\t\t\t\telse\n");
				fprintf(fil, "\t\t\t\t{\n");
				fprintf(fil, "\t\t\t\t\ttmp += s[i];\n");
				fprintf(fil, "\t\t\t\t}\n");
				fprintf(fil, "\t\t\t}\n");
				fprintf(fil, "\t\t\tif (tmp.size())\n");
				fprintf(fil, "\t\t\t\tthis -> %s.push_back(tmp);\n",t -> column);
				fprintf(fil, "\t\t}\n");
				//
				i = 0;
			}
			else if (*t -> ctype)
			{
				if (use_stl)
				{
					fprintf(fil, "\t\tthis -> %s = q.getstr(%d);",t -> column,t -> num);
					sprintf(slask,"%d",t -> num);
					i = strlen(t -> column) + strlen(slask);
				}
				else
				{
					fprintf(fil, "\t\tstrcpy(this -> %s,q.getstr(%d));",t -> column,t -> num);
					sprintf(slask,"%d",t -> num);
					i = strlen(t -> column) + strlen(slask) + 6;
				}
			}
			else
			{
				fprintf(fil, "\t\t// %d - %s %s\n",t -> num,t -> column,t -> typ);
				i = 0;
			}
			if (i)
			{
				strcpy(slask,"\t\t\t");
				slask[40 - i] = 0;
				fprintf(fil, "%s// %d - %s %s\n",slask,t -> num,t -> column,t -> typ);
			}
		}
	}
	fprintf(fil, "\t\tnew_object = 0;\n");
	fprintf(fil, "\t} else\n");
	fprintf(fil, "\t\tclear();\n");
	fprintf(fil, "\tq.free_result();\n");

	fprintf(fil, "}\n\n");

	// spawn from query
	if (use_wrapped)
		fprintf(fil, "void %s::spawn(Wrapped::IQuery *qd,int offset)\n",classname);
	else
		fprintf(fil, "void %s::spawn(Query *qd,int offset)\n",classname);
	fprintf(fil, "{\n");
	fprintf(fil, "\tclear();\n");
	fprintf(fil, "\n");
	{
		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			int i = 0;
			if (!strcmp(t -> ctype,"float"))
			{
				fprintf(fil, "\tthis -> %s = (float)qd->getnum(%d + offset);",t -> column,t -> num);
				sprintf(slask,"%d",t -> num);
				i = strlen(t -> column) + strlen(slask);
			}
			else if (!strcmp(t -> ctype,"bool"))
			{
				fprintf(fil, "\tthis -> %s = qd->getnum(%d + offset) > 0;",t -> column,t -> num);
				sprintf(slask,"%d",t -> num);
				i = strlen(t -> column) + strlen(slask);
			}
			else if (!strcmp(t -> ctype,"long") && t -> bitsize == 64)
			{
				// %! bigint
				if (t -> uns)
				{
					fprintf(fil, "\tthis -> %s = qd->getubigint(%d + offset);",t -> column,t -> num);
					sprintf(slask,"%d",t -> num);
					i = strlen(t -> column) + strlen(slask) + 4;
				}
				else
				{
					fprintf(fil, "\tthis -> %s = qd->getbigint(%d + offset);",t -> column,t -> num);
					sprintf(slask,"%d",t -> num);
					i = strlen(t -> column) + strlen(slask) + 3;
				}
			}
			else if (!strcmp(t -> ctype,"long"))
			{
				if (t -> uns)
				{
					fprintf(fil, "\tthis -> %s = qd->getuval(%d + offset);",t -> column,t -> num);
					sprintf(slask,"%d",t -> num);
					i = strlen(t -> column) + strlen(slask) + 1;
				}
				else
				{
					fprintf(fil, "\tthis -> %s = qd->getval(%d + offset);",t -> column,t -> num);
					sprintf(slask,"%d",t -> num);
					i = strlen(t -> column) + strlen(slask);
				}
			}
			else if (!strcmp(t -> ctype,"text"))
			{
				if (use_stl)
				{
					fprintf(fil, "\tthis -> %s = qd->getstr(%d + offset);",t -> column,t -> num);
					sprintf(slask,"%d",t -> num);
					i = strlen(t -> column) + strlen(slask);
				}
				else
				{
					fprintf(fil, "\tthis -> %s = new char[strlen(qd -> getstr(%d + offset)) + 1000];\n",t -> column,t -> num);
					fprintf(fil, "\tstrcpy(this -> %s,qd->getstr(%d));",t -> column,t -> num);
					sprintf(slask,"%d",t -> num);
					i = strlen(t -> column) + strlen(slask) + 6;
				}
			}
			else if (!strcmp(t -> ctype,"mediumtext"))
			{
				fprintf(fil, "\t{\n");
				fprintf(fil, "\t\tstd::string s = qd->getstr(%d + offset);\n",t -> num);
				fprintf(fil, "\t\tstd::string tmp;\n"); //char tmp[10000];\n");
				fprintf(fil, "\t\tfor (size_t i = 0; i < s.size(); i++)\n");
				fprintf(fil, "\t\t{\n");
				fprintf(fil, "\t\t\tif (s[i] == '\\n')\n");
				fprintf(fil, "\t\t\t{\n");
				fprintf(fil, "\t\t\t\tthis -> %s.push_back(tmp);\n",t -> column);
				fprintf(fil, "\t\t\t\ttmp = \"\";\n");
				fprintf(fil, "\t\t\t}\n");
				fprintf(fil, "\t\t\telse\n");
				fprintf(fil, "\t\t\t{\n");
				fprintf(fil, "\t\t\t\ttmp += s[i];\n");
				fprintf(fil, "\t\t\t}\n");
				fprintf(fil, "\t\t}\n");
				fprintf(fil, "\t\tif (tmp.size())\n");
				fprintf(fil, "\t\t\tthis -> %s.push_back(tmp);\n",t -> column);
				fprintf(fil, "\t}\n");
				//
				i = 0;
			}
			else if (*t -> ctype)
			{
				if (use_stl)
				{
					fprintf(fil, "\tthis -> %s = qd -> getstr(%d + offset);",t -> column,t -> num);
					sprintf(slask,"%d",t -> num);
					i = strlen(t -> column) + strlen(slask);
				}
				else
				{
					fprintf(fil, "\tstrcpy(this -> %s,qd -> getstr(%d + offset));",t -> column,t -> num);
					sprintf(slask,"%d",t -> num);
					i = strlen(t -> column) + strlen(slask) + 6;
				}
			}
			else
			{
				fprintf(fil, "\t// %d - %s %s\n",t -> num,t -> column,t -> typ);
				i = 0;
			}
			if (i)
			{
				strcpy(slask,"\t\t\t");
				slask[40 - i] = 0;
				fprintf(fil, "%s// %d - %s %s\n",slask,t -> num,t -> column,t -> typ);
			}
		}
	}
	fprintf(fil, "}\n\n");

/*
 * Create container class (see container.h for an example)
 */

	if (container)
	{
		fprintf(fil, "\n");
		fprintf(fil, "/**\n **  Begin class '%s'\n **/\n",cclassname);
		fprintf(fil, "\n");

		// constructor cclassname(db) - empty list
		fprintf(fil, "%s::%s(Database *db,ListType lt)\n{\n",cclassname,cclassname);
		fprintf(fil, "\tdatabase = db;\n");
		fprintf(fil, "\tbase = NULL;\n");
		fprintf(fil, "\t%ss = NULL;\n",lwrclassname);
		fprintf(fil, "\tqty = 0;\n");
		fprintf(fil, "\t_lt = lt;\n");
		fprintf(fil, "}\n");
		fprintf(fil, "\n");

		// constructor cclassname(db,sql)
		fprintf(fil, "%s::%s(Database *db,char *sql,ListType lt)\n{\n",cclassname,cclassname);
		fprintf(fil, "\tQuery q(*db);\n");
		fprintf(fil, "\t%s *item,*p;\n",cstruct);
		fprintf(fil, "\tlong l;\n");
		fprintf(fil, "\n");
		fprintf(fil, "\tdatabase = db;\n");
		fprintf(fil, "\tbase = NULL;\n");
		fprintf(fil, "\t%ss = NULL;\n",lwrclassname);
		fprintf(fil, "\tqty = 0;\n");
		fprintf(fil, "\t_lt = lt;\n");
		fprintf(fil, "\n");
		fprintf(fil, "\tq.get_result(sql);\n");
		fprintf(fil, "\twhile (q.fetch_row())\n");
		fprintf(fil, "\t{\n");
		fprintf(fil, "\t\titem = new %s;\n",cstruct);
		fprintf(fil, "\t\titem -> next = NULL;\n");
		fprintf(fil, "\t\titem -> %s = new %s(db,&q);\n",lwrclassname,classname);
		fprintf(fil, "\t\tif (!base)\n");
		fprintf(fil, "\t\t\tbase = item;\n");
		fprintf(fil, "\t\telse\n");
		fprintf(fil, "\t\t{\n");
		fprintf(fil, "\t\t\tp = base;\n");
		fprintf(fil, "\t\t\twhile (p -> next)\n");
		fprintf(fil, "\t\t\t\tp = p -> next;\n");
		fprintf(fil, "\t\t\tp -> next = item;\n");
		fprintf(fil, "\t\t}\n");
		fprintf(fil, "\t\tqty++;\n");
		fprintf(fil, "\t}\n");
		fprintf(fil, "\tq.free_result();\n");
		fprintf(fil, "\n");
		fprintf(fil, "\t%ss = new %s *[qty];\n",lwrclassname,classname);
		fprintf(fil, "\tl = 0;\n");
		fprintf(fil, "\tfor (p = base; p; p = p -> next)\n");
		fprintf(fil, "\t\t%ss[l++] = p -> %s;\n",lwrclassname,lwrclassname);
		fprintf(fil, "}\n");
		fprintf(fil, "\n");

		// destructor
		fprintf(fil, "%s::~%s()\n{\n",cclassname,cclassname);
		fprintf(fil, "\t%s *q,*tmp;\n",cstruct);
		fprintf(fil, "\tfor (q = base; q; q = tmp)\n");
		fprintf(fil, "\t{\n");
		fprintf(fil, "\t\tif (_lt == Direct)\n");
		fprintf(fil, "\t\t\tdelete q -> %s;\n",lwrclassname);
		fprintf(fil, "\t\ttmp = q -> next;\n");
		fprintf(fil, "\t\tdelete q;\n");
		fprintf(fil, "\t}\n");
		fprintf(fil, "\tdelete %ss;\n",lwrclassname);
		fprintf(fil, "}\n");
		fprintf(fil, "\n");

		// save()
		fprintf(fil, "void %s::save()\n{\n",cclassname);
		fprintf(fil, "\t%s *q;\n",cstruct);
		fprintf(fil, "\tfor (q = base; q; q = q -> next)\n");
		fprintf(fil, "\t\tq -> %s -> save();\n",lwrclassname);
		fprintf(fil, "}\n");
		fprintf(fil, "\n");

		// baseptr()
		fprintf(fil, "%s *%s::baseptr()\n{\n",cstruct,cclassname);
		fprintf(fil, "\treturn base;\n");
		fprintf(fil, "}\n");
		fprintf(fil, "\n");

		// get...()
		fprintf(fil, "%s *%s::get%s(long ix)\n{\n",classname,cclassname,lwrclassname);
		fprintf(fil, "\treturn %ss[ix];\n",lwrclassname);
		fprintf(fil, "}\n");
		fprintf(fil, "\n");

		// add(classname *)
		fprintf(fil, "void %s::add(%s *ix)\n{\n",cclassname,classname);
		fprintf(fil, "\t%s *p,*item = new %s;\n",cstruct,cstruct);
		fprintf(fil, "\tlong l;\n");
		fprintf(fil, "\n");
		fprintf(fil, "\titem -> next = NULL;\n");
		fprintf(fil, "\titem -> %s = ix;\n",lwrclassname);
		fprintf(fil, "\tif (!base)\n");
		fprintf(fil, "\t\tbase = item;\n");
		fprintf(fil, "\telse\n");
		fprintf(fil, "\t{\n");
		fprintf(fil, "\t\tp = base;\n");
		fprintf(fil, "\t\twhile (p -> next)\n");
		fprintf(fil, "\t\t\tp = p -> next;\n");
		fprintf(fil, "\t\tp -> next = item;\n");
		fprintf(fil, "\t}\n");
		fprintf(fil, "\tqty++;\n");
		fprintf(fil, "\tdelete %ss;\n",lwrclassname);

		fprintf(fil, "\t%ss = new %s *[qty];\n",lwrclassname,classname);
		fprintf(fil, "\tl = 0;\n");
		fprintf(fil, "\tfor (p = base; p; p = p -> next)\n");
		fprintf(fil, "\t\t%ss[l++] = p -> %s;\n",lwrclassname,lwrclassname);

		fprintf(fil, "}\n");
		fprintf(fil, "\n");

		// remove(classname *)
		fprintf(fil, "void %s::remove(%s *ix)\n",cclassname,classname);
		fprintf(fil, "{\n");
		fprintf(fil, "\t%s *p,*q = NULL;\n",cstruct);
		fprintf(fil, "\tlong l;\n");
		fprintf(fil, "\tfor (p = base; p; p = p -> next)\n");
		fprintf(fil, "\t\tif (p -> %s == ix)\n",lwrclassname);
		fprintf(fil, "\t\t{\n");
		fprintf(fil, "\t\t\tif (q)\n");
		fprintf(fil, "\t\t\t\tq -> next = p -> next;\n");
		fprintf(fil, "\t\t\telse\n");
		fprintf(fil, "\t\t\t\tbase = p -> next;\n");
		fprintf(fil, "\t\t\tbreak;\n");
		fprintf(fil, "\t\t} else\n");
		fprintf(fil, "\t\t\tq = p;\n");
		fprintf(fil, "\tif (p)\n");
		fprintf(fil, "\t{\n");
		fprintf(fil, "\t\tdelete p;\n");
		fprintf(fil, "\t\tqty--;\n");

		fprintf(fil, "\t\tdelete %ss;\n",lwrclassname);

		fprintf(fil, "\t\t%ss = new %s *[qty];\n",lwrclassname,classname);
		fprintf(fil, "\t\tl = 0;\n");
		fprintf(fil, "\t\tfor (p = base; p; p = p -> next)\n");
		fprintf(fil, "\t\t\t%ss[l++] = p -> %s;\n",lwrclassname,lwrclassname);

		fprintf(fil, "\t}\n");
		fprintf(fil, "}\n");
		fprintf(fil, "\n");

		// find_...()
		fprintf(fil, "// find_xxx()\n\n");

		for (fields_l::iterator it = table -> fieldsbase.begin(); it != table -> fieldsbase.end(); it++)
		{
			FIELDS *t = *it;
			strcpy(slask,typestring(t));
			if (0&&*slask)
			{
				fprintf(fil, "%s *%s::find_%s(%six)\n{\n",classname,cclassname,t -> column,slask);
				fprintf(fil, "\t%s *q;\n",cstruct);
				fprintf(fil, "\tfor (q = base; q; q = q -> next)\n");
				if (!strcmp(t -> ctype,"long") || !strcmp(t -> ctype,"float"))
				{
					fprintf(fil, "\t\tif (q -> %s -> %s == ix)\n",lwrclassname,t -> column);
				}
				else
				{
					fprintf(fil, "\t\tif (!strcasecmp(q -> %s -> %s,ix))\n",lwrclassname,t -> column);
				}
				fprintf(fil, "\t\t\tbreak;\n");
				fprintf(fil, "\treturn q ? q -> %s : NULL;\n",lwrclassname);
				fprintf(fil, "}\n");
				fprintf(fil, "\n");
			}
		}

		// private

		fprintf(fil, "// End of implementation of class '%s'\n",cclassname);
		fprintf(fil, "\n");
	} // if (container)

} // generate_libc


