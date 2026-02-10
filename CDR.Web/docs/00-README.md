# CDR.Web Dokümantasyon

## 📚 Dokümantasyon Haritası

Bu dokümantasyon context engineering için optimize edilmiştir. Her dosya bağımsız olarak kullanılabilir.

### Öğrenme Yolu (Sequential)
1. **[01-Overview.md](01-Overview.md)** - Proje amacı, stack, temel kavramlar
2. **[02-ProjectStructure.md](02-ProjectStructure.md)** - Klasör yapısı, layouts, views
3. **[03-Architecture.md](03-Architecture.md)** - Composition API, TypeScript patterns
4. **[04-StateManagement.md](04-StateManagement.md)** - Pinia stores, reactivity
5. **[05-Routing.md](05-Routing.md)** - Router configuration, guards, navigation
6. **[06-APIIntegration.md](06-APIIntegration.md)** - Axios, interceptors, authentication
7. **[07-Components.md](07-Components.md)** - Component design, Element Plus, reusability
8. **[08-Authentication.md](08-Authentication.md)** - Login flow, JWT, authorization
9. **[09-Styling.md](09-Styling.md)** - Sass, theming, CSS structure
10. **[10-ErrorHandling.md](10-ErrorHandling.md)** - Error patterns, validation, logging

### Modüler Erişim (By Topic)
- **State Management**: 04-StateManagement.md
- **Data Fetching**: 06-APIIntegration.md + 04-StateManagement.md
- **Authentication**: 08-Authentication.md + 05-Routing.md
- **UI Components**: 07-Components.md + 09-Styling.md
- **Page Development**: 03-Architecture.md + 02-ProjectStructure.md
- **Error Handling**: 10-ErrorHandling.md + 06-APIIntegration.md

### Hızlı Referanslar
- Setup: 01-Overview.md#running
- Component Template: 07-Components.md#component-structure
- Store Pattern: 04-StateManagement.md
- API Calls: 06-APIIntegration.md
- Auth Flow: 08-Authentication.md

---

## 🎯 Bu Dokümantasyon Neyi Kapsar?

✅ **Kapsanan Konular:**
- Vue 3 Composition API + TypeScript
- Pinia state management
- Vue Router with role-based guards
- Axios HTTP client with JWT interceptors
- Element Plus UI component library
- Vite build system
- Responsive design patterns
- Form validation (VeeValidate)
- Dashboard & reporting views
- Authentication & authorization

❌ **Kapsamayan Konular:**
- Detailed Vue 3 Options API (using Composition API)
- Advanced Pinia plugins
- Deep dive into ApexCharts (library docs)
- Server-side rendering (Vite client-side only)

---

## 💡 Context Engineering Tips

Bu dokümantasyon aşağıdaki amaçlarla kullanılabilir:

1. **Component Development**: 07-Components.md + 03-Architecture.md'nin başında okuyun
2. **Feature Development**: İlgili store → API route → component sıralamasını izleyin
3. **State Debugging**: 04-StateManagement.md'deki patterns'ı referans alın
4. **API Integration**: 06-APIIntegration.md'yi kullanım case'leriniz için customize edin
5. **Styling**: 09-Styling.md'deki naming conventions'ı izleyin

---

**İçindekiler Tablosu**: Aşağıdaki dosyaların herbiri bağımsız olarak okunabilir.
