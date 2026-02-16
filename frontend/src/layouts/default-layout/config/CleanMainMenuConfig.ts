import type { MenuItem } from "@/layouts/default-layout/config/types";

const MainMenuConfig: Array<MenuItem> = [
  {
    pages: [
      {
        heading: "dashboard",
        route: "/dashboard",
        keenthemesIcon: "element-11",
        bootstrapIcon: "bi-app-indicator",
        roles: ["Admin"],
      },
      {
        heading: "emailReports",
        route: "/email-reports",
        keenthemesIcon: "sms",
        bootstrapIcon: "bi-envelope",
        roles: ["Admin"],
      },
      {
        heading: "breakTimeline",
        route: "/breaks",
        keenthemesIcon: "coffee",
        bootstrapIcon: "bi-cup-hot",
        roles: ["Admin", "Central"],
      },
    ],
  },
];

export default MainMenuConfig;
