"use client"
import { ModeToggle } from '@/components/mode-toggle';
import { Button } from '@/components/ui/button';
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from '@/components/ui/dropdown-menu';
import { SidebarTrigger, useSidebar } from '@/components/ui/sidebar';
import { CircleUserRound, LogOut, UserRound } from 'lucide-react'
import Link from 'next/link';
import { useRouter } from 'next/navigation';


const AppHeader = () => {
  const { state } = useSidebar();
  const isCollapsed = state === "collapsed";
  const router = useRouter();

  async function handleLogout() {
    await fetch("/api/auth/logout", { method: "POST" })
    router.push("/login")
  }

  return (
    <div className={` flex shrink-0 items-center gap-2 border-b bg-background px-4 transition-all duration-400 ${isCollapsed ? 'h-12' : 'h-16'}`}>
      <div className="flex items-center gap-2">
        <SidebarTrigger className="p-4.5 cursor-pointer" />
      </div>
      <div className="flex items-center gap-2 ml-auto">
        <ModeToggle />
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" size="icon" className='cursor-pointer'>
              <CircleUserRound className="h-4 w-4" />
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end">
            <DropdownMenuItem asChild>
              <Link href="/profile" className="flex items-center gap-2 cursor-pointer">
                <UserRound className="h-4 w-4" />
                Thông tin cá nhân
              </Link>
            </DropdownMenuItem>
            <DropdownMenuItem onClick={handleLogout} className="flex items-center gap-2 cursor-pointer">
              <LogOut className="h-4 w-4" />
              Đăng xuất
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </div>
  )
}

export default AppHeader