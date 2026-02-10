# CDR.Web Project Structure

**Last Updated**: January 2026  

---

## 📁 Complete Project Layout

```
CDR.Web/
├── docs/                    # 📚 DOCUMENTATION
│   ├── 00-README.md
│   ├── 01-Overview.md
│   ├── 03-Architecture.md
│   ├── 04-StateManagement.md
│   └── 06-APIIntegration.md
│
├── public/                  # 🖼️ STATIC ASSETS
│   ├── splash-screen.css
│   └── media/
│       ├── images/
│       ├── icons/
│       └── fonts/
│
├── src/                     # 🔧 SOURCE CODE
│   ├── App.vue              # Root component
│   ├── main.ts              # Entry point (Pinia, Router, i18n)
│   │
│   ├── core/                # Core framework setup
│   │   ├── plugins/         # Vue plugins
│   │   │   ├── apexcharts.ts    # Chart library init
│   │   │   ├── i18n.ts          # Internationalization
│   │   │   ├── inline-svg.ts    # SVG loader
│   │   │   ├── keenthemes.ts    # Icon library
│   │   │   ├── prismjs.ts       # Code highlighting
│   │   │   └── vee-validate.ts  # Form validation
│   │   │
│   │   └── services/
│   │       └── ApiService.ts    # Axios HTTP client ⭐
│   │
│   ├── assets/              # Images, fonts, etc.
│   │   ├── css/
│   │   ├── fonts/
│   │   ├── images/
│   │   └── scss/
│   │
│   ├── components/          # 🧩 Reusable components
│   │   ├── activity-timeline-items/
│   │   ├── calendar/
│   │   ├── cards/           # Dashboard cards
│   │   │   ├── StatCard.vue
│   │   │   └── ChartCard.vue
│   │   ├── customers/       # Customer-related components
│   │   ├── dashboard-default-widgets/  # Dashboard widgets
│   │   ├── kt-datatable/    # Custom data table component
│   │   ├── menu/            # Menu components
│   │   ├── modals/          # Modal dialogs
│   │   ├── page-layouts/    # Layout components
│   │   ├── widgets/         # Utility widgets
│   │   └── ...
│   │
│   ├── layouts/             # 📄 Layout wrappers
│   │   ├── DefaultLayout.vue    # Main layout (header, sidebar)
│   │   ├── AuthLayout.vue       # Auth page layout
│   │   └── ...
│   │
│   ├── router/              # 🗺️ ROUTING
│   │   └── index.ts         # Route definitions & guards
│   │       ├── Routes config
│   │       ├── Role-based guards
│   │       └── Meta definitions
│   │
│   ├── stores/              # 🏪 PINIA STATE MANAGEMENT
│   │   ├── auth.ts          # Authentication state ⭐
│   │   ├── body.ts          # Layout state
│   │   ├── breaksTime.ts    # Break management state
│   │   ├── config.ts        # App config
│   │   ├── dashboard.ts     # Dashboard data
│   │   ├── operator.ts      # Operator data
│   │   ├── theme.ts         # Theme/styling state
│   │   ├── userStatistics.ts # User stats
│   │   └── consts/          # Store constants
│   │
│   └── views/               # 📄 PAGE COMPONENTS
│       ├── Dashboard.vue        # Main dashboard
│       ├── Reports/             # Reporting pages
│       │   ├── DailyReport.vue
│       │   ├── WeeklyReport.vue
│       │   └── MonthlyReport.vue
│       ├── Operators/           # Operator pages
│       │   ├── OperatorList.vue
│       │   └── OperatorDetail.vue
│       ├── Breaks/              # Break management
│       │   └── BreakSchedule.vue
│       ├── Auth/                # Auth pages
│       │   ├── Login.vue
│       │   └── Register.vue
│       └── ...
│
├── index.html               # HTML entry point
├── env.d.ts                 # Environment type definitions
├── vite.config.ts           # Vite configuration
├── tsconfig.json            # TypeScript config
├── tsconfig.config.json     # TypeScript config for vite
├── .eslintrc.cjs            # ESLint config
├── .prettierrc.json         # Code formatter config
├── package.json             # Dependencies
└── package-lock.json        # Dependency lock file
```

