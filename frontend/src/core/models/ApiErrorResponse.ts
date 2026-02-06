/**
 * ApiErrorResponse Interface
 * Standardized error response from API
 * Reference: design/01-api-contracts.md §4.1
 */
interface ApiErrorResponse {
  status: number; // HTTP status code
  message: string; // User-friendly message
  errors?: Record<string, string[]>; // Validation errors (400 only)
  timestamp: string; // ISO8601
  traceId?: string; // Request tracking ID
}

export default ApiErrorResponse;
