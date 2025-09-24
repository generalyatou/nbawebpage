# NBA App (Monorepo)

This repository contains a monorepo setup for an NBA application with separate backend and frontend projects.

## Repository Structure

```
nba-app/
  backend/    # .NET + EF Core + SQL Server
  frontend/   # React + Tailwind + PrimeReact
  README.md
```

- **Backend**: .NET 9 + EF Core, connected to SQL Server  
- **Frontend**: React with Tailwind + PrimeReact  
- **Database**: SQL Server 2022 running in Docker (on macOS).  
  > Note: On Windows, Docker is not required since SQL Server can run natively.

---

## Prerequisites

- [Docker Desktop](Mac only)  
- [.NET 9 SDK]  
- [Node.js 18+] 
- [Azure Data Studio] (optional, Mac)  

---

## Database Setup (macOS only)

### 1. Start SQL Server in Docker
Run once to create the container:
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Your_password123" \
  -p 1433:1433 --name sqlserver \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

Subsequent runs:
```bash
docker start sqlserver    # start
docker stop sqlserver     # stop
docker logs -f sqlserver  # optional: view startup logs
```

---

### 2. Connect with Azure Data Studio (optional)
```
Server: localhost,1433  
Authentication: SQL Login  
Username: sa  
Password: Your_password123  
```

---

### 3. Database Source
- Used the provided schema and seed data (no modifications were made).  
- Added **one stored procedure** to support the API.

---

## Stored Procedure

```sql
USE [NBA];
GO

IF OBJECT_ID('dbo.usp_NBA_Details', 'P') IS NOT NULL
    DROP PROCEDURE dbo.usp_NBA_Details;
GO

CREATE PROCEDURE dbo.usp_NBA_Details
    @StartDate DATETIME = NULL,   
    @EndDate   DATETIME = NULL,   
    @TeamID    INT       = NULL   
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        t.TeamID,
        t.Name    AS TeamName,
        t.Stadium,
        t.Logo,
        t.URL
    FROM dbo.Teams t
    WHERE (@TeamID IS NULL OR t.TeamID = @TeamID)
    ORDER BY t.TeamID;

    SELECT
        t.TeamID,
        t.Name       AS TeamName,
        t.Stadium,
        g.GameID,
        g.GameDateTime,
        CASE WHEN g.HomeTeamID = t.TeamID THEN g.HomeScore ELSE g.AwayScore END AS TeamScore,
        CASE WHEN g.HomeTeamID = t.TeamID THEN g.AwayScore ELSE g.HomeScore END AS OppScore,
        CASE WHEN g.HomeTeamID = t.TeamID THEN 1 ELSE 0 END AS IsHome,
        CASE WHEN g.HomeTeamID = t.TeamID THEN g.AwayTeamID ELSE g.HomeTeamID END AS OppTeamID,
        g.HomeTeamID,
        g.AwayTeamID,
        g.MVPPlayerID,
        CASE WHEN
            (CASE WHEN g.HomeTeamID = t.TeamID THEN g.HomeScore ELSE g.AwayScore END) >
            (CASE WHEN g.HomeTeamID = t.TeamID THEN g.AwayScore ELSE g.HomeScore END)
        THEN 1 ELSE 0 END AS IsWin,
        CASE WHEN
            (CASE WHEN g.HomeTeamID = t.TeamID THEN g.HomeScore ELSE g.AwayScore END) <
            (CASE WHEN g.HomeTeamID = t.TeamID THEN g.AwayScore ELSE g.HomeScore END)
        THEN 1 ELSE 0 END AS IsLoss,
        (CASE WHEN g.HomeTeamID = t.TeamID THEN g.HomeScore ELSE g.AwayScore END) -
        (CASE WHEN g.HomeTeamID = t.TeamID THEN g.AwayScore ELSE g.HomeScore END) AS Margin
    FROM dbo.Teams t
    JOIN dbo.Games g
      ON g.HomeTeamID = t.TeamID
    WHERE (@TeamID IS NULL OR t.TeamID = @TeamID)
      AND (@StartDate IS NULL OR g.GameDateTime >= @StartDate)
      AND (@EndDate   IS NULL OR g.GameDateTime < DATEADD(DAY, 1, @EndDate))

    UNION ALL

    SELECT
        t.TeamID,
        t.Name       AS TeamName,
        t.Stadium,
        g.GameID,
        g.GameDateTime,
        CASE WHEN g.AwayTeamID = t.TeamID THEN g.AwayScore ELSE g.HomeScore END AS TeamScore,
        CASE WHEN g.AwayTeamID = t.TeamID THEN g.HomeScore ELSE g.AwayScore END AS OppScore,
        CASE WHEN g.AwayTeamID = t.TeamID THEN 0 ELSE 1 END AS IsHome,
        CASE WHEN g.AwayTeamID = t.TeamID THEN g.HomeTeamID ELSE g.AwayTeamID END AS OppTeamID,
        g.HomeTeamID,
        g.AwayTeamID,
        g.MVPPlayerID,
        CASE WHEN
            (CASE WHEN g.AwayTeamID = t.TeamID THEN g.AwayScore ELSE g.HomeScore END) >
            (CASE WHEN g.AwayTeamID = t.TeamID THEN g.HomeScore ELSE g.AwayScore END)
        THEN 1 ELSE 0 END AS IsWin,
        CASE WHEN
            (CASE WHEN g.AwayTeamID = t.TeamID THEN g.AwayScore ELSE g.HomeScore END) <
            (CASE WHEN g.AwayTeamID = t.TeamID THEN g.HomeScore ELSE g.AwayScore END)
        THEN 1 ELSE 0 END AS IsLoss,
        (CASE WHEN g.AwayTeamID = t.TeamID THEN g.AwayScore ELSE g.HomeScore END) -
        (CASE WHEN g.AwayTeamID = t.TeamID THEN g.HomeScore ELSE g.AwayScore END) AS Margin
    FROM dbo.Teams t
    JOIN dbo.Games g
      ON g.AwayTeamID = t.TeamID
    WHERE (@TeamID IS NULL OR t.TeamID = @TeamID)
      AND (@StartDate IS NULL OR g.GameDateTime >= @StartDate)
      AND (@EndDate   IS NULL OR g.GameDateTime < DATEADD(DAY, 1, @EndDate))

    ORDER BY TeamID, GameDateTime;

    SELECT 
        tp.TeamID,
        p.PlayerID,
        p.[Name] AS PlayerName
    FROM dbo.Team_Player tp
    JOIN dbo.Players p ON p.PlayerID = tp.PlayerID
    WHERE (@TeamID IS NULL OR tp.TeamID = @TeamID)
    ORDER BY tp.TeamID, p.PlayerID;
END
GO
```

### Execute
```sql
EXEC dbo.usp_NBA_Details
```

Parameters are optional. Run without arguments to get all teams, games, and players.

---

## Notes

- Backend and frontend run as two separate applications.  
- Frontend communicates with the backend API.  