---

## 🎯 Where to Find Things

### I need to...

#### **Understand the project**
- Start: [01-Overview.md](docs/01-Overview.md)
- Then: [03-Architecture.md](docs/03-Architecture.md)

#### **Work with state management**
- Go to: [04-StateManagement.md](docs/04-StateManagement.md)
- Files: `stores/` directory

#### **Make API calls**
- Go to: [06-APIIntegration.md](docs/06-APIIntegration.md)
- File: `core/services/ApiService.ts`

#### **Create a new component**
1. Create file: `src/components/MyComponent.vue`
2. Follow structure: Template + Script setup + Scoped styles
3. Import in parent/page
4. Example: [03-Architecture.md](docs/03-Architecture.md#single-file-component-sfc)

#### **Add a new page**
1. Create store: `src/stores/myStore.ts`
2. Add API methods to store
3. Create view: `src/views/MyPage.vue`
4. Add route: `src/router/index.ts`
5. Add menu item in layout

#### **Debug state issues**
- Use Vue DevTools → Stores tab
- View store state in real-time
- Trace mutations

#### **Handle errors from API**
- Check: ApiService interceptors in `core/services/ApiService.ts`
- Check: Component try-catch blocks
- Check: Browser console for network errors

---

## 🔑 Key Files by Responsibility

### Entry & Configuration
- `src/main.ts` - App initialization, plugins, stores
- `index.html` - HTML template
- `vite.config.ts` - Build configuration

### Core Services
- `src/core/services/ApiService.ts` - HTTP client with interceptors ⭐
- `src/core/plugins/` - Vue plugins setup

### Routing & Navigation
- `src/router/index.ts` - Route definitions, guards

### State Management
- `src/stores/auth.ts` - Authentication (login, user info)
- `src/stores/dashboard.ts` - Dashboard data
- `src/stores/operator.ts` - Operator data
- `src/stores/*.ts` - Other domain stores

### UI Components
- `src/components/` - Reusable components
- `src/layouts/` - Page layouts
- `src/views/` - Page components

### Styling
- `src/assets/scss/` - Global styles
- Component `<style scoped>` blocks

---

## 🔄 Component Lifecycle Example

### User Login Flow

```
1. User enters credentials in Login.vue
   ↓
2. Form validation (VeeValidate)
   ↓
3. Call store action: authStore.login(email, password)
   ↓
4. Store action calls: ApiService.post('/account/login', {...})
   ↓
5. ApiService:
   ├─ Adds Content-Type header
   ├─ Sends request to backend
   ├─ Response interceptor processes response
   └─ Returns { token, refreshToken }
   ↓
6. Store saves token to localStorage
   ↓
7. Store updates reactive state: isLoggedIn = true
   ↓
8. Component reactivity updates UI
   ↓
9. Router guard checks isLoggedIn
   ↓
10. Redirect to Dashboard
```

---

## 📦 Dependencies Overview

### Main Libraries
```json
{
  "vue": "^3.4.34",
  "vite": "^5.3.5",
  "typescript": "5.3.3",
  "pinia": "^2.2.0",
  "vue-router": "^4.4.0",
  "axios": "^1.7.2",
  "element-plus": "^2.7.8",
  "apexcharts": "^3.51.0",
  "vee-validate": "^4.13.2",
  "vue-i18n": "9.13.1"
}
```

**Key choices:**
- **Pinia** over Vuex (simpler API)
- **Composition API** (better logic organization)
- **Element Plus** (complete UI library)
- **Axios** (flexible HTTP client)

---

## 🧪 Running & Building

### Development
```bash
npm run dev
# http://localhost:5173
```

### Type Checking
```bash
npm run type-check
```

### Linting
```bash
npm run lint
```

### Build for Production
```bash
npm run build
# Outputs to dist/

# Preview build
npm run preview
```

---

## 🔐 Authentication Flow

### Login
```
1. Login.vue submits form
   ↓
2. authStore.login(email, password)
   ↓
3. ApiService.post('/account/login', {email, password})
   ↓
4. Backend returns: { token, refreshToken }
   ↓
5. Store saves token to localStorage
   ↓
6. Router redirects to dashboard (auth guard)
```

### Protected Routes
```typescript
// router/index.ts
{
  path: '/dashboard',
  meta: { requiresAuth: true, roles: ['Admin', 'Manager'] },
  beforeEnter: (to, from, next) => {
    const authStore = useAuthStore();
    if (!authStore.isLoggedIn) {
      next('/login');
    } else if (to.meta.roles && !hasRequiredRole(authStore.user.role)) {
      next('/unauthorized');
    } else {
      next();
    }
  }
}
```

### API Request with Token
```
Every request includes:
Authorization: Bearer <token>

Via ApiService interceptor (automatic)
```

---

## 📁 Organizing New Features

### Example: Adding "User Reports" Feature

1. **Create Store**
   ```
   src/stores/userReports.ts
   - state: reports, selectedReport, loading
   - actions: fetchReports(), deleteReport()
   ```

2. **Create API Integration**
   ```
   In store:
   async function fetchReports() {
     const response = await ApiService.get('/report/user-reports');
     this.reports = response.data;
   }
   ```

3. **Create View**
   ```
   src/views/UserReports.vue
   - Fetch data on mount
   - Display using v-for
   - Show loading state
   ```

4. **Create Components (Optional)**
   ```
   src/components/UserReportCard.vue
   - Display single report
   - Emit events to parent
   ```

5. **Add Route**
   ```
   src/router/index.ts
   {
     path: '/user-reports',
     component: () => import('@/views/UserReports.vue'),
     meta: { requiresAuth: true }
   }
   ```

6. **Add Menu Item**
   ```
   DefaultLayout.vue sidebar
   - Link to /user-reports
   ```

---

## 💡 Key Concepts

| Concept | File | Explanation |
|---------|------|------------|
| **Reactive State** | stores/ | Pinia stores manage app state |
| **API Calls** | core/services/ApiService.ts | Axios with auto token injection |
| **Routing** | router/index.ts | Vue Router with guards |
| **Components** | components/, views/ | Vue SFCs (Composition API) |
| **Styles** | assets/scss/ + scoped | Sass + CSS modules |
| **Type Safety** | *.ts | TypeScript throughout |

---

## ⚠️ Security Checklist

- ✅ JWT token stored in localStorage
- ✅ Interceptor injects token in requests
- ✅ 401 redirects to login
- ✅ Route guards check authentication
- ⚠️ Consider: httpOnly cookies for production
- ⚠️ Consider: CSRF token if needed
- ⚠️ Consider: XSS protection (Vue auto-escapes)

---

## 🛠️ Common Tasks

### Fetch data on component mount
```typescript
onMounted(async () => {
  const store = useMyStore();
  await store.fetchData();
});
```

### Show loading state
```vue
<div v-if="loading" class="spinner">Loading...</div>
<div v-else>{{ data }}</div>
```

### Handle API errors
```typescript
try {
  await store.fetchData();
} catch (error) {
  ElMessage.error('Failed to fetch data');
}
```

### Update store from component
```typescript
// In component
await store.updateItem(id, newData);

// Store action
async function updateItem(id, data) {
  await ApiService.put(`/items/${id}`, data);
  // Update local state
  const index = items.value.findIndex(i => i.id === id);
  if (index !== -1) items.value[index] = data;
}
```

---

## 📚 Full Documentation Index

| Topic | File |
|-------|------|
| Overview | [01-Overview.md](docs/01-Overview.md) |
| Architecture | [03-Architecture.md](docs/03-Architecture.md) |
| State Management | [04-StateManagement.md](docs/04-StateManagement.md) |
| API Integration | [06-APIIntegration.md](docs/06-APIIntegration.md) |

---

## 🚀 Quick Start

```bash
# Install
npm install

# Develop
npm run dev

# Type check
npm run type-check

# Build
npm run build
```

