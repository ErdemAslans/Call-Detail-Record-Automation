/**
 * Sanitizer Utility
 * XSS prevention via DOMPurify
 * Reference: design/03-data-integrity.md §4.2
 */

import DOMPurify from 'dompurify';

/**
 * Sanitize HTML content - removes all tags
 * @param dirty Raw HTML string
 * @returns Safe text without HTML tags
 */
export function sanitizeHtml(dirty: string): string {
  return DOMPurify.sanitize(dirty, {
    ALLOWED_TAGS: [], // Strip ALL HTML tags
    ALLOWED_ATTR: [],
  });
}

/**
 * Sanitize value for safe display
 * @param value Any value (string, number, object)
 * @returns Safe string for display
 */
export function sanitizeForDisplay(value: unknown): string {
  if (value === null || value === undefined) return '';
  const str = String(value);
  return sanitizeHtml(str);
}

/**
 * Escape special characters for HTML display
 * @param str String to escape
 * @returns HTML-safe string
 */
export function escapeHtml(str: string): string {
  const escapeMap: Record<string, string> = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#x27;',
  };
  return str.replace(/[&<>"']/g, (char) => escapeMap[char] || char);
}

/**
 * Sanitize URL parameter for safe transmission
 * @param value URL parameter value
 * @returns URL-encoded string
 */
export function sanitizeUrlParam(value: string): string {
  return encodeURIComponent(sanitizeHtml(value));
}
