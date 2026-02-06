<template>
  <!--begin::Tables Widget 13-->
  <div :class="widgetClasses" class="card">
    <!--begin::Header-->
    <div class="card-header border-0 pt-5">
      <h3 class="card-title align-items-start flex-column">
        <span class="card-label fw-bold fs-3 mb-1">Recent Orders</span>

        <span class="text-muted mt-1 fw-semibold fs-7">Over 500 orders</span>
      </h3>
      <div class="card-toolbar">
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
        <Dropdown2></Dropdown2>
        <!--end::Menu-->
      </div>
    </div>
    <!--end::Header-->

    <!--begin::Body-->
    <div class="card-body py-3 position-relative">
      <!-- Loading Overlay (T028) -->
      <div v-if="isLoading" class="position-absolute top-0 start-0 w-100 h-100 bg-white bg-opacity-75 d-flex align-items-center justify-content-center" style="z-index: 10;">
        <div class="spinner-border text-primary" role="status">
          <span class="visually-hidden">Loading...</span>
        </div>
      </div>

      <!-- Empty State (T030) -->
      <div v-else-if="!isLoading && list.length === 0" class="d-flex align-items-center justify-content-center" style="height: 300px;">
        <EmptyState 
          title="Veri Bulunamadı" 
          description="Çağrı kaydı mevcut değil"
        />
      </div>

      <!--begin::Table container-->
      <div v-else class="table-responsive">
        <!--begin::Table-->
        <table
          class="table table-row-bordered table-row-gray-100 align-middle gs-0 gy-3"
        >
          <!--begin::Table head-->
          <thead>
            <tr class="fw-bold text-muted">
              <th class="w-25px">
                <div
                  class="form-check form-check-sm form-check-custom form-check-solid"
                >
                  <input
                    class="form-check-input"
                    type="checkbox"
                    @change="
                      checkedRows.length === 6
                        ? (checkedRows.length = 0)
                        : (checkedRows = [0, 1, 2, 3, 4, 5])
                    "
                  />
                </div>
              </th>
              <th class="min-w-150px">Lokasyon</th>
              <th class="min-w-140px">Departman</th>
              <th class="min-w-120px">Birim</th>
              <th class="min-w-120px">Mail Atan</th>
              <th class="min-w-120px">Tarih</th>
              <th class="min-w-120px">Arayan Kişi</th>
              <th class="min-w-120px">Dönüş Zamanı</th>
              <th class="min-w-100px text-end">Dönüş Yapan</th>
            </tr>
          </thead>
          <!--end::Table head-->

          <!--begin::Table body-->
          <tbody>
            <template v-for="(item, index) in list" :key="index">
              <tr>
                <td>
                  <div
                    class="form-check form-check-sm form-check-custom form-check-solid"
                  >
                    <input
                      class="form-check-input widget-13-check"
                      type="checkbox"
                      :value="index"
                      v-model="checkedRows"
                    />
                  </div>
                </td>

                <td>
                  <a
                    href="#"
                    class="text-gray-900 fw-bold text-hover-primary fs-6"
                    >{{ item.location }}</a
                  >
                </td>

                <td>
                  <a
                    href="#"
                    class="text-gray-900 fw-bold text-hover-primary d-block mb-1 fs-6"
                    >{{ item.department }}</a
                  >
                </td>

                <td>
                  <span
                    :class="`badge-light-${item.statusColor}`"
                    class="badge"
                    >{{ item.unit }}</span
                  >
                </td>

                <td>
                  <a
                    href="#"
                    class="text-gray-900 fw-bold text-hover-primary d-block mb-1 fs-6"
                    >{{ item.sender_email }}</a
                  >
                </td>

                <td class="text-gray-900 fw-bold text-hover-primary fs-6">
                  {{ item.date }}
                </td>

                <td class="text-gray-900 fw-bold text-hover-primary fs-6">
                  {{ item.caller }}
                </td>

                <td>
                  <span
                    :class="`badge-light-${item.statusColor}`"
                    class="badge"
                    >{{ item.callback_time }}</span
                  >
                </td>

                <td
                  class="text-end text-gray-900 fw-bold text-hover-primary fs-6"
                >
                  {{ item.responder }}
                </td>
              </tr>
            </template>
          </tbody>
          <!--end::Table body-->
          <tfoot>
            <nav aria-label="Page navigation">
              <ul class="pagination justify-content-center">
                <li class="page-item previous disabled">
                  <a href="#" class="page-link"><i class="previous"></i></a>
                </li>
                <li class="page-item"><a href="#" class="page-link">1</a></li>
                <li class="page-item active">
                  <a href="#" class="page-link">2</a>
                </li>
                <li class="page-item"><a href="#" class="page-link">3</a></li>
                <li class="page-item"><a href="#" class="page-link">4</a></li>
                <li class="page-item"><a href="#" class="page-link">5</a></li>
                <li class="page-item"><a href="#" class="page-link">6</a></li>
                <li class="page-item next">
                  <a href="#" class="page-link"><i class="next"></i></a>
                </li>
              </ul>
            </nav>
          </tfoot>
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
import { getAssetPath } from "@/core/helpers/assets";
import { defineComponent, ref } from "vue";
import Dropdown2 from "@/components/dropdown/Dropdown2.vue";
import EmptyState from "@/components/common/EmptyState.vue";

