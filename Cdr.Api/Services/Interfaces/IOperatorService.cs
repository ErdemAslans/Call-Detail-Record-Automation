using Cdr.Api.Entities;
using Cdr.Api.Models.Response.UserStatistics;
using MongoDB.Bson;
using Cdr.Api.Models.Response;

namespace Cdr.Api.Services.Interfaces;

/// <summary>
/// Service for managing and retrieving call center operator information
/// </summary>
public interface IOperatorService
{
    /// <summary>
    /// Retrieves user information by phone number
    /// </summary>
    /// <param name="number">The phone number of the user</param>
    /// <returns>Operator information</returns>
    Task<OperatorInfo> GetUserInfoAsync(string number);

    /// <summary>
    /// Retrieves information for all operators
    /// </summary>
    /// <returns>A list of all operators' information</returns>
    Task<List<OperatorInfo>> GetAllUsersInfoAsync();

    /// <summary>
    /// Retrieves all departments in the system
    /// </summary>
    /// <returns>A collection of departments</returns>
    Task<IEnumerable<Department>> GetAllDepartmentsAsync();

    /// <summary>
    /// Registers the start of an operator's break period
    /// </summary>
    /// <param name="username">The username of the operator taking a break</param>
    /// <param name="reason">Optional reason for the break</param>
    /// <returns>A tuple containing success status, break information, and a message</returns>
    Task<(bool Success, BreakResponseModel? BreakInfo, string? Message)> StartBreakAsync(string username, string? reason, DateTime plannedEndTime);

    /// <summary>
    /// Registers the end of an operator's break period
    /// </summary>
    /// <param name="breakId">The ID of the break to end</param>
    /// <returns>A tuple containing success status, break information, and a message</returns>
    Task<(bool Success, BreakResponseModel? BreakInfo, string? Message)> EndBreakAsync(ObjectId breakId);

    /// <summary>
    /// Retrieves an operator's break times within a specified date range
    /// </summary>
    /// <param name="username">The username of the operator</param>
    /// <param name="startDate">The start date of the range</param>
    /// <param name="endDate">The end date of the range</param>
    /// <returns>A list of break records for the specified operator and date range</returns>
    Task<List<BreakResponseModel>> GetUserBreakTimesAsync(string username, DateTime startDate, DateTime endDate);

    /// <summary>
    /// Retrieves the ongoing break for an operator (if any), regardless of date range
    /// </summary>
    Task<BreakResponseModel?> GetOngoingBreakAsync(string username);

    /// <summary>
    /// Force-ends any ongoing break for a given user (Admin only)
    /// </summary>
    Task<(bool Success, BreakResponseModel? BreakInfo, string? Message)> ForceEndBreakAsync(string userId);
}