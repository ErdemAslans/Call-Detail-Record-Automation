export interface ApiUrlConstants {
  DASHBOARD: string;
  LOGIN: string;
  REGISTER: string;
  FORGOT_PASSWORD: string;
  DASHBOARD_ANSWERED_CALLS: string;
  DASHBOARD_DAILY_CALL_REPORT: string;
  DASHBOARD_CALL_RECORDS: string;
  USER_STATISTICS: string;
  USER_LAST_CALLS: string;
  USER_INFO: string;
  OPERATORS: string;
  DEPARTMENTS: string;
  WORKING_HOURS: string;
  NON_WORKING_HOURS: string;
  BREAK_TIMES: string;
  USER_SPECIFIC_REPORT_EXPORT: string;
  BREAKS: string;
  START_NEW_BREAK: string;
  END_BREAK: string;
  // Email Report endpoints
  GENERATE_EMAIL_REPORT: string;
  SEND_EMAIL_REPORT: string;
  REPORT_EXECUTION_HISTORY: string;
  DOWNLOAD_REPORT: string;
}

const apiUrlConstants: ApiUrlConstants = {
  DASHBOARD: "dashboard",
  LOGIN: "account/login",
  REGISTER: "register",
  FORGOT_PASSWORD: "forgot_password",
  DASHBOARD_ANSWERED_CALLS: "report/answered-calls",
  DASHBOARD_DAILY_CALL_REPORT: "report/daily-call-report",
  DASHBOARD_CALL_RECORDS: "report/call-records",
  USER_STATISTICS: "report/number-statistics",
  USER_LAST_CALLS: "report/user-last-calls",
  USER_INFO: "report/user-info",
  OPERATORS: "operator/get-all",
  DEPARTMENTS: "operator/departments",
  WORKING_HOURS: "report/work-hours-statistics",
  NON_WORKING_HOURS: "report/non-work-hours-statistics",
  BREAK_TIMES: "report/break-times",
  USER_SPECIFIC_REPORT_EXPORT: "report/user-specific-report/export",
  BREAKS: "operator/user-break-times",
  START_NEW_BREAK: "operator/start-break",
  END_BREAK: "operator/end-break",
  // Email Report endpoints
  GENERATE_EMAIL_REPORT: "report/generate-email-report",
  SEND_EMAIL_REPORT: "report/send-email-report",
  REPORT_EXECUTION_HISTORY: "report/execution-history",
  DOWNLOAD_REPORT: "report/download",
};

export { apiUrlConstants };
