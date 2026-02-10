# API Integration with Axios

**Last Updated**: January 2026  
**HTTP Client**: Axios 1.7  

---

## 🌐 ApiService Overview

CDR.Web uses **ApiService** to encapsulate all HTTP requests with automatic token injection, error handling, and response interceptors.

### Location
```
src/core/services/ApiService.ts
```

---

## 🔧 ApiService Setup

### Initialization (main.ts)

```typescript
import ApiService from '@/core/services/ApiService';

const app = createApp(App);
app.use(createPinia());
app.use(router);

// Initialize ApiService with Vue app
ApiService.init(app);

app.mount('#app');
```

### Configuration

```typescript
// core/services/ApiService.ts
const API_URL = import.meta.env.VITE_APP_API_URL || 
                'https://localhost:5001/api';

class ApiService {
  private static instance: AxiosInstance;

  static init(app: App) {
    this.instance = axios.create({
      baseURL: API_URL,
      timeout: 30000,
      headers: {
        'Content-Type': 'application/json'
      }
    });

    // Add interceptors
    this.setupInterceptors();
  }
}
```

---

## 🔐 Interceptors

### Request Interceptor (JWT Token Injection)

```typescript
static setupInterceptors() {
  this.instance!.interceptors.request.use(
    (config) => {
      // Inject token from localStorage
      const token = localStorage.getItem('token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    },
    (error) => {
      return Promise.reject(error);
    }
  );
}
```

**Flow:**
```
1. Component calls: ApiService.get('/report/stats')
   ↓
2. Request interceptor runs
   ├─ Retrieve token from localStorage
   ├─ Add header: Authorization: Bearer <token>
   └─ Send request
   ↓
3. Backend validates token
   ├─ If valid: process request
   └─ If invalid: return 401
```

### Response Interceptor (Error Handling)

```typescript
this.instance!.interceptors.response.use(
  (response) => response,
  (error) => {
    const status = error.response?.status;

    if (status === 401) {
      // Unauthorized - redirect to login
      const authStore = useAuthStore();
      authStore.logout();
      router.push('/login');
    } else if (status === 403) {
      // Forbidden - show error message
      ElMessage.error('You do not have permission to access this resource');
    } else if (status === 500) {
      // Server error
      ElMessage.error('Server error. Please try again later.');
    }

    return Promise.reject(error);
  }
);
```

---

## 📡 API Methods

### GET Request

```typescript
// Simple GET
const response = await ApiService.get('/report/stats');
console.log(response.data);

// GET with query parameters
const response = await ApiService.get('/report/stats', {
  params: {
    startDate: '2024-01-01',
    endDate: '2024-01-31',
    departmentId: 5
  }
});
// URL: /report/stats?startDate=2024-01-01&endDate=2024-01-31&departmentId=5
```

### POST Request

```typescript
// Create new record
const response = await ApiService.post('/operators', {
  name: 'John Doe',
  extension: '101',
  departmentId: 1
});
console.log(response.data.id);  // New ID
```

### PUT Request

```typescript
// Update existing record
await ApiService.put('/operators/123', {
  name: 'Jane Doe',
  extension: '102'
});
```

### DELETE Request

```typescript
// Delete record
await ApiService.delete('/operators/123');
```

---

## 🔄 Async/Await Pattern

### Component Using API

```typescript
import { ref, onMounted } from 'vue';
import ApiService from '@/core/services/ApiService';

export default {
  setup() {
    const data = ref(null);
    const loading = ref(false);
    const error = ref(null);

    async function fetchData() {
      loading.value = true;
      error.value = null;
      
      try {
        const response = await ApiService.get('/data');
        data.value = response.data;
      } catch (err) {
        error.value = err.message;
      } finally {
        loading.value = false;
      }
    }

    onMounted(async () => {
      await fetchData();
    });

    return { data, loading, error, fetchData };
  }
};
```

### Store Using API

```typescript
// stores/reportStore.ts
export const useReportStore = defineStore('report', () => {
  const reports = ref<Report[]>([]);
  const loading = ref(false);

  async function fetchReports(startDate: Date, endDate: Date) {
    loading.value = true;
    
    try {
      const response = await ApiService.get('/report/list', {
        params: {
          startDate: startDate.toISOString(),
          endDate: endDate.toISOString()
        }
      });
      
      reports.value = response.data;
    } finally {
      loading.value = false;
    }
  }

  return { reports, loading, fetchReports };
});
```

