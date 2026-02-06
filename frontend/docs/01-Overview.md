# CDR.Web - Genel Bakış

**Last Updated**: January 2026  
**Framework**: Vue 3  
**Build Tool**: Vite  
**Language**: TypeScript  

---

## 📌 Proje Amacı

**CDR.Web**, CDR verilerini görselleştiren ve yöneten **Vue 3 admin dashboard**'dur. Operatör performansı, çağrı istatistikleri ve departman raporlarını Apache Charts ile görselleştirir.

**Ana Sorumluluklar:**
- CDR data visualization (charts, tables)
- Operator & department performance reports
- Break management
- User authentication & authorization
- Real-time dashboard updates
- Responsive design (web + mobile)

---

## 🔧 Tech Stack

| Katman | Teknoloji | Amaç |
|--------|-----------|------|
| **Framework** | Vue 3 | UI framework (Composition API) |
| **Language** | TypeScript | Type safety |
| **Build** | Vite 5 | Fast bundling & HMR |
| **State** | Pinia 2 | Store management |
| **Routing** | Vue Router 4 | Navigation & route guards |
| **HTTP** | Axios 1.7 | API requests |
| **UI Library** | Element Plus 2.7 | UI components |
| **Charts** | ApexCharts 3.51 | Data visualization |
| **Validation** | VeeValidate 4.13 | Form validation |
| **Styling** | Sass 1.77 | CSS preprocessing |
| **i18n** | Vue-i18n 9.13 | Internationalization |

---

## 🏗️ Mimari Katmanlar

```
┌─────────────────────────────────┐
│      Views (Pages)              │
│ • Dashboard, Reports            │
│ • Operator Stats, Breaks        │
└─────────────────┬───────────────┘
                  │
┌─────────────────▼───────────────┐
│    Components (UI Elements)     │
│ • Cards, Charts, Tables, Forms  │
└─────────────────┬───────────────┘
                  │
┌─────────────────▼───────────────┐
│  Stores (Pinia State)           │
│ • auth, dashboard, operator     │
│ • breaksTime, userStatistics    │
└─────────────────┬───────────────┘
                  │
┌─────────────────▼───────────────┐
│  Services (API Calls)           │
│ • ApiService (Axios)            │
│ • Interceptors (JWT auth)       │
└─────────────────┬───────────────┘
                  │
┌─────────────────▼───────────────┐
│   Cdr.Api (Backend)             │
│ • REST endpoints                │
│ • Authentication                │
│ • Data aggregation              │
└─────────────────────────────────┘
```

---

## 📂 Proje Yapısı

```
CDR.Web/
├── public/              # Static assets
├── src/
│   ├── components/      # Reusable Vue components
│   │   ├── calendar/
│   │   ├── cards/
│   │   ├── dashboard-default-widgets/
│   │   ├── kt-datatable/  # Custom data table
│   │   ├── modals/
│   │   └── widgets/
│   ├── core/           # App setup & plugins
│   │   ├── plugins/    # Vue plugins (i18n, validation, etc.)
│   │   └── services/   # ApiService
│   ├── layouts/        # Layout wrappers
│   │   ├── DefaultLayout.vue
│   │   └── ...
│   ├── router/         # Vue Router config
│   │   └── index.ts    # Routes & guards
│   ├── stores/         # Pinia stores
│   │   ├── auth.ts
│   │   ├── dashboard.ts
│   │   ├── operator.ts
│   │   └── ...
│   ├── views/          # Page components
│   │   ├── Dashboard.vue
│   │   ├── Reports/
│   │   └── ...
│   ├── assets/         # Images, fonts, etc.
│   ├── App.vue         # Root component
│   └── main.ts         # Entry point
├── index.html          # HTML template
├── vite.config.ts      # Vite configuration
├── tsconfig.json       # TypeScript config
└── package.json        # Dependencies
```

---

## 🚀 Başlangıç

