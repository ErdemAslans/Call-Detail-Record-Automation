# API Reference: Email Reports

**Last Updated**: January 2026  
**OpenAPI Version**: 3.0.3  
**Base URL**: `/report`  

---

## Overview

The Email Reports API provides endpoints for generating, sending, and managing automated CDR (Call Detail Record) email reports. All endpoints require Admin role authorization.

---

## Authentication

All endpoints require JWT Bearer authentication with Admin role:

```http
Authorization: Bearer <jwt_token>
```

---

## Endpoints

### POST /report/generate-email-report

Generate a CDR email report with aggregated metrics. Optionally sends the report via email.

#### Request Headers

| Header | Type | Required | Description |
|--------|------|----------|-------------|
| `Authorization` | string | Yes | JWT Bearer token |
| `Content-Type` | string | Yes | `application/json` |

#### Request Body

```json
{
  "reportType": "Weekly",
  "startDate": "2026-01-06T00:00:00Z",
  "endDate": "2026-01-12T23:59:59Z",
  "sendEmail": true,
  "emailRecipients": [
    "admin@dogusoto.com",
    "manager@dogusoto.com"
  ]
}
```

| Field | Type | Required | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `reportType` | string | Yes | Report period type | `Weekly` or `Monthly` |
| `startDate` | datetime | No | Custom period start | ISO 8601 format |
| `endDate` | datetime | No | Custom period end | ISO 8601 format |
| `sendEmail` | boolean | No | Send report via email | Default: `false` |
| `emailRecipients` | string[] | No | Email addresses | Valid email format |

**Note**: If `startDate`/`endDate` are omitted, the system calculates the period based on `reportType`:
- **Weekly**: Previous Monday 00:00 to Sunday 23:59
- **Monthly**: Previous month 1st 00:00 to last day 23:59

#### Response: 200 OK

```json
{
  "reportId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "reportType": "Weekly",
  "generatedAt": "2026-01-13T02:00:15Z",
  "periodStartDate": "2026-01-06T00:00:00Z",
  "periodEndDate": "2026-01-12T23:59:59Z",
  "fileName": "CDR_Weekly_Report_2026-01-06_2026-01-12.xlsx",
  "fileSizeBytes": 45678,
  "recordsProcessed": 12345,
  "generationDurationMs": 2345,
  "metricsSummary": {
    "totalCalls": 12345,
    "answeredCalls": 10234,
    "missedCalls": 1567,
    "abandonedCalls": 544,
    "answerRate": 82.91,
    "averageCallDuration": 245.5,
    "averageWaitTime": 18.3,
    "peakHour": 14,
    "peakHourCalls": 789
  },
  "emailsSent": true,
  "recipientsCount": 2,
  "successfulDeliveries": 2,
  "failedDeliveries": 0,
  "deliveryStatus": [
    {
      "email": "admin@dogusoto.com",
      "status": "Sent",
      "sentAt": "2026-01-13T02:00:18Z",
      "errorMessage": null
    },
    {
      "email": "manager@dogusoto.com",
      "status": "Sent",
      "sentAt": "2026-01-13T02:00:19Z",
      "errorMessage": null
    }
  ]
}
```

#### Response: 400 Bad Request

```json
{
  "code": "ValidationError",
  "message": "Start date must be before end date",
  "details": null,
  "timestamp": "2026-01-13T02:00:00Z"
}
```

#### Response: 401 Unauthorized

```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401
}
```

#### Response: 403 Forbidden

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "detail": "User does not have Admin role"
}
```

#### Response: 408 Request Timeout

```json
{
  "code": "RequestTimeout",
  "message": "Report generation exceeded maximum allowed time",
  "details": null,
  "timestamp": "2026-01-13T02:30:00Z"
}
```

#### Response: 500 Internal Server Error

```json
{
  "code": "InternalServerError",
  "message": "An unexpected error occurred",
  "details": "SMTP connection failed",
  "timestamp": "2026-01-13T02:00:00Z"
}
```

#### cURL Example

```bash
curl -X POST "https://api.dogusoto.com/report/generate-email-report" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "reportType": "Weekly",
    "sendEmail": true,
    "emailRecipients": ["admin@dogusoto.com"]
  }'
```

---

### POST /report/send-email-report

Send a previously generated report to specified recipients. Uses Hangfire for asynchronous delivery.

#### Request Body

```json
{
  "reportExecutionId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "emailRecipients": [
    "finance@dogusoto.com",
    "operations@dogusoto.com"
  ]
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `reportExecutionId` | GUID | Yes | Previous report execution ID |
| `emailRecipients` | string[] | Yes | Target email addresses |

#### Response: 200 OK

```json
{
  "reportExecutionId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "totalRecipients": 2,
  "overallStatus": "Queued"
}
```

#### Response: 404 Not Found

```json
{
  "code": "ReportNotFound",
  "message": "Report execution with ID a1b2c3d4-e5f6-7890-abcd-ef1234567890 not found",
  "timestamp": "2026-01-13T02:00:00Z"
}
```

#### cURL Example

```bash
curl -X POST "https://api.dogusoto.com/report/send-email-report" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -H "Content-Type: application/json" \
  -d '{
    "reportExecutionId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "emailRecipients": ["finance@dogusoto.com"]
  }'
```

---

### GET /report/execution-history

