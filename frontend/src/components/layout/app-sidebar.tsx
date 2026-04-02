"use client"
import { Sidebar, SidebarHeader, SidebarMenu, SidebarMenuButton, SidebarMenuItem, useSidebar } from "@/components/ui/sidebar";

import AppSidebarContent from "@/components/layout/app-sidebar-content";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { ChevronDown, ChevronsUpDown, GalleryVerticalEnd } from "lucide-react";

export function AppSidebar() {
  const { state } = useSidebar();

  return (
    <>
      <Sidebar collapsible="icon" variant="sidebar" className="transition-all duration-200">
        <SidebarHeader>
          <SidebarMenu>
            <SidebarMenuItem>
              <SidebarMenuButton
                size="lg"
              >
                <div className="flex aspect-square size-8 items-center justify-center rounded-lg bg-sidebar-primary text-sidebar-primary-foreground font-bold">
                  A
                </div>
                <div className="flex flex-col gap-0.5 leading-none">
                  <span className="font-medium">Automation</span>
                  <span className="">Easy to use</span>
                </div>
                {/* <ChevronsUpDown className="ml-auto" /> */}
              </SidebarMenuButton>
            </SidebarMenuItem>
          </SidebarMenu>
        </SidebarHeader>
        <AppSidebarContent />
      </Sidebar>
    </>

  )
}
