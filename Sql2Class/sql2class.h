#include <vector>
#include <string>
#include <list>

	typedef struct fieldsstruct
	{
		std::string GetCType(std::string& intyp,std::string& suffix);
		struct fieldsstruct *next;
		int num;
		char column[200];
		char typ[200];
		char ctype[200];
		int length;		// char length of ctype
		char comment[100];
		bool ignore;		// ignore field in insert/update
		bool uns;		// unsigned integer field
		int bitsize;
		bool enum_set;		// ENUM or SET type
		bool set;               // type is SET
		std::vector<std::string> mvec; // ENUM / SET values
		std::string sqlname;	// original sql column name
	} FIELDS;
	typedef std::vector<FIELDS *> fields_v;
	typedef std::list<FIELDS *> fields_l;

	struct INDEX
	{
		INDEX(const std::string& n,bool p,bool u) : name(n),primary(p),unique(u) {}
		std::string name;
		bool primary;
		bool unique;
		fields_v fields;
		bool ambig;
	};
	typedef std::vector<INDEX *> index_v;

	typedef struct tblsstruct
	{
		struct tblsstruct *next;
		std::string name;
		fields_l fieldsbase;
		index_v index;
		INDEX *primary;
		INDEX *unique;
	} TBLS;
	typedef std::vector<TBLS *> tbls_l;

extern	tbls_l tblsbase;
extern	std::string namespc;
extern	short container;
extern	std::string baseclass;
extern	bool mediumtext; // vector<string> needed
extern	bool use_stl;
extern	bool use_sqlite; // use sqlite3 wrapper, not mysql
extern	std::string package;
extern	bool license;
extern	std::string queryclass;
extern	std::string cmdline;
extern	bool use_odbc; // use odbc wrapper, not mysql
extern	bool make_getset;
extern	bool mangle_names;
extern	bool use_wrapped; // use sqlwrapped wrapper (mysql/sqlite3/odbc), not mysql
