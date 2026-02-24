<template>
  <Teleport to="body">
    <Transition name="shift-lock">
      <div v-if="isLocked" class="shift-lock-overlay">
        <div class="shift-lock-backdrop"></div>
        <div class="shift-lock-content">
          <div class="shift-lock-card">
            <div class="shift-lock-icon">
              <svg xmlns="http://www.w3.org/2000/svg" width="64" height="64" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round">
                <circle cx="12" cy="12" r="10"/>
                <polyline points="12 6 12 12 16 14"/>
              </svg>
            </div>

            <h1 class="shift-lock-title">{{ $t('shiftLock_title') }}</h1>
            <p class="shift-lock-subtitle">{{ $t('shiftLock_subtitle') }}</p>

            <div class="shift-lock-time">
              {{ currentTime }}
            </div>

            <div class="shift-lock-info">
              <div class="shift-lock-info-item" v-if="shiftEndTime">
                <span class="shift-lock-info-label">{{ $t('shiftLock_endedAt') }}</span>
                <span class="shift-lock-info-value">{{ shiftEndTime }}</span>
              </div>
            </div>

            <button
              class="btn btn-success btn-lg shift-lock-btn"
              :disabled="isStarting"
              @click="startShift"
            >
              <span v-if="!isStarting" class="d-flex align-items-center gap-2">
                <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
                  <path d="M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4"/>
                  <polyline points="10 17 15 12 10 7"/>
                  <line x1="15" y1="12" x2="3" y2="12"/>
                </svg>
                {{ $t('shiftLock_startShift') }}
              </span>
              <span v-else class="d-flex align-items-center gap-2">
                <span class="spinner-border spinner-border-sm"></span>
                {{ $t('pleaseWait') }}
              </span>
            </button>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script lang="ts">
import { defineComponent, ref, computed, onMounted, onUnmounted } from "vue";
import { useBreaksStore } from "@/stores/breaksTime";
import { useAuthStore } from "@/stores/auth";
import { ElMessage } from "element-plus";
import i18n from "@/core/plugins/i18n";

export default defineComponent({
  name: "ShiftLockOverlay",
  setup() {
    const breaksStore = useBreaksStore();
    const authStore = useAuthStore();

    const isLocked = ref(false);
    const isStarting = ref(false);
    const ongoingBreakId = ref<string | null>(null);
    const shiftEndTime = ref<string | null>(null);
    const currentTime = ref("");
    let checkInterval: ReturnType<typeof setInterval> | null = null;
    let clockInterval: ReturnType<typeof setInterval> | null = null;

    const isCentral = computed(() => authStore.hasRole("Central") && !authStore.hasRole("Admin"));

    const updateClock = () => {
      const now = new Date();
      currentTime.value = now.toLocaleTimeString("tr-TR", {
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
        timeZone: "Europe/Istanbul",
      });
    };

    const formatTime = (dateStr: string): string => {
      return new Date(dateStr).toLocaleTimeString("tr-TR", {
        hour: "2-digit",
        minute: "2-digit",
        timeZone: "Europe/Istanbul",
      });
    };

    const checkShiftStatus = async () => {
      if (!isCentral.value) return;

      try {
        const ongoing = await breaksStore.fetchOngoingBreak();
        if (ongoing && ongoing.breakType === "EndOfShift") {
          isLocked.value = true;
          ongoingBreakId.value = ongoing.id;
          shiftEndTime.value = formatTime(ongoing.startTime);
        } else {
          isLocked.value = false;
          ongoingBreakId.value = null;
          shiftEndTime.value = null;
        }
      } catch {
        // Silently fail - don't lock on network error
      }
    };

    const startShift = async () => {
      if (!ongoingBreakId.value || isStarting.value) return;

      isStarting.value = true;
      try {
        await breaksStore.endBreak(ongoingBreakId.value);
        ElMessage.success(i18n.global.t("shiftStartedSuccess"));
        // Reload page to refresh all state (breaks, timeline, etc.)
        window.location.reload();
      } catch {
        ElMessage.error(i18n.global.t("shiftStartError"));
        isStarting.value = false;
      }
    };

    onMounted(() => {
      updateClock();
      checkShiftStatus();

      // Update clock every second
      clockInterval = setInterval(updateClock, 1000);

      // Check shift status every 30 seconds
      checkInterval = setInterval(checkShiftStatus, 30000);
    });

    onUnmounted(() => {
      if (checkInterval) clearInterval(checkInterval);
      if (clockInterval) clearInterval(clockInterval);
    });

    return {
      isLocked,
      isStarting,
      currentTime,
      shiftEndTime,
      startShift,
    };
  },
});
</script>

<style scoped>
.shift-lock-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  width: 100vw;
  height: 100vh;
  z-index: 99999;
  display: flex;
  align-items: center;
  justify-content: center;
}

.shift-lock-backdrop {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.85);
  backdrop-filter: blur(12px);
}

.shift-lock-content {
  position: relative;
  z-index: 1;
  width: 100%;
  max-width: 480px;
  padding: 0 24px;
}

.shift-lock-card {
  background: #ffffff;
  border-radius: 20px;
  padding: 48px 40px;
  text-align: center;
  box-shadow: 0 25px 60px rgba(0, 0, 0, 0.3);
}

.shift-lock-icon {
  color: #f59e0b;
  margin-bottom: 24px;
  animation: pulse-icon 2s ease-in-out infinite;
}

@keyframes pulse-icon {
  0%, 100% { opacity: 1; transform: scale(1); }
  50% { opacity: 0.7; transform: scale(1.05); }
}

.shift-lock-title {
  font-size: 24px;
  font-weight: 700;
  color: #1a1a2e;
  margin-bottom: 8px;
}

.shift-lock-subtitle {
  font-size: 15px;
  color: #6b7280;
  margin-bottom: 32px;
  line-height: 1.5;
}

.shift-lock-time {
  font-size: 48px;
  font-weight: 300;
  color: #1a1a2e;
  font-variant-numeric: tabular-nums;
  margin-bottom: 24px;
  letter-spacing: 2px;
}

.shift-lock-info {
  margin-bottom: 32px;
}

.shift-lock-info-item {
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 8px;
  padding: 10px 20px;
  background: #fef3c7;
  border-radius: 10px;
}

.shift-lock-info-label {
  font-size: 13px;
  color: #92400e;
  font-weight: 500;
}

.shift-lock-info-value {
  font-size: 15px;
  color: #92400e;
  font-weight: 700;
}

.shift-lock-btn {
  width: 100%;
  padding: 14px 32px;
  font-size: 16px;
  font-weight: 600;
  border-radius: 12px;
  transition: all 0.2s ease;
}

.shift-lock-btn:not(:disabled):hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 24px rgba(16, 185, 129, 0.4);
}

/* Transition */
.shift-lock-enter-active {
  transition: opacity 0.3s ease;
}
.shift-lock-enter-active .shift-lock-card {
  transition: transform 0.3s ease, opacity 0.3s ease;
}
.shift-lock-leave-active {
  transition: opacity 0.3s ease;
}
.shift-lock-leave-active .shift-lock-card {
  transition: transform 0.3s ease, opacity 0.3s ease;
}
.shift-lock-enter-from {
  opacity: 0;
}
.shift-lock-enter-from .shift-lock-card {
  transform: scale(0.9);
  opacity: 0;
}
.shift-lock-leave-to {
  opacity: 0;
}
.shift-lock-leave-to .shift-lock-card {
  transform: scale(0.9);
  opacity: 0;
}
</style>
