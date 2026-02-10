import type { App } from "vue";
import type { AxiosResponse, AxiosError } from "axios";
import axios from "axios";
import VueAxios from "vue-axios";
import JwtService from "@/core/services/JwtService";
import router from "@/router"; // Yönlendirme için
import Swal from "sweetalert2";
import { ElMessage } from "element-plus";
import i18n from "../plugins/i18n";

/**
 * @description service to call HTTP request via Axios
 */
class ApiService {
  /**
   * @description property to share vue instance
   */
  public static vueInstance: App;

  /**
   * @description initialize vue axios
   */
  public static init(app: App<Element>) {
    ApiService.vueInstance = app;
    ApiService.vueInstance.use(VueAxios, axios);
    ApiService.vueInstance.axios.defaults.baseURL =
      import.meta.env.VITE_APP_API_URL;

    // Response Interceptor ekleniyor
    ApiService.vueInstance.axios.interceptors.response.use(
      (response: AxiosResponse) => {
        // Başarılı cevapları olduğu gibi döndürüyoruz
        return response;
      },
      async (error: AxiosError) => {
        // Hata kodunu alalım
        const status = error.response?.status;

        // Global hata yönetimi (401, 403, vb.)
        switch (status) {
          case 401:
            // 401 - Unauthorized (Session Expired)
            await Swal.fire({
              icon: "warning",
              title: i18n.global.t("auth.sessionExpired").toString(),
              showConfirmButton: false,
              timer: 1500,
            });
            router.push({ name: "sign-in" });
            break;

          case 403:
            // 403 - Forbidden
            router.push({ name: "403" });
            break;

          default:
            ElMessage.error(error.message || i18n.global.t("error.unknown"));
            break;
        }

        // Hata durumunda promise'in reddedilmesi
        return Promise.reject(error);
      },
    );
  }

  /**
   * @description set the default HTTP request headers
   */
  public static async setHeader(): Promise<void> {
    // Token'ı al
    const token = JwtService.getToken();

    if (token) {
      // Header'a token ekle
      ApiService.vueInstance.axios.defaults.headers.common["Authorization"] =
        `Bearer ${token}`;
      ApiService.vueInstance.axios.defaults.headers.common["Accept"] =
        "application/json";
      
      // Set Accept-Language header based on current locale
      ApiService.vueInstance.axios.defaults.headers.common["Accept-Language"] =
        i18n.global.locale.value || "tr-TR";
    } else {
      // Eğer token yoksa, session expired durumu işleyebilirsiniz
      await this.handleSessionExpired();
    }

    // Token geçerliliği kontrol et ve yenile
    if (token && JwtService.isTokenExpired(token)) {
      await this.handleSessionExpired();
    }
  }

  /**
   * @description Handle session expired scenario by clearing the token
   */
  private static async handleSessionExpired(): Promise<void> {
    // Eğer token geçersizse, oturumu kapat
    JwtService.destroyToken();
    router.push({ name: "sign-in" });
  }

  /**
   * @description send the GET HTTP request
   * @param resource: string
   * @param params: AxiosRequestConfig
   * @returns Promise<AxiosResponse>
   */
  public static async query(resource: string, params: any): Promise<AxiosResponse> {
    await this.setHeader();
    return ApiService.vueInstance.axios.get(resource, params);
  }

  /**
   * @description send the GET HTTP request
   * @param resource: string
   * @param slug: string
   * @returns Promise<AxiosResponse>
   */
  public static async get(
    resource: string,
    slug = "" as string,
  ): Promise<AxiosResponse> {
    await this.setHeader();
    const url = slug ? `${resource}/${slug}` : resource;
    return ApiService.vueInstance.axios.get(url);
  }

  /**
   * @description set the POST HTTP request
   * @param resource: string
   * @param params: AxiosRequestConfig
   * @returns Promise<AxiosResponse>
   */
  public static async post(resource: string, params: any): Promise<AxiosResponse> {
    await this.setHeader();
    return ApiService.vueInstance.axios.post(`${resource}`, params);
  }

  /**
   * @description send the UPDATE HTTP request
   * @param resource: string
   * @param slug: string
   * @param params: AxiosRequestConfig
   * @returns Promise<AxiosResponse>
   */
  public static async update(
    resource: string,
    slug: string,
    params: any,
  ): Promise<AxiosResponse> {
    await this.setHeader();
    return ApiService.vueInstance.axios.put(`${resource}/${slug}`, params);
  }

  /**
   * @description Send the PUT HTTP request
   * @param resource: string
   * @param params: AxiosRequestConfig
   * @returns Promise<AxiosResponse>
   */
  public static async put(resource: string, params: any): Promise<AxiosResponse> {
    await this.setHeader();
    return ApiService.vueInstance.axios.put(`${resource}`, params);
  }

  /**
   * @description Send the DELETE HTTP request
   * @param resource: string
   * @returns Promise<AxiosResponse>
   */
  public static async delete(resource: string): Promise<AxiosResponse> {
    await this.setHeader();
    return ApiService.vueInstance.axios.delete(resource);
  }
}

export default ApiService;