export default defineComponent({
  name: "kt-widget-12",
  components: {
    Dropdown2,
    EmptyState,
  },
  props: {
    widgetClasses: String,
  },
  setup() {
    const checkedRows = ref<Array<number>>([]);
    const isLoading = ref(false);
    const list = [
      {
        location: "İstanbul",
        department: "IT",
        unit: "Yazılım",
        sender_email: "ali@example.com",
        date: "2023-10-01",
        caller: "Veli",
        callback_time: "10:00",
        responder: "Ahmet",
        statusColor: "primary",
      },
      {
        location: "Ankara",
        department: "HR",
        unit: "İşe Alım",
        sender_email: "ayse@example.com",
        date: "2023-10-02",
        caller: "Fatma",
        callback_time: "11:00",
        responder: "Mehmet",
        statusColor: "secondary",
      },
      {
        location: "İzmir",
        department: "Finans",
        unit: "Muhasebe",
        sender_email: "mehmet@example.com",
        date: "2023-10-03",
        caller: "Ayşe",
        callback_time: "12:00",
        responder: "Ali",
        statusColor: "success",
      },
      {
        location: "Bursa",
        department: "Satış",
        unit: "Pazarlama",
        sender_email: "veli@example.com",
        date: "2023-10-04",
        caller: "Ahmet",
        callback_time: "13:00",
        responder: "Ayşe",
        statusColor: "info",
      },
      {
        location: "Antalya",
        department: "Destek",
        unit: "Müşteri Hizmetleri",
        sender_email: "fatma@example.com",
        date: "2023-10-05",
        caller: "Mehmet",
        callback_time: "14:00",
        responder: "Veli",
        statusColor: "warning",
      },
      {
        location: "Adana",
        department: "Lojistik",
        unit: "Nakliye",
        sender_email: "ahmet@example.com",
        date: "2023-10-06",
        caller: "Ali",
        callback_time: "15:00",
        responder: "Fatma",
        statusColor: "danger",
      },
      {
        location: "Gaziantep",
        department: "Üretim",
        unit: "Fabrika",
        sender_email: "ayse@example.com",
        date: "2023-10-07",
        caller: "Ayşe",
        callback_time: "16:00",
        responder: "Mehmet",
        statusColor: "light",
      },
      {
        location: "Konya",
        department: "Ar-Ge",
        unit: "Geliştirme",
        sender_email: "mehmet@example.com",
        date: "2023-10-08",
        caller: "Veli",
        callback_time: "17:00",
        responder: "Ali",
        statusColor: "dark",
      },
      {
        location: "Kayseri",
        department: "Kalite",
        unit: "Kontrol",
        sender_email: "veli@example.com",
        date: "2023-10-09",
        caller: "Fatma",
        callback_time: "18:00",
        responder: "Ahmet",
        statusColor: "primary",
      },
      {
        location: "Eskişehir",
        department: "Planlama",
        unit: "Strateji",
        sender_email: "fatma@example.com",
        date: "2023-10-10",
        caller: "Mehmet",
        callback_time: "19:00",
        responder: "Veli",
        statusColor: "secondary",
      },
      {
        location: "İstanbul",
        department: "IT",
        unit: "Yazılım",
        sender_email: "ali@example.com",
        date: "2023-10-11",
        caller: "Veli",
        callback_time: "10:00",
        responder: "Ahmet",
        statusColor: "success",
      },
      {
        location: "Ankara",
        department: "HR",
        unit: "İşe Alım",
        sender_email: "ayse@example.com",
        date: "2023-10-12",
        caller: "Fatma",
        callback_time: "11:00",
        responder: "Mehmet",
        statusColor: "info",
      },
      {
        location: "İzmir",
        department: "Finans",
        unit: "Muhasebe",
        sender_email: "mehmet@example.com",
        date: "2023-10-13",
        caller: "Ayşe",
        callback_time: "12:00",
        responder: "Ali",
        statusColor: "warning",
      },
      {
        location: "Bursa",
        department: "Satış",
        unit: "Pazarlama",
        sender_email: "veli@example.com",
        date: "2023-10-14",
        caller: "Ahmet",
        callback_time: "13:00",
        responder: "Ayşe",
        statusColor: "danger",
      },
      {
        location: "Antalya",
        department: "Destek",
        unit: "Müşteri Hizmetleri",
        sender_email: "fatma@example.com",
        date: "2023-10-15",
        caller: "Mehmet",
        callback_time: "14:00",
        responder: "Veli",
        statusColor: "light",
      },
      {
        location: "Adana",
        department: "Lojistik",
        unit: "Nakliye",
        sender_email: "ahmet@example.com",
        date: "2023-10-16",
        caller: "Ali",
        callback_time: "15:00",
        responder: "Fatma",
        statusColor: "dark",
      },
      {
        location: "Gaziantep",
        department: "Üretim",
        unit: "Fabrika",
        sender_email: "ayse@example.com",
        date: "2023-10-17",
        caller: "Ayşe",
        callback_time: "16:00",
        responder: "Mehmet",
        statusColor: "primary",
      },
      {
        location: "Konya",
        department: "Ar-Ge",
        unit: "Geliştirme",
        sender_email: "mehmet@example.com",
        date: "2023-10-18",
        caller: "Veli",
        callback_time: "17:00",
        responder: "Ali",
        statusColor: "secondary",
      },
      {
        location: "Kayseri",
        department: "Kalite",
        unit: "Kontrol",
        sender_email: "veli@example.com",
        date: "2023-10-19",
        caller: "Fatma",
        callback_time: "18:00",
        responder: "Ahmet",
        statusColor: "success",
      },
      {
        location: "Eskişehir",
        department: "Planlama",
        unit: "Strateji",
        sender_email: "fatma@example.com",
        date: "2023-10-20",
        caller: "Mehmet",
        callback_time: "19:00",
        responder: "Veli",
        statusColor: "info",
      },
      {
        location: "İstanbul",
        department: "IT",
        unit: "Yazılım",
        sender_email: "ali@example.com",
        date: "2023-10-21",
        caller: "Veli",
        callback_time: "10:00",
        responder: "Ahmet",
        statusColor: "warning",
      },
      {
        location: "Ankara",
        department: "HR",
        unit: "İşe Alım",
        sender_email: "ayse@example.com",
        date: "2023-10-22",
        caller: "Fatma",
        callback_time: "11:00",
        responder: "Mehmet",
        statusColor: "danger",
      },
      {
        location: "İzmir",
        department: "Finans",
        unit: "Muhasebe",
        sender_email: "mehmet@example.com",
        date: "2023-10-23",
        caller: "Ayşe",
        callback_time: "12:00",
        responder: "Ali",
        statusColor: "light",
      },
      {
        location: "Bursa",
        department: "Satış",
        unit: "Pazarlama",
        sender_email: "veli@example.com",
        date: "2023-10-24",
        caller: "Ahmet",
        callback_time: "13:00",
        responder: "Ayşe",
        statusColor: "dark",
      },
      {
        location: "Antalya",
        department: "Destek",
        unit: "Müşteri Hizmetleri",
        sender_email: "fatma@example.com",
        date: "2023-10-25",
        caller: "Mehmet",
        callback_time: "14:00",
        responder: "Veli",
        statusColor: "primary",
      },
      {
        location: "Adana",
        department: "Lojistik",
        unit: "Nakliye",
        sender_email: "ahmet@example.com",
        date: "2023-10-26",
        caller: "Ali",
        callback_time: "15:00",
        responder: "Fatma",
        statusColor: "secondary",
      },
      {
        location: "Gaziantep",
        department: "Üretim",
        unit: "Fabrika",
        sender_email: "ayse@example.com",
        date: "2023-10-27",
        caller: "Ayşe",
        callback_time: "16:00",
        responder: "Mehmet",
        statusColor: "success",
      },
      {
        location: "Konya",
        department: "Ar-Ge",
        unit: "Geliştirme",
        sender_email: "mehmet@example.com",
        date: "2023-10-28",
        caller: "Veli",
        callback_time: "17:00",
        responder: "Ali",
        statusColor: "info",
      },
      {
        location: "Kayseri",
        department: "Kalite",
        unit: "Kontrol",
        sender_email: "veli@example.com",
        date: "2023-10-29",
        caller: "Fatma",
        callback_time: "18:00",
        responder: "Ahmet",
        statusColor: "warning",
      },
      {
        location: "Eskişehir",
        department: "Planlama",
        unit: "Strateji",
        sender_email: "fatma@example.com",
        date: "2023-10-30",
        caller: "Mehmet",
        callback_time: "19:00",
        responder: "Veli",
        statusColor: "danger",
      },
      {
        location: "İstanbul",
        department: "IT",
        unit: "Yazılım",
        sender_email: "ali@example.com",
        date: "2023-10-31",
        caller: "Veli",
        callback_time: "10:00",
        responder: "Ahmet",
        statusColor: "light",
      },
      {
        location: "Ankara",
        department: "HR",
        unit: "İşe Alım",
        sender_email: "ayse@example.com",
        date: "2023-10-32",
        caller: "Fatma",
        callback_time: "11:00",
        responder: "Mehmet",
        statusColor: "dark",
      },
      {
        location: "İzmir",
        department: "Finans",
        unit: "Muhasebe",
        sender_email: "mehmet@example.com",
        date: "2023-10-33",
        caller: "Ayşe",
        callback_time: "12:00",
        responder: "Ali",
        statusColor: "primary",
      },
      {
        location: "Bursa",
        department: "Satış",
        unit: "Pazarlama",
        sender_email: "veli@example.com",
        date: "2023-10-34",
        caller: "Ahmet",
        callback_time: "13:00",
        responder: "Ayşe",
        statusColor: "secondary",
      },
      {
        location: "Antalya",
        department: "Destek",
        unit: "Müşteri Hizmetleri",
        sender_email: "fatma@example.com",
        date: "2023-10-35",
        caller: "Mehmet",
        callback_time: "14:00",
        responder: "Veli",
        statusColor: "success",
      },
      {
        location: "Adana",
        department: "Lojistik",
        unit: "Nakliye",
        sender_email: "ahmet@example.com",
        date: "2023-10-36",
        caller: "Ali",
        callback_time: "15:00",
        responder: "Fatma",
        statusColor: "info",
      },
      {
        location: "Gaziantep",
        department: "Üretim",
        unit: "Fabrika",
        sender_email: "ayse@example.com",
        date: "2023-10-37",
        caller: "Ayşe",
        callback_time: "16:00",
        responder: "Mehmet",
        statusColor: "warning",
      },
      {
        location: "Konya",
        department: "Ar-Ge",
        unit: "Geliştirme",
        sender_email: "mehmet@example.com",
        date: "2023-10-38",
        caller: "Veli",
        callback_time: "17:00",
        responder: "Ali",
        statusColor: "danger",
      },
      {
        location: "Kayseri",
        department: "Kalite",
        unit: "Kontrol",
        sender_email: "veli@example.com",
        date: "2023-10-39",
        caller: "Fatma",
        callback_time: "18:00",
        responder: "Ahmet",
        statusColor: "light",
      },
      {
        location: "Eskişehir",
        department: "Planlama",
        unit: "Strateji",
        sender_email: "fatma@example.com",
        date: "2023-10-40",
        caller: "Mehmet",
        callback_time: "19:00",
        responder: "Veli",
        statusColor: "dark",
      },
      {
        location: "İstanbul",
        department: "IT",
        unit: "Yazılım",
        sender_email: "ali@example.com",
        date: "2023-10-41",
        caller: "Veli",
        callback_time: "10:00",
        responder: "Ahmet",
        statusColor: "primary",
      },
      {
        location: "Ankara",
        department: "HR",
        unit: "İşe Alım",
        sender_email: "ayse@example.com",
        date: "2023-10-42",
        caller: "Fatma",
        callback_time: "11:00",
        responder: "Mehmet",
        statusColor: "secondary",
      },
      {
        location: "İzmir",
        department: "Finans",
        unit: "Muhasebe",
        sender_email: "mehmet@example.com",
        date: "2023-10-43",
        caller: "Ayşe",
        callback_time: "12:00",
        responder: "Ali",
        statusColor: "success",
      },
      {
        location: "Bursa",
        department: "Satış",
        unit: "Pazarlama",
        sender_email: "veli@example.com",
        date: "2023-10-44",
        caller: "Ahmet",
        callback_time: "13:00",
        responder: "Ayşe",
        statusColor: "info",
      },
      {
        location: "Antalya",
        department: "Destek",
        unit: "Müşteri Hizmetleri",
        sender_email: "fatma@example.com",
        date: "2023-10-45",
        caller: "Mehmet",
        callback_time: "14:00",
        responder: "Veli",
        statusColor: "warning",
      },
      {
        location: "Adana",
        department: "Lojistik",
        unit: "Nakliye",
        sender_email: "ahmet@example.com",
        date: "2023-10-46",
        caller: "Ali",
        callback_time: "15:00",
        responder: "Fatma",
        statusColor: "danger",
      },
    ];

    return {
      list,
      checkedRows,
      getAssetPath,
      isLoading,
    };
  },
});
</script>
