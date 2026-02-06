# State Management with Pinia

**Last Updated**: January 2026  
**Library**: Pinia 2.x  

---

## 🏪 Pinia Overview

Pinia is the official state management library for Vue 3. It replaces Vuex with a simpler API.

### Store Anatomy

```typescript
// stores/exampleStore.ts
import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

export const useExampleStore = defineStore('example', () => {
  // STATE (Reactive)
  const count = ref(0);
  const items = ref<Item[]>([]);
  
  // COMPUTED (Derived State)
  const doubledCount = computed(() => count.value * 2);
  
  // ACTIONS (Mutations + Async)
  function increment() {
    count.value++;
  }
  
  async function fetchItems() {
    try {
      const response = await ApiService.get('/items');
      items.value = response.data;
    } catch (error) {
      console.error('Failed to fetch items', error);
    }
  }
  
  // RETURN (Expose to components)
  return { 
    count, 
    items, 
    doubledCount, 
    increment, 
    fetchItems 
  };
});
```

---

## 🏗️ CDR.Web Stores

### 1. auth.ts (Authentication)

```typescript
export const useAuthStore = defineStore('auth', () => {
  const user = ref<User | null>(null);
  const token = ref<string | null>(localStorage.getItem('token'));
  const isLoggedIn = computed(() => !!token.value);

  async function login(email: string, password: string) {
    const response = await ApiService.post('/account/login', { email, password });
    token.value = response.data.token;
    localStorage.setItem('token', token.value);
    // Fetch user profile
    await fetchProfile();
  }

  async function logout() {
    token.value = null;
    user.value = null;
    localStorage.removeItem('token');
  }

  async function fetchProfile() {
    const response = await ApiService.get('/account/profile');
    user.value = response.data;
  }

  return { user, token, isLoggedIn, login, logout, fetchProfile };
});
```

### 2. dashboard.ts (Dashboard Data)

```typescript
export const useDashboardStore = defineStore('dashboard', () => {
  const dailyStats = ref<DailyStat[]>([]);
  const weeklyStats = ref<WeeklyStat[]>([]);
  const topOperators = ref<Operator[]>([]);
  const loading = ref(false);

  async function loadDashboard(startDate: Date, endDate: Date) {
    loading.value = true;
    try {
      const [daily, weekly, operators] = await Promise.all([
        ApiService.get('/report/daily', { params: { startDate, endDate } }),
        ApiService.get('/report/weekly', { params: { startDate, endDate } }),
        ApiService.get('/report/top-operators')
      ]);
      
      dailyStats.value = daily.data;
      weeklyStats.value = weekly.data;
      topOperators.value = operators.data;
    } finally {
      loading.value = false;
    }
  }

  return { dailyStats, weeklyStats, topOperators, loading, loadDashboard };
});
```

### 3. operator.ts (Operator Management)

```typescript
export const useOperatorStore = defineStore('operator', () => {
  const operators = ref<Operator[]>([]);
  const selectedOperator = ref<Operator | null>(null);
  const stats = ref<OperatorStats | null>(null);

  async function fetchOperators() {
    const response = await ApiService.get('/operators');
    operators.value = response.data;
  }

  async function selectOperator(operatorId: string) {
    const response = await ApiService.get(`/operators/${operatorId}`);
    selectedOperator.value = response.data;
    await fetchOperatorStats();
  }

  async function fetchOperatorStats() {
    if (!selectedOperator.value) return;
    
    const response = await ApiService.get(
      `/report/operator/${selectedOperator.value.id}`
    );
    stats.value = response.data;
  }

  return { 
    operators, 
    selectedOperator, 
    stats, 
    fetchOperators, 
    selectOperator, 
    fetchOperatorStats 
  };
});
```

---

## 💾 Using Stores in Components

### Basic Usage

```typescript
import { useOperatorStore } from '@/stores/operator';

export default {
  setup() {
    const operatorStore = useOperatorStore();
    
    onMounted(async () => {
      await operatorStore.fetchOperators();
    });
    
    return {
      operators: computed(() => operatorStore.operators),
      loading: computed(() => operatorStore.loading),
      selectOperator: operatorStore.selectOperator
    };
  }
};
```

### With Script Setup

