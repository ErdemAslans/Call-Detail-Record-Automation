<template>
  <div class="empty-state d-flex flex-column align-items-center justify-content-center py-10">
    <i :class="iconClass" class="fs-3x text-gray-400 mb-4"></i>
    <h4 class="text-gray-600 fw-semibold mb-2">{{ title }}</h4>
    <p class="text-gray-500 text-center mb-4" style="max-width: 400px">
      {{ description }}
    </p>
    <slot name="action">
      <el-button v-if="showAction" type="primary" @click="$emit('action')">
        {{ actionLabel }}
      </el-button>
    </slot>
  </div>
</template>

<script setup lang="ts">
withDefaults(
  defineProps<{
    title?: string;
    description?: string;
    iconClass?: string;
    showAction?: boolean;
    actionLabel?: string;
  }>(),
  {
    title: 'Veri Bulunamadı',
    description: 'Seçilen kriterlere uygun kayıt bulunmamaktadır.',
    iconClass: 'ki-outline ki-document',
    showAction: false,
    actionLabel: 'Tekrar Dene',
  },
);

defineEmits(['action']);
</script>

<style scoped>
.empty-state {
  min-height: 300px;
}
</style>