---

## 💥 Error Handling

### Try-Catch Pattern

```typescript
async function importData(file: File) {
  try {
    const formData = new FormData();
    formData.append('file', file);
    
    const response = await ApiService.post('/import', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    
    ElMessage.success('Data imported successfully');
    return response.data;
    
  } catch (error: unknown) {
    if (axios.isAxiosError(error)) {
      // Axios error
      if (error.response?.status === 400) {
        ElMessage.error(error.response.data.message);
      } else if (error.response?.status === 413) {
        ElMessage.error('File too large');
      } else {
        ElMessage.error('Import failed');
      }
    } else {
      // Other error
      ElMessage.error('Unknown error occurred');
    }
  }
}
```

### Common HTTP Status Codes

| Status | Meaning | Action |
|--------|---------|--------|
| 200 | OK | Data returned |
| 201 | Created | Resource created |
| 400 | Bad Request | Invalid input - show validation error |
| 401 | Unauthorized | Redirect to login |
| 403 | Forbidden | Show permission error |
| 404 | Not Found | Resource doesn't exist |
| 500 | Server Error | Show generic error message |

---

## 📝 Common API Patterns

### Pattern 1: Pagination

```typescript
async function fetchOperators(page: number = 1, pageSize: number = 10) {
  const response = await ApiService.get('/operators', {
    params: { page, pageSize }
  });
  
  return {
    items: response.data.items,
    total: response.data.total
  };
}
```

### Pattern 2: Filtering & Sorting

```typescript
async function fetchReports(filter: ReportFilter) {
  const response = await ApiService.get('/reports', {
    params: {
      ...filter,
      sortBy: 'date',
      sortOrder: 'desc'
    }
  });
  
  return response.data;
}
```

### Pattern 3: Batch Operations

```typescript
async function deleteMultiple(ids: string[]) {
  await Promise.all(
    ids.map(id => ApiService.delete(`/operators/${id}`))
  );
}
```

### Pattern 4: File Upload

```typescript
async function uploadReport(file: File) {
  const formData = new FormData();
  formData.append('file', file);
  formData.append('type', 'monthly');
  
  return ApiService.post('/reports/upload', formData, {
    headers: {
      'Content-Type': 'multipart/form-data'
    },
    onUploadProgress: (progress) => {
      const percentCompleted = 
        Math.round((progress.loaded * 100) / progress.total);
      console.log(`Upload ${percentCompleted}%`);
    }
  });
}
```

### Pattern 5: Download File

```typescript
async function downloadReport(reportId: string) {
  const response = await ApiService.get(
    `/reports/${reportId}/download`,
    { responseType: 'blob' }
  );
  
  // Create download link
  const url = window.URL.createObjectURL(response.data);
  const link = document.createElement('a');
  link.href = url;
  link.download = `report-${reportId}.xlsx`;
  link.click();
}
```

---

## 🔌 Custom Axios Instance

For special cases (e.g., file download):

```typescript
const downloadInstance = axios.create({
  baseURL: API_URL,
  responseType: 'blob',
  timeout: 60000  // Longer timeout for large files
});

// Add token to downloads too
downloadInstance.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export { downloadInstance };
```

---

## 🧪 Testing API Calls

### Mock API Responses

```typescript
import { vi } from 'vitest';
import ApiService from '@/core/services/ApiService';

describe('Report API', () => {
  it('should fetch reports', async () => {
    // Mock the API response
    vi.spyOn(ApiService, 'get').mockResolvedValue({
      data: [
        { id: 1, name: 'Report 1' },
        { id: 2, name: 'Report 2' }
      ]
    });

    const response = await ApiService.get('/reports');
    expect(response.data).toHaveLength(2);
  });
});
```

---

## ⚠️ Security Considerations

### ✅ Best Practices
- Token in localStorage (accessible to XSS)
- Token injected automatically in all requests
- HTTPS enforced in production
- Axios timeout configured

### ⚠️ Considerations
- 🟡 localStorage vulnerable to XSS attacks
- 🟡 Consider httpOnly cookies for production
- 🟡 Implement CSRF token if needed
- 🟡 Validate all server responses

### 🔒 Improvement: httpOnly Cookies

```typescript
// Backend sets: Set-Cookie: token=...; httpOnly; Secure
// Frontend doesn't need to manage token

const instance = axios.create({
  baseURL: API_URL,
  withCredentials: true  // Send cookies with requests
});
```

