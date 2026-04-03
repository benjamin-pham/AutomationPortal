import { BookUser, KeyRound, LayoutDashboard, LucideIcon, PackageSearch, SquareKanban } from "lucide-react";

export interface IMenuItem {
    title: string;
    url?: string;
    icon: LucideIcon;
    subItems?: IMenuItem[];
}

export const menuItems: IMenuItem[] = [
    {
        title: "Dashboard",
        url: "/",
        icon: LayoutDashboard,
    },
    {
        title: "Users",
        url: "/users",
        icon: BookUser,
    },
    {
        title: "Gemini Keys",
        url: "/gemini-keys",
        icon: KeyRound,
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