namespace Cdr.Api.Services
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Cdr.Api.Interfaces;
    using Cdr.Api.Models.Response.UserStatistics;
    using Cdr.Api.Services.Interfaces;
    using Cdr.Api.Entities;
    using Cdr.Api.Helpers;
    using MongoDB.Bson;
    using Cdr.Api.Models.Entities;
    using System.Collections.Generic;
    using Cdr.Api.Models.Response;
    using Microsoft.Extensions.Logging;

    public class OperatorService : IOperatorService
    {
        private readonly IOperatorRepository _operatorRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IBreakRepository _breakRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OperatorService> _logger;

        public OperatorService(IOperatorRepository operatorRepository, IDepartmentRepository departmentRepository, IBreakRepository breakRepository, IMapper mapper, ILogger<OperatorService> logger)
        {
            _operatorRepository = operatorRepository;
            _departmentRepository = departmentRepository;
            _breakRepository = breakRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OperatorInfo> GetUserInfoAsync(string number)
        {
            return await _operatorRepository.GetUserInfoAsync(number);
        }

        public async Task<List<OperatorInfo>> GetAllUsersInfoAsync()
        {
            return await _operatorRepository.GetAllUsersInfoAsync();
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _departmentRepository.GetAllAsync();
        }

        public async Task<(bool Success, BreakResponseModel? BreakInfo, string? Message)> StartBreakAsync(string username, string? reason, DateTime plannedEndTime)
        {
            var user = await _operatorRepository.GetUserByUsernameAsync(username);
            if (await _breakRepository.HasOngoingBreakAsync(user.Id))
            {
                return (false, null, "User already has an ongoing break.");
            }

            if (await _breakRepository.HasOngoingShiftEndAsync(user.Id))
            {
                return (false, null, "Cannot start break - shift has ended. Start your shift first.");
            }

            if (plannedEndTime <= DateTime.UtcNow)
            {
                return (false, null, "Planned end time must be in the future.");
            }

            if ((plannedEndTime - DateTime.UtcNow).TotalHours > 4)
            {
                return (false, null, "Break duration cannot exceed 4 hours.");
            }

            var breakInfo = new Break
            {
                UserId = user.Id,
                StartTime = DateTime.UtcNow,
                PlannedEndTime = plannedEndTime,
                Reason = reason
            };
            await _breakRepository.StartBreakAsync(breakInfo);
            var breakResponse = _mapper.Map<BreakResponseModel>(breakInfo);
            return (true, breakResponse, "Break started successfully.");
        }

        public async Task<(bool Success, BreakResponseModel? BreakInfo, string? Message)> EndBreakAsync(ObjectId breakId)
        {
            if (await _breakRepository.IsBreakEndedAsync(breakId))
            {
                return (false, null, "Break is already ended.");
            }

            await _breakRepository.EndBreakAsync(breakId, DateTime.UtcNow);
            var breakInfo = await _breakRepository.GetBreakByIdAsync(breakId);
            var breakResponse = _mapper.Map<BreakResponseModel>(breakInfo);
            return (true, breakResponse, "Break ended successfully.");
        }

        public async Task<List<BreakResponseModel>> GetUserBreakTimesAsync(string username, DateTime startDate, DateTime endDate)
        {
            var user = await _operatorRepository.GetUserByUsernameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("GetUserBreakTimesAsync: Operator not found for username {Username}", username);
                return new List<BreakResponseModel>();
            }
            _logger.LogInformation("GetUserBreakTimesAsync: Found operator {OperatorId} for username {Username}", user.Id, username);
            var breaks = await _breakRepository.GetBreaksByUserIdAndDateRangeAsync(user.Id, startDate, endDate);
            _logger.LogInformation("GetUserBreakTimesAsync: Found {Count} breaks for operator {OperatorId}", breaks.Count, user.Id);
            return _mapper.Map<List<BreakResponseModel>>(breaks);
        }

        public async Task<BreakResponseModel?> GetOngoingBreakAsync(string username)
        {
            var user = await _operatorRepository.GetUserByUsernameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("GetOngoingBreakAsync: Operator not found for username {Username}", username);
                return null;
            }
            var ongoingBreak = await _breakRepository.GetOngoingBreakAsync(user.Id);
            return ongoingBreak != null ? _mapper.Map<BreakResponseModel>(ongoingBreak) : null;
        }

        public async Task<(bool Success, BreakResponseModel? BreakInfo, string? Message)> EndShiftAsync(string username, string? reason)
        {
            var user = await _operatorRepository.GetUserByUsernameAsync(username);

            if (await _breakRepository.HasOngoingBreakAsync(user.Id))
            {
                return (false, null, "User has an ongoing break. End the break first.");
            }

            if (await _breakRepository.HasOngoingShiftEndAsync(user.Id))
            {
                return (false, null, "Shift is already ended.");
            }

            // PlannedEndTime = end of work day (16:45 Turkey time)
            var turkeyZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
            var nowTurkey = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, turkeyZone);
            var endOfDay = new DateTime(nowTurkey.Year, nowTurkey.Month, nowTurkey.Day, 16, 45, 0);
            var plannedEndUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(endOfDay, DateTimeKind.Unspecified), turkeyZone);

            var breakInfo = new Break
            {
                UserId = user.Id,
                StartTime = DateTime.UtcNow,
                PlannedEndTime = plannedEndUtc,
                Reason = reason ?? "Mesai bitimi",
                BreakType = "EndOfShift"
            };
            await _breakRepository.StartBreakAsync(breakInfo);
            var breakResponse = _mapper.Map<BreakResponseModel>(breakInfo);
            return (true, breakResponse, "Shift ended successfully.");
        }

        public async Task AutoEndAllShiftsAsync()
        {
            _logger.LogInformation("Starting auto end-of-shift for all operators");

            var turkeyZone = CdrReportHelper.TurkeyTimeZone;
            var nowTurkey = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, turkeyZone);
            var endOfDay = new DateTime(nowTurkey.Year, nowTurkey.Month, nowTurkey.Day, 16, 45, 0);
            var plannedEndUtc = TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(endOfDay, DateTimeKind.Unspecified), turkeyZone);

            var allOperators = await _operatorRepository.GetAllAsync();
            var endedCount = 0;

            foreach (var op in allOperators)
            {
                try
                {
                    // Skip if already has an EndOfShift record
                    if (await _breakRepository.HasOngoingShiftEndAsync(op.Id))
                        continue;

                    // End any ongoing regular break first
                    if (await _breakRepository.HasOngoingBreakAsync(op.Id))
                    {
                        var ongoingBreak = await _breakRepository.GetOngoingBreakAsync(op.Id);
                        if (ongoingBreak != null)
                        {
                            await _breakRepository.EndBreakAsync(ongoingBreak.Id, DateTime.UtcNow);
                            _logger.LogInformation("Auto-ended ongoing break for operator {Name} ({Id})", op.Name, op.Id);
                        }
                    }

                    // Create EndOfShift record
                    var shiftEnd = new Break
                    {
                        UserId = op.Id,
                        StartTime = DateTime.UtcNow,
                        PlannedEndTime = plannedEndUtc,
                        Reason = "Otomatik mesai bitimi",
                        BreakType = "EndOfShift"
                    };
                    await _breakRepository.StartBreakAsync(shiftEnd);
                    endedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to auto end shift for operator {Name} ({Id})", op.Name, op.Id);
                }
            }

            _logger.LogInformation("Auto end-of-shift completed. {Count} operators' shifts ended", endedCount);
        }

        public async Task<(bool Success, BreakResponseModel? BreakInfo, string? Message)> ForceEndBreakAsync(string userId)
        {
            var ongoingBreak = await _breakRepository.GetOngoingBreakAsync(userId);
            if (ongoingBreak == null)
            {
                return (false, null, "User has no ongoing break.");
            }

            await _breakRepository.EndBreakAsync(ongoingBreak.Id, DateTime.UtcNow);
            var updatedBreak = await _breakRepository.GetBreakByIdAsync(ongoingBreak.Id);
            var breakResponse = _mapper.Map<BreakResponseModel>(updatedBreak);
            return (true, breakResponse, "Break force-ended successfully.");
        }
    }
}