Retrieve recent report execution history for monitoring and auditing.

#### Query Parameters

| Parameter | Type | Required | Default | Description |
|-----------|------|----------|---------|-------------|
| `count` | integer | No | 20 | Number of executions to retrieve |

#### Response: 200 OK

```json
[
  {
    "executionId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "reportType": "Weekly",
    "triggerType": "Scheduled",
    "periodStartDate": "2026-01-06T00:00:00Z",
    "periodEndDate": "2026-01-12T23:59:59Z",
    "executionStatus": "Completed",
    "startedAt": "2026-01-13T02:00:00Z",
    "completedAt": "2026-01-13T02:00:18Z",
    "recordsProcessed": 12345,
    "recipientsCount": 5,
    "successfulDeliveries": 5,
    "failedDeliveries": 0
  },
  {
    "executionId": "b2c3d4e5-f6a7-8901-bcde-f23456789012",
    "reportType": "Monthly",
    "triggerType": "OnDemand",
    "periodStartDate": "2025-12-01T00:00:00Z",
    "periodEndDate": "2025-12-31T23:59:59Z",
    "executionStatus": "Completed",
    "startedAt": "2026-01-01T02:00:00Z",
    "completedAt": "2026-01-01T02:01:45Z",
    "recordsProcessed": 54321,
    "recipientsCount": 3,
    "successfulDeliveries": 2,
    "failedDeliveries": 1
  }
]
```

#### cURL Example

```bash
curl -X GET "https://api.dogusoto.com/report/execution-history?count=10" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
```

---

### GET /report/download/{executionId}

Download the Excel report file for a specific execution.

#### Path Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `executionId` | GUID | Yes | Report execution ID |

#### Response: 200 OK

Returns binary Excel file with headers:

```http
Content-Type: application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
Content-Disposition: attachment; filename="CDR_Weekly_Report_2026-01-06_2026-01-12.xlsx"
```

#### Response: 404 Not Found

```json
{
  "code": "ReportNotFound",
  "message": "Report not found or file not available",
  "timestamp": "2026-01-13T02:00:00Z"
}
```

#### cURL Example

```bash
curl -X GET "https://api.dogusoto.com/report/download/a1b2c3d4-e5f6-7890-abcd-ef1234567890" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..." \
  -o report.xlsx
```

---

## Data Models

### CdrReportRequest

```typescript
interface CdrReportRequest {
  reportType: 'Weekly' | 'Monthly';
  startDate?: string;  // ISO 8601 datetime
  endDate?: string;    // ISO 8601 datetime
  sendEmail?: boolean;
  emailRecipients?: string[];
}
```

### CdrEmailReportResponse

```typescript
interface CdrEmailReportResponse {
  reportId: string;           // GUID
  reportType: string;
  generatedAt: string;        // ISO 8601 datetime
  periodStartDate: string;
  periodEndDate: string;
  fileName: string;
  fileSizeBytes: number;
  recordsProcessed: number;
  generationDurationMs: number;
  metricsSummary: MetricsSummary;
  emailsSent: boolean;
  recipientsCount: number;
  successfulDeliveries: number;
  failedDeliveries: number;
  deliveryStatus: EmailDeliveryStatus[];
}
```

### MetricsSummary

```typescript
interface MetricsSummary {
  totalCalls: number;
  answeredCalls: number;
  missedCalls: number;
  abandonedCalls: number;
  answerRate: number;          // Percentage (0-100)
  averageCallDuration: number; // Seconds
  averageWaitTime: number;     // Seconds
  peakHour: number;            // 0-23
  peakHourCalls: number;
}
```

### EmailDeliveryStatus

```typescript
interface EmailDeliveryStatus {
  email: string;
  status: 'Queued' | 'Sending' | 'Sent' | 'Failed' | 'Bounced';
  sentAt?: string;
  errorMessage?: string;
}
```

### ApiErrorResponse

```typescript
interface ApiErrorResponse {
  code: string;
  message: string;
  details?: string;
  timestamp: string;
}
```

---

## Error Codes

| Code | HTTP Status | Description |
|------|-------------|-------------|
| `ValidationError` | 400 | Invalid request parameters |
| `InvalidEmailAddresses` | 400 | One or more emails are invalid |
| `ReportNotFound` | 404 | Report execution ID not found |
| `RequestTimeout` | 408 | Report generation exceeded 30 min |
| `ReportGenerationFailed` | 500 | Failed to generate report |
| `InternalServerError` | 500 | Unexpected server error |

---

## Rate Limits

| Endpoint | Limit | Window |
|----------|-------|--------|
| `generate-email-report` | 10 requests | per hour per user |
| `send-email-report` | 20 requests | per hour per user |
| `execution-history` | 60 requests | per minute |
| `download` | 30 requests | per hour per user |

---

## Webhook Events (Future)

Reserved for future webhook integration:

| Event | Description |
|-------|-------------|
| `report.generated` | Report generation completed |
| `report.sent` | All emails delivered |
| `report.failed` | Report generation failed |
| `email.bounced` | Email delivery bounced |

---

## See Also

- [08-Email-Reporting.md](08-Email-Reporting.md) - Architecture documentation
- [03-Architecture.md](03-Architecture.md) - Overall API architecture
- [07-Hangfire.md](07-Hangfire.md) - Background job documentation
