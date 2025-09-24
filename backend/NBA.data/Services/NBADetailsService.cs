using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NBA.common.Entities;

namespace NBA.data.Services;

public interface INbaDetailsService
{
    Task<NBADetails> RetrieveNBADetailsAsync(DateTime? start, DateTime? end, int? teamId, CancellationToken ct);
}
public class NBADetailsService : INbaDetailsService
{
    private readonly IDbContextFactory<NBAContext> _dbContextFactory;

    public NBADetailsService(IDbContextFactory<NBAContext> _dbContextFactory) => _dbContextFactory = _dbContextFactory;

    public async Task<NBADetails> RetrieveNBADetailsAsync(DateTime? start, DateTime? end, int? teamId, CancellationToken ct)
    {

        var teamsRowRecords = new List<TeamRow>();
        var teamGamesRowRecords = new List<TeamGameRow>();
        var rosterRowRecords = new List<RosterRow>();

        //Get connection with the DB
        await using var context = await _dbContextFactory.CreateDbContextAsync(ct);
        await context.Database.OpenConnectionAsync(ct);

        try
        {
            using var cmd = context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "dbo.usp_NBA_Details";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@StartDate", (object?)start ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@EndDate", (object?)end ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@TeamID", (object?)teamId ?? DBNull.Value));

            using var reader = await cmd.ExecuteReaderAsync();

            //Teams records
            teamsRowRecords = new List<TeamRow>();
            while (await reader.ReadAsync())
            {
                teamsRowRecords.Add(new TeamRow(
                    reader.GetInt32(0),                 // TeamID
                    reader.GetString(1),                // TeamName
                    reader.GetString(2),                // Stadium
                    reader.IsDBNull(3) ? null : reader.GetString(3), // Logo
                    reader.IsDBNull(4) ? null : reader.GetString(4)
                ));
            }

            //Games Records 
            await reader.NextResultAsync();

            teamGamesRowRecords = new List<TeamGameRow>();
            while (await reader.ReadAsync())
            {
                teamGamesRowRecords.Add(new TeamGameRow(
                     reader.GetInt32(0), // TeamID
                    reader.GetString(1), // TeamName
                    reader.GetString(2), // Stadium
                    reader.GetInt32(3), // GameID
                    reader.GetDateTime(4), // GameDateTime
                    reader.GetInt32(5), // TeamScore
                    reader.GetInt32(6),  // OppScore
                    reader.GetInt32(7), // IsHome
                    reader.GetInt32(8),// OppTeamID
                    reader.GetInt32(9),// HomeTeamID
                    reader.GetInt32(10), // AwayTeamID
                    reader.GetInt32(11),  // MVPPlayerID
                    reader.GetInt32(12),  // IsWin
                    reader.GetInt32(13), // IsLoss
                    reader.GetInt32(14) // Margin
                ));
            }

            await reader.NextResultAsync();
            rosterRowRecords = new List<RosterRow>();
            while (await reader.ReadAsync())
            {
                rosterRowRecords.Add(new RosterRow(
                    reader.GetInt32(0), // TeamID
                    reader.GetInt32(1),// PlayerID
                    reader.GetString(2)// PlayerName
                ));
            }

              return new NBADetails(teamsRowRecords, teamGamesRowRecords, rosterRowRecords);
        }
        catch (Exception ex)
        {
            //In production enviroment this would be a Logger dependency injected
            Console.Write("There was an issue with the DB Connection", ex.Message);
            return new NBADetails(teamsRowRecords, teamGamesRowRecords, rosterRowRecords);

        }
        finally
        {
            //resource optimisation
            await context.Database.CloseConnectionAsync();
        }
    }

}
