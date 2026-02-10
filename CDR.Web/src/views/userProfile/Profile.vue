<template>
  <!--begin::Navbar-->
  <div class="card mb-5 mb-xl-10">
    <div class="card-header align-items-center border-0 mt-4">
      <h3 class="card-title align-items-start flex-column">
        <span v-if="title" class="fw-bold mb-2 text-gray-900">{{
          $t(title)
        }}</span>
        <span class="text-muted fw-semibold fs-7">{{ periodBadge }}</span>
      </h3>
      <div class="card-toolbar d-flex align-items-center gap-2">
        <!--begin::Period Tabs-->
        <div data-kt-buttons="true">
          <a
            class="btn btn-sm btn-color-muted btn-active btn-active-primary px-4 me-1"
            :class="{ active: selectedPeriod === 'today' }"
            @click="changePeriod('today')"
          >{{ $t("daily") }}</a>
          <a
            class="btn btn-sm btn-color-muted btn-active btn-active-primary px-4 me-1"
            :class="{ active: selectedPeriod === 'week' }"
            @click="changePeriod('week')"
          >{{ $t("weekly") }}</a>
          <a
            class="btn btn-sm btn-color-muted btn-active btn-active-primary px-4 me-1"
            :class="{ active: selectedPeriod === 'month' }"
            @click="changePeriod('month')"
          >{{ $t("monthly") }}</a>
        </div>
        <!--end::Period Tabs-->
        <!--begin::Menu-->
        <button
          type="button"
          class="btn btn-sm btn-icon btn-color-primary btn-active-light-primary"
          data-kt-menu-trigger="click"
          data-kt-menu-placement="bottom-end"
          data-kt-menu-flip="top-end"
        >
          <KTIcon icon-name="category" icon-class="fs-2" />
        </button>
        <ProfileReport
          :start-date="startDate"
          :end-date="endDate"
          :number="number"
          @update-dates="updateDates"
        />
        <!--end::Menu-->
      </div>
    </div>

    <div class="card-body pt-9 pb-0">
      <!--begin::Details-->
      <div class="d-flex flex-wrap flex-sm-nowrap mb-3">
        <!--begin: Pic-->
        <div class="me-7 mb-4">
          <div
            class="symbol symbol-100px symbol-lg-160px symbol-fixed position-relative"
          >
            <img :src="getAssetPath('media/auth/agency.png')" alt="image" />
            <div
              class="position-absolute translate-middle bottom-0 start-100 mb-6 bg-success rounded-circle border border-4 border-white h-20px w-20px"
            ></div>
          </div>
        </div>
        <!--end::Pic-->

        <!--begin::Info-->
        <div class="flex-grow-1">
          <!--begin::Title-->
          <div
            class="d-flex justify-content-between align-items-start flex-wrap mb-2"
          >
            <!--begin::User-->
            <div class="d-flex flex-column">
              <!--begin::Name-->
              <div class="d-flex align-items-center mb-2">
                <a
                  href="#"
                  class="text-gray-800 text-hover-primary fs-2 fw-bold me-1"
                >
                  {{ userInfo.name }} - {{ userInfo?.number }}</a
                >
              </div>
              <!--end::Name-->

              <!--begin::Info-->
              <div class="d-flex flex-wrap fw-semibold fs-6 mb-4 pe-2">
                <a
                  href="#"
                  class="d-flex align-items-center text-gray-500 text-hover-primary me-5 mb-2"
                >
                  <KTIcon icon-name="profile-circle" icon-class="fs-4 me-1" />
                  {{ userInfo.position }}
                </a>
                <a
                  href="#"
                  class="d-flex align-items-center text-gray-500 text-hover-primary me-5 mb-2"
                >
                  <KTIcon icon-name="geolocation" icon-class="fs-4 me-1" />
                  {{ userInfo.department }}
                </a>
                <a
                  href="#"
                  class="d-flex align-items-center text-gray-500 text-hover-primary mb-2"
                >
                  <KTIcon icon-name="phone" icon-class="fs-4 me-1" />
                  {{ userInfo?.number }}
                </a>
              </div>
              <!--end::Info-->
            </div>
            <!--end::User-->
          </div>
          <!--end::Title-->

          <!--begin::Stats-->
          <div class="d-flex flex-wrap flex-stack">
            <!--begin::Wrapper-->
            <div class="d-flex flex-column flex-grow-1 pe-8">
              <!--begin::Stats-->
              <div class="d-flex flex-wrap">
                <!--begin::Stat-->
                <div
                  class="border border-gray-300 border-dashed rounded min-w-125px py-3 px-4 me-6 mb-3"
                >
                  <!--begin::Number-->
                  <div class="d-flex align-items-center">
                    <KTIcon
                      icon-name="call"
                      icon-class="fs-3 text-success me-2"
                    />
                    <div class="fs-2 fw-bold">
                      {{ userStatistics?.answeredCallCount }}
                    </div>
                  </div>
                  <!--end::Number-->

                  <!--begin::Label-->
                  <div class="fw-semibold fs-6 text-gray-500">
                    {{ translate("answeredCallCount") }}
                  </div>
                  <!--end::Label-->
                </div>
                <!--end::Stat-->

                <!--begin::Stat-->
                <div
                  class="border border-gray-300 border-dashed rounded min-w-125px py-3 px-4 me-6 mb-3"
                >
                  <!--begin::Number-->
                  <div class="d-flex align-items-center">
                    <KTIcon
                      icon-name="disconnect"
                      icon-class="fs-3 text-danger me-2"
                    />
                    <div
                      class="fs-2 fw-bold"
                      data-kt-countup="true"
                      data-kt-countup-value="75"
                    >
                      {{ userStatistics?.missedCallCount }}
                    </div>
                  </div>
                  <!--end::Number-->

                  <!--begin::Label-->
                  <div class="fw-semibold fs-6 text-gray-500">
                    {{ translate("missedCallCount") }}
                  </div>
                  <!--end::Label-->
                </div>
                <!--end::Stat-->

                <!--begin::Stat-->
                <div
                  class="border border-gray-300 border-dashed rounded min-w-125px py-3 px-4 me-6 mb-3"
                >
                  <!--begin::Number-->
                  <div class="d-flex align-items-center">
                    <KTIcon
                      icon-name="coffee"
                      icon-class="fs-3 text-warning me-2"
                    />
                    <div class="fs-2 fw-bold">
                      {{ userStatistics?.onBreakCallCount ?? 0 }}
                    </div>
                  </div>
                  <!--end::Number-->

                  <!--begin::Label-->
                  <div class="fw-semibold fs-6 text-gray-500">
                    {{ translate("onBreakCallCount") }}
                  </div>
                  <!--end::Label-->
                </div>
                <!--end::Stat-->

                <!--begin::Stat-->
                <div
                  class="border border-gray-300 border-dashed rounded min-w-125px py-3 px-4 me-6 mb-3"
                >
                  <!--begin::Number-->
                  <div class="d-flex align-items-center">
                    <KTIcon
                      icon-name="arrow-up"
                      icon-class="fs-3 text-success me-2"
                    />
                    <div
                      class="fs-2 fw-bold"
                      data-kt-countup="true"
                      data-kt-countup-value="60"
                      data-kt-countup-prefix="%"
                    >
                      {{ userStatistics?.answeredCallRatio }}%
                    </div>
                  </div>
                  <!--end::Number-->

                  <!--begin::Label-->
                  <div class="fw-semibold fs-6 text-gray-500">
                    {{ translate("answeredCallRate") }}
                  </div>
                  <!--end::Label-->
                </div>
                <!--end::Stat-->
              </div>
              <!--end::Stats-->
            </div>
            <!--end::Wrapper-->
          </div>
          <!--end::Stats-->
        </div>
        <!--end::Info-->
      </div>
      <!--end::Details-->

      <!--begin::Navs-->
      <div class="d-flex overflow-auto h-55px">
        <ul
          class="nav nav-stretch nav-line-tabs nav-line-tabs-2x border-transparent fs-5 fw-bold flex-nowrap"
        >
          <!--begin::Nav item-->
          <li class="nav-item">
            <a class="nav-link text-active-primary me-6" active-class="active">
              {{ translate("statistics") }}
            </a>
          </li>
        </ul>
      </div>
      <!--begin::Navs-->
    </div>
  </div>
  <!--end::Navbar-->
  <UserStatistics
    :minDuration="DateHelper.formatDuration(userStatistics.minDuration)"
    :maxDuration="DateHelper.formatDuration(userStatistics.maxDuration)"
    :averageDuration="DateHelper.formatDuration(userStatistics.avgDuration)"
    :startDate="startDate"
    :endDate="endDate"
    :number="number"
  />