### Ön Koşullar
- Node.js 16+
- npm or yarn
- Running Cdr.Api backend (default: https://localhost:5001)

### Kurulum
```bash
cd CDR.Web

# Install dependencies
npm install

# Start development server
npm run dev
```

**Output:**
```
  VITE v5.3.5  ready in 123 ms

  ➜  Local:   http://localhost:5173/
  ➜  press h to show help
```

### Konfigürasyon

#### Environment Variables (.env)
```env
VITE_APP_API_URL=https://localhost:5001/api
VITE_APP_TITLE=CDR Dashboard
```

**Location**: `.env.development`, `.env.production`

#### API Base URL (core/services/ApiService.ts)
```typescript
const API_URL = import.meta.env.VITE_APP_API_URL || 
                'https://localhost:5001/api';
```

---

## 🔐 Güvenlik Modeli

### Authentication Flow
```
1. User enters credentials
   ↓
2. POST /api/account/login
   ↓
3. Backend returns JWT token + refreshToken
   ↓
4. Store token in localStorage
   ↓
5. ApiService adds token to headers
   ↓
6. All requests include: Authorization: Bearer <token>
   ↓
7. If 401: redirect to login
   ↓
8. If 403: show unauthorized message
```

### Authorization
- **JWT Bearer Token**: Stateless auth
- **Role-Based Guards**: Routes protected by user roles
- **HTTPS Enforced**: Production only

---

## 🎯 Key Features

### 1. Responsive Dashboard
- Real-time call statistics
- Operator performance charts
- Department breakdowns

### 2. Reporting
- Daily/Weekly/Monthly reports
- Export to Excel
- Advanced filtering

### 3. User Management
- Login/Logout
- Role-based access control
- Profile management

### 4. Break Management
- Track operator breaks
- Break history
- Scheduling

---

## 📊 Integration with Cdr.Api

### Data Flow
```
Vue Component
    │
    ├─ Dispatch Pinia action
    │
    ├─ ApiService.get/post/put/delete()
    │
    ├─ Axios sends HTTP request
    │
    ├─ JWT interceptor adds token
    │
    ├─ Cdr.Api handles request
    │
    ├─ Response returned
    │
    ├─ Update Pinia store
    │
    └─ Component reactivity updates UI
```

### Example: Fetch Operator Stats
```typescript
// 1. In component
const { operatorStats } = useOperatorStore();
await fetchOperatorStats();  // Pinia action

// 2. In store (operator.ts)
async fetchOperatorStats() {
  const response = await ApiService.get('/report/operator-stats');
  this.operatorStats = response.data;
}

// 3. ApiService handles:
//    - Token injection
//    - Error handling
//    - Response intercepting
```

---

## 📚 Dokümantasyon Haritası

Derinlemesine öğrenme için:
- **Architecture**: [03-Architecture.md](03-Architecture.md)
- **State Management**: [04-StateManagement.md](04-StateManagement.md)
- **Routing**: [05-Routing.md](05-Routing.md)
- **API Integration**: [06-APIIntegration.md](06-APIIntegration.md)
- **Components**: [07-Components.md](07-Components.md)
- **Authentication**: [08-Authentication.md](08-Authentication.md)

---

## 💡 Key Concepts

| Konsept | Açıklama |
|---------|----------|
| **Composition API** | Vue 3 way to organize component logic |
| **Reactive References** | `ref()`, `computed()` for reactivity |
| **Pinia Stores** | Centralized state management |
| **Route Guards** | Meta-based role checking |
| **JWT Tokens** | Stateless authentication |
| **Interceptors** | Auto token injection, error handling |

---

## ⚠️ Security Considerations

- ✅ **JWT Bearer Tokens**: Stateless auth
- ✅ **HTTPS Enforced**: Production connections
- ✅ **Role-Based Guards**: Route protection
- ✅ **CORS Enabled**: API whitelisting
- ⚠️ **localStorage**: Token stored in browser (not httpOnly)
- ⚠️ **No CSRF Protection**: Token-based approach

---

## 🔄 Tipik Geliştirme Akışı

```
1. Create new Pinia store (stores/newStore.ts)
2. Add API methods (ApiService)
3. Create Vue component (views/ or components/)
4. Use store in component template
5. Add route to router (router/index.ts)
6. Test in browser

Example:
├─ stores/userStats.ts (state, actions)
├─ components/UserStatsCard.vue (UI)
├─ views/UserStatsPage.vue (layout)
└─ router/index.ts (route definition)
```

---

## 🛠️ Common Commands

```bash
# Development
npm run dev           # Start dev server

# Build
npm run build         # Production build
npm run build-only    # Build only (no type check)

# Type checking
npm run type-check    # Check TypeScript

# Linting
npm run lint          # Run ESLint with auto-fix

# Preview
npm run preview       # Preview production build
```

