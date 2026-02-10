# Copilot Instructions for CDR.Web

## Project Overview
CDR.Web is a **Vue 3 + TypeScript + Vite** frontend application built with a modern SPA architecture. It's an Enterprise UI template with role-based access control, JWT authentication, and a comprehensive component library.

## Architecture & Key Patterns

### Route Structure (`src/router/index.ts`)
- **Three-layout system**: `DefaultLayout` (authenticated), `AuthLayout` (login/signup), `SystemLayout` (error pages)
- **Auth middleware**: Routes check `to.meta.middleware === "auth"` with role-based access control
- **Role enforcement**: Routes define allowed roles in meta (e.g., `roles: ["Admin"]`); unauthorized access redirects to `/403`
- **Guard pattern**: `router.beforeEach()` validates tokens and enforces authentication before route access

### State Management (Pinia - `src/stores/`)
- **Central auth store** (`auth.ts`): Manages JWT tokens, user roles, and authentication state
- **Specialized stores**: `config.ts` (layout), `theme.ts` (dark/light mode), `body.ts` (CSS classes), `dashboard.ts`, `operator.ts`, `breaksTime.ts`, `userStatistics.ts`
- **Token handling**: JWT decoded from Microsoft identity claims (`http://schemas.microsoft.com/ws/2008/06/identity/claims/role`)
- **Role format**: Roles can be single values or arrays; always normalize to array in `setAuth()`

### Authentication & API (`src/core/services/`)
- **JwtService**: Stores/retrieves tokens from localStorage
- **ApiService**: Axios wrapper with **response interceptors** for global error handling
  - 401 errors: Redirect to sign-in with SweetAlert2 notification
  - 403 errors: Redirect to `/403` page
  - Other errors: Show ElMessage notifications
- **Base URL**: Set from `VITE_APP_API_URL` environment variable

### Layout System (`src/layouts/default-layout/`)
- **Component structure**: Header → Sidebar → Content → Footer (flexbox-based `kt_app_*` ID structure)
- **KT prefix convention**: All layout components use `KT` prefix (KTHeader, KTSidebar, etc.)
- **Page initialization**: 
  - On mount: Config overrides from localStorage, theme mode set, components initialized
  - Bootstrap tooltip directive registered globally via `app.directive("tooltip")`

### Component Patterns (`src/components/`)
- **Feature folders**: Organized by feature (calendar/, cards/, dashboard-default-widgets/, modals/, wizards/, etc.)
- **Reusable widgets**: Dashboard widgets (Widget1-Widget10+), activity timeline items, data tables
- **Third-party integration**: ApexCharts, Quill editor, FullCalendar, TinyMCE, Element Plus

## Critical Developer Workflows

### Build & Development
```bash
npm install                    # Install dependencies
npm run dev                   # Start dev server (Vite hot reload)
npm run build                 # Type-check + bundle for production
npm run type-check            # Run vue-tsc validation
npm run lint                  # ESLint fix on all Vue/TS files
```

### Key Configuration Files
- **vite.config.ts**: `@` alias points to `./src`, chunk size limit 3000KB
- **tsconfig.json**: BaseUrl `/`, `@/*` maps to `./src/*`, extends `@vue/tsconfig/tsconfig.web.json`
- **.env**: Requires `VITE_APP_API_URL` and `VITE_APP_NAME` variables

## Project-Specific Conventions

### Naming & Structure
- **Stores**: Verb + noun pattern (`breaksTime.ts`, `userStatistics.ts`); use `defineStore("storeName", () => {...})`
- **Router meta**: Always include `pageTitle` and `breadcrumbs` for navigation
- **Component names**: Match file names (PascalCase), prefix layout components with `KT`
- **API constants**: Centralized in `src/stores/consts/ApiUrlConstants.ts`

### State Flow
```
API Response → ApiService Interceptors → Error Handling (Swal/ElMessage)
→ On auth errors: Router guards + purgeAuth() + redirect to sign-in
```

### Form Validation
- Uses **Vee-Validate** + **Yup** for validation schema
- Initialized via `initVeeValidate()` in main.ts

### Styling
- **SCSS architecture**: `src/assets/sass/` with imported third-party themes (Bootstrap, Element Plus, ApexCharts)
- **Icon systems**: Bootstrap Icons, FontAwesome, SocIcon, Line Awesome, KT duotone/outline/solid icons
- **Global styles**: Applied in `App.vue`'s style block

## Integration Points & External Dependencies

| Dependency | Purpose | Usage |
|-----------|---------|-------|
| **Vue Router 4** | Client-side routing | Route guards + meta-based authorization |
| **Pinia 2** | State management | Store definitions in `src/stores/` |
| **Axios + VueAxios** | HTTP client | ApiService wrapper with interceptors |
| **Element Plus** | UI component library | Form inputs, messages, dialogs |
| **ApexCharts** | Chart library | Dashboard visualizations |
| **i18n** | Internationalization | Error/UI messages via i18n.global.t() |
| **Bootstrap 5** | CSS framework | Layout grid, utilities |
| **JWT-Decode** | Token parsing | Extract claims for user roles |
| **SweetAlert2** | Modal dialogs | Auth session expiry notifications |

## Common Patterns to Follow

1. **New routes**: Always include `meta: { pageTitle, breadcrumbs, roles?, middleware }` in route definition
2. **New stores**: Use Pinia composition API (ref/computed), export useXyzStore function
3. **New components**: Keep in feature folder, use relative imports for sibling components
4. **API calls**: Use `ApiService.post(apiUrlConstants.ENDPOINT, data)` pattern for consistency
5. **Role checks**: Use `authStore.hasRole(roleName)` method, not direct array access
6. **Error handling**: Trust ApiService interceptors for HTTP errors; handle domain logic errors in components
