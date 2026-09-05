"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { LayoutDashboard, Users, UserPlus, Settings, LogOut } from "lucide-react";
import { useAuth } from "@/contexts/AuthContext";
import { ProtectedRoute } from "@/components/ProtectedRoute";

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  const { logout } = useAuth();
  const pathname = usePathname();

  const isActive = (path: string) => {
    if (path === "/admin/dashboard" && pathname === "/admin/dashboard") return true;
    if (path !== "/admin/dashboard" && pathname.startsWith(path)) return true;
    return false;
  };
  
  const baseDesktopLinkClass = "flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors duration-200";
  const inactiveDesktopClass = "text-gray-700 dark:text-gray-300 hover:bg-slate-100 dark:hover:bg-slate-800";
  const activeDesktopClass = "bg-red-50 dark:bg-red-900/10 text-red-700 dark:text-red-400 font-semibold ring-1 ring-red-200 dark:ring-red-900/50";

  const getDesktopLinkClass = (path: string) => {
    return `${baseDesktopLinkClass} ${isActive(path) ? activeDesktopClass : inactiveDesktopClass}`;
  };

  const baseMobileLinkClass = "flex flex-col items-center transition-colors duration-200 px-3 py-2 rounded-lg";
  const inactiveMobileClass = "text-gray-500 hover:text-red-600 dark:hover:text-red-400";
  const activeMobileClass = "text-red-700 dark:text-red-400 bg-red-50 dark:bg-red-900/10";

  const getMobileLinkClass = (path: string) => {
    return `${baseMobileLinkClass} ${isActive(path) ? activeMobileClass : inactiveMobileClass}`;
  };

  return (
    <ProtectedRoute allowedRoles={["Admin", "SystemAdmin"]}>
      <div className="flex h-full bg-slate-50 dark:bg-slate-900">
        {/* Sidebar Navigation */}
        <aside className="w-64 border-r border-gray-200 dark:border-slate-800 bg-white dark:bg-slate-900 flex flex-col hidden md:flex">
          <div className="p-6">
            <h2 className="text-xs uppercase font-bold text-gray-500 tracking-wider">Super Admin</h2>
          </div>
          <nav className="px-4 space-y-2 flex-1">
            <Link href="/admin/dashboard" className={getDesktopLinkClass("/admin/dashboard")}>
              <LayoutDashboard className="w-5 h-5" />
              <span>Overview</span>
            </Link>
            <Link href="/admin/users" className={getDesktopLinkClass("/admin/users")}>
              <Users className="w-5 h-5" />
              <span>Users</span>
            </Link>
            <Link href="/admin/invite" className={getDesktopLinkClass("/admin/invite")}>
              <UserPlus className="w-5 h-5" />
              <span>Invite Admin</span>
            </Link>
            <Link href="/admin/settings" className={getDesktopLinkClass("/admin/settings")}>
              <Settings className="w-5 h-5" />
              <span>Settings</span>
            </Link>
          </nav>
          
          {/* Bottom Sidebar - Sign Out */}
          <div className="p-4 border-t border-gray-200 dark:border-slate-800 mt-auto">
            <button 
              onClick={() => logout()}
              className="flex items-center space-x-3 px-4 py-3 w-full text-gray-700 dark:text-gray-300 hover:bg-red-50 hover:text-red-600 dark:hover:bg-slate-800 rounded-lg transition-colors"
            >
              <LogOut className="w-5 h-5" />
              <span className="font-medium">Sign Out</span>
            </button>
          </div>
        </aside>

        {/* Main Content Area */}
        <main className="flex-1 flex flex-col min-h-0 overflow-y-auto pb-20 md:pb-0">
          <div className="flex-1 p-6 lg:p-8">
            {children}
          </div>
        </main>
        
        {/* Mobile Bottom Tab Navigation */}
        <div className="md:hidden fixed bottom-0 left-0 right-0 border-t border-gray-200 dark:border-slate-800 bg-white dark:bg-slate-900 pb-safe z-50">
          <nav className="flex justify-around p-2">
            <Link href="/admin/dashboard" className={getMobileLinkClass("/admin/dashboard")}>
              <LayoutDashboard className="w-6 h-6" />
              <span className="text-[10px] mt-1 font-medium">Overview</span>
            </Link>
            <Link href="/admin/users" className={getMobileLinkClass("/admin/users")}>
              <Users className="w-6 h-6" />
              <span className="text-[10px] mt-1 font-medium">Users</span>
            </Link>
            <Link href="/admin/settings" className={getMobileLinkClass("/admin/settings")}>
              <Settings className="w-6 h-6" />
              <span className="text-[10px] mt-1 font-medium">Settings</span>
            </Link>
          </nav>
        </div>
      </div>
    </ProtectedRoute>
  );
}
