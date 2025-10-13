using Microsoft.EntityFrameworkCore;
using QuerySheper.Models;

namespace QuerySheper.Application
{
    public interface ISimpleQueryService
    {
        Task<(object Data, int RowCount, TimeSpan ExecutionTime)> ExecuteQueryOnDatabase(DatabaseConfig db, string sql);
    }
}


