-- Count users across different databases
-- This query demonstrates cross-database data comparison

SELECT 
    COUNT(*) as TotalUsers,
    'Active' as Status
FROM Users 
WHERE IsActive = 1 
    AND CreatedDate > '2024-01-01';

-- Alternative query for databases without Users table
SELECT 
    0 as TotalUsers,
    'No Users Table' as Status
WHERE NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users');
