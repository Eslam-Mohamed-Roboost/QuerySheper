-- Example SELECT query for testing
-- This query will work across multiple database types

SELECT 
    1 as Id,
    'Test Query' as Description,
    GETDATE() as CurrentTime,
    'QuerySheper' as Application;

-- Additional test data
SELECT 
    'Database' as Category,
    'Multi-DB Query' as Feature,
    @@VERSION as DatabaseVersion;
