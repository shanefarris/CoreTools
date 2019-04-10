-- Problem: Could not obtain exclusive lock on database 'model'
Use master 
GO
IF EXISTS(SELECT request_session_id  FROM sys.dm_tran_locks
WHERE resource_database_id = DB_ID('Model'))
PRINT 'Model Database being used by some other session'
ELSE
PRINT 'Model Database not used by other session'

SELECT request_session_id  FROM sys.dm_tran_locks
WHERE resource_database_id = DB_ID('Model')

--DBCC InputBuffer(57) 
--Kill 57