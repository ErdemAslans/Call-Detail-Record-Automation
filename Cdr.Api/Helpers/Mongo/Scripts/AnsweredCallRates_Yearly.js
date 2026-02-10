use('cdr');
db.getCollection('incoming_calls').aggregate([
  // Project year and flags for dateTime.connect and duration
  { $project: { 
      year: { $year: '$dateTime.origination' }, 
      connectAndDuration: { $cond: { if: { $and: [{ $ne: ['$dateTime.connect', null] }, { $gt: ['$duration', 0] }] }, then: 1, else: 0 } } 
  }},
  // Group by year and calculate total records and records with dateTime.connect and duration > 0
  { $group: { _id: { year: '$year' }, totalRecords: { $sum: 1 }, connectAndDuration: { $sum: '$connectAndDuration' } } },
  // Project the percentage of records with dateTime.connect and duration > 0
  { $project: { year: '$_id.year', totalRecords: 1, connectAndDuration: 1, percentage: { $multiply: [{ $divide: ['$connectAndDuration', '$totalRecords'] }, 100] } } },
  // Sort by year
  { $sort: { 'year': 1 } }
]);