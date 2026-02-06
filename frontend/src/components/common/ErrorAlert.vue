<template>
  <el-alert
    v-if="message"
    :title="message"
    type="error"
    show-icon
    :closable="closable"
    @close="$emit('dismiss')"
  >
    <template v-if="showRetry" #default>
      <el-button size="small" type="primary" @click="$emit('retry')">
        {{ retryLabel }}
      </el-button>
    </template>
  </el-alert>
</template>

<script setup lang="ts">
withDefaults(
  defineProps<{
    message: string | null;
    showRetry?: boolean;
    closable?: boolean;
    retryLabel?: string;
  }>(),
  {
    showRetry: false,
    closable: true,
    retryLabel: 'Tekrar Dene',
  },
);

defineEmits(['retry', 'dismiss']);
</script>
