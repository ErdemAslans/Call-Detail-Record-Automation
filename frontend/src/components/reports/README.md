# CDR Email Reports - Frontend Components

**Location**: `CDR.Web/src/components/reports/`  
**Last Updated**: January 2026  

---

## Overview

This directory contains Vue 3 components for the automated CDR email reporting feature. Administrators can generate reports, send them via email, and view execution history.

---

## Components

### CdrReportGenerator.vue

**Purpose**: Main component for generating and sending CDR reports.

**Features**:
- Report type selection (Weekly/Monthly)
- Optional custom date range picker
- Real-time metrics preview
- Email recipient management
- Delivery status tracking

**Usage**:
```vue
<template>
  <CdrReportGenerator />
</template>

<script setup lang="ts">
import CdrReportGenerator from '@/components/reports/CdrReportGenerator.vue'
</script>
```

**Props**: None (uses Pinia store for state)

**Events**: None (actions dispatch to store)

---

### ReportExecutionHistory.vue

**Purpose**: Display historical report executions with download and error viewing.

**Features**:
- Paginated execution list
- Status badges (Success/Failed/Running)
- Report download functionality
- Error message modal
- Duration formatting

**Usage**:
```vue
<template>
  <ReportExecutionHistory />
</template>

<script setup lang="ts">
import ReportExecutionHistory from '@/components/reports/ReportExecutionHistory.vue'
</script>
```

**Props**: None (uses Pinia store for state)

**Events**: None (actions dispatch to store)

---

## State Management

### emailReport Store

**Location**: `CDR.Web/src/stores/emailReport.ts`

**State**:
```typescript
interface EmailReportState {
  loading: boolean
  error: string | null
  currentReport: CdrEmailReportResponse | null
  executionHistory: ReportExecutionSummary[]
}
```

**Actions**:
| Action | Description |
|--------|-------------|
| `generateReport(params)` | Generate a new report |
| `sendReport(params)` | Send existing report to recipients |
| `fetchExecutionHistory()` | Load execution history |
| `downloadReport(executionId)` | Download report file |
| `clearError()` | Clear error state |

**Getters**:
| Getter | Description |
|--------|-------------|
| `hasCurrentReport` | Whether a report is loaded |
| `formattedPeriod` | Human-readable date range |
| `metricsDisplayData` | Formatted metrics for UI |

---

## API Integration

### Endpoints Used

| Endpoint | Store Action |
|----------|--------------|
| `POST /report/generate-email-report` | `generateReport()` |
| `POST /report/send-email-report` | `sendReport()` |
| `GET /report/execution-history` | `fetchExecutionHistory()` |
| `GET /report/download/{id}` | `downloadReport()` |

### Error Handling

All API errors are caught and stored in `emailReport.error`. Components display errors using Element Plus `ElMessage`:

```typescript
import { ElMessage } from 'element-plus'

watch(() => emailReportStore.error, (error) => {
  if (error) {
    ElMessage.error(error)
  }
})
```

---

## i18n Translations

Translation keys are located in `src/core/plugins/i18n.ts`:

**English** (`en.emailReports.*`):
- `title`: "Email Reports"
- `generate`: "Generate Report"
- `reportType`: "Report Type"
- etc.

**Turkish** (`tr.emailReports.*`):
- `title`: "E-posta Raporları"
- `generate`: "Rapor Oluştur"
- `reportType`: "Rapor Türü"
- etc.

---

## Routing

**Route**: `/email-reports`  
**Component**: `EmailReports.vue`  
**Authorization**: Admin role required

```typescript
{
  path: "/email-reports",
  name: "email-reports",
  component: () => import("@/views/dashboard/reports/EmailReports.vue"),
  meta: {
    pageTitle: "Email Reports",
    roles: ["Admin"]
  }
}
```

---

## Component Dependencies

```
EmailReports.vue (Page)
├── CdrReportGenerator.vue
│   ├── Element Plus: ElCard, ElForm, ElDatePicker, ElSelect, ElButton
│   ├── KTIcon (custom icon component)
│   └── emailReport store
│
└── ReportExecutionHistory.vue
    ├── Element Plus: ElTable, ElPagination, ElTag, ElDialog
    └── emailReport store
```

---

## Development Guidelines

### Adding New Metrics

1. Update `MetricsSummary` interface in `emailReport.ts`
2. Add translation keys in `i18n.ts` (en + tr)
3. Add metric card in `CdrReportGenerator.vue` metrics section

### Adding New Report Types

1. Update `ReportType` enum in `emailReport.ts`
2. Add option to report type selector
3. Update backend `CdrReportRequest` model
4. Add i18n translations

### Styling

Components use Element Plus with custom KT theme overrides. Use existing CSS variables:

```css
/* Primary colors */
var(--el-color-primary)
var(--el-color-success)
var(--el-color-warning)
var(--el-color-danger)

/* Background */
var(--el-bg-color)
var(--el-bg-color-overlay)
```

---

## Testing

### Unit Tests (Future)

```bash
# Run component tests
npm run test:unit -- --filter=reports

# Coverage
npm run test:unit -- --coverage
```

### E2E Tests (Future)

```bash
# Run Playwright tests
npm run test:e2e -- --grep="email reports"
```

---

## Troubleshooting

### Common Issues

1. **Report generation fails**
   - Check backend logs for MongoDB query errors
   - Verify date range is valid (< 366 days)

2. **Emails not sending**
   - Verify SMTP configuration in `appsettings.json`
   - Check Hangfire dashboard for job status

3. **Download returns 404**
   - Report may need regeneration
   - Check `ReportExecutionLogs` table for status

---

## See Also

- [Backend Documentation](../../../Cdr.Api/docs/08-Email-Reporting.md)
- [API Reference](../../../Cdr.Api/docs/09-API-Reference-EmailReports.md)
- [Vue 3 Composition API](https://vuejs.org/guide/extras/composition-api-faq.html)
- [Pinia Documentation](https://pinia.vuejs.org/)
