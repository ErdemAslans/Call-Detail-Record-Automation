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

        public async Task<(bool Success, BreakResponseModel? BreakInfo, string? Message)> StartBreakAsync(string username, string? reason)
        {
            var user = await _operatorRepository.GetUserByUsernameAsync(username);
            if (user == null)
                return (false, null, "User not found.");

            if (await _breakRepository.HasOngoingBreakAsync(user.Id))
            {
                return (false, null, "User already has an ongoing break.");
            }

            var breakInfo = new Break
            {
                UserId = user.Id,
                StartTime = DateTime.UtcNow,
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
                return new List<BreakResponseModel>();

            var breaks = await _breakRepository.GetBreaksByUserIdAndDateRangeAsync(user.Id, startDate, endDate);
            return _mapper.Map<List<BreakResponseModel>>(breaks);
        }
    }
}