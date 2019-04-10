#ifdef _WIN32
#pragma warning(disable:4786)
#endif

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "sql2class.h"

#include "utils.h"


#ifndef _WIN32
void strlwr(char *s)
{
	int i;

	for (i = 0; i < (int)strlen(s); i++)
		if (s[i] > 64 && s[i] <= 'Z')
			s[i] |= 32;
}

void strupr(char *s)
{
	int i;

	for (i = 0; i < (int)strlen(s); i++)
		if (s[i] > 96 && s[i] <= 'z')
			s[i] -= 32;
}
#endif

char *typestring(FIELDS *t)
{
static	char slask[200];

	if (!strcmp(t -> ctype,"float"))
		sprintf(slask,"float ");
	else
	if (!strcmp(t -> ctype,"long"))
		sprintf(slask,"long ");
	else
	if (!strcmp(t -> ctype,"text"))
		sprintf(slask,"char *");
	else
	if (*t -> ctype)
		sprintf(slask,"char *");
	else
		*slask = 0;

	return slask;
}

