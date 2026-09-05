"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { LayoutDashboard, Calendar, MessageSquare, User, Search } from "lucide-react";
import { ProtectedRoute } from "@/components/ProtectedRoute";

export default function PatientLayout({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();

  const isActive = (path: string) => {
    if (path === "/patient/dashboard" && pathname === "/patient/dashboard") return true;
    if (path !== "/patient/dashboard" && pathname.startsWith(path)) return true;
    return false;
  };
  
  const baseDesktopLinkClass = "flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors duration-200";
  const inactiveDesktopClass = "text-gray-700 dark:text-gray-300 hover:bg-slate-100 dark:hover:bg-slate-800";
  const activeDesktopClass = "bg-sky-50 dark:bg-slate-800 text-[var(--color-primary-600)] dark:text-sky-400 font-semibold ring-1 ring-[var(--color-primary-600)] dark:ring-sky-700";

  const getDesktopLinkClass = (path: string) => {
    return `${baseDesktopLinkClass} ${isActive(path) ? activeDesktopClass : inactiveDesktopClass}`;
  };

  const baseMobileLinkClass = "flex flex-col items-center transition-colors duration-200 px-3 py-2 rounded-lg";
  const inactiveMobileClass = "text-gray-500 hover:text-[var(--color-primary-600)] dark:hover:text-sky-400";
  const activeMobileClass = "text-[var(--color-primary-600)] dark:text-sky-400 bg-sky-50 dark:bg-slate-800";

  const getMobileLinkClass = (path: string) => {
    return `${baseMobileLinkClass} ${isActive(path) ? activeMobileClass : inactiveMobileClass}`;
  };

  return (
    <ProtectedRoute allowedRoles={["Patient"]}>
      <div className="flex h-full bg-slate-50 dark:bg-slate-900">
        {/* Sidebar Navigation */}
        <aside className="w-64 border-r border-gray-200 dark:border-slate-800 bg-white dark:bg-slate-900 hidden md:block">
          <div className="p-6">
            <h2 className="text-xs uppercase font-bold text-gray-500 tracking-wider">Patient Portal</h2>
          </div>
          <nav className="px-4 space-y-2">
            <Link href="/patient/dashboard" className={getDesktopLinkClass("/patient/dashboard")}>
              <LayoutDashboard className="w-5 h-5" />
              <span>Dashboard</span>
            </Link>
            <Link href="/patient/search" className={getDesktopLinkClass("/patient/search")}>
              <Search className="w-5 h-5" />
              <span>Find Doctor</span>
            </Link>
            <Link href="/patient/appointments" className={getDesktopLinkClass("/patient/appointments")}>
              <Calendar className="w-5 h-5" />
              <span>Appointments</span>
            </Link>
            <Link href="/patient/messages" className={getDesktopLinkClass("/patient/messages")}>
              <MessageSquare className="w-5 h-5" />
              <span>Messages</span>
            </Link>
            <Link href="/patient/profile" className={getDesktopLinkClass("/patient/profile")}>
              <User className="w-5 h-5" />
              <span>My Profile</span>
            </Link>
          </nav>
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
            <Link href="/patient/dashboard" className={getMobileLinkClass("/patient/dashboard")}>
              <LayoutDashboard className="w-6 h-6" />
              <span className="text-[10px] mt-1 font-medium">Dashboard</span>
            </Link>
            <Link href="/patient/search" className={getMobileLinkClass("/patient/search")}>
              <Search className="w-6 h-6" />
              <span className="text-[10px] mt-1 font-medium">Find Doctor</span>
            </Link>
            <Link href="/patient/appointments" className={getMobileLinkClass("/patient/appointments")}>
              <Calendar className="w-6 h-6" />
              <span className="text-[10px] mt-1 font-medium">Bookings</span>
            </Link>
            <Link href="/patient/messages" className={getMobileLinkClass("/patient/messages")}>
              <MessageSquare className="w-6 h-6" />
              <span className="text-[10px] mt-1 font-medium">Inbox</span>
            </Link>
            <Link href="/patient/profile" className={getMobileLinkClass("/patient/profile")}>
              <User className="w-6 h-6" />
              <span className="text-[10px] mt-1 font-medium">Profile</span>
            </Link>
          </nav>
        </div>
      </div>
    </ProtectedRoute>
  );
}
