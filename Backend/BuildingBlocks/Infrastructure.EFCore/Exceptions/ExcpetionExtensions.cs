using System.Data.Common;

namespace Infrastructure.EFCore.Exceptions {
    public static class ExcpetionExtensions {
        public static bool IsSqlState (this DbException ex, string sqlState) {
            return ex.SqlState?.StartsWith(sqlState) ?? false;
        }
    }
}
