import { Sidebar } from "@/components/ui/sidebar";

import AppSidebarContent from "@/components/layout/app-sidebar-content";

export function AppSidebar() {
  return (
    <Sidebar collapsible="icon" variant="sidebar" className="transition-all duration-200">
      <AppSidebarContent />
    </Sidebar>
  )
}
