-- Database information query
-- Shows database version, name, and current time

SELECT 
    DB_NAME() as DatabaseName,
    @@VERSION as Version,
    GETDATE() as CurrentTime,
    SERVERPROPERTY('ProductVersion') as ProductVersion,
    SERVERPROPERTY('Edition') as Edition;