</template>

<script lang="ts">
import { getAssetPath } from "@/core/helpers/assets";
import { useUserStatisticsStore } from "@/stores/userStatistics";
import { computed, defineComponent, onMounted, ref, provide, watch } from "vue";
import { useI18n } from "vue-i18n";
import UserStatistics from "./UserStatistics.vue";
import ProfileReport from "./ProfileReport.vue";
import DateHelper from "@/core/helpers/DateHelper";
import { useRoute } from "vue-router";

export default defineComponent({
  name: "kt-profile",
  props: {
    operatorNumber: String,
    title: String,
  },
  components: {
    UserStatistics,
    ProfileReport,
  },
  setup(props) {
    const { t, te } = useI18n();
    const translate = (text: string) => {
      if (te(text)) {
        return t(text);
      } else {
        return text;
      }
    };

    const userStatisticsStore = useUserStatisticsStore();
    const userStatistics = ref<IUserStatistics>({} as IUserStatistics);
    const userInfo = ref<IUserInfo>({} as IUserInfo);
    const route = useRoute();
    const number = ref(props.operatorNumber ?? (route.params.number as string));
    const selectedPeriod = ref<'today' | 'week' | 'month'>('month');
    const { start, end } = DateHelper.getDateRange('month');
    const startDate = ref(start);
    const endDate = ref(end);

    const changePeriod = (period: 'today' | 'week' | 'month') => {
      selectedPeriod.value = period;
      const range = DateHelper.getDateRange(period);
      startDate.value = range.start;
      endDate.value = range.end;
    };

    const periodBadge = computed(() => {
      const s = new Date(startDate.value);
      const e = new Date(endDate.value);
      const opts: Intl.DateTimeFormatOptions = {
        day: 'numeric',
        month: 'long',
        year: 'numeric',
        timeZone: 'Europe/Istanbul',
      };
      if (selectedPeriod.value === 'today') {
        return s.toLocaleDateString('tr-TR', opts);
      }
      return `${s.toLocaleDateString('tr-TR', { day: 'numeric', month: 'long', timeZone: 'Europe/Istanbul' })} - ${e.toLocaleDateString('tr-TR', opts)}`;
    });

    const updateDates = (newStartDate: string, newEndDate: string) => {
      startDate.value = newStartDate;
      endDate.value = newEndDate;
    };

    provide("number", number);

    onMounted(async () => {
      const params = {
        startDate: startDate.value,
        endDate: endDate.value,
        number: number.value,
      };
      userStatistics.value =
        await userStatisticsStore.fetchUserStatistics(params);
      userInfo.value = await userStatisticsStore.fetchUserInfo(params.number);
    });

    watch(
      () => [startDate.value, endDate.value, number],
      async () => {
        const params = {
          startDate: startDate.value,
          endDate: endDate.value,
          number: number.value,
        };
        userStatistics.value =
          await userStatisticsStore.fetchUserStatistics(params);
        userInfo.value = await userStatisticsStore.fetchUserInfo(params.number);
      },
    );

    return {
      getAssetPath,
      translate,
      userStatistics,
      DateHelper,
      userInfo,
      number,
      startDate,
      endDate,
      updateDates,
      selectedPeriod,
      changePeriod,
      periodBadge,
    };
  },
});
</script>
