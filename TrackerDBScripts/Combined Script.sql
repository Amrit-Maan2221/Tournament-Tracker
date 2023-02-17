CREATE DATABASE Tournament;
GO

USE Tournament;
GO


CREATE TABLE [dbo].[People]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [FirstName] NVARCHAR(100) NOT NULL, 
    [LastName] NVARCHAR(100) NOT NULL, 
    [EmailAddress] NVARCHAR(200) NOT NULL, 
    [CellphoneNumber] NVARCHAR(20) NULL
)


CREATE TABLE [dbo].[Prizes]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [PlaceNumber] INT NOT NULL, 
    [PlaceName] NVARCHAR(50) NOT NULL, 
    [PrizeAmount] MONEY NOT NULL, 
    [PrizePercentage] FLOAT NOT NULL
)



CREATE TABLE [dbo].[Teams]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TeamName] NVARCHAR(100) NOT NULL
)


CREATE TABLE [dbo].[TeamMembers]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TeamId] INT NOT NULL, 
    [PersonId] INT NOT NULL, 
    CONSTRAINT [FK_TeamMembers_ToTeams] FOREIGN KEY ([TeamId]) REFERENCES [Teams]([Id]), 
    CONSTRAINT [FK_TeamMembers_ToPeople] FOREIGN KEY ([PersonId]) REFERENCES [People]([Id])
)


CREATE TABLE [dbo].[Tournaments]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TournamentName] NVARCHAR(200) NOT NULL, 
    [EntryFee] MONEY NOT NULL, 
    [Active] BIT NOT NULL
)











CREATE TABLE [dbo].[Matchups]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TournamentId] INT NOT NULL, 
    [WinnerId] INT NULL, 
    [MatchupRound] INT NOT NULL, 
    CONSTRAINT [FK_Matchups_ToTeams] FOREIGN KEY ([WinnerId]) REFERENCES [Teams]([Id]), 
    CONSTRAINT [FK_Matchups_ToTournaments] FOREIGN KEY ([TournamentId]) REFERENCES [Tournaments]([Id])
)











CREATE TABLE [dbo].[TournamentEntries]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,  
    [TournamentId] INT NOT NULL, 
    [TeamId] INT NOT NULL, 
    CONSTRAINT [FK_TournamentEntries_ToTournaments] FOREIGN KEY ([TournamentId]) REFERENCES Tournaments([Id]), 
    CONSTRAINT [FK_TournamentEntries_ToTeams] FOREIGN KEY ([TeamId]) REFERENCES [Teams]([Id])
)



CREATE TABLE [dbo].[TournamentPrizes]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TournamentId] INT NOT NULL, 
    [PrizeId] INT NOT NULL, 
    CONSTRAINT [FK_TournamentPrizes_ToTournaments] FOREIGN KEY ([TournamentId]) REFERENCES [Tournaments]([Id]), 
    CONSTRAINT [FK_TournamentPrizes_ToPrizes] FOREIGN KEY ([PrizeId]) REFERENCES [Prizes]([Id])
)





CREATE TABLE [dbo].[MatchupEntries]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [MatchupId] INT NOT NULL, 
    [ParentMatchupId] INT NULL, 
    [TeamCompetingId] INT NULL, 
    [Score] DECIMAL NULL, 
    CONSTRAINT [FK_MatchupEntries_ToMatchups] FOREIGN KEY ([MatchupId]) REFERENCES [Matchups]([Id]), 
    CONSTRAINT [FK_MatchupEntries_ToParentMatchups] FOREIGN KEY ([ParentMatchupId]) REFERENCES [Matchups]([Id]), 
    CONSTRAINT [FK_MatchupEntries_ToTeams] FOREIGN KEY ([TeamCompetingId]) REFERENCES [Teams]([Id])
)
GO

-- Stored Procedures


CREATE PROCEDURE [dbo].[spMatchupEntries_GetByMatchup]
	@MatchupId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT	[MatchupEntries].[Id], 
			[MatchupEntries].[MatchupId], 
			[MatchupEntries].[ParentMatchupId], 
			[MatchupEntries].[TeamCompetingId], 
			[MatchupEntries].[Score]
	FROM	[dbo].[MatchupEntries]
	WHERE	[MatchupEntries].[MatchupId] = @MatchupId;

END
GO


CREATE PROCEDURE [dbo].[spMatchupEntries_Insert]
	@MatchupId int,
	@ParentMatchupId int,
	@TeamCompetingId int,
	@Id int = 0 output
AS
BEGIN

	SET NOCOUNT ON;

	INSERT INTO [dbo].[MatchupEntries] (MatchupId, ParentMatchupId, TeamCompetingId)
	VALUES (@MatchupId, @ParentMatchupId, @TeamCompetingId);

	SELECT @Id = SCOPE_IDENTITY();

END
GO


CREATE PROCEDURE [dbo].[spMatchupEntries_Update]
	@Id int,
	@TeamCompetingId int = null,
	@Score float = null
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE	[dbo].[MatchupEntries]
	SET		TeamCompetingId = @TeamCompetingId,
			Score = @Score 
	WHERE	Id = @Id;

END
GO


CREATE PROCEDURE [dbo].[spMatchups_GetByTournament]
	@TournamentId int
AS
BEGIN 

	SET NOCOUNT ON;

	SELECT		[Matchups].[Id], 
				[Matchups].[TournamentId], 
				[Matchups].[WinnerId], 
				[Matchups].[MatchupRound]
	FROM		[dbo].[Matchups]
	WHERE		Matchups.TournamentId = @TournamentId
	ORDER BY	Matchups.MatchupRound;

