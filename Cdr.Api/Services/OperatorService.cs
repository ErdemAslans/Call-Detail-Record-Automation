namespace Cdr.Api.Services
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Cdr.Api.Interfaces;   
    using Cdr.Api.Models.Response.UserStatistics;
    using Cdr.Api.Services.Interfaces;
    using Cdr.Api.Entities;
    using MongoDB.Bson;
    using Cdr.Api.Models.Entities;
    using System.Collections.Generic;
    using Cdr.Api.Models.Response;

    public class OperatorService : IOperatorService
    {
        private readonly IOperatorRepository _operatorRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IBreakRepository _breakRepository;
        private readonly IMapper _mapper;

        public OperatorService(IOperatorRepository operatorRepository, IDepartmentRepository departmentRepository, IBreakRepository breakRepository, IMapper mapper)
        {
            _operatorRepository = operatorRepository;
            _departmentRepository = departmentRepository;
            _breakRepository = breakRepository;
            _mapper = mapper;
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
            var breaks = await _breakRepository.GetBreaksByUserIdAndDateRangeAsync(user.Id, startDate, endDate);
            return _mapper.Map<List<BreakResponseModel>>(breaks);
        }

        public async Task<BreakResponseModel?> GetOngoingBreakAsync(string username)
        {
            var user = await _operatorRepository.GetUserByUsernameAsync(username);
            var ongoingBreak = await _breakRepository.GetOngoingBreakAsync(user.Id);
            return ongoingBreak != null ? _mapper.Map<BreakResponseModel>(ongoingBreak) : null;
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