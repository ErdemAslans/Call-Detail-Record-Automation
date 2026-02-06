use('cdr');
db.getCollection('incoming_calls').aggregate([
  // Match records within October 2023
  { $match: { 'dateTime.origination': { $gte: new Date('2023-10-30T00:00:00Z'), $lt: new Date('2024-11-06T00:00:00Z') } } },
  // Project year, month, and flags for dateTime.connect and duration
  { $project: { year: { $year: '$dateTime.origination' }, month: { $month: '$dateTime.origination' }, connectAndDuration: { $cond: { if: { $and: [{ $ne: ['$dateTime.connect', null] }, { $gt: ['$duration', 0] }] }, then: 1, else: 0 } } } },
  // Group by year and month and calculate total records and records with dateTime.connect and duration > 0
  { $group: { _id: { year: '$year', month: '$month' }, totalRecords: { $sum: 1 }, connectAndDuration: { $sum: '$connectAndDuration' } } },
  // Project the percentage of records with dateTime.connect and duration > 0
  { $project: { year: '$_id.year', month: '$_id.month', totalRecords: 1, connectAndDuration: 1, percentage: { $multiply: [{ $divide: ['$connectAndDuration', '$totalRecords'] }, 100] } } },
  // Sort by year and month
  { $sort: { 'year': 1, 'month': 1 } }
]);