```vue
<script setup lang="ts">
import { useOperatorStore } from '@/stores/operator';
import { computed, onMounted } from 'vue';

const operatorStore = useOperatorStore();

// Access state with computed (auto-unwrapped)
const operators = computed(() => operatorStore.operators);

// Call actions directly
onMounted(async () => {
  await operatorStore.fetchOperators();
});
</script>

<template>
  <div>
    <div v-for="op in operators" :key="op.id">
      {{ op.name }}
      <button @click="operatorStore.selectOperator(op.id)">
        Select
      </button>
    </div>
  </div>
</template>
```

---

## 🔄 Store Patterns

### Pattern 1: Loading States

```typescript
const loading = ref(false);
const error = ref<string | null>(null);

async function fetchData() {
  loading.value = true;
  error.value = null;
  
  try {
    const response = await ApiService.get('/data');
    // Process data
  } catch (err) {
    error.value = err.message;
  } finally {
    loading.value = false;
  }
}
```

**In Component:**
```vue
<template>
  <div v-if="loading" class="spinner">Loading...</div>
  <div v-else-if="error" class="error">{{ error }}</div>
  <div v-else>{{ data }}</div>
</template>
```

### Pattern 2: Pagination

```typescript
const currentPage = ref(1);
const pageSize = ref(10);
const items = ref<Item[]>([]);
const totalCount = ref(0);

const paginatedItems = computed(() => {
  return items.value.slice(
    (currentPage.value - 1) * pageSize.value,
    currentPage.value * pageSize.value
  );
});

async function fetchPage() {
  const response = await ApiService.get('/items', {
    params: {
      page: currentPage.value,
      pageSize: pageSize.value
    }
  });
  
  items.value = response.data.items;
  totalCount.value = response.data.total;
}

function nextPage() {
  if (currentPage.value < Math.ceil(totalCount.value / pageSize.value)) {
    currentPage.value++;
    fetchPage();
  }
}
```

### Pattern 3: Filter & Search

```typescript
const allItems = ref<Item[]>([]);
const searchQuery = ref('');
const selectedDepartment = ref<string | null>(null);

const filteredItems = computed(() => {
  return allItems.value.filter(item => {
    const matchesSearch = 
      item.name.toLowerCase().includes(searchQuery.value.toLowerCase());
    
    const matchesDept = 
      !selectedDepartment.value || 
      item.department === selectedDepartment.value;
    
    return matchesSearch && matchesDept;
  });
});

function setSearch(query: string) {
  searchQuery.value = query;
}

function setDepartment(dept: string | null) {
  selectedDepartment.value = dept;
}
```

---

## 🔌 Store Subscriptions

Monitor store changes:

```typescript
const authStore = useAuthStore();

// Subscribe to changes
const unsubscribe = authStore.$subscribe((mutation, state) => {
  console.log('Store updated:', mutation, state);
  // Could save to localStorage
});

// Cleanup
onUnmounted(() => {
  unsubscribe();
});
```

---

## 📦 Store Organization

### File Structure
```
stores/
├── index.ts          # Re-export all stores
├── auth.ts           # Authentication state
├── dashboard.ts      # Dashboard data
├── operator.ts       # Operator data
├── breaksTime.ts     # Break management
└── userStatistics.ts # User statistics
```

### index.ts (Central Export)
```typescript
// Re-export for convenience
export { useAuthStore } from './auth';
export { useDashboardStore } from './dashboard';
export { useOperatorStore } from './operator';
```

**Usage:**
```typescript
import { useAuthStore, useOperatorStore } from '@/stores';
```

---

## 🎯 Best Practices

### ✅ DO

```typescript
// Actions with clear names
async function loadOperatorStats() { }
function selectOperator(id: string) { }

// Separate concerns
const authStore = useAuthStore();  // Auth logic
const dashboardStore = useDashboardStore();  // Dashboard data

// Handle loading/error states
const loading = ref(false);
const error = ref(null);
```

### ❌ DON'T

```typescript
// Generic names
async function fetch() { }

// Mixed concerns
const store = defineStore('everything', () => {
  // Auth + dashboard + operators all mixed
});

// Ignoring errors
async function loadData() {
  const response = await ApiService.get('/data');
  // No error handling!
}
```

---

## 🔍 Debugging with Vue DevTools

1. Install Vue DevTools browser extension
2. Open DevTools → Vue tab
3. Expand "Stores" to see all Pinia stores
4. Click on store to inspect state
5. Mutation history tracking

### DevTools Inspector
```
└─ example
    ├─ count: 5
    ├─ items: [...]
    ├─ doubledCount: 10
    └─ [Actions]
        ├─ increment
        └─ fetchItems
```

