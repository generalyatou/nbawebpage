using System;
using NBA.common.Entities;

namespace NBA.Business.Services;

public interface INbaDetailsMapping
{
    List<TeamSummaries> BuildSummariesMapping(NBADetails facts);
}

public class NBADetailsMapping : INbaDetailsMapping
{
    public List<TeamSummaries> BuildSummariesMapping(NBADetails facts)
    {
        var teamByTeamId = facts.Teams.ToDictionary(team => team.TeamID); //Key TeamID, Value TeamRowObj
        var rosterByTeamId = facts.Roster.GroupBy(r => r.TeamID).ToDictionary(g => g.Key, g => g.ToList()); //Key Team ID from Grouping, Value RosterRow that was grouped by the TeamID
        var playerNameById = facts.Roster.ToDictionary(r => r.PlayerID, r => r.PlayerName); //Key PlayerID, Value Player name

        //Following performs mapping. In another solution alot of the calculations would be done in the DB as it is more optimal in place as opposed to first retrieiving then calculating

        List<TeamSummaries> TeamSummaries = new List<TeamSummaries>();

        TeamSummaries = facts.Teams.Select(t =>
        {
            //gather the games and order them by date time so that the latest game can be taken easily. 

            var teamGames = facts.Games.Where(g => g.TeamID == t.TeamID).OrderBy(g => g.GameDateTime).ToList();


            //Figure out values for properties of the final table
            var played = teamGames.Count();
            var won = teamGames.Sum(g => g.IsWin);
            var lost = teamGames.Sum(g => g.IsLoss);
            var home = teamGames.Count(g => g.IsHome == 1);
            var away = teamGames.Count(g => g.IsHome == 0);

            var lastGame = teamGames.LastOrDefault();
            var lastStadium = lastGame is null ? null : teamByTeamId[lastGame.HomeTeamID].Stadium; //lastGame is only correct half the time since your last game could be at their stadium

            var biggestWinRecords = teamGames.OrderByDescending(g => g.Margin).FirstOrDefault();
            var biggestLossRow = teamGames.OrderBy(g => g.Margin).FirstOrDefault();



            //MVP = the person with the most MVP
            string? playerMvpName = null;
            if (teamGames.Count > 0 && facts.Roster.Any())
            {
                var rosterIdsWithoutDupe = facts.Roster.Select(r => r.PlayerID).ToHashSet();

                int? mvpWinnerByFreqAndRecent = teamGames
                .Select(g => g.MVPPlayerID) // 1) take MVP ids from the team's games
                .Where(rosterIdsWithoutDupe.Contains)            // 2) keep only MVPs who are on this team's roster as the roster will have all teams players. Then when checking the contains you are looking for the most freq and recent mvp that is in that roster
                .GroupBy(id => id)                    // 3) group MVPs by player
                .OrderByDescending(g => g.Count())    //    most MVPs wins
                .ThenByDescending(g =>                //    tie-break: most recent MVP date just incase
                    teamGames.Where(x => x.MVPPlayerID == g.Key).Max(x => x.GameDateTime))
                .ThenBy(g => g.Key)                   //    final stable tie-break i.e. if still get two same place then just order it by a default ordering using player id and always choose that once
                .Select(g => (int?)g.Key)
                .FirstOrDefault(); //return the first player on the list with playerid.

                playerMvpName = mvpWinnerByFreqAndRecent is not null ? facts.Roster.Where(x => x.PlayerID == mvpWinnerByFreqAndRecent)?.FirstOrDefault()?.PlayerName : "";
            }

            return new TeamSummaries(
                TeamName: t.TeamName,
                Stadium: t.Stadium,
                Logo: t.Logo,
                MVP: playerMvpName,
                Played: played,
                Won: won,
                Lost: lost,
                PlayedHome: home,
                PlayedAway: away,
                BiggestWin: biggestWinRecords is null ? "-" : (biggestWinRecords.TeamScore - biggestWinRecords.OppScore).ToString(),
                BiggestLoss: biggestLossRow is null ? "-" : (biggestLossRow.TeamScore - biggestLossRow.OppScore).ToString(),
                LastGameStadium: lastStadium ?? "-",
                 LastGameDate: lastGame?.GameDateTime.ToString("yyyy-MM-dd") ?? "—"
                );

        })
        .OrderBy(s => s.TeamName)
        .ToList();

        return TeamSummaries;
    }
}
