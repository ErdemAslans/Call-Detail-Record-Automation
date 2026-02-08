import { ref, computed } from "vue";
import { defineStore } from "pinia";
import ApiService from "@/core/services/ApiService";
import JwtService from "@/core/services/JwtService";
import { apiUrlConstants } from "@/stores/consts/ApiUrlConstants";

export interface User {
  token: string;
  refreshToken: string;
  username?: string;
  roles: string[];
}

function getUserFromToken(): User {
  const token = JwtService.getToken();
  const refreshToken = JwtService.getRefreshToken();
  const decodedToken = token ? JwtService.decodeToken(token) : {};
  const username = decodedToken["unique_name"];
  const rolesData =
    decodedToken[
      "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    ] || [];

  return {
    token: token || "",
    refreshToken: refreshToken || "",
    username: username || "",
    roles: Array.isArray(rolesData) ? rolesData : [rolesData],
  };
}

export const useAuthStore = defineStore("auth", () => {
  const errors = ref({});
  const user = ref<User>(getUserFromToken());
  const isAuthenticated = ref(!!user.value.token);

  // Kullanıcının belirli bir rolü olup olmadığını kontrol et
  function hasRole(role: string): boolean {
    return user.value.roles.includes(role);
  }

  function setAuth(authUser: User) {
    isAuthenticated.value = true;

    // JWT içindeki rolleri düzgün şekilde ayarla
    const decodedToken = JwtService.decodeToken(authUser.token);
    const rolesData =
      decodedToken[
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
      ] || [];

    user.value = {
      ...authUser,
      roles: Array.isArray(rolesData) ? rolesData : [rolesData], // Tek rol mü dizi mi kontrol et
    };

    errors.value = {};
    JwtService.saveToken(user.value.token);
    JwtService.saveRefreshToken(user.value.refreshToken);
  }

  function setError(error: any) {
    errors.value = { ...error };
  }

  function purgeAuth() {
    isAuthenticated.value = false;
    user.value = { token: "", refreshToken: "", roles: [] };
    errors.value = {};
    JwtService.destroyToken();
  }

  function login(credentials: { email: string; password: string }) {
    return ApiService.post(apiUrlConstants.LOGIN, credentials)
      .then(({ data }) => {
        setAuth(data);
      })
      .catch((error: any) => {
        setError(error.response?.data?.errors || { general: "Bir hata oluştu" });
      });
  }

  function logout() {
    purgeAuth();
  }

  function register(credentials: { username: string; password: string }) {
    return ApiService.post(apiUrlConstants.REGISTER, credentials)
      .then(({ data }) => {
        setAuth(data);
      })
      .catch((error: any) => {
        setError(error.response?.data?.errors || { general: "Bir hata oluştu" });
      });
  }

  function forgotPassword(email: string) {
    return ApiService.post(apiUrlConstants.FORGOT_PASSWORD, { email })
      .then(() => {
        setError({});
      })
      .catch((error: any) => {
        setError(error.response?.data?.errors || { general: "Bir hata oluştu" });
      });
  }

  return {
    errors,
    user,
    isAuthenticated,
    hasRole,
    login,
    logout,
    register,
    forgotPassword,
    // verifyAuth,
  };
});
