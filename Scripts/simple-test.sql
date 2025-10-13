-- Simple test query for local execution
-- This will work on both SQL Server and PostgreSQL

SELECT 
    'Hello QuerySheper!' as Message,
    42 as Answer,
    NOW() as CurrentTime;

-- Test data
SELECT 
    1 as Id,
    'Test' as Name,
    'Local Execution' as Type;
