// MongoDB init script for CDR database
// Bu script container başladığında otomatik çalışır

// Admin kullanıcısı ile bağlan
db = db.getSiblingDB('admin');

// CDR database'ini oluştur
db = db.getSiblingDB('cdr');

// Collections oluştur
db.createCollection('incoming_calls');
db.createCollection('users');
db.createCollection('logs');

// İndeksler oluştur (performans için)
db.incoming_calls.createIndex({ "timestamp": 1 });
db.incoming_calls.createIndex({ "caller_number": 1 });
db.incoming_calls.createIndex({ "called_number": 1 });
db.incoming_calls.createIndex({ "date": 1 });

db.users.createIndex({ "phone_number": 1 }, { unique: true });

db.logs.createIndex({ "timestamp": 1 });
db.logs.createIndex({ "level": 1 });

// Uygulama için kullanıcı oluştur (güvenlik için)
db.createUser({
  user: "cdr_app",
  pwd: "cdr_app_password123",
  roles: [
    {
      role: "readWrite",
      db: "cdr"
    }
  ]
});

print("CDR database initialized successfully!");
