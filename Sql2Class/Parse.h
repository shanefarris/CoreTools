class Parse
{
public:
	Parse();
	Parse(char *);
	Parse(char *,char *);
	~Parse();
	short issplit(char);
	void getsplit(void);
	void getsplit(char *);
	void getword(void);
	void getword(char *);
	void getword(char *,char *,int);
	void getrest(char *);
	long getvalue(void);
	void setbreak(char);
	int getwordlen(void);
	int getrestlen(void);
	void enablebreak(char c) {
		pa_enable = c;
	}
	void disablebreak(char c) {
		pa_disable = c;
	}

private:
	char *pa_the_str;
	char *pa_splits;
	char *pa_ord;
	int   pa_the_ptr;
	char  pa_breakchar;
	char  pa_enable;
	char  pa_disable;
};

