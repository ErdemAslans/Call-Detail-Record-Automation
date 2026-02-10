/**
 * ErrorLogger Service
 * Structured error logging for frontend debugging and monitoring
 * Reference: design/02-error-handling.md §2
 */

export interface ErrorLogEntry {
  timestamp: string; // ISO8601: "2026-01-10T14:30:00.000Z"
  level: 'error' | 'warn' | 'info';
  component: string; // Vue component name (e.g., "MixedChart")
  action: string; // Action attempted (e.g., "fetchLocationStats")
  endpoint: string; // API endpoint (e.g., "/api/report/location-statistics")
  status: number | null; // HTTP status (null for network errors)
  message: string; // Error message
  context?: {
    // Additional context
    dateRange?: string;
    pageIndex?: number;
    userId?: string;
  };
  stack?: string; // Stack trace (development only)
}

export class ErrorLogger {
  static log(entry: Partial<ErrorLogEntry>): void {
    const fullEntry: ErrorLogEntry = {
      timestamp: new Date().toISOString(),
      level: entry.level || 'error',
      component: entry.component || 'Unknown',
      action: entry.action || 'Unknown',
      endpoint: entry.endpoint || '',
      status: entry.status || null,
      message: entry.message || 'Unknown error',
      context: entry.context || {},
    };

    // Development: console with color
    if (import.meta.env.DEV) {
      const color =
        fullEntry.level === 'error'
          ? 'color: #F1416C'
          : fullEntry.level === 'warn'
            ? 'color: #FFC700'
            : 'color: #50CD89';
      console.group(`%c[${fullEntry.level.toUpperCase()}] ${fullEntry.component}`, color);
      console.log('Timestamp:', fullEntry.timestamp);
      console.log('Action:', fullEntry.action);
      console.log('Endpoint:', fullEntry.endpoint);
      console.log('Status:', fullEntry.status);
      console.log('Message:', fullEntry.message);
      console.log('Context:', fullEntry.context);
      if (entry.stack) console.log('Stack:', entry.stack);
      console.groupEnd();
    }

    // Production: silent (can be integrated with logging service)
    if (import.meta.env.PROD) {
      // Silent in production - could send to logging service
      // Example: fetch('/logs', { method: 'POST', body: JSON.stringify(fullEntry) })
    }
  }

  static error(
    component: string,
    action: string,
    message: string,
    endpoint?: string,
    status?: number,
    context?: Record<string, unknown>,
  ): void {
    this.log({
      level: 'error',
      component,
      action,
      message,
      endpoint,
      status: status || null,
      context: context as any,
    });
  }

  static warn(
    component: string,
    action: string,
    message: string,
    endpoint?: string,
    context?: Record<string, unknown>,
  ): void {
    this.log({
      level: 'warn',
      component,
      action,
      message,
      endpoint,
      context: context as any,
    });
  }

  static info(
    component: string,
    action: string,
    message: string,
    context?: Record<string, unknown>,
  ): void {
    this.log({
      level: 'info',
      component,
      action,
      message,
      context: context as any,
    });
  }
}
