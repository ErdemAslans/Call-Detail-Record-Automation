import {
  createRouter,
  createWebHistory,
  type RouteRecordRaw,
} from "vue-router";
import { useAuthStore } from "@/stores/auth";
import { useConfigStore } from "@/stores/config";

const routes: Array<RouteRecordRaw> = [
  {
    path: "/",
    redirect: "/dashboard",
    component: () => import("@/layouts/default-layout/DefaultLayout.vue"),
    meta: {
      middleware: "auth",
    },
    children: [
      {
        path: "/dashboard",
        name: "dashboard",
        component: () => import("@/views/Dashboard.vue"),
        meta: {
          pageTitle: "Dashboard",
          breadcrumbs: ["dashboard"],
          roles: ["Admin"],
        },
      },
      {
        path: "/crafted/modals/wizards/two-factor-auth",
        name: "modals-wizards-two-factor-auth",
        component: () =>
          import("@/views/crafted/modals/wizards/TwoFactorAuth.vue"),
        meta: {
          pageTitle: "Two Factory Auth",
          breadcrumbs: ["Crafted", "Modals", "Wizards"],
        },
      },
      {
        path: "/userProfile/:number",
        name: "user-profile",
        component: () => import("@/views/userProfile/Profile.vue"),
        meta: {
          breadcrumbs: ["Profile"],
          pageTitle: "User Profile",
        },
        props: true,
        children: [
          {
            path: "statistics",
            name: "device-statistics",
            component: () => import("@/views/userProfile/UserStatistics.vue"),
            meta: {
              pageTitle: "Overview",
            },
            props: true,
          },
          {
            path: "settings",
            name: "device-settings",
            component: () => import("@/views/crafted/account/Settings.vue"),
            meta: {
              pageTitle: "Settings",
            },
          },
        ],
      },
      {
        path: "/breaks",
        name: "break-timeline",
        component: () => import("@/views/BreakTimeline.vue"),
        meta: {
          pageTitle: "Breaks",
          breadcrumbs: ["Breaks"],
          roles: ["Central"],
        },
      },
      {
        path: "/email-reports",
        name: "email-reports",
        component: () => import("@/views/dashboard/reports/EmailReports.vue"),
        meta: {
          pageTitle: "Email Reports",
          breadcrumbs: ["Reports", "Email Reports"],
          roles: ["Admin"],
        },
      },
    ],
  },
  {
    path: "/",
    component: () => import("@/layouts/AuthLayout.vue"),
    children: [
      {
        path: "/sign-in",
        name: "sign-in",
        component: () =>
          import("@/views/crafted/authentication/basic-flow/SignIn.vue"),
        meta: {
          pageTitle: "Sign In",
        },
      },
      {
        path: "/sign-up",
        name: "sign-up",
        component: () =>
          import("@/views/crafted/authentication/basic-flow/SignUp.vue"),
        meta: {
          pageTitle: "Sign Up",
        },
      },
      {
        path: "/password-reset",
        name: "password-reset",
        component: () =>
          import("@/views/crafted/authentication/basic-flow/PasswordReset.vue"),
        meta: {
          pageTitle: "Password reset",
        },
      },
    ],
  },
  {
    path: "/",
    component: () => import("@/layouts/SystemLayout.vue"),
    children: [
      {
        // the 404 route, when none of the above matches
        path: "/404",
        name: "404",
        component: () => import("@/views/crafted/authentication/Error404.vue"),
        meta: {
          pageTitle: "Error 404",
        },
      },
      {
        path: "/500",
        name: "500",
        component: () => import("@/views/crafted/authentication/Error500.vue"),
        meta: {
          pageTitle: "Error 500",
        },
      },
      {
        path: "/403",
        name: "403",
        component: () => import("@/views/crafted/authentication/Error403.vue"),
        meta: {
          pageTitle: "Error 403",
        },
      },
    ],
  },
  {
    path: "/:pathMatch(.*)*",
    redirect: "/404",
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
  scrollBehavior(to) {
    // If the route has a hash, scroll to the section with the specified ID; otherwise, scroll toc the top of the page.
    if (to.hash) {
      return {
        el: to.hash,
        top: 80,
        behavior: "smooth",
      };
    } else {
      return {
        top: 0,
        left: 0,
        behavior: "smooth",
      };
    }
  },
});

router.beforeEach((to, from, next) => {
  const authStore = useAuthStore();
  const configStore = useConfigStore();

  // current page view title
  document.title = `${to.meta.pageTitle} - ${import.meta.env.VITE_APP_NAME}`;

  // reset config to initial state
  configStore.resetLayoutConfig();

  // verify auth token before each page change
  // authStore.verifyAuth();

  // before page access check if page requires authentication
  // Check if the route requires authentication
  if (to.meta.middleware === "auth") {
    // If not authenticated, redirect to the sign-in page
    if (!authStore.isAuthenticated) {
      return next({ name: "sign-in", query: { returnUrl: to.fullPath } });
    }

    // Check if the route requires specific roles
    if (Array.isArray(to.meta.roles)) {
      const hasRequiredRole = to.meta.roles.some((role: string) =>
        authStore.user.roles.includes(role),
      );

      // If user doesn't have the required role, redirect to 403 page
      if (!hasRequiredRole) {
        return next({ name: "403" });
      }
    }

    // User is authenticated and authorized, proceed to the route
    return next();
  }

  // If the route doesn't require authentication, just proceed
  return next();
});

export default router;
