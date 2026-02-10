# Vue 3 Architecture & Patterns

**Last Updated**: January 2026  
**Pattern**: Composition API + TypeScript  

---

## 🏗️ Composition API Overview

Vue 3 **Composition API** organizes component logic into reusable functions.

### vs Options API

#### ❌ Options API (Old)
```javascript
export default {
  data() { return { count: 0 } },
  computed: { doubled() { return this.count * 2 } },
  methods: { increment() { this.count++ } }
}
```

#### ✅ Composition API (Vue 3)
```typescript
export default {
  setup() {
    const count = ref(0);
    const doubled = computed(() => count.value * 2);
    const increment = () => count.value++;
    
    return { count, doubled, increment };
  }
}
```

**Benefits:**
- Logic grouped by feature (not by type)
- Easy code reuse (composables)
- Better TypeScript support
- Smaller bundle size

---

## 🔄 Reactivity System

### ref() - Reactive References

```typescript
import { ref } from 'vue';

const count = ref(0);
console.log(count.value);  // 0
count.value++;             // Must access .value
```

**When to use:**
- Primitive values (string, number, boolean)
- External state that needs reactive updates

### reactive() - Reactive Objects

```typescript
import { reactive } from 'vue';

const state = reactive({
  count: 0,
  user: { name: 'John' }
});

state.count++;      // Direct access (no .value!)
console.log(state.user.name);
```

**When to use:**
- Objects with multiple properties
- Nested structures

### computed() - Derived State

```typescript
import { computed, ref } from 'vue';

const count = ref(0);

// Computed properties are cached
const doubled = computed(() => count.value * 2);

// Watchable computed
const doubledWritable = computed({
  get: () => count.value * 2,
  set: (val) => count.value = val / 2
});
```

**Benefits:**
- Cached (only recalculates when dependencies change)
- Lazy (only runs when accessed)
- Reactive

---

## 👁️ Watchers

### watch() - React to Changes

```typescript
import { watch, ref } from 'vue';

const count = ref(0);

// Simple watch
watch(count, (newValue, oldValue) => {
  console.log(`Count changed from ${oldValue} to ${newValue}`);
});

// Watch multiple sources
watch([count, name], ([newCount, newName], [oldCount, oldName]) => {
  console.log(`Count or name changed`);
});

// Watch with options
watch(count, () => {
  console.log('Count changed');
}, { 
  immediate: true,     // Run immediately
  deep: true           // Watch nested properties
});
```

### watchEffect() - Auto-Tracking

```typescript
import { watchEffect, ref } from 'vue';

const count = ref(0);

// Automatically tracks all reactive dependencies
watchEffect(() => {
  console.log(`Count is now ${count.value}`);
  // This runs whenever count.value changes
});
```

---

## 🎣 Composables (Reusable Logic)

### Pattern: Custom Composable

```typescript
// composables/useCounter.ts
import { ref, computed } from 'vue';

export function useCounter(initialValue = 0) {
  const count = ref(initialValue);
  
  const doubled = computed(() => count.value * 2);
  
  const increment = () => count.value++;
  const decrement = () => count.value--;
  const reset = () => count.value = initialValue;
  
  return { count, doubled, increment, decrement, reset };
}
```

### Using Composables

```vue
<template>
  <div>
    <p>Count: {{ count }}</p>
    <p>Doubled: {{ doubled }}</p>
    <button @click="increment">+</button>
    <button @click="decrement">-</button>
  </div>
</template>

<script setup lang="ts">
import { useCounter } from '@/composables/useCounter';

const { count, doubled, increment, decrement } = useCounter(0);
</script>
```

### Common Composables in CDR.Web

```typescript
// composables/useApi.ts
export function useApi() {
  const loading = ref(false);
  const error = ref(null);
  
  async function fetchData(url) {
    loading.value = true;
    try {
      return await ApiService.get(url);
    } catch (e) {
      error.value = e.message;
    } finally {
      loading.value = false;
    }
  }
  
  return { loading, error, fetchData };
}
```

---

## 🧩 Component Structure

### Single File Component (SFC)

