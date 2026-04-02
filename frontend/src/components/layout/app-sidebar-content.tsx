"use client"
import { SidebarContent, SidebarGroup, SidebarMenu, SidebarMenuButton, SidebarMenuItem, SidebarMenuSub, SidebarMenuSubButton, SidebarMenuSubItem, useSidebar } from '@/components/ui/sidebar'
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover'
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from '@/components/ui/collapsible'
import { ChevronDown, LayoutDashboard, type LucideIcon, SquareKanban, PackageSearch } from "lucide-react";
import Link from 'next/link';
import { usePathname } from 'next/navigation';

// Menu items.
interface IMenuItem {
  title: string;
  url?: string;
  icon: LucideIcon;
  subItems?: IMenuItem[];
}
const items: IMenuItem[] = [
  {
    title: "Dashboard",
    url: "/",
    icon: LayoutDashboard,
  },
  {
    title: "Reports",
    icon: SquareKanban,
    subItems: [
      {
        title: "Services",
        url: "/reports/services",
        icon: PackageSearch,
      }

    ]
  },
  {
    title: "Datatable demo",
    url: "/datatable",
    icon: LayoutDashboard,
  },
]
function AppSidebarContent() {
  return (
    <SidebarContent>
      <SidebarGroup>
        <SidebarMenu>
          {items.map(item => (
            <SidebarMenuItem key={item.title}>
              <RenderMenuItem item={item} />
            </SidebarMenuItem>
          ))}
        </SidebarMenu>
      </SidebarGroup>
    </SidebarContent>
  )
}

function RenderMenuItem({ item }: { item: IMenuItem }) {
  const { state } = useSidebar();
  const isCollapsed = state === "collapsed";
  const pathname = usePathname();
  const isActive = (item: IMenuItem) => {
    if (item.url && (pathname === item.url || pathname.startsWith(item.url + "/")))
      return true;
    if (item.subItems) {
      return item.subItems.some(
        (subItem: IMenuItem) =>
          subItem.url &&
          (pathname === subItem.url || pathname.startsWith(subItem.url + "/"))
      );
    }
    return false;
  }
  if (item.subItems && isCollapsed) {
    // Render popover for collapsed sidebar with submenu
    return (
      <Popover>
        <PopoverTrigger asChild>
          <SidebarMenuButton
            tooltip={item.title}
            isActive={isActive(item)}
            className='cursor-pointer'
          >
            <item.icon />
            <span>{item.title}</span>
            <CollapseComponent />
          </SidebarMenuButton>
        </PopoverTrigger>
        <PopoverContent side="right" align="start" className='p-2'>
          <p className='font-medium p-2'>{item.title}</p>
          <SidebarMenu>
            {item.subItems.map((subItem: IMenuItem) => (
              <SidebarMenuItem key={subItem.title}>
                <SidebarMenuButton asChild isActive={isActive(subItem)}>
                  <Link href={subItem.url!}>
                    <subItem.icon />
                    <span>{subItem.title}</span>
                  </Link>
                </SidebarMenuButton>
              </SidebarMenuItem>
            ))}
          </SidebarMenu>
        </PopoverContent>
      </Popover>
    )
  }
  else if (item.subItems) {
    // Render collapsible for expanded sidebar with submenu
    return (
      <Collapsible defaultOpen={isActive(item)} className="group/collapsible">
        <CollapsibleTrigger asChild>
          <SidebarMenuButton
            tooltip={item.title}
            isActive={isActive(item)}
            className='cursor-pointer'
          >
            <item.icon />
            <span>{item.title}</span>
            <CollapseComponent />
          </SidebarMenuButton>
        </CollapsibleTrigger>
        <CollapsibleContent>
          <SidebarMenuSub>
            {item.subItems.map((subItem: IMenuItem) => (
              <SidebarMenuSubItem key={subItem.title}>
                <SidebarMenuSubButton asChild
                  isActive={isActive(subItem)}
                >
                  <Link href={subItem.url!}>
                    <subItem.icon />
                    <span>{subItem.title}</span>
                  </Link>
                </SidebarMenuSubButton>
              </SidebarMenuSubItem>
            ))}
          </SidebarMenuSub>
        </CollapsibleContent>
      </Collapsible>
    )
  }
  else {
    // Render simple menu item
    return (
      <SidebarMenuButton asChild isActive={isActive(item)}>
        <Link href={item.url!}>
          <item.icon />
          <span>{item.title}</span>
        </Link>
      </SidebarMenuButton>
    )
  }
}

function CollapseComponent() {
  return (
    <ChevronDown className="ml-auto transition-transform group-data-[state=open]/collapsible:rotate-180" />
  )
}

export default AppSidebarContent