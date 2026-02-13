using CallCenter.Helpers;
using Cdr.Api.Context;
using Cdr.Api.Entities;
using Cdr.Api.Entities.Cdr;
using Cdr.Api.Helpers;
using Cdr.Api.Interfaces;
using Cdr.Api.Models;
using Cdr.Api.Models.Entities;
using Cdr.Api.Models.Pagination;
using Cdr.Api.Models.Response;
using Cdr.Api.Models.Response.Dashboard;
using Cdr.Api.Models.Response.UserStatistics;
using Common.Enums;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Cdr.Api.Repositories;

public class CdrRecordsRepository : ReadonlyMongoRepository<CdrRecord>, ICdrRecordsRepository
{
    private readonly IMongoCollection<Operator> _userCollection;
    private readonly IMongoCollection<Department> _departmentCollection;
    private readonly IMongoCollection<Break> _breakCollection;

    public CdrRecordsRepository(MongoDbContext context, IOptions<MongoDbSettings> mongoDbSettings)
        : base(context, mongoDbSettings.Value.CollectionName)
    {
        _userCollection = context.GetCollection<Operator>("users");
        _departmentCollection = context.GetCollection<Department>("departments");
        _breakCollection = context.GetCollection<Break>(mongoDbSettings.Value.BreakCollectionName);
    }

    /// <summary>
    /// Applies global filter to only include CDR records where phone numbers start with "8036"
    /// This implements a domain business rule similar to EF Core's QueryFilter
    /// </summary>
    private FilterDefinition<CdrRecord> ApplyGlobalFilter()
    {
        return Builders<CdrRecord>.Filter.Or(
            Builders<CdrRecord>.Filter.And(
                Builders<CdrRecord>.Filter.Ne(x => x.OriginalCalledParty, null),
                Builders<CdrRecord>.Filter.Regex(x => x.OriginalCalledParty!.Number, new BsonRegularExpression("^8036.*"))
            ),
            Builders<CdrRecord>.Filter.And(
                Builders<CdrRecord>.Filter.Ne(x => x.CallingParty, null),
                Builders<CdrRecord>.Filter.Regex(x => x.CallingParty!.Number, new BsonRegularExpression("^8036.*"))
            ),
            Builders<CdrRecord>.Filter.And(
                Builders<CdrRecord>.Filter.Ne(x => x.FinalCalledParty, null),
                Builders<CdrRecord>.Filter.Regex(x => x.FinalCalledParty!.Number, new BsonRegularExpression("^8036.*"))
            )
        );
    }

    public async Task<IEnumerable<CdrRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        // Convert Turkey local dates to UTC for MongoDB query
        var (startUtc, endUtc) = TurkeyTimeProvider.ConvertDateRangeToUtc(startDate, endDate);
        
        var filter = Builders<CdrRecord>.Filter.And(
            ApplyGlobalFilter(),
            Builders<CdrRecord>.Filter.Gte(x => x.DateTime!.Connect, startUtc),
            Builders<CdrRecord>.Filter.Lt(x => x.DateTime!.Disconnect, endUtc)
        );
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<WeeklyAnsweredCallRate>> GetWeeklyAnsweredCallsAsync(DateTime startDate, DateTime endDate)
    {
        var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
        var todayTurkey = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, turkeyTimeZone).Date;
        
        var results = new List<WeeklyAnsweredCallRate>();
        
        // Rolling 7 days from today backwards (today is first, 6 days ago is last)
        for (int i = 0; i < 7; i++)
        {
            var dayDate = todayTurkey.AddDays(-i);
            
            // Convert Turkey local day boundaries to UTC for MongoDB query
            var dayStartUtc = TimeZoneInfo.ConvertTimeToUtc(dayDate, turkeyTimeZone);
            var dayEndUtc = TimeZoneInfo.ConvertTimeToUtc(dayDate.AddDays(1), turkeyTimeZone);
            
            var dayFilter = Builders<CdrRecord>.Filter.And(
                ApplyGlobalFilter(),
                Builders<CdrRecord>.Filter.Gte(x => x.DateTime!.Origination, dayStartUtc),
                Builders<CdrRecord>.Filter.Lt(x => x.DateTime!.Origination, dayEndUtc)
            );
            
            var dayAggregate = await _collection.Aggregate()
                .Match(dayFilter)
                .Group(new BsonDocument
                {
                    { "_id", BsonNull.Value },
                    { "totalRecords", new BsonDocument("$sum", 1) },
                    { "connectAndDuration", new BsonDocument("$sum", new BsonDocument("$cond", new BsonDocument
                        {
                            { "if", new BsonDocument("$and", new BsonArray
                                {
                                    new BsonDocument("$ne", new BsonArray { "$dateTime.connect", BsonNull.Value }),
                                    new BsonDocument("$gt", new BsonArray { "$duration", 0 })
                                })
                            },
                            { "then", 1 },
                            { "else", 0 }
                        })
                    )}
                })
                .FirstOrDefaultAsync();
            
            int totalRecords = dayAggregate?["totalRecords"].AsInt32 ?? 0;
            int connectAndDuration = dayAggregate?["connectAndDuration"].AsInt32 ?? 0;
            double percentage = totalRecords > 0 ? (double)connectAndDuration / totalRecords * 100 : 0;
            
            results.Add(new WeeklyAnsweredCallRate
            {
                Year = dayDate.Year,
                Month = dayDate.Month,
                DayOfWeek = (int)dayDate.DayOfWeek,
                Date = dayDate,
                TotalRecords = totalRecords,
                ConnectAndDuration = connectAndDuration,
                Percentage = percentage
            });
        }
        
