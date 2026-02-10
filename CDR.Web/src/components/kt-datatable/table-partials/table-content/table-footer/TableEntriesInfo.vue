<template>
  <div
    class="col-sm-12 col-md-4 d-flex align-items-center justify-content-center justify-content-md-end"
  >
    <span class="text-gray-600">
      <span
        v-html="$t('showingEntries', { startEntry, endEntry, count })"
      ></span>
    </span>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "table-entries-info",
  props: {
    startEntry: { type: Number, required: true },
    endEntry: { type: Number, required: true },
    count: { type: Number, required: true },
  },
});
</script>
