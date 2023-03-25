using Microsoft.EntityFrameworkCore;

namespace EFCore101
{
    partial class MyEmployeeConnectionContext : DbContext
    {
        public long GetNextSequenceValue(string genName)
        {
            using (var cmd = Database.GetDbConnection().CreateCommand())
            {
                Database.GetDbConnection().Open();
                cmd.CommandText = "SELECT gen_id(" + genName + ", 1) from rdb$database";
                var obj = cmd.ExecuteScalar();
                return (long)obj;
            }
        }
    }
}