        return results;
    }

    public async Task<IEnumerable<MonthlyAnsweredCallRate>> GetMonthlyAnsweredCallsAsync(DateTime startDate, DateTime endDate)
    {
        // Convert Turkey local dates to UTC for MongoDB query
        var (startUtc, endUtc) = TurkeyTimeProvider.ConvertDateRangeToUtc(startDate, endDate);

        var filter = Builders<CdrRecord>.Filter.And(
            ApplyGlobalFilter(),
            Builders<CdrRecord>.Filter.Gte(x => x.DateTime!.Origination, startUtc),
            Builders<CdrRecord>.Filter.Lt(x => x.DateTime!.Origination, endUtc)
        );

        // Use Turkey timezone for date extraction to ensure correct month/year grouping
        var turkeyTimezone = "Europe/Istanbul";

        var aggregate = await _collection.Aggregate()
            .Match(filter)
            .Project(new BsonDocument
            {
                { "year", new BsonDocument("$year", new BsonDocument
                    {
                        { "date", "$dateTime.origination" },
                        { "timezone", turkeyTimezone }
                    })
                },
                { "month", new BsonDocument("$month", new BsonDocument
                    {
                        { "date", "$dateTime.origination" },
                        { "timezone", turkeyTimezone }
                    })
                },
                { "connectAndDuration", new BsonDocument("$cond", new BsonDocument
                    {
                        { "if", new BsonDocument("$and", new BsonArray
                            {
                                new BsonDocument("$ne", new BsonArray { "$dateTime.connect", BsonNull.Value }),
                                new BsonDocument("$gt", new BsonArray { "$duration", 0 })
                            })
                        },
                        { "then", 1 },
                        { "else", 0 }
                    })
                }
            })
            .Group(new BsonDocument
            {
                { "_id", new BsonDocument
                    {
                        { "year", "$year" },
                        { "month", "$month" }
                    }
                },
                { "totalRecords", new BsonDocument("$sum", 1) },
                { "connectAndDuration", new BsonDocument("$sum", "$connectAndDuration") }
            })
            .Project(new BsonDocument
            {
                { "year", "$_id.year" },
                { "month", "$_id.month" },
                { "totalRecords", 1 },
                { "connectAndDuration", 1 },
                { "percentage", new BsonDocument("$multiply", new BsonArray
                    {
                        new BsonDocument("$divide", new BsonArray { "$connectAndDuration", "$totalRecords" }),
                        100
                    })
                }
            })
            .Sort(new BsonDocument
            {
                { "year", 1 },
                { "month", 1 }
            })
            .ToListAsync();

        return aggregate.Select(doc => new MonthlyAnsweredCallRate
        {
            Year = doc["year"].AsInt32,
            Month = doc["month"].AsInt32,
            TotalRecords = doc["totalRecords"].AsInt32,
            ConnectAndDuration = doc["connectAndDuration"].AsInt32,
            Percentage = doc["percentage"].AsDouble
        });
    }

    public async Task<IEnumerable<YearlyAnsweredCallRate>> GetYearlyAnsweredCallsAsync(DateTime startDate, DateTime endDate)
    {
        // Convert Turkey local dates to UTC for MongoDB query
        var (startUtc, endUtc) = TurkeyTimeProvider.ConvertDateRangeToUtc(startDate, endDate);

        var filter = Builders<CdrRecord>.Filter.And(
            ApplyGlobalFilter(),
            Builders<CdrRecord>.Filter.Gte(x => x.DateTime!.Origination, startUtc),
            Builders<CdrRecord>.Filter.Lt(x => x.DateTime!.Origination, endUtc)
        );

        // Use Turkey timezone for date extraction to ensure correct quarter/year grouping
        var turkeyTimezone = "Europe/Istanbul";

        var aggregate = await _collection.Aggregate()
            .Match(filter)
            .Project(new BsonDocument
            {
                { "year", new BsonDocument("$year", new BsonDocument
                    {
                        { "date", "$dateTime.origination" },
                        { "timezone", turkeyTimezone }
                    })
                },
                { "month", new BsonDocument("$month", new BsonDocument
                    {
                        { "date", "$dateTime.origination" },
                        { "timezone", turkeyTimezone }
                    })
                },
                { "connectAndDuration", new BsonDocument("$cond", new BsonDocument
                    {
                        { "if", new BsonDocument("$and", new BsonArray
                            {
                                new BsonDocument("$ne", new BsonArray { "$dateTime.connect", BsonNull.Value }),
                                new BsonDocument("$gt", new BsonArray { "$duration", 0 })
                            })
                        },
                        { "then", 1 },
                        { "else", 0 }
                    })
                }
            })
            .Group(new BsonDocument
            {
                { "_id", new BsonDocument
                    {
                        { "year", "$year" },
                        { "quarter", new BsonDocument("$cond", new BsonDocument
                            {
                                { "if", new BsonDocument("$lte", new BsonArray { "$month", 3 }) },
                                { "then", "Q1" },
                                { "else", new BsonDocument("$cond", new BsonDocument
                                    {
                                        { "if", new BsonDocument("$lte", new BsonArray { "$month", 6 }) },
                                        { "then", "Q2" },
                                        { "else", new BsonDocument("$cond", new BsonDocument
                                            {
                                                { "if", new BsonDocument("$lte", new BsonArray { "$month", 9 }) },
                                                { "then", "Q3" },
                                                { "else", "Q4" }
                                            })
                                        }
                                    })
                                }
                            })
                        }
                    }
                },
                { "totalRecords", new BsonDocument("$sum", 1) },
                { "connectAndDuration", new BsonDocument("$sum", "$connectAndDuration") }
            })
            .Project(new BsonDocument
            {
                { "year", "$_id.year" },
                { "quarter", "$_id.quarter" },
                { "totalRecords", 1 },
                { "connectAndDuration", 1 },
                { "percentage", new BsonDocument("$multiply", new BsonArray
                    {
                        new BsonDocument("$divide", new BsonArray { "$connectAndDuration", "$totalRecords" }),
                        100
                    })
                }
            })
            .Sort(new BsonDocument
            {
                { "year", 1 },
                { "quarter", 1 }
            })
            .ToListAsync();

        return aggregate.Select(doc => new YearlyAnsweredCallRate
        {
            Year = doc["year"].AsInt32,
            Quarter = doc["quarter"].AsString,
            TotalRecords = doc["totalRecords"].AsInt32,
            ConnectAndDuration = doc["connectAndDuration"].AsInt32,
            Percentage = doc["percentage"].AsDouble
        });
    }

    public async Task<DailyCallReport> GetDailyCallReportAsync(DateTime date)
    {
        // Convert Turkey local date boundaries to UTC for MongoDB query
        var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
        var startDateUtc = TimeZoneInfo.ConvertTimeToUtc(date.Date, turkeyTimeZone);
        var endDateUtc = TimeZoneInfo.ConvertTimeToUtc(date.Date.AddDays(1), turkeyTimeZone);

        var filter = Builders<CdrRecord>.Filter.And(
            ApplyGlobalFilter(),
            Builders<CdrRecord>.Filter.Gte(x => x.DateTime!.Origination, startDateUtc),
            Builders<CdrRecord>.Filter.Lt(x => x.DateTime!.Origination, endDateUtc)
        );

        var aggregate = await _collection.Aggregate()
            .Match(filter)
            .Group(new BsonDocument
            {
                { "_id", BsonNull.Value },
                { "totalCalls", new BsonDocument("$sum", 1) },
                { "answeredCalls", new BsonDocument("$sum", new BsonDocument("$cond", new BsonDocument
                    {
                        { "if", new BsonDocument("$and", new BsonArray
                            {
                                new BsonDocument("$ne", new BsonArray { "$dateTime.connect", BsonNull.Value }),
                                new BsonDocument("$gt", new BsonArray { "$duration", 0 })
                            })
                        },
                        { "then", 1 },
                        { "else", 0 }
                    })
                )},
                { "missedCalls", new BsonDocument("$sum", new BsonDocument("$cond", new BsonDocument
                    {
                        { "if", new BsonDocument("$eq", new BsonArray { "$dateTime.connect", BsonNull.Value }) },
                        { "then", 1 },
                        { "else", 0 }
                    })
                )},
                { "totalDuration", new BsonDocument("$sum", "$duration") }
            })
            .FirstOrDefaultAsync();

        if (aggregate == null)
        {
            return new DailyCallReport();
        }

        var originalMissedCalls = aggregate["missedCalls"].AsInt32;

        // Cross-check missed calls against break times
        var breaksByPhone = await GetBreaksByPhoneNumberAsync(startDateUtc, endDateUtc);
        var onBreakCount = 0;

        if (breaksByPhone.Count > 0)
        {
            // Get missed calls with their origination time and target number
            var missedFilter = Builders<CdrRecord>.Filter.And(
                filter,
                Builders<CdrRecord>.Filter.Eq(x => x.DateTime!.Connect, null)
            );
            var missedCalls = await _collection.Find(missedFilter)
                .Project(Builders<CdrRecord>.Projection
                    .Include(x => x.DateTime!.Origination)
                    .Include(x => x.FinalCalledParty!.Number)
                    .Include(x => x.OriginalCalledParty!.Number))
                .ToListAsync();

            foreach (var call in missedCalls)
            {
                var origination = call["dateTime"]["origination"].ToUniversalTime();
                var finalNumber = call.Contains("finalCalledParty") && !call["finalCalledParty"].IsBsonNull
                    ? call["finalCalledParty"]["number"].AsString : null;
                var originalNumber = call.Contains("originalCalledParty") && !call["originalCalledParty"].IsBsonNull
                    ? call["originalCalledParty"]["number"].AsString : null;

                // Check if the called party was on break
                var numberToCheck = finalNumber ?? originalNumber;
                if (numberToCheck != null && breaksByPhone.TryGetValue(numberToCheck, out var breaks))
                {
                    if (IsCallDuringBreak(origination, breaks))
                        onBreakCount++;
                }
            }
        }

        return new DailyCallReport
        {
            TotalCalls = aggregate["totalCalls"].AsInt32,
            AnsweredCalls = aggregate["answeredCalls"].AsInt32,
            MissedCalls = originalMissedCalls - onBreakCount,
            OnBreakCalls = onBreakCount,
            TotalDuration = aggregate["totalDuration"].AsInt32
        };
    }

    public async Task<PagedResult<CdrListItem>> GetCallReportListAsync(CdrFilter filter)
    {
        // Convert Turkey local dates to UTC for MongoDB query
        var (startUtc, endUtc) = TurkeyTimeProvider.ConvertDateRangeToUtc(filter.StartDate, filter.EndDate);
        
        var builder = Builders<CdrRecord>.Filter;
        var filterDefinition = Builders<CdrRecord>.Filter.And(
            ApplyGlobalFilter(),
            Builders<CdrRecord>.Filter.Gte(x => x.DateTime!.Origination, startUtc),
            Builders<CdrRecord>.Filter.Lt(x => x.DateTime!.Origination, endUtc)
        );

        if (filter.CallDirection.HasValue)
        {
            filterDefinition &= Builders<CdrRecord>.Filter.Eq(x => x.CallDirection, filter.CallDirection.Value);
        }

        if (!string.IsNullOrEmpty(filter.User))
        {
            filterDefinition &= Builders<CdrRecord>.Filter.Or(
                Builders<CdrRecord>.Filter.And(
                    Builders<CdrRecord>.Filter.Ne(x => x.OriginalCalledParty, null),
                    Builders<CdrRecord>.Filter.Eq(x => x.OriginalCalledParty!.Number, filter.User)
                ),
                Builders<CdrRecord>.Filter.And(
                    Builders<CdrRecord>.Filter.Ne(x => x.CallingParty, null),
                    Builders<CdrRecord>.Filter.Eq(x => x.CallingParty!.Number, filter.User)
                ),
                Builders<CdrRecord>.Filter.And(
                    Builders<CdrRecord>.Filter.Ne(x => x.FinalCalledParty, null),
                    Builders<CdrRecord>.Filter.Eq(x => x.FinalCalledParty!.Number, filter.User)
                )
            );
        }

        var totalCount = await _collection.CountDocumentsAsync(filterDefinition);
        var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

        var sortDefinition = filter.Orders.Any()
            ? Builders<CdrRecord>.Sort.Combine(
                filter.Orders.Select(order => order.DirectionDesc
                    ? Builders<CdrRecord>.Sort.Ascending(order.ColumnName)
                    : Builders<CdrRecord>.Sort.Descending(order.ColumnName))
            )
            : Builders<CdrRecord>.Sort.Descending(x => x.DateTime!.Origination);

        var items = await _collection.Aggregate()
            .Match(filterDefinition)
            .Sort(sortDefinition)
            .Skip((filter.PageIndex - 1) * filter.PageSize)
            .Limit(filter.PageSize)
            .Lookup("users", "callingParty.number", "phone_number", "callingPartyUsers")
            .Unwind("callingPartyUsers", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Lookup("departments", "callingPartyUsers.department_id", "_id", "callingPartyDepartments")
            .Unwind("callingPartyDepartments", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Lookup("users", "finalCalledParty.number", "phone_number", "finalCalledPartyUsers")
            .Unwind("finalCalledPartyUsers", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Lookup("departments", "finalCalledPartyUsers.department_id", "_id", "finalCalledPartyDepartments")
            .Unwind("finalCalledPartyDepartments", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Lookup("users", "originalCalledParty.number", "phone_number", "originalCalledPartyUsers")
            .Unwind("originalCalledPartyUsers", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Lookup("departments", "originalCalledPartyUsers.department_id", "_id", "originalCalledPartyDepartments")
            .Unwind("originalCalledPartyDepartments", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Group(new BsonDocument
            {
                { "_id", "$_id" },
                { "duration", new BsonDocument("$first", "$duration") },
                { "originalCalledParty", new BsonDocument("$first", "$originalCalledParty") },
                { "finalCalledParty", new BsonDocument("$first", "$finalCalledParty") },
                { "callingParty", new BsonDocument("$first", "$callingParty") },
                { "dateTime", new BsonDocument("$first", "$dateTime") },
                { "orig", new BsonDocument("$first", "$orig") },
                { "callingPartyUserName", new BsonDocument("$first", "$callingPartyUsers.name") },
                { "callingPartyDepartmentName", new BsonDocument("$first", "$callingPartyDepartments.name") },
                { "finalCalledPartyUserName", new BsonDocument("$first", "$finalCalledPartyUsers.name") },
                { "finalCalledPartyDepartmentName", new BsonDocument("$first", "$finalCalledPartyDepartments.name") },
                { "originalCalledPartyUserName", new BsonDocument("$first", "$originalCalledPartyUsers.name") },
                { "originalCalledPartyDepartmentName", new BsonDocument("$first", "$originalCalledPartyDepartments.name") },
                { "redirectReason", new BsonDocument("$first", "$lastRedirect.reason") },
                { "callDirection", new BsonDocument("$first", "$callDirection") }
            })
            .ToListAsync();

        var resultItems = items.Select(cdr => new CdrListItem
        {
            Id = cdr["_id"].AsObjectId.ToString(),
            Duration = cdr["duration"].AsInt32,
            HasRedirected = cdr["originalCalledParty"]["number"].AsString != cdr["finalCalledParty"]["number"].AsString,
            OriginalCalledPartyNumber = cdr["originalCalledParty"]["number"].AsString,
            CallingPartyNumber = cdr["callingParty"]["number"].AsString,
            DateTimeOrigination = cdr["dateTime"]["origination"].ToNullableUniversalTime() ?? default,
            DateTimeConnect = cdr["dateTime"]["connect"].ToNullableUniversalTime(),
            CallType = CdrDeciderHelper.DecideCallType(cdr["duration"].AsInt32, cdr["dateTime"]["connect"].ToNullableUniversalTime()),
            UserName = cdr.Contains("callingPartyUserName") && !cdr["callingPartyUserName"].IsBsonNull ? cdr["callingPartyUserName"].AsString : string.Empty,
            DepartmentName = cdr.Contains("callingPartyDepartmentName") && !cdr["callingPartyDepartmentName"].IsBsonNull ? cdr["callingPartyDepartmentName"].AsString : string.Empty,
            FinalCalledPartyNumber = cdr["finalCalledParty"]["number"].AsString,
            FinalCalledPartyUserName = cdr.Contains("finalCalledPartyUserName") && !cdr["finalCalledPartyUserName"].IsBsonNull ? cdr["finalCalledPartyUserName"].AsString : string.Empty,
            FinalCalledPartyDepartmentName = cdr.Contains("finalCalledPartyDepartmentName") && !cdr["finalCalledPartyDepartmentName"].IsBsonNull ? cdr["finalCalledPartyDepartmentName"].AsString : string.Empty,
            OriginalCalledPartyUserName = cdr.Contains("originalCalledPartyUserName") && !cdr["originalCalledPartyUserName"].IsBsonNull ? cdr["originalCalledPartyUserName"].AsString : string.Empty,
            OriginalCalledPartyDepartmentName = cdr.Contains("originalCalledPartyDepartmentName") && !cdr["originalCalledPartyDepartmentName"].IsBsonNull ? cdr["originalCalledPartyDepartmentName"].AsString : string.Empty,
            RedirectReason = cdr.Contains("redirectReason") && !cdr["redirectReason"].IsBsonNull ? (RedirectReason)cdr["redirectReason"].AsInt32 : (RedirectReason?)null,
            CallDirection = cdr.Contains("callDirection") && !cdr["callDirection"].IsBsonNull ? (CallDirection)cdr["callDirection"].AsInt32 : (CallDirection?)null // Add this line
        }).ToList();

        return new PagedResult<CdrListItem>
        {
            Items = resultItems,
            TotalCount = totalCount,
            PageSize = filter.PageSize,
            PageIndex = filter.PageIndex,
            TotalPages = totalPages
        };
    }

    public async Task<NumberStatistics> GetNumberStatisticsByNumberAsync(string number, DateTime startDate, DateTime endDate)
    {
        // Convert Turkey local dates to UTC for MongoDB query
        var (startUtc, endUtc) = TurkeyTimeProvider.ConvertDateRangeToUtc(startDate, endDate);
        
        // Base filter: date range + global filter
        var baseFilter = Builders<CdrRecord>.Filter.And(
            ApplyGlobalFilter(),
            Builders<CdrRecord>.Filter.Gte(x => x.DateTime!.Origination, startUtc),
            Builders<CdrRecord>.Filter.Lt(x => x.DateTime!.Origination, endUtc)
        );

        // Filter for incoming calls where this person received the call (finalCalledParty == number)
        var incomingFilter = Builders<CdrRecord>.Filter.And(
            baseFilter,
            Builders<CdrRecord>.Filter.Eq(x => x.CallDirection, CallDirection.Incoming),
            Builders<CdrRecord>.Filter.And(
                Builders<CdrRecord>.Filter.Ne(x => x.FinalCalledParty, null),
                Builders<CdrRecord>.Filter.Eq(x => x.FinalCalledParty!.Number, number)
            )
        );

        // Facet aggregation for incoming call statistics
        var aggregate = await _collection.Aggregate()
            .Match(incomingFilter)
            .Facet(
                // Answered: incoming calls where finalCalledParty == number && duration > 0
                new AggregateFacet<CdrRecord, BsonDocument>("answeredCalls", PipelineDefinition<CdrRecord, BsonDocument>.Create(new[]
                {
                    new BsonDocument("$match", new BsonDocument
                    {
                        { "duration", new BsonDocument("$gt", 0) }
                    }),
                    new BsonDocument("$count", "count")
                })),
                // Missed: incoming calls where finalCalledParty == number && duration == 0
                new AggregateFacet<CdrRecord, BsonDocument>("missedCalls", PipelineDefinition<CdrRecord, BsonDocument>.Create(new[]
                {
                    new BsonDocument("$match", new BsonDocument
                    {
                        { "duration", 0 }
                    }),
                    new BsonDocument("$count", "count")
                })),
                // Total incoming calls count
                new AggregateFacet<CdrRecord, BsonDocument>("totalIncoming", PipelineDefinition<CdrRecord, BsonDocument>.Create(new[]
                {
                    new BsonDocument("$count", "count")
                }))
            )
            .FirstOrDefaultAsync();

        // Separate query for redirected calls: incoming where originalCalledParty == number && finalCalledParty != number
        var redirectedFilter = Builders<CdrRecord>.Filter.And(
            baseFilter,
            Builders<CdrRecord>.Filter.Eq(x => x.CallDirection, CallDirection.Incoming),
            Builders<CdrRecord>.Filter.And(
                Builders<CdrRecord>.Filter.Ne(x => x.OriginalCalledParty, null),
                Builders<CdrRecord>.Filter.Eq(x => x.OriginalCalledParty!.Number, number)
            ),
            Builders<CdrRecord>.Filter.And(
                Builders<CdrRecord>.Filter.Ne(x => x.FinalCalledParty, null),
                Builders<CdrRecord>.Filter.Ne(x => x.FinalCalledParty!.Number, number)
            )
        );

        var redirectedCount = await _collection.CountDocumentsAsync(redirectedFilter);

        // Separate aggregation for all calls duration (min/max/avg from ALL calls, not just incoming)
        var allCallsFilter = Builders<CdrRecord>.Filter.And(
            ApplyGlobalFilter(),
            Builders<CdrRecord>.Filter.Or(
                Builders<CdrRecord>.Filter.And(
                    Builders<CdrRecord>.Filter.Ne(x => x.OriginalCalledParty, null),
                    Builders<CdrRecord>.Filter.Eq(x => x.OriginalCalledParty!.Number, number)
                ),
                Builders<CdrRecord>.Filter.And(
                    Builders<CdrRecord>.Filter.Ne(x => x.CallingParty, null),
                    Builders<CdrRecord>.Filter.Eq(x => x.CallingParty!.Number, number)
                ),
                Builders<CdrRecord>.Filter.And(
                    Builders<CdrRecord>.Filter.Ne(x => x.FinalCalledParty, null),
                    Builders<CdrRecord>.Filter.Eq(x => x.FinalCalledParty!.Number, number)
                )
            ),
            Builders<CdrRecord>.Filter.Gte(x => x.DateTime!.Origination, startUtc),
            Builders<CdrRecord>.Filter.Lt(x => x.DateTime!.Origination, endUtc)
        );

        var durationAggregate = await _collection.Aggregate()
            .Match(allCallsFilter)
            .Match(new BsonDocument("duration", new BsonDocument("$gt", 0)))
            .Group(new BsonDocument
            {
                { "_id", BsonNull.Value },
                { "minDuration", new BsonDocument("$min", "$duration") },
                { "maxDuration", new BsonDocument("$max", "$duration") },
                { "avgDuration", new BsonDocument("$avg", "$duration") }
            })
            .FirstOrDefaultAsync();

        var answeredCalls = aggregate?.Facets.FirstOrDefault(f => f.Name == "answeredCalls")?.Output<BsonDocument>().FirstOrDefault();
        var missedCalls = aggregate?.Facets.FirstOrDefault(f => f.Name == "missedCalls")?.Output<BsonDocument>().FirstOrDefault();
        var totalIncoming = aggregate?.Facets.FirstOrDefault(f => f.Name == "totalIncoming")?.Output<BsonDocument>().FirstOrDefault();

        var answeredCallCount = answeredCalls?["count"].AsInt32 ?? 0;
        var missedCallCount = missedCalls?["count"].AsInt32 ?? 0;
        var totalIncomingCount = totalIncoming?["count"].AsInt32 ?? 0;
        var minDuration = durationAggregate?["minDuration"].AsInt32 ?? 0;
        var maxDuration = durationAggregate?["maxDuration"].AsInt32 ?? 0;
        var avgDuration = durationAggregate?["avgDuration"].AsDouble ?? 0;

        // Cross-check missed calls against this operator's breaks
        var onBreakCallCount = 0;
        var breaksByPhone = await GetBreaksByPhoneNumberAsync(startUtc, endUtc);
        if (breaksByPhone.TryGetValue(number, out var userBreaks) && missedCallCount > 0)
        {
            // Get missed calls for this operator with origination times
            var missedCallsFilter = Builders<CdrRecord>.Filter.And(
                incomingFilter,
                Builders<CdrRecord>.Filter.Eq(x => x.Duration, 0)
            );
            var missedCallDocs = await _collection.Find(missedCallsFilter)
                .Project(Builders<CdrRecord>.Projection.Include(x => x.DateTime!.Origination))
                .ToListAsync();

            foreach (var call in missedCallDocs)
            {
                var origination = call["dateTime"]["origination"].ToUniversalTime();
                if (IsCallDuringBreak(origination, userBreaks))
                    onBreakCallCount++;
            }
        }

        return new NumberStatistics
        {
            Number = number,
            IncomingCallCount = totalIncomingCount + (int)redirectedCount, // Total incoming = answered + missed + redirected
            AnsweredCallCount = answeredCallCount,
            MissedCallCount = missedCallCount - onBreakCallCount,
            RedirectedCallCount = (int)redirectedCount,
            OnBreakCallCount = onBreakCallCount,
            MinDuration = minDuration,
            MaxDuration = maxDuration,
            AvgDuration = avgDuration
        };
    }

    public async Task<PagedResult<UserCallListItem>> GetUserLastCallsAsync(UserStatisticsFilter filter)
    {
        // Convert Turkey local dates to UTC for MongoDB query
        var (startUtc, endUtc) = TurkeyTimeProvider.ConvertDateRangeToUtc(filter.StartDate, filter.EndDate);
        
        var filterDefinition = Builders<CdrRecord>.Filter.And(
            ApplyGlobalFilter(),
            Builders<CdrRecord>.Filter.Or(
                Builders<CdrRecord>.Filter.And(
                    Builders<CdrRecord>.Filter.Ne(x => x.OriginalCalledParty, null),
                    Builders<CdrRecord>.Filter.Eq(x => x.OriginalCalledParty!.Number, filter.Number)
                ),
                Builders<CdrRecord>.Filter.And(
                    Builders<CdrRecord>.Filter.Ne(x => x.CallingParty, null),
                    Builders<CdrRecord>.Filter.Eq(x => x.CallingParty!.Number, filter.Number)
                ),
                Builders<CdrRecord>.Filter.And(
                    Builders<CdrRecord>.Filter.Ne(x => x.FinalCalledParty, null),
                    Builders<CdrRecord>.Filter.Eq(x => x.FinalCalledParty!.Number, filter.Number)
                )
            ),
            Builders<CdrRecord>.Filter.Gte(x => x.DateTime!.Origination, startUtc),
            Builders<CdrRecord>.Filter.Lt(x => x.DateTime!.Origination, endUtc)
        );

        var totalCount = await _collection.CountDocumentsAsync(filterDefinition);
        var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

        var items = await _collection.Aggregate()
            .Match(filterDefinition)
            .Sort(Builders<CdrRecord>.Sort.Descending(x => x.DateTime.Origination))
            .Skip((filter.PageIndex - 1) * filter.PageSize)
            .Limit(filter.PageSize)
            .Lookup("users", "callingParty.number", "phone_number", "callingPartyUsers")
            .Unwind("callingPartyUsers", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Lookup("departments", "callingPartyUsers.department_id", "_id", "callingPartyDepartments")
            .Unwind("callingPartyDepartments", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Lookup("users", "originalCalledParty.number", "phone_number", "originalCalledPartyUsers")
            .Unwind("originalCalledPartyUsers", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Lookup("departments", "originalCalledPartyUsers.department_id", "_id", "originalCalledPartyDepartments")
            .Unwind("originalCalledPartyDepartments", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Lookup("users", "finalCalledParty.number", "phone_number", "finalCalledPartyUsers")
            .Unwind("finalCalledPartyUsers", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Lookup("departments", "finalCalledPartyUsers.department_id", "_id", "finalCalledPartyDepartments")
            .Unwind("finalCalledPartyDepartments", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Group(new BsonDocument
            {
                { "_id", "$_id" },
                { "duration", new BsonDocument("$first", "$duration") },
                { "originalCalledParty", new BsonDocument("$first", "$originalCalledParty") },
                { "finalCalledParty", new BsonDocument("$first", "$finalCalledParty") },
                { "callingParty", new BsonDocument("$first", "$callingParty") },
                { "dateTime", new BsonDocument("$first", "$dateTime") },
                { "callingPartyUserName", new BsonDocument("$first", "$callingPartyUsers.name") },
                { "callingPartyDepartmentName", new BsonDocument("$first", "$callingPartyDepartments.name") },
                { "finalCalledPartyUserName", new BsonDocument("$first", "$finalCalledPartyUsers.name") },
                { "finalCalledPartyDepartmentName", new BsonDocument("$first", "$finalCalledPartyDepartments.name") },
                { "originalCalledPartyUserName", new BsonDocument("$first", "$originalCalledPartyUsers.name") },
                { "originalCalledPartyDepartmentName", new BsonDocument("$first", "$originalCalledPartyDepartments.name") },
                { "callDirection", new BsonDocument("$first", "$callDirection") }
            })
            .ToListAsync();

        var resultItems = items.Select(cdr => new UserCallListItem
        {
            Id = cdr["_id"].AsObjectId.ToString(),
            Duration = cdr["duration"].AsInt32,
            CallType = CdrDeciderHelper.DecideCallType(cdr["duration"].AsInt32, cdr["dateTime"]["connect"].ToNullableUniversalTime()),
            DateTimeOrigination = cdr["dateTime"]["origination"].ToNullableUniversalTime() ?? default,
            DateTimeDisconnect = cdr["dateTime"]["disconnect"].ToNullableUniversalTime(),
            CallingPartyNumber = cdr["callingParty"]["number"].AsString,
            OriginalCalledPartyNumber = cdr["originalCalledParty"]["number"].AsString,
            FinalCalledPartyNumber = cdr["finalCalledParty"]["number"].AsString,
            CallingPartyUserName = cdr.Contains("callingPartyUserName") && !cdr["callingPartyUserName"].IsBsonNull ? cdr["callingPartyUserName"].AsString : string.Empty,
            CallingPartyDepartmentName = cdr.Contains("callingPartyDepartmentName") && !cdr["callingPartyDepartmentName"].IsBsonNull ? cdr["callingPartyDepartmentName"].AsString : string.Empty,
            OriginalCalledPartyUserName = cdr.Contains("originalCalledPartyUserName") && !cdr["originalCalledPartyUserName"].IsBsonNull ? cdr["originalCalledPartyUserName"].AsString : string.Empty,
            OriginalCalledPartyDepartmentName = cdr.Contains("originalCalledPartyDepartmentName") && !cdr["originalCalledPartyDepartmentName"].IsBsonNull ? cdr["originalCalledPartyDepartmentName"].AsString : string.Empty,
            FinalCalledPartyUserName = cdr.Contains("finalCalledPartyUserName") && !cdr["finalCalledPartyUserName"].IsBsonNull ? cdr["finalCalledPartyUserName"].AsString : string.Empty,
            FinalCalledPartyDepartmentName = cdr.Contains("finalCalledPartyDepartmentName") && !cdr["finalCalledPartyDepartmentName"].IsBsonNull ? cdr["finalCalledPartyDepartmentName"].AsString : string.Empty,
            CallDirection = cdr.Contains("callDirection") && !cdr["callDirection"].IsBsonNull ? (CallDirection)cdr["callDirection"].AsInt32 : (CallDirection?)null
        }).ToList();

        return new PagedResult<UserCallListItem>
        {
            Items = resultItems,
            TotalCount = totalCount,
            PageSize = filter.PageSize,
            PageIndex = filter.PageIndex,
            TotalPages = totalPages
        };
    }

    public async Task<UserSpecificReport> GetUserCalls(StatisticsFilter filter)
    {
        // Convert Turkey local dates to UTC for MongoDB query
        var (startUtc, endUtc) = TurkeyTimeProvider.ConvertDateRangeToUtc(filter.StartDate, filter.EndDate);
        
        var filterDefinition = Builders<CdrRecord>.Filter.And(
            ApplyGlobalFilter(),
            Builders<CdrRecord>.Filter.Or(
                Builders<CdrRecord>.Filter.And(
                    Builders<CdrRecord>.Filter.Ne(x => x.OriginalCalledParty, null),
                    Builders<CdrRecord>.Filter.Eq(x => x.OriginalCalledParty!.Number, filter.Number)
                ),
                Builders<CdrRecord>.Filter.And(
                    Builders<CdrRecord>.Filter.Ne(x => x.CallingParty, null),
                    Builders<CdrRecord>.Filter.Eq(x => x.CallingParty!.Number, filter.Number)
                ),
                Builders<CdrRecord>.Filter.And(
                    Builders<CdrRecord>.Filter.Ne(x => x.FinalCalledParty, null),
                    Builders<CdrRecord>.Filter.Eq(x => x.FinalCalledParty!.Number, filter.Number)
                )
            ),
            Builders<CdrRecord>.Filter.Gte(x => x.DateTime!.Origination, startUtc),
            Builders<CdrRecord>.Filter.Lt(x => x.DateTime!.Origination, endUtc)
        );

        var items = await _collection.Aggregate()
            .Match(filterDefinition)
            .Sort(Builders<CdrRecord>.Sort.Descending(x => x.DateTime!.Origination))
            .Lookup("users", "callingParty.number", "phone_number", "callingPartyUsers")
            .Unwind("callingPartyUsers", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Lookup("departments", "callingPartyUsers.department_id", "_id", "callingPartyDepartments")
            .Unwind("callingPartyDepartments", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Lookup("users", "originalCalledParty.number", "phone_number", "originalCalledPartyUsers")
            .Unwind("originalCalledPartyUsers", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Lookup("departments", "originalCalledPartyUsers.department_id", "_id", "originalCalledPartyDepartments")
            .Unwind("originalCalledPartyDepartments", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Lookup("users", "finalCalledParty.number", "phone_number", "finalCalledPartyUsers")
            .Unwind("finalCalledPartyUsers", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Lookup("departments", "finalCalledPartyUsers.department_id", "_id", "finalCalledPartyDepartments")
            .Unwind("finalCalledPartyDepartments", new AggregateUnwindOptions<BsonDocument> { PreserveNullAndEmptyArrays = true })
            .Group(new BsonDocument
            {
                { "_id", "$_id" },
                { "duration", new BsonDocument("$first", "$duration") },
                { "originalCalledParty", new BsonDocument("$first", "$originalCalledParty") },
                { "finalCalledParty", new BsonDocument("$first", "$finalCalledParty") },
                { "callingParty", new BsonDocument("$first", "$callingParty") },
                { "dateTime", new BsonDocument("$first", "$dateTime") },
                { "callingPartyUserName", new BsonDocument("$first", "$callingPartyUsers.name") },
                { "callingPartyDepartmentName", new BsonDocument("$first", "$callingPartyDepartments.name") },
                { "finalCalledPartyUserName", new BsonDocument("$first", "$finalCalledPartyUsers.name") },
                { "finalCalledPartyDepartmentName", new BsonDocument("$first", "$finalCalledPartyDepartments.name") },
                { "originalCalledPartyUserName", new BsonDocument("$first", "$originalCalledPartyUsers.name") },
                { "originalCalledPartyDepartmentName", new BsonDocument("$first", "$originalCalledPartyDepartments.name") },
                { "redirectReason", new BsonDocument("$first", "$lastRedirect.reason") },
                { "callDirection", new BsonDocument("$first", "$callDirection") }
            }).ToListAsync();

        var resultItems = items.Select(cdr => new UserCallListItem
        {
            Id = cdr["_id"].AsObjectId.ToString(),
            Duration = cdr["duration"].AsInt32,
            CallType = CdrDeciderHelper.DecideCallType(cdr["duration"].AsInt32, cdr["dateTime"]["connect"].ToNullableUniversalTime()),
            DateTimeOrigination = cdr["dateTime"]["origination"].ToNullableUniversalTime() ?? default,
            DateTimeDisconnect = cdr["dateTime"]["disconnect"].ToNullableUniversalTime(),
            CallingPartyNumber = cdr["callingParty"]["number"].AsString,
            OriginalCalledPartyNumber = cdr["originalCalledParty"]["number"].AsString,
            FinalCalledPartyNumber = cdr["finalCalledParty"]["number"].AsString,
            CallingPartyUserName = cdr.Contains("callingPartyUserName") && !cdr["callingPartyUserName"].IsBsonNull ? cdr["callingPartyUserName"].AsString : string.Empty,
            CallingPartyDepartmentName = cdr.Contains("callingPartyDepartmentName") && !cdr["callingPartyDepartmentName"].IsBsonNull ? cdr["callingPartyDepartmentName"].AsString : string.Empty,
            OriginalCalledPartyUserName = cdr.Contains("originalCalledPartyUserName") && !cdr["originalCalledPartyUserName"].IsBsonNull ? cdr["originalCalledPartyUserName"].AsString : string.Empty,
            OriginalCalledPartyDepartmentName = cdr.Contains("originalCalledPartyDepartmentName") && !cdr["originalCalledPartyDepartmentName"].IsBsonNull ? cdr["originalCalledPartyDepartmentName"].AsString : string.Empty,
            FinalCalledPartyUserName = cdr.Contains("finalCalledPartyUserName") && !cdr["finalCalledPartyUserName"].IsBsonNull ? cdr["finalCalledPartyUserName"].AsString : string.Empty,
            FinalCalledPartyDepartmentName = cdr.Contains("finalCalledPartyDepartmentName") && !cdr["finalCalledPartyDepartmentName"].IsBsonNull ? cdr["finalCalledPartyDepartmentName"].AsString : string.Empty,
            // RedirectReason = cdr.Contains("redirectReason") && !cdr["redirectReason"].IsBsonNull ? (RedirectReason)cdr["redirectReason"].AsInt32 : (RedirectReason?)null,
            CallDirection = cdr.Contains("callDirection") && !cdr["callDirection"].IsBsonNull ? (CallDirection)cdr["callDirection"].AsInt32 : (CallDirection?)null
        }).OrderByDescending(o => o.DateTimeOrigination).ToList();

        var (workHours, nonWorkHours) = SeparateCallsByWorkHours(resultItems);

        // Fetch breaks for this operator to cross-check missed calls
        var breaksByPhone = await GetBreaksByPhoneNumberAsync(startUtc, endUtc);
        breaksByPhone.TryGetValue(filter.Number!, out var operatorBreaks);

        var workHoursStatistics = CalculateCallStatistics(workHours, filter.Number!, operatorBreaks);
        var nonWorkHoursStatistics = CalculateCallStatistics(nonWorkHours, filter.Number!, operatorBreaks);

        var breakTimes = GetBreakTimes(workHours, filter.Number!);

        // You can now use workHoursStatistics, nonWorkHoursStatistics, and breakTimes as needed

        return new UserSpecificReport
        {
            WorkHours = workHours,
            NonWorkHours = nonWorkHours,
            CallDetails = resultItems,
            WorkHoursStatistics = workHoursStatistics,
            NonWorkHoursStatistics = nonWorkHoursStatistics,
            BreakTimes = breakTimes
        };
    }

    private (List<UserCallListItem> WorkHours, List<UserCallListItem> NonWorkHours) SeparateCallsByWorkHours(List<UserCallListItem> calls)
    {
        var turkeyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");

        var workHours = calls.Where(call =>
        {
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(call.DateTimeOrigination, turkeyTimeZone).TimeOfDay;
            return localTime >= new TimeSpan(7, 45, 0) && localTime <= new TimeSpan(16, 45, 0);
        }).ToList();

        var nonWorkHours = calls.Where(call =>
        {
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(call.DateTimeOrigination, turkeyTimeZone).TimeOfDay;
            return localTime < new TimeSpan(7, 45, 0) || localTime > new TimeSpan(16, 45, 0);
        }).ToList();

        return (workHours, nonWorkHours);
    }

    private CallStatistics CalculateCallStatistics(List<UserCallListItem> calls, string number, List<Break>? operatorBreaks = null)
    {
        var totalCalls = calls.Count;
        var incomingCalls = calls.Where(call => call.CallDirection == CallDirection.Incoming).ToList();
        var outgoingCalls = calls.Count(call => call.CallDirection == CallDirection.Outgoing);
        var answeredCalls = incomingCalls.Count(call => call.Duration > 0 && call.FinalCalledPartyNumber == number);
        var missedCalls = incomingCalls.Count(call => call.Duration == 0 && call.FinalCalledPartyNumber == number);
        var redirectedCalls = incomingCalls.Count(call => call.OriginalCalledPartyNumber == number && call.FinalCalledPartyNumber != number);
        var totalDuration = calls.Sum(call => call.Duration);

        // Cross-check missed calls against breaks
        var onBreakCalls = 0;
        if (operatorBreaks != null && operatorBreaks.Count > 0)
        {
            var missedCallList = incomingCalls.Where(call => call.Duration == 0 && call.FinalCalledPartyNumber == number);
            foreach (var call in missedCallList)
            {
                if (IsCallDuringBreak(call.DateTimeOrigination, operatorBreaks))
                    onBreakCalls++;
            }
        }

        return new CallStatistics
        {
            TotalCalls = totalCalls,
            IncomingCalls = incomingCalls.Count,
            OutgoingCalls = outgoingCalls,
            AnsweredCalls = answeredCalls,
            MissedCalls = missedCalls - onBreakCalls,
            RedirectedCalls = redirectedCalls,
            OnBreakCalls = onBreakCalls,
            TotalDuration = totalDuration
        };
    }

    private List<BreakTime> GetBreakTimes(List<UserCallListItem> workHours, string number)
    {
        var breakTimes = new List<BreakTime>();
        UserCallListItem? breakStartCall = null;

        var incomingCalls = workHours.Where(call => call.CallDirection == CallDirection.Incoming).OrderBy(call => call.DateTimeOrigination);

        foreach (var call in incomingCalls)
        {
            var hasRedirected = call.OriginalCalledPartyNumber != call.FinalCalledPartyNumber;

            if (hasRedirected)
            {
                if (breakStartCall == null)
                {
                    breakStartCall = call;
                }
            }
            else if (breakStartCall != null)
            {
                breakTimes.Add(new BreakTime
                {
                    BreakStart = breakStartCall.DateTimeOrigination,
                    BreakEnd = call.DateTimeOrigination
                });
                breakStartCall = null;
            }
        }

        // If there is a break start without a corresponding break end, add it to the list
        if (breakStartCall != null)
        {
            breakTimes.Add(new BreakTime
            {
                BreakStart = breakStartCall.DateTimeOrigination,
                BreakEnd = null
            });
        }

        return breakTimes;
    }

    public async Task<DepartmentCallStatisticsByCallDirection> GetDepartmentCallStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        // Convert Turkey local dates to UTC for MongoDB query
        var (startUtc, endUtc) = TurkeyTimeProvider.ConvertDateRangeToUtc(startDate, endDate);
        
        var filter = Builders<CdrRecord>.Filter.And(
            ApplyGlobalFilter(),
            Builders<CdrRecord>.Filter.Gte(x => x.DateTime!.Origination, startUtc),
            Builders<CdrRecord>.Filter.Lt(x => x.DateTime!.Origination, endUtc)
        );

        var cdrList = await _collection.Find(filter).ToListAsync();

        var users = await _userCollection.Aggregate()
            .Lookup<Operator, Department, Operator>(
                _departmentCollection,
                u => u.DepartmentId,
                d => d.Id,
                u => u.Department)
            .Unwind(u => u.Department, new AggregateUnwindOptions<Operator> { PreserveNullAndEmptyArrays = true })
            .ToListAsync();

        users = users.DistinctBy(u => u.PhoneNumber).ToList();

        // Fetch all breaks in this time range for on-break call detection
        var breaksByPhone = await GetBreaksByPhoneNumberAsync(startUtc, endUtc);

        var departmentCalls = new DepartmentCallStatisticsByCallDirection
        {
            Incoming = cdrList
                .Where(cdr => cdr.CallDirection == CallDirection.Incoming)
                .SelectMany(cdr => new[]
                {
                    new { cdr, User = users.FirstOrDefault(u => u.PhoneNumber == cdr.CallingParty?.Number)},
                    new { cdr, User = users.FirstOrDefault(u => u.PhoneNumber == cdr.OriginalCalledParty?.Number) }
                })
                .Where(x => x.User != null && x.User.Department != null)
                .GroupBy(x => x.User.Department.Id)
                .Select(g =>
                {
                    var total = g.Count();
                    // Redirected: user is OriginalCalledParty but NOT FinalCalledParty (call forwarded away)
                    var redirected = g.Count(x =>
                        x.User?.PhoneNumber != null &&
                        x.cdr.OriginalCalledParty?.Number == x.User.PhoneNumber &&
                        x.cdr.FinalCalledParty?.Number != null &&
                        x.cdr.FinalCalledParty.Number != x.User.PhoneNumber);
                    // Answered: connected calls excluding redirected ones
                    var answered = g.Count(x =>
                        x.cdr.DateTime.Connect != null && x.cdr.Duration > 0 &&
                        !(x.User?.PhoneNumber != null &&
                          x.cdr.OriginalCalledParty?.Number == x.User.PhoneNumber &&
                          x.cdr.FinalCalledParty?.Number != null &&
                          x.cdr.FinalCalledParty.Number != x.User.PhoneNumber));
                    var missed = total - redirected - answered;
                    // Count on-break calls: non-redirected missed calls where the called party was on break
                    var onBreak = g.Count(x =>
                        !(x.User?.PhoneNumber != null &&
                          x.cdr.OriginalCalledParty?.Number == x.User.PhoneNumber &&
                          x.cdr.FinalCalledParty?.Number != null &&
                          x.cdr.FinalCalledParty.Number != x.User.PhoneNumber) &&
                        (x.cdr.DateTime.Connect == null || x.cdr.Duration == 0) &&
                        x.cdr.DateTime.Origination.HasValue &&
                        x.User?.PhoneNumber != null &&
                        breaksByPhone.TryGetValue(x.User.PhoneNumber, out var userBreaks) &&
                        IsCallDuringBreak(x.cdr.DateTime.Origination.Value, userBreaks));
                    return new DepartmentCallStatistics
                    {
                        DepartmentName = g.First().User.Department.Name,
                        TotalCalls = total,
                        AnsweredCalls = answered,
                        MissedCalls = missed - onBreak,
                        OnBreakCalls = onBreak,
                        RedirectedCalls = redirected,
                        AnsweredCallRate = (total - redirected - onBreak) > 0 ? Math.Round((double)answered / (total - redirected - onBreak) * 100, 2) : 0,
                    };
                }).ToList(),
            Outgoing = cdrList
                .Where(cdr => cdr.CallDirection == CallDirection.Outgoing)
                .SelectMany(cdr => new[]
                {
                    new { cdr, User = users.FirstOrDefault(u => u.PhoneNumber == cdr.CallingParty?.Number)},
                    new { cdr, User = users.FirstOrDefault(u => u.PhoneNumber == cdr.OriginalCalledParty?.Number) }
                })
                .Where(x => x.User != null && x.User.Department != null)
                .GroupBy(x => x.User.Department.Id)
                .Select(g => new DepartmentCallStatistics
                {
                    DepartmentName = g.First().User.Department.Name,
                    TotalCalls = g.Count(),
                    AnsweredCalls = g.Count(x => x.cdr.DateTime.Connect != null && x.cdr.Duration > 0),
                    MissedCalls = g.Count(x => x.cdr.DateTime.Connect == null || x.cdr.Duration == 0),
                    AnsweredCallRate = g.Count() > 0 ? Math.Round((double)g.Count(x => x.cdr.DateTime.Connect != null && x.cdr.Duration > 0) / g.Count() * 100, 2) : 0,
                }).ToList(),
            Internal = cdrList
                .Where(cdr => cdr.CallDirection == CallDirection.Internal)
                .SelectMany(cdr => new[]
                {
                    new { cdr, User = users.FirstOrDefault(u => u.PhoneNumber == cdr.CallingParty?.Number)},
                    new { cdr, User = users.FirstOrDefault(u => u.PhoneNumber == cdr.OriginalCalledParty?.Number) }
                })
                .Where(x => x.User != null && x.User.Department != null)
                .GroupBy(x => x.User.Department.Id)
                .Select(g => new DepartmentCallStatistics
                {
                    DepartmentName = g.First().User.Department.Name,
                    TotalCalls = g.Count(),
                    AnsweredCalls = g.Count(x => x.cdr.DateTime.Connect != null && x.cdr.Duration > 0),
                    MissedCalls = g.Count(x => x.cdr.DateTime.Connect == null || x.cdr.Duration == 0),
                    AnsweredCallRate = g.Count() > 0 ? Math.Round((double)g.Count(x => x.cdr.DateTime.Connect != null && x.cdr.Duration > 0) / g.Count() * 100, 2) : 0,
                }).ToList()
        };

        return departmentCalls;
    }

    /// <summary>
    /// Retrieves all breaks that overlap with the given UTC time range.
    /// Returns breaks grouped by user's phone number for efficient lookup.
    /// </summary>
    private async Task<Dictionary<string, List<Break>>> GetBreaksByPhoneNumberAsync(DateTime startUtc, DateTime endUtc)
    {
        // Find breaks that overlap with the time range
        var breakFilter = Builders<Break>.Filter.And(
            Builders<Break>.Filter.Lt(b => b.StartTime, endUtc),
            Builders<Break>.Filter.Or(
                // Break still ongoing (endTime is null) - use plannedEndTime for range check
                Builders<Break>.Filter.And(
                    Builders<Break>.Filter.Eq(b => b.EndTime, null),
                    Builders<Break>.Filter.Gt(b => b.PlannedEndTime, startUtc)
                ),
                // Break ended - endTime > startUtc
                Builders<Break>.Filter.And(
                    Builders<Break>.Filter.Ne(b => b.EndTime, null),
                    Builders<Break>.Filter.Gt(b => b.EndTime, startUtc)
                )
            )
        );

        var breaks = await _breakCollection.Find(breakFilter).ToListAsync();
        if (breaks.Count == 0) return new Dictionary<string, List<Break>>();

        // Get user phone numbers for these breaks
        var userIds = breaks.Select(b => b.UserId).Distinct().ToList();
        var userFilter = Builders<Operator>.Filter.In(u => u.Id, userIds);
        var users = await _userCollection.Find(userFilter).ToListAsync();
        var userPhoneMap = users.ToDictionary(u => u.Id, u => u.PhoneNumber);

        // Group breaks by phone number
        var result = new Dictionary<string, List<Break>>();
        foreach (var b in breaks)
        {
            if (userPhoneMap.TryGetValue(b.UserId, out var phone))
            {
                if (!result.ContainsKey(phone))
                    result[phone] = new List<Break>();
                result[phone].Add(b);
            }
        }
        return result;
    }

    /// <summary>
    /// Checks if a call origination time falls within any of the given breaks.
    /// Uses the effective end time: EndTime if break was ended, PlannedEndTime otherwise.
    /// For old breaks without PlannedEndTime, falls back to EndTime.
    /// </summary>
    private static bool IsCallDuringBreak(DateTime callOrigination, List<Break> breaks)
    {
        foreach (var b in breaks)
        {
            var effectiveEnd = b.EndTime ?? b.PlannedEndTime;
            // For old records without plannedEndTime (default DateTime), skip
            if (effectiveEnd == default) continue;
            if (callOrigination >= b.StartTime && callOrigination <= effectiveEnd)
                return true;
        }
        return false;
    }

    public async Task<(int WorkHoursCalls, int AfterHoursCalls)> GetWorkHoursCallCountsAsync(
        DateTime startDate, DateTime endDate, List<DateOnly> holidayDates)
    {
        var (startUtc, endUtc) = TurkeyTimeProvider.ConvertDateRangeToUtc(startDate, endDate);

        var filter = Builders<CdrRecord>.Filter.And(
            ApplyGlobalFilter(),
            Builders<CdrRecord>.Filter.Eq(x => x.CallDirection, CallDirection.Incoming),
            Builders<CdrRecord>.Filter.Gte(x => x.DateTime!.Origination, startUtc),
            Builders<CdrRecord>.Filter.Lt(x => x.DateTime!.Origination, endUtc)
        );

        var originationTimes = await _collection.Find(filter)
            .Project(Builders<CdrRecord>.Projection.Include(x => x.DateTime!.Origination))
            .ToListAsync();

        int workHours = 0, afterHours = 0;
        foreach (var doc in originationTimes)
        {
            var origination = doc["dateTime"]["origination"].ToUniversalTime();
            if (CdrReportHelper.IsWithinWorkHours(origination, holidayDates))
                workHours++;
            else
                afterHours++;
        }

        return (workHours, afterHours);
    }
}