import { jwtDecode } from "jwt-decode";
import ApiService from "./ApiService";
const ID_TOKEN_KEY = "id_token" as string;
const REFRESH_TOKEN_KEY = "refresh_token" as string;

/**
 * @description get token from localStorage
 */
export const getToken = (): string | null => {
  return window.localStorage.getItem(ID_TOKEN_KEY);
};

/**
 * @description save token into localStorage
 * @param token: string
 */
export const saveToken = (token: string): void => {
  window.localStorage.setItem(ID_TOKEN_KEY, token);
};

/**
 * @description remove token from localStorage
 */
export const destroyToken = (): void => {
  window.localStorage.removeItem(ID_TOKEN_KEY);
};

/**
 * @description get refresh token from localStorage
 */
export const getRefreshToken = (): string | null => {
  return window.localStorage.getItem(REFRESH_TOKEN_KEY);
};

/**
 * @description save refresh token into localStorage
 * @param refreshToken: string
 */
export const saveRefreshToken = (refreshToken: string): void => {
  window.localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
};

/**
 * @description remove refresh token from localStorage
 */
export const destroyRefreshToken = (): void => {
  window.localStorage.removeItem(REFRESH_TOKEN_KEY);
};

/**
 * @description decode the token
 * @param token: string
 * @returns decoded token object
 */
export const decodeToken = (token: string): any => {
  return jwtDecode(token);
};

/**
 * @description check if token is expired
 * @param token: string
 * @returns boolean - whether the token is expired
 */
export const isTokenExpired = (token: string): boolean => {
  try {
    const decoded = decodeToken(token);
    const exp = decoded.exp;
    return exp * 1000 < Date.now();
  } catch (error) {
    return true; // Token is invalid or expired
  }
};

/**
 * @description refresh the access token using refresh token
 * @returns string | null - new access token or null if refresh failed
 */
export const refreshToken = async (): Promise<string | null> => {
  const refreshToken = getRefreshToken();
  if (!refreshToken) {
    return null; // No refresh token available
  }

  try {
    // Call API to get a new token using the refresh token
    const response = await ApiService.post("/auth/refresh-token", {
      refreshToken: refreshToken,
    });
    const newAccessToken = response.data.token;
    saveToken(newAccessToken);
    return newAccessToken;
  } catch (error) {
    return null; // Failed to refresh token
  }
};

export default {
  getToken,
  saveToken,
  destroyToken,
  getRefreshToken,
  saveRefreshToken,
  destroyRefreshToken,
  decodeToken,
  isTokenExpired,
  refreshToken,
};
