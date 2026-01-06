CREATE OR ALTER PROCEDURE GetTop3EmployeesWithHighestRewards
AS
BEGIN
    SET NOCOUNT ON;

    SELECT TOP 3
        up.Id AS UserId,
        up.FirstName,
        up.LastName,
        ua.RewardBalance,
        SUM(rt.PointsDelta) AS TotalPointsEarned
    FROM UserProfiles up
    INNER JOIN UserAccounts ua ON up.Id = ua.UserId
    INNER JOIN RewardTransactions rt ON ua.Id = rt.UserId
    GROUP BY up.Id, up.FirstName, up.LastName, ua.RewardBalance
    ORDER BY TotalPointsEarned DESC;
END;
