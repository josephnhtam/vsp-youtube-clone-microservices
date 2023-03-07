using Infrastructure.EFCore.TransactionalEvents.Models;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Exceptions;
using System.Text;

namespace Infrastructure.EFCore.TransactionalEvents {
    public class NpgsqlTransactionalEventsCommandResolver : ITransactionalEventsCommandResolver {

        private static string? PoolEventsGroupCmd;
        private static string? UpsertEventsGroupCmd;

        public IQueryable<TransactionalEventsGroup> PollEventsGroups (DbContext context, int fetchCount) {
            string cmd;

            if (PoolEventsGroupCmd != null) {
                cmd = PoolEventsGroupCmd;
            } else {
                GetSchemaAndTableName<TransactionalEventsGroup>(context, out var schema, out var table);

                StringBuilder sb = new StringBuilder();

                if (string.IsNullOrEmpty(schema)) {
                    sb.Append($@"SELECT * FROM ""{table}"" ");
                } else {
                    sb.Append($@"SELECT * FROM ""{schema}"".""{table}"" ");
                }

                sb.Append(@"WHERE ""AvailableDate"" < {0} ");

                sb.Append(@"ORDER BY ""AvailableDate"" ASC LIMIT {1} ");

                sb.Append("FOR UPDATE SKIP LOCKED");

                cmd = sb.ToString();
                PoolEventsGroupCmd = cmd;
            }

            return context.Set<TransactionalEventsGroup>().FromSqlRaw(cmd, DateTimeOffset.UtcNow, fetchCount);
        }

        public async Task<long> UpsertEventsGroupAndGetLastSequenceNumber (DbContext context, string groupId, int eventsCount, DateTimeOffset currentTime, TimeSpan availableDelay) {
            string cmd;

            if (UpsertEventsGroupCmd != null) {
                cmd = UpsertEventsGroupCmd;
            } else {
                GetSchemaAndTableName<TransactionalEventsGroup>(context, out var schema, out var table);

                StringBuilder sb = new StringBuilder();

                string target;

                if (string.IsNullOrEmpty(schema)) {
                    target = $@"""{table}""";
                } else {
                    target = $@"""{schema}"".""{table}""";
                }

                sb.Append($@"INSERT INTO {target} ");

                sb.Append(@"(""Id"", ""CreateDate"", ""AvailableDate"", ""LastSequenceNumber"") VALUES ");

                sb.Append($@"(@groupId, @createDate, @availableDate, @eventsCount) ");

                sb.Append($@"ON CONFLICT (""Id"") DO UPDATE SET ");

                sb.Append($@"""LastSequenceNumber"" = {target}.""LastSequenceNumber"" + @eventsCount ");

                sb.Append($@"RETURNING ""LastSequenceNumber""");

                cmd = sb.ToString();
                UpsertEventsGroupCmd = cmd;
            }

            using (var dbCmd = context.Database.GetDbConnection().CreateCommand()) {
                dbCmd.CommandType = System.Data.CommandType.Text;
                dbCmd.CommandText = cmd;

                {
                    var param = dbCmd.CreateParameter();
                    param.ParameterName = "groupId";
                    param.Value = groupId;
                    dbCmd.Parameters.Add(param);
                }

                {
                    var param = dbCmd.CreateParameter();
                    param.ParameterName = "createDate";
                    param.Value = currentTime;
                    dbCmd.Parameters.Add(param);
                }

                {
                    var param = dbCmd.CreateParameter();
                    param.ParameterName = "availableDate";
                    param.Value = currentTime + availableDelay;
                    dbCmd.Parameters.Add(param);
                }

                {
                    var param = dbCmd.CreateParameter();
                    param.ParameterName = "eventsCount";
                    param.Value = eventsCount;
                    dbCmd.Parameters.Add(param);
                }

                using (var reader = await dbCmd.ExecuteReaderAsync()) {
                    if (await reader.ReadAsync()) {
                        return reader.GetInt64(0);
                    } else {
                        throw new TransientException();
                    }
                }
            }
        }
        private static void GetSchemaAndTableName<T> (DbContext context, out string? schema, out string? table) {
            var entityType = context.Model.FindEntityType(typeof(T));

            if (entityType == null) {
                throw new InvalidOperationException("Entity type TransactionalEventsGroup not found");
            }

            schema = entityType.GetSchema();
            table = entityType.GetTableName();
        }

    }
}
