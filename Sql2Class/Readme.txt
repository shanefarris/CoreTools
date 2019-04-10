1. dump the data into a text file: 
sqlite3.exe test3.db .dump > test.txt

2. Use these arguments:
sql2class -lib CoreDal out_db.txt -sqlite -namespace Core -prefix .

Note: requires the "FileDatabase" to be built
Note: tends to have problems if the line is ended with only a CR and not a CRLF, might be from fget()