END
GO



CREATE PROCEDURE [dbo].[spMatchups_Insert]
	@TournamentId int,
	@MatchupRound int,
	@Id int = 0 output
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Matchups] (TournamentId, MatchupRound)
	VALUES(@TournamentId, @MatchupRound);

	SELECT @Id = SCOPE_IDENTITY();

END
GO




CREATE PROCEDURE [dbo].[spMatchups_Update]
	@Id int,
	@WinnerId int
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[Matchups]
	SET WinnerId = @WinnerId
	WHERE id = @id;

END
GO



CREATE PROCEDURE [dbo].[spPeople_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT	[People].[Id], 
			[People].[FirstName], 
			[People].[LastName], 
			[People].[EmailAddress], 
			[People].[CellphoneNumber]
	FROM	[dbo].[People];
END
GO


CREATE PROCEDURE [dbo].[spPeople_Insert]
	@FirstName nvarchar(100),
	@LastName nvarchar(100),
	@EmailAddress nvarchar(100),
	@CellphoneNumber nvarchar(20),
	@Id int = 0 output
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[People] (FirstName, LastName, EmailAddress, CellphoneNumber)
	VALUES (@FirstName, @LastName, @EmailAddress, @CellphoneNumber);

	SELECT @Id = SCOPE_IDENTITY();
END
GO


CREATE PROCEDURE [dbo].[spPrizes_GetByTournament]
	@TournamentId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT		[Prizes].[Id], 
				[Prizes].[PlaceNumber], 
				[Prizes].[PlaceName], 
				[Prizes].[PrizeAmount], 
				[Prizes].[PrizePercentage]
	FROM		[dbo].[Prizes]
	INNER JOIN	[dbo].[TournamentPrizes]
	ON			TournamentPrizes.PrizeId = Prizes.Id
	WHERE		TournamentPrizes.TournamentId = @TournamentId;

END
GO


CREATE PROCEDURE [dbo].[spPrizes_Insert]
	@PlaceNumber int, 
	@PlaceName nvarchar(50), 
	@PrizeAmount money, 
	@PrizePercentage float,
	@Id int = 0 output
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Prizes] (PlaceNumber, PlaceName, PrizeAmount, PrizePercentage)
	VALUES (@PlaceNumber, @PlaceName, @PrizeAmount, @PrizePercentage);

	SELECT @Id = SCOPE_IDENTITY();

END
GO




CREATE PROCEDURE [dbo].[spTeam_getByTournament]
	@TournamentId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT		[Teams].[Id], 
				[Teams].[TeamName]
	FROM		[dbo].[Teams]
	INNER JOIN	[dbo].[TournamentEntries]
	ON			TournamentEntries.TeamId = Teams.Id
	WHERE		[TournamentEntries].[TournamentId] = @TournamentId;

END
GO


CREATE PROCEDURE [dbo].[spTeamMembers_GetByTeam]
	@TeamId int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT		[People].[Id], 
				[People].[FirstName], 
				[People].[LastName], 
				[People].[EmailAddress], 
				[People].[CellphoneNumber]
	FROM		[dbo].[TeamMembers]
	INNER JOIN	[dbo].[People]
	ON			[People].[Id] = [TeamMembers].[PersonId]
	WHERE		[TeamMembers].[TeamId] = @TeamId;

END
GO



CREATE PROCEDURE [dbo].[spTeamMembers_Insert]
	@TeamId int,
	@PersonId int,
	@id int = 0 output
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[TeamMembers] (TeamId, PersonId)
	VALUES (@TeamId, @PersonId)

	SELECT @Id = SCOPE_IDENTITY();
END
GO




CREATE PROCEDURE [dbo].[spTeams_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT	[Teams].[Id], 
			[Teams].[TeamName]
	FROM	[dbo].[Teams];

END
GO





CREATE PROCEDURE [dbo].[spTeams_Insert]
	@TeamName nvarchar(100),
	@Id int = 0 output
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Teams] (TeamName)
	VALUES (@TeamName)

	SELECT @Id = SCOPE_IDENTITY();
END
GO



CREATE PROCEDURE [dbo].[spTournamentEntries_Insert]
	@TournamentId int,
	@TeamId int,
	@Id int = 0 output
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[TournamentEntries] (TournamentId, TeamId)
	VALUES (@TournamentId, @TeamId)

	SELECT @Id = SCOPE_IDENTITY();

END
GO




CREATE PROCEDURE [dbo].[spTournamentPrizes_Insert]
	@TournamentId int,
	@PrizeId int,
	@Id int = 0 output
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[TournamentPrizes] (TournamentId, PrizeId)
	VALUES (@TournamentId, @PrizeId);

	SELECT @Id = SCOPE_IDENTITY();
END
GO



CREATE PROCEDURE [dbo].[spTournaments_Complete]
	@Id int
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[Tournaments]
	SET Active = 0
	WHERE Id = @Id;

END
GO



CREATE PROCEDURE [dbo].[spTournaments_GetAll]
AS
BEGIN

	SET NOCOUNT ON;
	
	SELECT	Tournaments.Id, 
			Tournaments.TournamentName, 
			Tournaments.EntryFee, 
			Tournaments.Active
	FROM	[dbo].[Tournaments]
	WHERE	Active = 1;

END
GO





CREATE PROCEDURE [dbo].[spTournaments_Insert]
	@TournamentName nvarchar(200),
	@EntryFee money,
	@id int = 0 output
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Tournaments] (TournamentName, EntryFee, Active)
	VALUES(@TournamentName, @EntryFee, 1);

	SELECT @Id = SCOPE_IDENTITY();
END
GO