```vue
<template>
  <!-- HTML -->
  <div class="operator-card">
    <h2>{{ operator.name }}</h2>
    <p>Calls today: {{ stats.totalCalls }}</p>
    <button @click="refreshStats">Refresh</button>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import type { Operator, CallStats } from '@/types';

// Props
interface Props {
  operator: Operator;
}

const props = withDefaults(defineProps<Props>(), {});

// Emits
const emit = defineEmits<{
  statsUpdated: [stats: CallStats];
}>();

// State
const stats = ref<CallStats | null>(null);
const loading = ref(false);

// Computed
const efficiency = computed(() => {
  if (!stats.value) return 0;
  return (stats.value.answeredCalls / stats.value.totalCalls) * 100;
});

// Methods
async function refreshStats() {
  loading.value = true;
  try {
    const response = await ApiService.get(`/report/operator/${props.operator.id}`);
    stats.value = response.data;
    emit('statsUpdated', stats.value);
  } finally {
    loading.value = false;
  }
}

// Lifecycle
onMounted(async () => {
  await refreshStats();
});
</script>

<style scoped lang="scss">
.operator-card {
  padding: 1rem;
  border: 1px solid #ddd;
  border-radius: 4px;
}

h2 {
  margin: 0;
  font-size: 1.25rem;
}
</style>
```

---

## 📦 Lifecycle Hooks

```typescript
import { 
  onMounted, 
  onUpdated, 
  onUnmounted,
  onBeforeMount,
  onBeforeUpdate,
  onBeforeUnmount
} from 'vue';

export default {
  setup() {
    onBeforeMount(() => {
      console.log('Before component mounts');
    });

    onMounted(() => {
      console.log('Component mounted - fetch data here');
      // Fetch initial data
    });

    onBeforeUpdate(() => {
      console.log('Before component updates');
    });

    onUpdated(() => {
      console.log('Component updated');
    });

    onBeforeUnmount(() => {
      console.log('Before cleanup');
    });

    onUnmounted(() => {
      console.log('Component destroyed - cleanup resources');
      // Unsubscribe, clear timers, etc.
    });
  }
};
```

### Common Pattern: Fetch on Mount

```typescript
onMounted(async () => {
  try {
    const data = await ApiService.get('/endpoint');
    storeData(data);
  } catch (error) {
    showError(error);
  }
});
```

---

## 🔄 Props & Emits Pattern

### Parent → Child (Props)

```typescript
// Child component
interface Props {
  title: string;
  count?: number;
  disabled?: boolean;
}

const props = withDefaults(defineProps<Props>(), {
  count: 0,
  disabled: false
});

// Accessing
console.log(props.title);
```

```vue
<!-- Parent component -->
<ChildComponent 
  title="My Title" 
  :count="10"
/>
```

### Child → Parent (Emits)

```typescript
// Child component
const emit = defineEmits<{
  increment: [value: number];
  close: [];
}>();

function handleClick() {
  emit('increment', 5);
}
```

```vue
<!-- Parent component -->
<ChildComponent 
  @increment="count += $event"
  @close="showChild = false"
/>
```

---

## 🎯 TypeScript Patterns

### Type Safety

```typescript
// Types file: types/index.ts
export interface Operator {
  id: string;
  name: string;
  extension: string;
  department: Department;
}

export interface CallStats {
  totalCalls: number;
  answeredCalls: number;
  avgDuration: number;
}
```

### Using Types in Components

```typescript
import type { Operator, CallStats } from '@/types';

const operator = ref<Operator | null>(null);
const stats = ref<CallStats | null>(null);

async function fetchOperator(id: string): Promise<Operator> {
  const response = await ApiService.get(`/operators/${id}`);
  return response.data as Operator;
}
```

---

## 📝 Component Best Practices

### ✅ DO
```vue
<script setup lang="ts">
// Use <script setup>
import { ref, computed } from 'vue';

const count = ref(0);
const doubled = computed(() => count.value * 2);
</script>
```

### ❌ DON'T
```vue
<script lang="ts">
export default {
  setup() {
    // Old export default pattern
  }
}
</script>
```

### ✅ DO
```typescript
// Extract logic into composables
const { count, increment } = useCounter();
```

### ❌ DON'T
```typescript
// Don't put all logic in component
// Complex logic clutters template
```

---

## 🔗 Integration with Pinia

### Component using Store

```typescript
import { useAuthStore } from '@/stores/auth';

export default {
  setup() {
    const authStore = useAuthStore();
    
    // Access state
    const isLoggedIn = computed(() => authStore.isLoggedIn);
    
    // Call action
    const logout = () => authStore.logout();
    
    return { isLoggedIn, logout };
  }
};
```

