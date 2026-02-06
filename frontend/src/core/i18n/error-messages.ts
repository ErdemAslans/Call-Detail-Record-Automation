/**
 * Error Messages i18n
 * Turkish error messages for frontend error handling
 * Reference: design/02-error-handling.md §3.1
 */

const errorMessages = {
  // HTTP Status Errors
  'error.badRequest': 'Geçersiz istek. Lütfen girdiğiniz bilgileri kontrol edin.',
  'error.unauthorized': 'Oturumunuz sona erdi. Lütfen tekrar giriş yapın.',
  'error.forbidden': 'Bu işlem için yetkiniz bulunmamaktadır.',
  'error.notFound': 'İstenen kaynak bulunamadı.',
  'error.serverError': 'Sunucu hatası oluştu. Lütfen daha sonra tekrar deneyin.',
  'error.serviceUnavailable': 'Servis geçici olarak kullanılamıyor. Lütfen bekleyin.',

  // Network Errors
  'error.timeout': 'İstek zaman aşımına uğradı. Lütfen tekrar deneyin.',
  'error.offline': 'İnternet bağlantınızı kontrol edin.',
  'error.network': 'Ağ hatası oluştu. Lütfen bağlantınızı kontrol edin.',

  // Validation Errors
  'error.validation': 'Lütfen tüm alanları doğru şekilde doldurun.',
  'error.invalidDate': 'Geçersiz tarih formatı.',
  'error.dateRange': 'Başlangıç tarihi bitiş tarihinden önce olmalıdır.',

  // Data Errors
  'error.noData': 'Gösterilecek veri bulunamadı.',
  'error.loadFailed': 'Veriler yüklenemedi. Lütfen tekrar deneyin.',

  // Generic
  'error.unknown': 'Beklenmeyen bir hata oluştu.',
  'error.retry': 'Tekrar Dene',
};

export default errorMessages;
