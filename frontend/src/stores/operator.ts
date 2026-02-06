import ResponseMessageService from "@/core/helpers/ResponseMessageService";
import ApiService from "@/core/services/ApiService";
import { defineStore } from "pinia";
import { ref } from "vue";
import { apiUrlConstants } from "./consts/ApiUrlConstants";

interface Operator {
  name: string;
  department: string;
  position: string;
  number: string;
}

interface Department {
  id: string;
  name: string;
}

export const useOperatorStore = defineStore("user", () => {
  const errors = ref({});
  const operators = ref<Operator[]>([]);
  const departments = ref<Department[]>([]);

  function setError(error: any) {
    errors.value = { ...error };
  }

  function fetchOperators() {
    const url = `${apiUrlConstants.OPERATORS}`;
    return ApiService.get(url)
      .then(({ data }) => {
        ResponseMessageService.showMessageByType(
          "user_fetchOperators",
          "success",
        );
        operators.value = data;
      })
      .catch(({ response }) => {
        setError(response.data.errors);
      });
  }

  function fetchDepartments() {
    const url = `${apiUrlConstants.DEPARTMENTS}`;
    return ApiService.get(url)
      .then(({ data }) => {
        ResponseMessageService.showMessageByType(
          "user_fetchDepartments",
          "success",
        );
        departments.value = data;
      })
      .catch(({ response }) => {
        setError(response.data.errors);
      });
  }

  return {
    operators,
    departments,
    fetchOperators,
    fetchDepartments,
  };
});
