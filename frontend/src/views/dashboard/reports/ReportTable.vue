<template>
  <!--begin::Tables Widget 13-->
  <div :class="widgetClasses" class="card">
    <!--begin::Header-->
    <div class="card-header border-0 pt-5">
      <h3 class="card-title align-items-start flex-column">
        <span class="card-label fw-bold fs-3 mb-1">{{
          translate("incomingCalls")
        }}</span>

        <span class="text-muted mt-1 fw-semibold fs-7"
          >{{ translate("overCallsSummary", { count: incomingCallSummary }) }}</span
        >
      </h3>
    </div>
    <!--end::Header-->

    <!--begin::Body-->
    <div class="card-body py-3">
      <!--begin::Table container-->
      <div class="table-responsive">
        <!--begin::Table-->
        <table
          class="table table-row-bordered table-row-gray-100 align-middle gs-0 gy-3"
        >
          <!--begin::Table head-->
          <thead>
            <tr class="fw-bold text-muted">
              <th class="min-w-150px">{{ translate("group") }}</th>
              <th>{{ translate("incomingCallCount") }}</th>
              <th>{{ translate("missedCallCount") }}</th>
              <th>{{ translate("answeredCallCount") }}</th>
              <th>{{ translate("missedCallRate") }}</th>
              <th>{{ translate("answeredCallRate") }}</th>
              <th class="text-end">{{ translate("maxWaitingTime") }}</th>
            </tr>
          </thead>
          <!--end::Table head-->

          <!--begin::Table body-->
          <tbody>
            <template v-for="(item, index) in list" :key="index">
              <tr>
                <td class="text-gray-900 fw-bold text-hover-primary fs-6">
                  <span
                    :class="`badge-light-${item.groupColor}`"
                    class="badge"
                    >{{ item.group }}</span
                  >
                </td>

                <td class="text-gray-900 fw-bold text-hover-primary fs-6">
                  {{ item.incomingCallCount }}
                </td>

                <td class="text-gray-900 fw-bold text-hover-primary fs-6">
                  {{ item.missedCallCount }}
                </td>

                <td class="text-gray-900 fw-bold text-hover-primary fs-6">
                  {{ item.answeredCallCount }}
                </td>

                <td class="text-gray-900 fw-bold text-hover-primary fs-6">
                  {{ item.missedCallRate }}
                </td>

                <td class="text-gray-900 fw-bold text-hover-primary fs-6">
                  {{ item.answeredCallRate }} %
                </td>

                <td class="text-end">
                  <span :class="`badge-light-danger`" class="badge"
                    >{{ item.maxWaitTime }} sn</span
                  >
                </td>
              </tr>
            </template>
          </tbody>
          <!--end::Table body-->
        </table>
        <!--end::Table-->
      </div>
      <!--end::Table container-->
    </div>
    <!--begin::Body-->
  </div>
  <!--end::Tables Widget 13-->
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import Dropdown2 from "@/components/dropdown/Dropdown2.vue";
import { useI18n } from "vue-i18n";

export default defineComponent({
  name: "kt-widget-12",
  components: {
    Dropdown2,
  },
  props: {
    widgetClasses: String,
  },
  setup() {
    const { t, te } = useI18n();
    const translate = (text: string, params?: Record<string, unknown>) => {
      if (te(text)) {
        return t(text, params ?? {});
      } else {
        return text;
      }
    };
    const list = [
      {
        group: translate("salesConsultant"),
        groupColor: "success",
        incomingCallCount: 10,
        missedCallCount: 5,
        answeredCallCount: 5,
        missedCallRate: 50,
        answeredCallRate: 50,
        maxWaitTime: 108,
      },
      {
        group: translate("salesRepresentative"),
        groupColor: "info",
        incomingCallCount: 3,
        missedCallCount: 0,
        answeredCallCount: 3,
        missedCallRate: 0,
        answeredCallRate: 100,
        maxWaitTime: 0,
      },
    ];

    var incomingCallSummary = list.reduce((acc, item) => {
      return acc + item.incomingCallCount;
    }, 0);

    return {
      list,
      translate,
      incomingCallSummary,
    };
  },
});
</script>
